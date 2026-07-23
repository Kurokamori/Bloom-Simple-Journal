using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class ConditionRepository
{
    private readonly Database _database;

    public ConditionRepository(Database database) => _database = database;

    public IReadOnlyList<Condition> All(bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "WHERE is_archived = 0";
        return connection.Query<Condition>(
            $"SELECT * FROM conditions {filter} ORDER BY sort_order, name;").ToList();
    }

    public Condition? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Condition>(
            "SELECT * FROM conditions WHERE id = @id;", new { id });
    }

    public long Insert(Condition condition)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO conditions (name, color, icon, notes, sort_order, is_archived, created_at) " +
            "VALUES (@Name, @Color, @Icon, @Notes, @SortOrder, @IsArchived, @CreatedAt); " +
            "SELECT last_insert_rowid();",
            new
            {
                condition.Name, condition.Color, condition.Icon, condition.Notes,
                condition.SortOrder, condition.IsArchived,
                CreatedAt = condition.CreatedAt == default ? DateTime.Now : condition.CreatedAt
            });
    }

    public void Update(Condition condition)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE conditions SET name = @Name, color = @Color, icon = @Icon, notes = @Notes, " +
            "sort_order = @SortOrder, is_archived = @IsArchived WHERE id = @Id;",
            new
            {
                condition.Id, condition.Name, condition.Color, condition.Icon,
                condition.Notes, condition.SortOrder, condition.IsArchived
            });
    }

    public void SetArchived(long id, bool archived)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE conditions SET is_archived = @archived WHERE id = @id;",
            new { id, archived });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM conditions WHERE id = @id;", new { id });
    }

    public IReadOnlyList<Symptom> Symptoms(long conditionId, bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "AND is_archived = 0";
        return connection.Query<Symptom>(
            $"SELECT * FROM symptoms WHERE condition_id = @conditionId {filter} ORDER BY sort_order, name;",
            new { conditionId }).ToList();
    }

    public IReadOnlyList<Symptom> AllSymptoms(bool includeArchived = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeArchived ? string.Empty : "WHERE is_archived = 0";
        return connection.Query<Symptom>(
            $"SELECT * FROM symptoms {filter} ORDER BY sort_order, name;").ToList();
    }

    public Symptom? FindSymptom(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Symptom>(
            "SELECT * FROM symptoms WHERE id = @id;", new { id });
    }

    public long InsertSymptom(Symptom symptom)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO symptoms (condition_id, name, track_scale, track_text, scale_min, scale_max, " +
            "widget, color, sort_order, is_archived, created_at) VALUES " +
            "(@ConditionId, @Name, @TrackScale, @TrackText, @ScaleMin, @ScaleMax, @Widget, @Color, " +
            "@SortOrder, @IsArchived, @CreatedAt); SELECT last_insert_rowid();",
            new
            {
                symptom.ConditionId, symptom.Name, symptom.TrackScale, symptom.TrackText,
                symptom.ScaleMin, symptom.ScaleMax, Widget = (int)symptom.Widget, symptom.Color,
                symptom.SortOrder, symptom.IsArchived,
                CreatedAt = symptom.CreatedAt == default ? DateTime.Now : symptom.CreatedAt
            });
    }

    public void UpdateSymptom(Symptom symptom)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE symptoms SET name = @Name, track_scale = @TrackScale, track_text = @TrackText, " +
            "scale_min = @ScaleMin, scale_max = @ScaleMax, widget = @Widget, color = @Color, " +
            "sort_order = @SortOrder, is_archived = @IsArchived WHERE id = @Id;",
            new
            {
                symptom.Id, symptom.Name, symptom.TrackScale, symptom.TrackText, symptom.ScaleMin,
                symptom.ScaleMax, Widget = (int)symptom.Widget, symptom.Color, symptom.SortOrder,
                symptom.IsArchived
            });
    }

    public void SetSymptomArchived(long id, bool archived)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE symptoms SET is_archived = @archived WHERE id = @id;",
            new { id, archived });
    }

    public void DeleteSymptom(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM symptoms WHERE id = @id;", new { id });
    }
}
