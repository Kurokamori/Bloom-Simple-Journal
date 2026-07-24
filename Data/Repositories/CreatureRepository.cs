using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class CreatureRepository
{
    private readonly Database _database;

    public CreatureRepository(Database database) => _database = database;

    public IReadOnlyList<Creature> All()
    {
        using SqliteConnection connection = _database.Open();
        return connection.Query<Creature>("SELECT * FROM creatures ORDER BY sort_order, unlock_cost;").ToList();
    }

    public Creature? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<Creature>("SELECT * FROM creatures WHERE id = @id;", new { id });
    }

    public long Insert(Creature creature)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO creatures (name, species, blurb, image_path, silhouette_path, accent_color, rarity, " +
            "unlock_cost, is_unlocked, discovered_at, affection, times_pet, last_pet_at, sort_order) " +
            "VALUES (@Name, @Species, @Blurb, @ImagePath, @SilhouettePath, @AccentColor, @Rarity, @UnlockCost, " +
            "@IsUnlocked, @DiscoveredAt, @Affection, @TimesPet, @LastPetAt, @SortOrder); SELECT last_insert_rowid();",
            new
            {
                creature.Name, creature.Species, creature.Blurb, creature.ImagePath, creature.SilhouettePath,
                creature.AccentColor, Rarity = (int)creature.Rarity, creature.UnlockCost, creature.IsUnlocked,
                creature.DiscoveredAt, creature.Affection, creature.TimesPet, creature.LastPetAt, creature.SortOrder
            });
    }

    public void Unlock(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE creatures SET is_unlocked = 1, discovered_at = @now WHERE id = @id AND is_unlocked = 0;",
            new { id, now = DateTime.Now });
    }

    public void RecordPet(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE creatures SET times_pet = times_pet + 1, " +
            "affection = MIN(100, affection + 1), last_pet_at = @now WHERE id = @id;",
            new { id, now = DateTime.Now });
    }

    public int CountUnlocked()
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<int>("SELECT COUNT(1) FROM creatures WHERE is_unlocked = 1;");
    }
}
