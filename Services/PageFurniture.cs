using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bloom.Models;

namespace Bloom.Services;

public static class PageFurniture
{
    public static UIElement Build(double width, double height, PageBackground background, string fontFamily)
    {
        Color baseColor = BackgroundBrushFactory.ParseColor(background.Value, Colors.White);
        Canvas canvas = new()
        {
            Width = width,
            Height = height,
            IsHitTestVisible = false
        };

        canvas.Children.Add(new Line
        {
            X1 = PageGrid.MarginLineX,
            Y1 = 0,
            X2 = PageGrid.MarginLineX,
            Y2 = height,
            Stroke = new SolidColorBrush(MarginColor(baseColor)),
            StrokeThickness = 1.2
        });

        double titleY = PageGrid.TitleLineY(fontFamily);
        canvas.Children.Add(new Line
        {
            X1 = PageGrid.PaddingSide,
            Y1 = titleY,
            X2 = width - PageGrid.PaddingSide,
            Y2 = titleY,
            Stroke = new SolidColorBrush(TitleLineColor(baseColor)),
            StrokeThickness = 1.6
        });

        return canvas;
    }

    private static Color MarginColor(Color baseColor)
    {
        Color rose = Luminance(baseColor) > 0.5
            ? Color.FromRgb(0xC8, 0x6B, 0x6B)
            : Color.FromRgb(0xD9, 0x8C, 0x8C);
        return Blend(baseColor, rose, 0.65);
    }

    private static Color TitleLineColor(Color baseColor)
    {
        Color contrast = Luminance(baseColor) > 0.5 ? Colors.Black : Colors.White;
        return Blend(baseColor, contrast, 0.30);
    }

    private static double Luminance(Color color) =>
        (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255.0;

    private static Color Blend(Color a, Color b, double t) => Color.FromRgb(
        (byte)(a.R * (1 - t) + b.R * t),
        (byte)(a.G * (1 - t) + b.G * t),
        (byte)(a.B * (1 - t) + b.B * t));
}
