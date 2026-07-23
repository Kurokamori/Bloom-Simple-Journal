using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class AssetRepository
{
    private readonly Database _database;

    public AssetRepository(Database database) => _database = database;

    public IReadOnlyList<Sticker> Stickers(bool unlockedOnly = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = unlockedOnly ? "WHERE is_unlocked = 1" : string.Empty;
        return connection.Query<Sticker>(
            $"SELECT * FROM stickers {filter} ORDER BY category, sort_order, name;").ToList();
    }

    public long InsertSticker(Sticker sticker)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO stickers (name, category, image_path, is_unlocked, unlock_cost, sort_order) " +
            "VALUES (@Name, @Category, @ImagePath, @IsUnlocked, @UnlockCost, @SortOrder); SELECT last_insert_rowid();",
            new
            {
                sticker.Name, Category = (int)sticker.Category, sticker.ImagePath,
                sticker.IsUnlocked, sticker.UnlockCost, sticker.SortOrder
            });
    }

    public void UnlockSticker(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE stickers SET is_unlocked = 1 WHERE id = @id;", new { id });
    }

    public IReadOnlyList<PageBackground> Backgrounds(bool unlockedOnly = false)
    {
        using SqliteConnection connection = _database.Open();
        string filter = unlockedOnly ? "WHERE is_unlocked = 1" : string.Empty;
        return connection.Query<PageBackground>(
            $"SELECT * FROM page_backgrounds {filter} ORDER BY sort_order, name;").ToList();
    }

    public PageBackground? BackgroundByKey(string key)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<PageBackground>(
            "SELECT * FROM page_backgrounds WHERE key = @key;", new { key });
    }

    public long InsertBackground(PageBackground background)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO page_backgrounds (key, name, kind, value, image_path, is_unlocked, unlock_cost, sort_order) " +
            "VALUES (@Key, @Name, @Kind, @Value, @ImagePath, @IsUnlocked, @UnlockCost, @SortOrder); " +
            "SELECT last_insert_rowid();",
            new
            {
                background.Key, background.Name, background.Kind, background.Value, background.ImagePath,
                background.IsUnlocked, background.UnlockCost, background.SortOrder
            });
    }

    public void UnlockBackground(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE page_backgrounds SET is_unlocked = 1 WHERE id = @id;", new { id });
    }
}
