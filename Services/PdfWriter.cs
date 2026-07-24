using System.Globalization;
using System.IO;
using System.Text;

namespace Bloom.Services;

public sealed record PdfImagePage(byte[] JpegData, int PixelWidth, int PixelHeight);

public static class PdfWriter
{
    public static void Write(Stream output, IReadOnlyList<PdfImagePage> pages, double dpi)
    {
        List<long> offsets = new();
        long position = 0;

        void WriteAscii(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            output.Write(bytes, 0, bytes.Length);
            position += bytes.Length;
        }

        void WriteRaw(byte[] bytes)
        {
            output.Write(bytes, 0, bytes.Length);
            position += bytes.Length;
        }

        int BeginObject()
        {
            offsets.Add(position);
            return offsets.Count;
        }

        WriteAscii("%PDF-1.7\n%âãÏÓ\n");

        int catalogNumber = BeginObject();
        int pagesNumber = catalogNumber + 1;
        WriteAscii($"{catalogNumber} 0 obj\n<< /Type /Catalog /Pages {pagesNumber} 0 R >>\nendobj\n");

        int reservedPages = BeginObject();
        int firstPageObject = reservedPages + 1;
        List<int> pageObjectNumbers = new();
        for (int i = 0; i < pages.Count; i++)
        {
            pageObjectNumbers.Add(firstPageObject + i * 3);
        }

        string kids = string.Join(" ", pageObjectNumbers.Select(n => $"{n} 0 R"));
        WriteAscii($"{pagesNumber} 0 obj\n<< /Type /Pages /Count {pages.Count} /Kids [{kids}] >>\nendobj\n");

        for (int i = 0; i < pages.Count; i++)
        {
            PdfImagePage page = pages[i];
            int pageObject = pageObjectNumbers[i];
            int contentObject = pageObject + 1;
            int imageObject = pageObject + 2;

            double widthPoints = page.PixelWidth * 72.0 / dpi;
            double heightPoints = page.PixelHeight * 72.0 / dpi;
            string w = widthPoints.ToString("0.##", CultureInfo.InvariantCulture);
            string h = heightPoints.ToString("0.##", CultureInfo.InvariantCulture);

            BeginObject();
            WriteAscii(
                $"{pageObject} 0 obj\n<< /Type /Page /Parent {pagesNumber} 0 R " +
                $"/MediaBox [0 0 {w} {h}] " +
                $"/Resources << /XObject << /Im0 {imageObject} 0 R >> >> " +
                $"/Contents {contentObject} 0 R >>\nendobj\n");

            string contentStream =
                $"q\n{w} 0 0 {h} 0 0 cm\n/Im0 Do\nQ\n";
            byte[] contentBytes = Encoding.ASCII.GetBytes(contentStream);
            BeginObject();
            WriteAscii($"{contentObject} 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
            WriteRaw(contentBytes);
            WriteAscii("endstream\nendobj\n");

            BeginObject();
            WriteAscii(
                $"{imageObject} 0 obj\n<< /Type /XObject /Subtype /Image " +
                $"/Width {page.PixelWidth} /Height {page.PixelHeight} " +
                $"/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode " +
                $"/Length {page.JpegData.Length} >>\nstream\n");
            WriteRaw(page.JpegData);
            WriteAscii("\nendstream\nendobj\n");
        }

        long xrefPosition = position;
        int totalObjects = offsets.Count;
        WriteAscii($"xref\n0 {totalObjects + 1}\n");
        WriteAscii("0000000000 65535 f \n");
        foreach (long offset in offsets)
        {
            WriteAscii($"{offset.ToString("D10", CultureInfo.InvariantCulture)} 00000 n \n");
        }

        WriteAscii(
            $"trailer\n<< /Size {totalObjects + 1} /Root {catalogNumber} 0 R >>\n" +
            $"startxref\n{xrefPosition}\n%%EOF\n");
    }
}
