namespace Bloom.Models;

public sealed class ChartPoint
{
    public DateOnly Date { get; set; }
    public double? Value { get; set; }
}

public sealed class ChartSeries
{
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#B79CE8";
    public List<ChartPoint> Points { get; set; } = new();
}
