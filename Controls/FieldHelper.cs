namespace Bloom.Controls;

public static class FieldHelper
{
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached(
            "Placeholder", typeof(string), typeof(FieldHelper), new PropertyMetadata(string.Empty));

    public static string GetPlaceholder(DependencyObject element) =>
        (string)element.GetValue(PlaceholderProperty);

    public static void SetPlaceholder(DependencyObject element, string value) =>
        element.SetValue(PlaceholderProperty, value);

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(FieldHelper),
            new PropertyMetadata(new CornerRadius(12)));

    public static CornerRadius GetCornerRadius(DependencyObject element) =>
        (CornerRadius)element.GetValue(CornerRadiusProperty);

    public static void SetCornerRadius(DependencyObject element, CornerRadius value) =>
        element.SetValue(CornerRadiusProperty, value);
}
