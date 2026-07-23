namespace Bloom.Models;

public sealed class Alter
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Pronouns { get; set; }
    public string? Role { get; set; }
    public string? Age { get; set; }
    public string Color { get; set; } = "#8FD3C7";
    public string? Description { get; set; }
    public string? AvatarPath { get; set; }
    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class FrontEvent
{
    public long Id { get; set; }
    public long AlterId { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public bool Fronted { get; set; } = true;
    public bool CoConscious { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int? Mood { get; set; }
    public string? Opinion { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
