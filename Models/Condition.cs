namespace Bloom.Models;

public sealed class Condition
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#C9A7EB";
    public string Icon { get; set; } = "🌸";
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class Symptom
{
    public long Id { get; set; }
    public long ConditionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool TrackScale { get; set; } = true;
    public bool TrackText { get; set; } = true;
    public int ScaleMin { get; set; } = 1;
    public int ScaleMax { get; set; } = 10;
    public SymptomWidget Widget { get; set; } = SymptomWidget.Standard;
    public string Color { get; set; } = "#C9A7EB";
    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class SymptomLog
{
    public long Id { get; set; }
    public long SymptomId { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public int? ScaleValue { get; set; }
    public string? TextValue { get; set; }
    public DateTime CreatedAt { get; set; }
}
