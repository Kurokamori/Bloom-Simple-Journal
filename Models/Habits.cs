namespace Bloom.Models;

public sealed class Habit
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "🌱";
    public string Color { get; set; } = "#A7D7C5";
    public HabitCadence Cadence { get; set; } = HabitCadence.Daily;
    public int IntervalDays { get; set; } = 1;
    public int TargetPerPeriod { get; set; } = 1;
    public int CoinReward { get; set; } = 5;
    public string? Note { get; set; }
    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class HabitLog
{
    public long Id { get; set; }
    public long HabitId { get; set; }
    public string LogDate { get; set; } = string.Empty;
    public int Count { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
}

public sealed class TaskItem
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? DueDate { get; set; }
    public bool IsDone { get; set; }
    public DateTime? DoneAt { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public int CoinReward { get; set; } = 8;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
