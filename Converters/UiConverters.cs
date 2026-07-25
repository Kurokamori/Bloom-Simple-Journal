using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Bloom.Controls;
using Bloom.Models;
using Bloom.Services;

namespace Bloom.Converters;

public sealed class StickerContentConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Sticker sticker)
        {
            string? path = ArtPaths.Resolve(sticker.ImagePath);
            System.Windows.Media.Imaging.BitmapImage? bitmap = Images.LoadFrozen(path);
            if (bitmap is not null)
            {
                return new Image { Source = bitmap, Width = 30, Height = 30, Stretch = Stretch.Uniform };
            }
            GlyphIcon icon = new()
            {
                Glyph = StickerArt.Glyph(sticker),
                GlyphSize = 22,
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            icon.SetResourceReference(GlyphIcon.TintProperty, "Primary");
            return icon;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b ? !b : true;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b ? !b : false;
}

public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        if (IsInverted(parameter))
        {
            flag = !flag;
        }
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility v && v == Visibility.Visible;

    private static bool IsInverted(object? parameter) =>
        parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase);
}

public sealed class StringEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool empty = value is null || string.IsNullOrEmpty(value.ToString());
        bool showWhenEmpty = !(parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase));
        bool visible = showWhenEmpty ? empty : !empty;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isNull = value is null || (value is string s && string.IsNullOrEmpty(s));
        bool showWhenNull = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        bool visible = showWhenNull ? isNull : !isNull;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        int count = value switch
        {
            int i => i,
            System.Collections.ICollection c => c.Count,
            _ => 0
        };
        bool showWhenEmpty = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        bool visible = showWhenEmpty ? count == 0 : count > 0;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not null && parameter is not null &&
        value.ToString()!.Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b && parameter is not null)
        {
            return Enum.Parse(targetType, parameter.ToString()!);
        }
        return Binding.DoNothing;
    }
}

public sealed class EqualityToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Equals(value?.ToString(), parameter?.ToString());

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && b && parameter is not null ? parameter : Binding.DoNothing;
}

public sealed class EqualsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool equal = Equals(value?.ToString(), parameter?.ToString());
        return equal ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            }
            catch (FormatException)
            {
            }
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color.ToString();
        }
        return Binding.DoNothing;
    }
}

public sealed class HexToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(hex);
            }
            catch (FormatException)
            {
            }
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Color color ? color.ToString() : Binding.DoNothing;
}

public sealed class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && b ? 1.0 : 0.42;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class MoodGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        int v = value is int i ? i : 5;
        return Bloom.ViewModels.Journal.MoodGlyphs.ForValue(v);
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public sealed class ScaleToWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length >= 2 &&
            values[0] is double fraction &&
            values[1] is double available)
        {
            return Math.Max(0, Math.Min(1, fraction)) * available;
        }
        return 0d;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        Array.Empty<object>();
}

public sealed class MathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        double number = System.Convert.ToDouble(value, culture);
        if (parameter is string expr && expr.Length > 1)
        {
            char op = expr[0];
            if (double.TryParse(expr[1..], NumberStyles.Any, CultureInfo.InvariantCulture, out double operand))
            {
                return op switch
                {
                    '*' => number * operand,
                    '/' => operand == 0 ? number : number / operand,
                    '+' => number + operand,
                    '-' => number - operand,
                    _ => number
                };
            }
        }
        return number;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
