using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class SymptomLogRepository
{
    private readonly Database _database;

    public SymptomLogRepository(Database database) => _database = database;

    public IReadOnlyList<SymptomLog> ForDate(string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<SymptomLog>(
            "SELECT * FROM symptom_logs WHERE entry_date = @entryDate ORDER BY id;",
            new { entryDate }).ToList();
    }

    public SymptomLog? ForSymptomOnDate(long symptomId, string entryDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<SymptomLog>(
            "SELECT * FROM symptom_logs WHERE symptom_id = @symptomId AND entry_date = @entryDate " +
            "ORDER BY id DESC LIMIT 1;", new { symptomId, entryDate });
    }

    public long Upsert(long symptomId, string entryDate, int? scaleValue, string? textValue)
    {
        using SqliteConnection connection = _database.Open();
        SymptomLog? existing = connection.QuerySingleOrDefault<SymptomLog>(
            "SELECT * FROM symptom_logs WHERE symptom_id = @symptomId AND entry_date = @entryDate " +
            "ORDER BY id DESC LIMIT 1;", new { symptomId, entryDate });

        if (existing is not null)
        {
            connection.Execute(
                "UPDATE symptom_logs SET scale_value = @scaleValue, text_value = @textValue WHERE id = @id;",
                new { existing.Id, scaleValue, textValue });
            return existing.Id;
        }

        return connection.ExecuteScalar<long>(
            "INSERT INTO symptom_logs (symptom_id, entry_date, scale_value, text_value, created_at) " +
            "VALUES (@symptomId, @entryDate, @scaleValue, @textValue, @createdAt); SELECT last_insert_rowid();",
            new { symptomId, entryDate, scaleValue, textValue, createdAt = DateTime.Now });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM symptom_logs WHERE id = @id;", new { id });
    }

    public IReadOnlyList<SymptomLog> History(long symptomId, string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<SymptomLog>(
            "SELECT * FROM symptom_logs WHERE symptom_id = @symptomId " +
            "AND entry_date >= @startDate AND entry_date <= @endDate ORDER BY entry_date;",
            new { symptomId, startDate, endDate }).ToList();
    }

    public IReadOnlyList<SymptomLog> HistoryRange(string startDate, string endDate)
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<SymptomLog>(
            "SELECT * FROM symptom_logs WHERE entry_date >= @startDate AND entry_date <= @endDate " +
            "ORDER BY entry_date;", new { startDate, endDate }).ToList();
    }
}
