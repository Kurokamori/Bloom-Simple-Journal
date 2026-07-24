namespace Bloom.Models;

public sealed class Reward
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Cost { get; set; } = 25;
    public string Icon { get; set; } = "🎁";
    public RewardCategory Category { get; set; } = RewardCategory.Treat;
    public bool IsCustom { get; set; }
    public bool IsArchived { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class RewardRedemption
{
    public long Id { get; set; }
    public long RewardId { get; set; }
    public string RewardName { get; set; } = string.Empty;
    public int Cost { get; set; }
    public DateTime RedeemedAt { get; set; }
}
