using Bloom.Data;
using System.IO;
using System.IO.Compression;

namespace Bloom.Services;

public sealed class BackupResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Path { get; init; }
}

public sealed class BackupService
{
    private readonly Database _database;

    public BackupService(Database database) => _database = database;

    public BackupResult CreateBackup(string destinationZipPath)
    {
        try
        {
            _database.Checkpoint();
            string? directory = Path.GetDirectoryName(destinationZipPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(destinationZipPath))
            {
                File.Delete(destinationZipPath);
            }

            using ZipArchive archive = ZipFile.Open(destinationZipPath, ZipArchiveMode.Create);
            AddFileIfExists(archive, _database.FilePath, "bloom.db");

            string artRoot = ArtPaths.Root;
            if (Directory.Exists(artRoot))
            {
                foreach (string file in Directory.EnumerateFiles(artRoot, "*.*", SearchOption.AllDirectories))
                {
                    string relative = Path.Combine("Art", Path.GetRelativePath(artRoot, file));
                    archive.CreateEntryFromFile(file, relative.Replace('\\', '/'), CompressionLevel.Optimal);
                }
            }

            return new BackupResult { Success = true, Path = destinationZipPath, Message = "Backup saved." };
        }
        catch (Exception ex)
        {
            return new BackupResult { Success = false, Message = ex.Message };
        }
    }

    public BackupResult RestoreBackup(string sourceZipPath)
    {
        try
        {
            if (!File.Exists(sourceZipPath))
            {
                return new BackupResult { Success = false, Message = "Backup file not found." };
            }

            using ZipArchive archive = ZipFile.OpenRead(sourceZipPath);
            ZipArchiveEntry? dbEntry = archive.GetEntry("bloom.db");
            if (dbEntry is null)
            {
                return new BackupResult { Success = false, Message = "This file is not a Bloom backup." };
            }

            string stagedDb = _database.FilePath + ".restore";
            using (Stream entryStream = dbEntry.Open())
            using (FileStream target = File.Create(stagedDb))
            {
                entryStream.CopyTo(target);
            }

            DeleteWalArtifacts(_database.FilePath);
            File.Copy(stagedDb, _database.FilePath, overwrite: true);
            File.Delete(stagedDb);

            string artRoot = ArtPaths.Root;
            Directory.CreateDirectory(artRoot);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (!entry.FullName.StartsWith("Art/", StringComparison.OrdinalIgnoreCase) || entry.Length == 0)
                {
                    continue;
                }
                string relative = entry.FullName["Art/".Length..];
                string destination = Path.Combine(artRoot, relative.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                entry.ExtractToFile(destination, overwrite: true);
            }

            return new BackupResult { Success = true, Message = "Backup restored. Bloom will restart." };
        }
        catch (Exception ex)
        {
            return new BackupResult { Success = false, Message = ex.Message };
        }
    }

    public string SuggestBackupName() =>
        $"bloom-backup-{DateTime.Now:yyyy-MM-dd_HHmm}.bloombak";

    private static void AddFileIfExists(ZipArchive archive, string path, string entryName)
    {
        if (File.Exists(path))
        {
            archive.CreateEntryFromFile(path, entryName, CompressionLevel.Optimal);
        }
    }

    private static void DeleteWalArtifacts(string dbPath)
    {
        foreach (string suffix in new[] { "-wal", "-shm" })
        {
            string path = dbPath + suffix;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
