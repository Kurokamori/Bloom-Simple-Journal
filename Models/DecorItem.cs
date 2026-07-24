namespace Bloom.Models;

public sealed class DecorItem
{
    public int Page { get; set; }
    public string Ref { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Scale { get; set; } = 1.0;
    public double Rotation { get; set; }
    public int Z { get; set; }
}

public sealed class PageComposition
{
    public string BackgroundKey { get; set; } = "paper-cream";
    public string FontFamily { get; set; } = "Segoe UI";
    public List<DecorItem> Items { get; set; } = new();
}
