using Bloom.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Bloom.Data.Repositories;

public sealed class AlterRepository
{
    private readonly Database _database;

    public AlterRepository(Database database) => _database = database;

    public IReadOnlyList<Alter> All(bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "WHERE is_archived = 0";
        return connection.Query<Alter>(
            $"SELECT * FROM alters {filter} ORDER BY sort_order, name;").ToList();
    }

    public Alter? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Alter>("SELECT * FROM alters WHERE id = @id;", new { id });
    }

    public long Insert(Alter alter)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO alters (name, pronouns, role, age, color, description, avatar_path, sort_order, " +
            "is_archived, created_at) VALUES (@Name, @Pronouns, @Role, @Age, @Color, @Description, " +
            "@AvatarPath, @SortOrder, @IsArchived, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                alter.Name,
                alter.Pronouns,
                alter.Role,
                alter.Age,
                alter.Color,
                alter.Description,
                alter.AvatarPath,
                alter.SortOrder,
                alter.IsArchived,
                CreatedAt = alter.CreatedAt == default ? DateTime.Now : alter.CreatedAt
            });
    }

    public void Update(Alter alter)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE alters SET name = @Name, pronouns = @Pronouns, role = @Role, age = @Age, " +
            "color = @Color, description = @Description, avatar_path = @AvatarPath, " +
            "sort_order = @SortOrder, is_archived = @IsArchived WHERE id = @Id;",
            new
            {
                alter.Id,
                alter.Name,
                alter.Pronouns,
                alter.Role,
                alter.Age,
                alter.Color,
                alter.Description,
                alter.AvatarPath,
                alter.SortOrder,
                alter.IsArchived
            });
    }

    public void SetArchived(long id, bool archived)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE alters SET is_archived = @archived WHERE id = @id;", new { id, archived });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM alters WHERE id = @id;", new { id });
    }

    public IReadOnlyList<FrontEvent> EventsForDate(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<FrontEvent>(
            "SELECT * FROM front_events WHERE entry_date = @entryDate ORDER BY id;",
            new { entryDate }).ToList();
    }

    public long InsertEvent(FrontEvent frontEvent)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO front_events (alter_id, entry_date, fronted, co_conscious, start_time, end_time, " +
            "mood, opinion, note, created_at) VALUES (@AlterId, @EntryDate, @Fronted, @CoConscious, " +
            "@StartTime, @EndTime, @Mood, @Opinion, @Note, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                frontEvent.AlterId,
                frontEvent.EntryDate,
                frontEvent.Fronted,
                frontEvent.CoConscious,
                frontEvent.StartTime,
                frontEvent.EndTime,
                frontEvent.Mood,
                frontEvent.Opinion,
                frontEvent.Note,
                CreatedAt = frontEvent.CreatedAt == default ? DateTime.Now : frontEvent.CreatedAt
            });
    }

    public void UpdateEvent(FrontEvent frontEvent)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE front_events SET alter_id = @AlterId, fronted = @Fronted, co_conscious = @CoConscious, " +
            "start_time = @StartTime, end_time = @EndTime, mood = @Mood, opinion = @Opinion, note = @Note " +
            "WHERE id = @Id;",
            new
            {
                frontEvent.Id,
                frontEvent.AlterId,
                frontEvent.Fronted,
                frontEvent.CoConscious,
                frontEvent.StartTime,
                frontEvent.EndTime,
                frontEvent.Mood,
                frontEvent.Opinion,
                frontEvent.Note
            });
    }

    public void DeleteEvent(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM front_events WHERE id = @id;", new { id });
    }

    public IReadOnlyList<FrontEvent> EventsForAlter(long alterId, string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<FrontEvent>(
            "SELECT * FROM front_events WHERE alter_id = @alterId AND entry_date >= @startDate " +
            "AND entry_date <= @endDate ORDER BY entry_date;",
            new { alterId, startDate, endDate }).ToList();
    }

    public IReadOnlyList<FrontEvent> EventsInRange(string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<FrontEvent>(
            "SELECT * FROM front_events WHERE entry_date >= @startDate AND entry_date <= @endDate " +
            "ORDER BY entry_date;", new { startDate, endDate }).ToList();
    }
}
