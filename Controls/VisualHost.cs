using System.Windows;
using System.Windows.Media;

namespace Bloom.Controls;

public sealed class VisualHost : FrameworkElement
{
    private Visual? _child;

    public Visual? Child
    {
        get => _child;
        set
        {
            if (ReferenceEquals(_child, value))
            {
                return;
            }
            if (_child is not null)
            {
                RemoveVisualChild(_child);
            }
            _child = value;
            if (_child is not null)
            {
                AddVisualChild(_child);
            }
            InvalidateMeasure();
            InvalidateVisual();
        }
    }

    protected override int VisualChildrenCount => _child is null ? 0 : 1;

    protected override Visual GetVisualChild(int index)
    {
        if (_child is null || index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return _child;
    }
}
