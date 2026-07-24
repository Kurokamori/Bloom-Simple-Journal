namespace Bloom.Models;

public sealed class CoinTransaction
{
    public long Id { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? RefType { get; set; }
    public long? RefId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class DailyEarn
{
    public long Id { get; set; }
    public string EarnKey { get; set; } = string.Empty;
    public string EarnDate { get; set; } = string.Empty;
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
