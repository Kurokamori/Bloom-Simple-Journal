using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bloom.Services;

public sealed class ExportService
{
    public const double DefaultDpi = 200.0;

    public RenderTargetBitmap Render(FrameworkElement element, double dpi = DefaultDpi)
    {
        double width = ResolveDimension(element.ActualWidth, element.Width, 800);
        double height = ResolveDimension(element.ActualHeight, element.Height, 1120);

        if (element.ActualWidth <= 0 || element.ActualHeight <= 0)
        {
            Size size = new(width, height);
            element.Measure(size);
            element.Arrange(new Rect(size));
            element.UpdateLayout();
        }

        int pixelWidth = (int)Math.Ceiling(width * dpi / 96.0);
        int pixelHeight = (int)Math.Ceiling(height * dpi / 96.0);

        RenderTargetBitmap target = new(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);
        target.Render(element);
        return target;
    }

    public void SavePng(FrameworkElement element, string path, double dpi = DefaultDpi)
    {
        RenderTargetBitmap bitmap = Render(element, dpi);
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        EnsureDirectory(path);
        using FileStream stream = File.Create(path);
        encoder.Save(stream);
    }

    public void SaveBitmapPng(BitmapSource source, string path)
    {
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(source));
        EnsureDirectory(path);
        using FileStream stream = File.Create(path);
        encoder.Save(stream);
    }

    public byte[] EncodeJpeg(BitmapSource source, int quality = 92)
    {
        JpegBitmapEncoder encoder = new() { QualityLevel = quality };
        BitmapSource opaque = FlattenToWhite(source);
        encoder.Frames.Add(BitmapFrame.Create(opaque));
        using MemoryStream stream = new();
        encoder.Save(stream);
        return stream.ToArray();
    }

    public void SavePdf(IReadOnlyList<FrameworkElement> elements, string path, double dpi = DefaultDpi)
    {
        List<PdfImagePage> pages = new();
        foreach (FrameworkElement element in elements)
        {
            RenderTargetBitmap bitmap = Render(element, dpi);
            byte[] jpeg = EncodeJpeg(bitmap);
            pages.Add(new PdfImagePage(jpeg, bitmap.PixelWidth, bitmap.PixelHeight));
        }

        EnsureDirectory(path);
        using FileStream stream = File.Create(path);
        PdfWriter.Write(stream, pages, dpi);
    }

    public void SavePdfFromBitmaps(IReadOnlyList<BitmapSource> bitmaps, string path, double dpi = DefaultDpi)
    {
        List<PdfImagePage> pages = new();
        foreach (BitmapSource bitmap in bitmaps)
        {
            byte[] jpeg = EncodeJpeg(bitmap);
            pages.Add(new PdfImagePage(jpeg, bitmap.PixelWidth, bitmap.PixelHeight));
        }
        EnsureDirectory(path);
        using FileStream stream = File.Create(path);
        PdfWriter.Write(stream, pages, dpi);
    }

    private static BitmapSource FlattenToWhite(BitmapSource source)
    {
        DrawingVisual visual = new();
        using (DrawingContext context = visual.RenderOpen())
        {
            Rect rect = new(0, 0, source.PixelWidth, source.PixelHeight);
            context.DrawRectangle(Brushes.White, null, rect);
            context.DrawImage(source, rect);
        }
        RenderTargetBitmap target = new(
            source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Pbgra32);
        target.Render(visual);
        return target;
    }

    private static double ResolveDimension(double actual, double declared, double fallback)
    {
        if (actual > 0)
        {
            return actual;
        }
        if (!double.IsNaN(declared) && declared > 0)
        {
            return declared;
        }
        return fallback;
    }

    private static void EnsureDirectory(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
