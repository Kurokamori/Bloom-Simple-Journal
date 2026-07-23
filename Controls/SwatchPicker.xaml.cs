using System.Windows;
using System.Windows.Controls;

namespace Bloom.Controls;

public partial class SwatchPicker : UserControl
{
    public static readonly DependencyProperty SelectedHexProperty =
        DependencyProperty.Register(
            nameof(SelectedHex), typeof(string), typeof(SwatchPicker),
            new FrameworkPropertyMetadata("#C9A7EB", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string SelectedHex
    {
        get => (string)GetValue(SelectedHexProperty);
        set => SetValue(SelectedHexProperty, value);
    }

    public static IReadOnlyList<string> Palette { get; } = new[]
    {
        "#C9A7EB", "#B79CE8", "#F48FB1", "#F6A9C0", "#F5A66E", "#F2C879",
        "#8FBF7F", "#8FD3C7", "#8FB8DE", "#A7C7E7", "#E88A9A", "#D8A7B1",
        "#A9C7A0", "#C4C0B6"
    };

    public SwatchPicker()
    {
        InitializeComponent();
        PaletteItems.ItemsSource = Palette;
    }

    private void OnSwatchClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string hex })
        {
            SelectedHex = hex;
        }
    }
}
