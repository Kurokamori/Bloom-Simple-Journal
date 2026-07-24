namespace Bloom.Models;

public sealed class Creature
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Blurb { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string SilhouettePath { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#F6C6D8";
    public CreatureRarity Rarity { get; set; } = CreatureRarity.Common;
    public int UnlockCost { get; set; } = 100;
    public bool IsUnlocked { get; set; }
    public DateTime? DiscoveredAt { get; set; }
    public int Affection { get; set; }
    public int TimesPet { get; set; }
    public DateTime? LastPetAt { get; set; }
    public int SortOrder { get; set; }
}
