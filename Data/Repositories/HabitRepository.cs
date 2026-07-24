using Bloom.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Bloom.Data.Repositories;

public sealed class HabitRepository
{
    private readonly Database _database;

    public HabitRepository(Database database) => _database = database;

    public IReadOnlyList<Habit> All(bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "WHERE is_archived = 0";
        return connection.Query<Habit>(
            $"SELECT * FROM habits {filter} ORDER BY sort_order, name;").ToList();
    }

    public Habit? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Habit>("SELECT * FROM habits WHERE id = @id;", new { id });
    }

    public long Insert(Habit habit)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO habits (name, icon, color, cadence, interval_days, target_per_period, coin_reward, " +
            "note, sort_order, is_archived, created_at) VALUES (@Name, @Icon, @Color, @Cadence, @IntervalDays, " +
            "@TargetPerPeriod, @CoinReward, @Note, @SortOrder, @IsArchived, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                habit.Name,
                habit.Icon,
                habit.Color,
                Cadence = (int)habit.Cadence,
                habit.IntervalDays,
                habit.TargetPerPeriod,
                habit.CoinReward,
                habit.Note,
                habit.SortOrder,
                habit.IsArchived,
                CreatedAt = habit.CreatedAt == default ? DateTime.Now : habit.CreatedAt
            });
    }

    public void Update(Habit habit)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE habits SET name = @Name, icon = @Icon, color = @Color, cadence = @Cadence, " +
            "interval_days = @IntervalDays, target_per_period = @TargetPerPeriod, coin_reward = @CoinReward, " +
            "note = @Note, sort_order = @SortOrder, is_archived = @IsArchived WHERE id = @Id;",
            new
            {
                habit.Id,
                habit.Name,
                habit.Icon,
                habit.Color,
                Cadence = (int)habit.Cadence,
                habit.IntervalDays,
                habit.TargetPerPeriod,
                habit.CoinReward,
                habit.Note,
                habit.SortOrder,
                habit.IsArchived
            });
    }

    public void SetArchived(long id, bool archived)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE habits SET is_archived = @archived WHERE id = @id;", new { id, archived });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM habits WHERE id = @id;", new { id });
    }

    public int CountOnDate(long habitId, string logDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>(
            "SELECT COALESCE(SUM(count), 0) FROM habit_logs WHERE habit_id = @habitId AND log_date = @logDate;",
            new { habitId, logDate });
    }

    public int CountInRange(long habitId, string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>(
            "SELECT COALESCE(SUM(count), 0) FROM habit_logs WHERE habit_id = @habitId " +
            "AND log_date >= @startDate AND log_date <= @endDate;",
            new { habitId, startDate, endDate });
    }

    public long AddLog(long habitId, string logDate, int count = 1)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO habit_logs (habit_id, log_date, count, created_at) " +
            "VALUES (@habitId, @logDate, @count, @createdAt); SELECT last_insert_rowid();",
            new { habitId, logDate, count, createdAt = DateTime.Now });
    }

    public void RemoveOneLog(long habitId, string logDate)
    {
        using SqliteConnection connection = _database.Open();
        long? latest = connection.QuerySingleOrDefault<long?>(
            "SELECT id FROM habit_logs WHERE habit_id = @habitId AND log_date = @logDate ORDER BY id DESC LIMIT 1;",
            new { habitId, logDate });
        if (latest is not null)
        {
            connection.Execute("DELETE FROM habit_logs WHERE id = @id;", new { id = latest });
        }
    }

    public IReadOnlyList<HabitLog> LogsInRange(long habitId, string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<HabitLog>(
            "SELECT * FROM habit_logs WHERE habit_id = @habitId AND log_date >= @startDate " +
            "AND log_date <= @endDate ORDER BY log_date;", new { habitId, startDate, endDate }).ToList();
    }
}
