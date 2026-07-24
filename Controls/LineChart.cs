using Bloom.Models;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using System.Windows;
using Microsoft.UI.Xaml.Media;

namespace Bloom.Controls;

public sealed class LineChart : FrameworkElement
{
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(nameof(Series), typeof(IReadOnlyList<ChartSeries>), typeof(LineChart),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StartDateProperty =
        DependencyProperty.Register(nameof(StartDate), typeof(DateOnly), typeof(LineChart),
            new FrameworkPropertyMetadata(default(DateOnly), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty EndDateProperty =
        DependencyProperty.Register(nameof(EndDate), typeof(DateOnly), typeof(LineChart),
            new FrameworkPropertyMetadata(default(DateOnly), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(LineChart),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(LineChart),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public IReadOnlyList<ChartSeries>? Series
    {
        get => (IReadOnlyList<ChartSeries>?)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    public DateOnly StartDate
    {
        get => (DateOnly)GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    public DateOnly EndDate
    {
        get => (DateOnly)GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
    }

    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    protected override void OnRender(DrawingContext dc)
    {
        double width = ActualWidth;
        double height = ActualHeight;
        if (width <= 20 || height <= 20)
        {
            return;
        }

        Brush gridBrush = ResolveBrush("Border", Color.FromRgb(0xE0, 0xE0, 0xE0));
        Brush textBrush = ResolveBrush("TextSubtle", Color.FromRgb(0x9A, 0x8F, 0xB0));
        Typeface typeface = new("Segoe UI");

        const double padLeft = 34;
        const double padBottom = 26;
        const double padTop = 12;
        const double padRight = 12;

        Rect plot = new(padLeft, padTop, Math.Max(10, width - padLeft - padRight), Math.Max(10, height - padTop - padBottom));

        int totalDays = Math.Max(1, EndDate.DayNumber - StartDate.DayNumber);
        double range = Math.Max(1, MaxValue - MinValue);

        double YFor(double value) => plot.Bottom - (value - MinValue) / range * plot.Height;
        double XFor(DateOnly date) => plot.Left + (double)(date.DayNumber - StartDate.DayNumber) / totalDays * plot.Width;

        Pen gridPen = new(gridBrush, 1) { DashStyle = new DashStyle(new double[] { 3, 4 }, 0) };
        gridPen.Freeze();
        int lines = 5;
        for (int i = 0; i <= lines; i++)
        {
            double value = MinValue + range * i / lines;
            double y = YFor(value);
            dc.DrawLine(gridPen, new Point(plot.Left, y), new Point(plot.Right, y));
            FormattedText label = new(((int)Math.Round(value)).ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 10, textBrush, 1.0);
            dc.DrawText(label, new Point(4, y - label.Height / 2));
        }

        int labelStep = Math.Max(1, totalDays / 6);
        for (int day = 0; day <= totalDays; day += labelStep)
        {
            DateOnly date = StartDate.AddDays(day);
            double x = XFor(date);
            FormattedText label = new(date.ToString("M/d", CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 10, textBrush, 1.0);
            dc.DrawText(label, new Point(x - label.Width / 2, plot.Bottom + 6));
        }

        if (Series is null)
        {
            return;
        }

        foreach (ChartSeries series in Series)
        {
            Color color = ParseColor(series.ColorHex);
            Brush fill = new SolidColorBrush(color);
            Pen pen = new(fill, 2.6) { LineJoin = PenLineJoin.Round, StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            pen.Freeze();

            List<Point> pixelPoints = new();
            StreamGeometry geometry = new();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                bool started = false;
                foreach (ChartPoint point in series.Points.Where(p => p.Value is not null).OrderBy(p => p.Date.DayNumber))
                {
                    Point pixel = new(XFor(point.Date), YFor(point.Value!.Value));
                    pixelPoints.Add(pixel);
                    if (!started)
                    {
                        ctx.BeginFigure(pixel, false, false);
                        started = true;
                    }
                    else
                    {
                        ctx.LineTo(pixel, true, true);
                    }
                }
            }
            geometry.Freeze();
            dc.DrawGeometry(null, pen, geometry);

            foreach (Point pixel in pixelPoints)
            {
                dc.DrawEllipse(ResolveBrush("Surface", Colors.White), pen, pixel, 3.4, 3.4);
            }
        }
    }

    private Brush ResolveBrush(string key, Color fallback)
    {
        Color color = fallback;
        if (TryFindResource(key) is SolidColorBrush themeBrush)
        {
            color = themeBrush.Color;
        }
        SolidColorBrush solid = new(color);
        solid.Freeze();
        return solid;
    }

    private static Color ParseColor(string hex)
    {
        try
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
        catch (FormatException)
        {
            return Color.FromRgb(0xB7, 0x9C, 0xE8);
        }
    }
}
