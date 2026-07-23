namespace Bloom.Models;

public sealed class Sticker
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public StickerCategory Category { get; set; } = StickerCategory.Cute;
    public string ImagePath { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; } = true;
    public int UnlockCost { get; set; }
    public int SortOrder { get; set; }
}

public sealed class PageBackground
{
    public long Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = "color";
    public string Value { get; set; } = "#FBF3EA";
    public string? ImagePath { get; set; }
    public bool IsUnlocked { get; set; } = true;
    public int UnlockCost { get; set; }
    public int SortOrder { get; set; }
}
