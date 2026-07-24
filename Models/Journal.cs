namespace Bloom.Models;

public sealed class Checkin
{
    public long Id { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public int Mood { get; set; } = 5;
    public int Energy { get; set; } = 5;
    public int Productivity { get; set; } = 5;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class WotCheckin
{
    public long Id { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public WotZone Zone { get; set; } = WotZone.Optimal;
    public int Intensity { get; set; } = 5;
    public string? Cause { get; set; }
    public string? Helped { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class FoodLog
{
    public long Id { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? MealTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class JournalBody
{
    public long Id { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public sealed class JournalPage
{
    public long Id { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public int PageIndex { get; set; }
    public string BackgroundId { get; set; } = "paper-cream";
    public string FontFamily { get; set; } = "Segoe UI";
    public string DecorJson { get; set; } = "[]";
    public DateTime UpdatedAt { get; set; }
}
