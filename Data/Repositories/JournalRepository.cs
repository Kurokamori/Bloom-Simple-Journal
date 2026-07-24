using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class JournalRepository
{
    private readonly Database _database;

    public JournalRepository(Database database) => _database = database;

    public IReadOnlyList<Checkin> Checkins(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<Checkin>(
            "SELECT * FROM checkins WHERE entry_date = @entryDate ORDER BY id;", new { entryDate }).ToList();
    }

    public long InsertCheckin(Checkin checkin)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO checkins (entry_date, mood, energy, productivity, note, created_at) " +
            "VALUES (@EntryDate, @Mood, @Energy, @Productivity, @Note, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                checkin.EntryDate, checkin.Mood, checkin.Energy, checkin.Productivity, checkin.Note,
                CreatedAt = checkin.CreatedAt == default ? DateTime.Now : checkin.CreatedAt
            });
    }

    public void UpdateCheckin(Checkin checkin)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE checkins SET mood = @Mood, energy = @Energy, productivity = @Productivity, " +
            "note = @Note WHERE id = @Id;",
            new { checkin.Id, checkin.Mood, checkin.Energy, checkin.Productivity, checkin.Note });
    }

    public void DeleteCheckin(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM checkins WHERE id = @id;", new { id });
    }

    public IReadOnlyList<Checkin> CheckinsInRange(string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<Checkin>(
            "SELECT * FROM checkins WHERE entry_date >= @startDate AND entry_date <= @endDate " +
            "ORDER BY entry_date, id;", new { startDate, endDate }).ToList();
    }

    public IReadOnlyList<WotCheckin> WotCheckins(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<WotCheckin>(
            "SELECT * FROM wot_checkins WHERE entry_date = @entryDate ORDER BY id;", new { entryDate }).ToList();
    }

    public long InsertWot(WotCheckin checkin)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO wot_checkins (entry_date, zone, intensity, cause, helped, note, created_at) " +
            "VALUES (@EntryDate, @Zone, @Intensity, @Cause, @Helped, @Note, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                checkin.EntryDate, Zone = (int)checkin.Zone, checkin.Intensity, checkin.Cause,
                checkin.Helped, checkin.Note,
                CreatedAt = checkin.CreatedAt == default ? DateTime.Now : checkin.CreatedAt
            });
    }

    public void UpdateWot(WotCheckin checkin)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE wot_checkins SET zone = @Zone, intensity = @Intensity, cause = @Cause, " +
            "helped = @Helped, note = @Note WHERE id = @Id;",
            new { checkin.Id, Zone = (int)checkin.Zone, checkin.Intensity, checkin.Cause, checkin.Helped, checkin.Note });
    }

    public void DeleteWot(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM wot_checkins WHERE id = @id;", new { id });
    }

    public IReadOnlyList<WotCheckin> WotInRange(string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<WotCheckin>(
            "SELECT * FROM wot_checkins WHERE entry_date >= @startDate AND entry_date <= @endDate " +
            "ORDER BY entry_date, id;", new { startDate, endDate }).ToList();
    }

    public IReadOnlyList<FoodLog> Foods(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<FoodLog>(
            "SELECT * FROM food_logs WHERE entry_date = @entryDate ORDER BY id;", new { entryDate }).ToList();
    }

    public long InsertFood(FoodLog food)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO food_logs (entry_date, name, note, meal_time, created_at) " +
            "VALUES (@EntryDate, @Name, @Note, @MealTime, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                food.EntryDate, food.Name, food.Note, food.MealTime,
                CreatedAt = food.CreatedAt == default ? DateTime.Now : food.CreatedAt
            });
    }

    public void UpdateFood(FoodLog food)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE food_logs SET name = @Name, note = @Note, meal_time = @MealTime WHERE id = @Id;",
            new { food.Id, food.Name, food.Note, food.MealTime });
    }

    public void DeleteFood(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM food_logs WHERE id = @id;", new { id });
    }

    public JournalBody GetBody(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        JournalBody? body = connection.QuerySingleOrDefault<JournalBody>(
            "SELECT * FROM journal_bodies WHERE entry_date = @entryDate;", new { entryDate });
        return body ?? new JournalBody { EntryDate = entryDate };
    }

    public void SaveBody(JournalBody body)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "INSERT INTO journal_bodies (entry_date, title, content, updated_at) " +
            "VALUES (@EntryDate, @Title, @Content, @UpdatedAt) " +
            "ON CONFLICT(entry_date) DO UPDATE SET title = excluded.title, content = excluded.content, " +
            "updated_at = excluded.updated_at;",
            new { body.EntryDate, body.Title, body.Content, UpdatedAt = DateTime.Now });
    }

    public IReadOnlyList<JournalPage> Pages(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<JournalPage>(
            "SELECT * FROM journal_pages WHERE entry_date = @entryDate ORDER BY page_index;",
            new { entryDate }).ToList();
    }

    public void SavePage(JournalPage page)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "INSERT INTO journal_pages (entry_date, page_index, background_id, font_family, decor_json, updated_at) " +
            "VALUES (@EntryDate, @PageIndex, @BackgroundId, @FontFamily, @DecorJson, @UpdatedAt) " +
            "ON CONFLICT(entry_date, page_index) DO UPDATE SET background_id = excluded.background_id, " +
            "font_family = excluded.font_family, decor_json = excluded.decor_json, updated_at = excluded.updated_at;",
            new
            {
                page.EntryDate, page.PageIndex, page.BackgroundId, page.FontFamily, page.DecorJson,
                UpdatedAt = DateTime.Now
            });
    }

    public void DeletePage(string entryDate, int pageIndex)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM journal_pages WHERE entry_date = @entryDate AND page_index = @pageIndex;",
            new { entryDate, pageIndex });
    }

    public IReadOnlyList<string> DatesWithContent()
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<string>(
            "SELECT entry_date FROM (" +
            "  SELECT entry_date FROM journal_bodies WHERE content <> '' OR title <> '' " +
            "  UNION SELECT entry_date FROM checkins " +
            "  UNION SELECT entry_date FROM wot_checkins " +
            "  UNION SELECT entry_date FROM food_logs " +
            "  UNION SELECT entry_date FROM symptom_logs " +
            "  UNION SELECT entry_date FROM front_events" +
            ") GROUP BY entry_date ORDER BY entry_date DESC;").ToList();
    }

    public int CountEntriesForDate(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>(
            "SELECT " +
            "(SELECT COUNT(1) FROM checkins WHERE entry_date = @entryDate) + " +
            "(SELECT COUNT(1) FROM wot_checkins WHERE entry_date = @entryDate) + " +
            "(SELECT COUNT(1) FROM food_logs WHERE entry_date = @entryDate) + " +
            "(SELECT COUNT(1) FROM symptom_logs WHERE entry_date = @entryDate) + " +
            "(SELECT COUNT(1) FROM front_events WHERE entry_date = @entryDate);",
            new { entryDate });
    }
}
