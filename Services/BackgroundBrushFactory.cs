using System.Windows;
using System.Windows.Media;
using Bloom.Models;

namespace Bloom.Services;

public static class BackgroundBrushFactory
{
    public static Brush Create(PageBackground background, double linePitch = 32, double linePhase = 0)
    {
        Color baseColor = ParseColor(background.Value, Colors.White);

        return background.Kind switch
        {
            "lined" => LinedBrush(baseColor, linePitch, linePhase),
            "grid" => DottedBrush(baseColor),
            "graph" => GraphBrush(baseColor),
            "image" => ImageBrush(background, baseColor),
            _ => new SolidColorBrush(baseColor)
        };
    }

    private static Brush LinedBrush(Color baseColor, double pitch, double phase)
    {
        Color line = LineColor(baseColor);
        double y = ((phase % pitch) + pitch) % pitch;
        DrawingGroup group = new();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(baseColor), null,
            new RectangleGeometry(new Rect(0, 0, pitch, pitch))));
        group.Children.Add(new GeometryDrawing(null, new Pen(new SolidColorBrush(line), 1),
            new LineGeometry(new Point(0, y), new Point(pitch, y))));
        return Tile(group, pitch, pitch);
    }

    private static Brush GraphBrush(Color baseColor)
    {
        Color line = LineColor(baseColor);
        DrawingGroup group = new();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(baseColor), null,
            new RectangleGeometry(new Rect(0, 0, 22, 22))));
        Pen pen = new(new SolidColorBrush(line), 0.8);
        group.Children.Add(new GeometryDrawing(null, pen, new LineGeometry(new Point(0, 21.5), new Point(22, 21.5))));
        group.Children.Add(new GeometryDrawing(null, pen, new LineGeometry(new Point(21.5, 0), new Point(21.5, 22))));
        return Tile(group, 22, 22);
    }

    private static Brush DottedBrush(Color baseColor)
    {
        Color dot = LineColor(baseColor);
        DrawingGroup group = new();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(baseColor), null,
            new RectangleGeometry(new Rect(0, 0, 26, 26))));
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(dot), null,
            new EllipseGeometry(new Point(13, 13), 1.4, 1.4)));
        return Tile(group, 26, 26);
    }

    private static Brush ImageBrush(PageBackground background, Color baseColor)
    {
        string? path = ArtPaths.Resolve(background.ImagePath);
        if (path is null)
        {
            return new SolidColorBrush(baseColor);
        }
        try
        {
            System.Windows.Media.Imaging.BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();
            return new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
        }
        catch (Exception)
        {
            return new SolidColorBrush(baseColor);
        }
    }

    private static DrawingBrush Tile(Drawing drawing, double width, double height)
    {
        DrawingBrush brush = new(drawing)
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, width, height),
            ViewportUnits = BrushMappingMode.Absolute,
            Stretch = Stretch.None
        };
        brush.Freeze();
        return brush;
    }

    private static Color LineColor(Color baseColor)
    {
        double luminance = (0.299 * baseColor.R + 0.587 * baseColor.G + 0.114 * baseColor.B) / 255.0;
        return luminance > 0.5
            ? Blend(baseColor, Colors.Black, 0.14)
            : Blend(baseColor, Colors.White, 0.18);
    }

    private static Color Blend(Color a, Color b, double t) => Color.FromRgb(
        (byte)(a.R * (1 - t) + b.R * t),
        (byte)(a.G * (1 - t) + b.G * t),
        (byte)(a.B * (1 - t) + b.B * t));

    public static Color ParseColor(string? hex, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return fallback;
        }
        try
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
        catch (FormatException)
        {
            return fallback;
        }
    }
}
