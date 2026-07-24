using Dapper;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Bloom.Data;

public sealed class Database
{
    private readonly string _connectionString;

    public string FilePath { get; }
    public string DataDirectory { get; }

    public Database(string? overrideDirectory = null)
    {
        DataDirectory = overrideDirectory ?? DefaultDataDirectory();
        Directory.CreateDirectory(DataDirectory);
        FilePath = Path.Combine(DataDirectory, "bloom.db");
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = FilePath,
            ForeignKeys = true,
            Pooling = true,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Default
        }.ToString();
    }

    public static string DefaultDataDirectory()
    {
        string root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "Bloom");
    }

    public SqliteConnection Open()
    {
        SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON; PRAGMA busy_timeout = 5000;";
        pragma.ExecuteNonQuery();
        return connection;
    }

    public void Initialize()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using (SqliteCommand walCommand = connection.CreateCommand())
        {
            walCommand.CommandText = "PRAGMA journal_mode = WAL; PRAGMA foreign_keys = ON;";
            walCommand.ExecuteNonQuery();
        }
        Migrations.Run(connection);
    }

    public void Checkpoint()
    {
        using SqliteConnection connection = Open();
        connection.Execute("PRAGMA wal_checkpoint(TRUNCATE);");
    }
}
