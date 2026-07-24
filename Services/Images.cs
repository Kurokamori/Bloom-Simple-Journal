using System.IO;

namespace Bloom.Services;

public static class Images
{
    private static readonly Dictionary<string, BitmapImage> Cache = new();

    public static BitmapImage? LoadFrozen(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }
        if (Cache.TryGetValue(path, out BitmapImage? cached))
        {
            return cached;
        }
        try
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();
            Cache[path] = bitmap;
            return bitmap;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void Invalidate(string path) => Cache.Remove(path);
}
