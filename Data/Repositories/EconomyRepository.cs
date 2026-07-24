using Bloom.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Bloom.Data.Repositories;

public sealed class EconomyRepository
{
    private readonly Database _database;

    public EconomyRepository(Database database) => _database = database;

    public int Balance()
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>("SELECT balance FROM wallet WHERE id = 1;");
    }

    public int LifetimeEarned()
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>("SELECT lifetime_earned FROM wallet WHERE id = 1;");
    }

    public int Adjust(int amount, string reason, string? refType = null, long? refId = null)
    {
        using SqliteConnection connection = _database.Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        connection.Execute(
            "UPDATE wallet SET balance = balance + @amount, " +
            "lifetime_earned = lifetime_earned + CASE WHEN @amount > 0 THEN @amount ELSE 0 END WHERE id = 1;",
            new { amount }, transaction);

        connection.Execute(
            "INSERT INTO coin_transactions (amount, reason, ref_type, ref_id, created_at) " +
            "VALUES (@amount, @reason, @refType, @refId, @createdAt);",
            new { amount, reason, refType, refId, createdAt = DateTime.Now }, transaction);

        int balance = connection.ExecuteScalar<int>(
            "SELECT balance FROM wallet WHERE id = 1;", transaction);
        transaction.Commit();
        return balance;
    }

    public bool TryClaimDaily(string earnKey, string earnDate, int amount, string reason)
    {
        using SqliteConnection connection = _database.Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        int inserted = connection.Execute(
            "INSERT OR IGNORE INTO daily_earn (earn_key, earn_date, amount, created_at) " +
            "VALUES (@earnKey, @earnDate, @amount, @createdAt);",
            new { earnKey, earnDate, amount, createdAt = DateTime.Now }, transaction);

        if (inserted == 0)
        {
            transaction.Rollback();
            return false;
        }

        connection.Execute(
            "UPDATE wallet SET balance = balance + @amount, lifetime_earned = lifetime_earned + @amount WHERE id = 1;",
            new { amount }, transaction);
        connection.Execute(
            "INSERT INTO coin_transactions (amount, reason, ref_type, ref_id, created_at) " +
            "VALUES (@amount, @reason, 'daily', NULL, @createdAt);",
            new { amount, reason, createdAt = DateTime.Now }, transaction);

        transaction.Commit();
        return true;
    }

    public bool HasClaimedDaily(string earnKey, string earnDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "SELECT COUNT(1) FROM daily_earn WHERE earn_key = @earnKey AND earn_date = @earnDate;",
            new { earnKey, earnDate }) > 0;
    }

    public IReadOnlyList<CoinTransaction> RecentTransactions(int limit = 60)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<CoinTransaction>(
            "SELECT id AS Id, amount AS Amount, reason AS Reason, ref_type AS RefType, " +
            "ref_id AS RefId, created_at AS CreatedAt FROM coin_transactions " +
            "ORDER BY id DESC LIMIT @limit;", new { limit }).ToList();
    }
}
