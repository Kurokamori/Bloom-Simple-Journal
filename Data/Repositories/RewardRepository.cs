using Bloom.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Bloom.Data.Repositories;

public sealed class RewardRepository
{
    private readonly Database _database;

    public RewardRepository(Database database) => _database = database;

    public IReadOnlyList<Reward> All(bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "WHERE is_archived = 0";
        return connection.Query<Reward>(
            $"SELECT * FROM rewards {filter} ORDER BY sort_order, cost;").ToList();
    }

    public Reward? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Reward>("SELECT * FROM rewards WHERE id = @id;", new { id });
    }

    public long Insert(Reward reward)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO rewards (name, description, cost, icon, category, is_custom, is_archived, sort_order, created_at) " +
            "VALUES (@Name, @Description, @Cost, @Icon, @Category, @IsCustom, @IsArchived, @SortOrder, @CreatedAt); " +
            "SELECT last_insert_rowid();",
            new
            {
                reward.Name,
                reward.Description,
                reward.Cost,
                reward.Icon,
                Category = (int)reward.Category,
                reward.IsCustom,
                reward.IsArchived,
                reward.SortOrder,
                CreatedAt = reward.CreatedAt == default ? DateTime.Now : reward.CreatedAt
            });
    }

    public void Update(Reward reward)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE rewards SET name = @Name, description = @Description, cost = @Cost, icon = @Icon, " +
            "category = @Category, is_custom = @IsCustom, is_archived = @IsArchived, sort_order = @SortOrder WHERE id = @Id;",
            new
            {
                reward.Id,
                reward.Name,
                reward.Description,
                reward.Cost,
                reward.Icon,
                Category = (int)reward.Category,
                reward.IsCustom,
                reward.IsArchived,
                reward.SortOrder
            });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM rewards WHERE id = @id;", new { id });
    }

    public long InsertRedemption(RewardRedemption redemption)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO reward_redemptions (reward_id, reward_name, cost, redeemed_at) " +
            "VALUES (@RewardId, @RewardName, @Cost, @RedeemedAt); SELECT last_insert_rowid();",
            new
            {
                redemption.RewardId,
                redemption.RewardName,
                redemption.Cost,
                RedeemedAt = redemption.RedeemedAt == default ? DateTime.Now : redemption.RedeemedAt
            });
    }

    public IReadOnlyList<RewardRedemption> RecentRedemptions(int limit = 40)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<RewardRedemption>(
            "SELECT * FROM reward_redemptions ORDER BY id DESC LIMIT @limit;", new { limit }).ToList();
    }
}
