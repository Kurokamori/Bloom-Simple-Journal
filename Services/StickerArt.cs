using System.IO;
using Bloom.Models;

namespace Bloom.Services;

public static class StickerArt
{
    private static readonly Dictionary<string, string> Glyphs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["heart"] = "❤️", ["star"] = "⭐", ["sparkle"] = "✨", ["sun"] = "☀️", ["cloud"] = "☁️",
        ["rainbow"] = "🌈", ["flower"] = "🌸", ["leaf"] = "🍃", ["mushroom"] = "🍄", ["cat"] = "🐱",
        ["bunny"] = "🐰", ["bear"] = "🐻", ["coffee"] = "☕", ["strawberry"] = "🍓", ["cupcake"] = "🧁",
        ["moon"] = "🌙", ["smile"] = "🙂", ["teardrop"] = "💧", ["bow"] = "🎀", ["crown"] = "👑",
        ["butterfly"] = "🦋", ["ghost"] = "👻", ["planet"] = "🪐", ["candle"] = "🕯️", ["tea"] = "🍵",
        ["frog"] = "🐸", ["snail"] = "🐌", ["crescent"] = "🌛", ["gem"] = "💎", ["paw"] = "🐾"
    };

    public static string KeyFromPath(string imagePath) =>
        Path.GetFileNameWithoutExtension(imagePath);

    public static string Glyph(Sticker sticker)
    {
        string key = KeyFromPath(sticker.ImagePath);
        return Glyphs.TryGetValue(key, out string? glyph) ? glyph : "⭐";
    }

    public static string GlyphForKey(string key) =>
        Glyphs.TryGetValue(key, out string? glyph) ? glyph : "⭐";

    public static string ReferenceFor(Sticker sticker)
    {
        string? resolved = ArtPaths.Resolve(sticker.ImagePath);
        return resolved is not null ? sticker.ImagePath : "emoji:" + Glyph(sticker);
    }
}
