using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bloom.Data.Repositories;

public sealed class SettingsRepository
{
    private readonly Database _database;

    public SettingsRepository(Database database) => _database = database;

    public string? Get(string key)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<string>(
            "SELECT value FROM app_settings WHERE key = @key;", new { key });
    }

    public string Get(string key, string fallback) => Get(key) ?? fallback;

    public bool GetBool(string key, bool fallback = false)
    {
        string? raw = Get(key);
        return raw is null ? fallback : raw == "1" || string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase);
    }

    public int GetInt(string key, int fallback = 0)
    {
        string? raw = Get(key);
        return raw is not null && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
            ? value
            : fallback;
    }

    public void Set(string key, string value)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "INSERT INTO app_settings (key, value) VALUES (@key, @value) " +
            "ON CONFLICT(key) DO UPDATE SET value = excluded.value;",
            new { key, value });
    }

    public void SetBool(string key, bool value) => Set(key, value ? "1" : "0");

    public void SetInt(string key, int value) => Set(key, value.ToString(CultureInfo.InvariantCulture));

    public Dictionary<string, string> All()
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<(string Key, string Value)>("SELECT key, value FROM app_settings;")
            .ToDictionary(row => row.Key, row => row.Value);
    }
}
