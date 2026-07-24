using System.IO;

namespace Bloom.Services;

public static class ArtPaths
{
    public static string Root { get; } = Path.Combine(AppContext.BaseDirectory, "Assets", "Art");

    public static string Stickers { get; } = Path.Combine(Root, "stickers");
    public static string Creatures { get; } = Path.Combine(Root, "creatures");
    public static string Backgrounds { get; } = Path.Combine(Root, "backgrounds");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(Stickers);
        Directory.CreateDirectory(Creatures);
        Directory.CreateDirectory(Backgrounds);
    }

    public static string StickerRelative(string key) => Path.Combine("stickers", key + ".png");

    public static string CreatureRelative(string key) => Path.Combine("creatures", key + ".png");

    public static string SilhouetteRelative(string key) => Path.Combine("creatures", key + "_silhouette.png");

    public static string? Resolve(string? relativeOrAbsolute)
    {
        if (string.IsNullOrWhiteSpace(relativeOrAbsolute))
        {
            return null;
        }
        if (Path.IsPathRooted(relativeOrAbsolute))
        {
            return File.Exists(relativeOrAbsolute) ? relativeOrAbsolute : null;
        }
        string full = Path.Combine(Root, relativeOrAbsolute);
        return File.Exists(full) ? full : null;
    }
}
