using Bloom.Common;
using Bloom.Data.Repositories;
using Bloom.Models;

namespace Bloom.Services;

public sealed class CoinEventArgs : EventArgs
{
    public int Amount { get; init; }
    public int Balance { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed class EconomyService
{
    private readonly EconomyRepository _economy;
    private readonly SettingsRepository _settings;

    public EconomyService(EconomyRepository economy, SettingsRepository settings)
    {
        _economy = economy;
        _settings = settings;
    }

    public event EventHandler<CoinEventArgs>? BalanceChanged;
    public event EventHandler<CoinEventArgs>? Earned;

    public string CurrencyName => _settings.Get("currency.name", "Petals");

    public string CurrencyGlyph => _settings.Get("currency.glyph", "✿");

    public int Balance => _economy.Balance();

    public int LifetimeEarned => _economy.LifetimeEarned();

    public int RewardOncePerDay(string key, int amount, string reason, string? date = null)
    {
        date ??= Dates.Today();
        if (_economy.TryClaimDaily(key, date, amount, reason))
        {
            int balance = _economy.Balance();
            RaiseEarned(amount, balance, reason);
            return amount;
        }
        return 0;
    }

    public bool HasEarnedToday(string key, string? date = null) =>
        _economy.HasClaimedDaily(key, date ?? Dates.Today());

    public int Grant(int amount, string reason, string? refType = null, long? refId = null)
    {
        int balance = _economy.Adjust(amount, reason, refType, refId);
        if (amount > 0)
        {
            RaiseEarned(amount, balance, reason);
        }
        else
        {
            BalanceChanged?.Invoke(this, new CoinEventArgs { Amount = amount, Balance = balance, Reason = reason });
        }
        return balance;
    }

    public bool TrySpend(int amount, string reason, string? refType = null, long? refId = null)
    {
        if (amount <= 0 || _economy.Balance() < amount)
        {
            return false;
        }
        int balance = _economy.Adjust(-amount, reason, refType, refId);
        BalanceChanged?.Invoke(this, new CoinEventArgs { Amount = -amount, Balance = balance, Reason = reason });
        return true;
    }

    public IReadOnlyList<CoinTransaction> History(int limit = 60) => _economy.RecentTransactions(limit);

    private void RaiseEarned(int amount, int balance, string reason)
    {
        CoinEventArgs args = new() { Amount = amount, Balance = balance, Reason = reason };
        Earned?.Invoke(this, args);
        BalanceChanged?.Invoke(this, args);
    }
}
