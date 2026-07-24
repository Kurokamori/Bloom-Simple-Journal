namespace Bloom.Controls;

public partial class GlyphIcon : UserControl
{
    public GlyphIcon()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
        nameof(Glyph),
        typeof(string),
        typeof(GlyphIcon),
        new PropertyMetadata(string.Empty));

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly DependencyProperty TintProperty = DependencyProperty.Register(
        nameof(Tint),
        typeof(Brush),
        typeof(GlyphIcon),
        new PropertyMetadata(Brushes.Black));

    public Brush Tint
    {
        get => (Brush)GetValue(TintProperty);
        set => SetValue(TintProperty, value);
    }

    public static readonly DependencyProperty GlyphSizeProperty = DependencyProperty.Register(
        nameof(GlyphSize),
        typeof(double),
        typeof(GlyphIcon),
        new PropertyMetadata(18.0));

    public double GlyphSize
    {
        get => (double)GetValue(GlyphSizeProperty);
        set => SetValue(GlyphSizeProperty, value);
    }
}
