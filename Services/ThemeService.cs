using Bloom.Data.Repositories;
using System.Collections.ObjectModel;

namespace Bloom.Services;

public sealed record ThemeInfo(string Id, string Name, bool IsDark, string PreviewPrimary, string PreviewBg);

public sealed class ThemeService
{
    private readonly SettingsRepository _settings;
    private ResourceDictionary? _overrides;

    public ThemeService(SettingsRepository settings) => _settings = settings;

    public event EventHandler? ThemeChanged;

    public IReadOnlyList<ThemeInfo> Catalog { get; } = new List<ThemeInfo>
    {
        new("Lavender", "Lavender Dream", false, "#B79CE8", "#F6F1FB"),
        new("Strawberry", "Strawberry Milk", false, "#F48FB1", "#FEF3F6"),
        new("Matcha", "Matcha Latte", false, "#8FBF7F", "#F1F7EF"),
        new("Peach", "Sunset Peach", false, "#F5A66E", "#FEF4EC"),
        new("CottonCandy", "Cotton Candy", false, "#C9A0E9", "#F5F1FB"),
        new("Midnight", "Midnight Bloom", true, "#B79CE8", "#201B2E"),
        new("Cocoa", "Cocoa Cocoon", true, "#E0A57A", "#29211D"),
        new("Blueberry", "Blueberry Night", true, "#8FB8E8", "#1A2130")
    };

    public string CurrentThemeId => _settings.Get("theme.id", "Lavender");

    public bool IsCurrentDark => Catalog.FirstOrDefault(t => t.Id == CurrentThemeId)?.IsDark ?? false;

    public void ApplySaved()
    {
        Apply(CurrentThemeId, persist: false);
        ReapplyOverrides();
    }

    public void Apply(string themeId, bool persist = true)
    {
        ThemeInfo info = Catalog.FirstOrDefault(t => t.Id == themeId) ?? Catalog[0];
        ResourceDictionary next = new()
        {
            Source = new Uri($"/Themes/{info.Id}.xaml", UriKind.Relative)
        };

        Collection<ResourceDictionary> merged = Application.Current.Resources.MergedDictionaries;
        List<ResourceDictionary> existing = merged.Where(d => d.Contains("Theme.Id")).ToList();
        foreach (ResourceDictionary stale in existing)
        {
            merged.Remove(stale);
        }
        merged.Insert(0, next);

        if (persist)
        {
            _settings.Set("theme.id", info.Id);
        }

        ReapplyOverrides();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public string? GetOverride(string tokenKey) => _settings.Get($"theme.override.{tokenKey}");

    public void SetOverride(string tokenKey, string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            _settings.Set($"theme.override.{tokenKey}", string.Empty);
        }
        else
        {
            _settings.Set($"theme.override.{tokenKey}", hex);
        }
        ReapplyOverrides();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearOverrides()
    {
        foreach (string token in OverridableTokens)
        {
            _settings.Set($"theme.override.{token}", string.Empty);
        }
        ReapplyOverrides();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public static IReadOnlyList<string> OverridableTokens { get; } = new[]
    {
        "Primary", "Secondary", "Accent", "Bg", "Surface", "Coin"
    };

    private void ReapplyOverrides()
    {
        Collection<ResourceDictionary> merged = Application.Current.Resources.MergedDictionaries;
        if (_overrides is null)
        {
            _overrides = new ResourceDictionary();
            merged.Add(_overrides);
        }
        else if (!merged.Contains(_overrides))
        {
            merged.Add(_overrides);
        }

        _overrides.Clear();
        foreach (string token in OverridableTokens)
        {
            string? hex = GetOverride(token);
            if (string.IsNullOrWhiteSpace(hex))
            {
                continue;
            }
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(hex);
                _overrides[$"C.{token}"] = color;
                if (token == "Primary")
                {
                    _overrides["C.PrimaryHover"] = Shift(color, 0.08);
                    _overrides["C.PrimarySoft"] = Mix(color, Colors.White, 0.82);
                }
            }
            catch (FormatException)
            {
            }
        }

        RebuildBrushes(merged);
    }

    private static void RebuildBrushes(Collection<ResourceDictionary> merged)
    {
        ResourceDictionary? brushes = merged.FirstOrDefault(
            d => d.Source is not null && d.Source.OriginalString.Contains("_Brushes", StringComparison.OrdinalIgnoreCase));
        if (brushes is null)
        {
            return;
        }

        int index = merged.IndexOf(brushes);
        ResourceDictionary fresh = new()
        {
            Source = new Uri("/Themes/_Brushes.xaml", UriKind.Relative)
        };
        merged.RemoveAt(index);
        merged.Insert(index, fresh);
    }

    private static Color Shift(Color color, double amount)
    {
        double factor = 1 + amount;
        return Color.FromArgb(color.A,
            ClampByte(color.R / factor),
            ClampByte(color.G / factor),
            ClampByte(color.B / factor));
    }

    private static Color Mix(Color a, Color b, double weightB)
    {
        double wa = 1 - weightB;
        return Color.FromArgb(255,
            ClampByte(a.R * wa + b.R * weightB),
            ClampByte(a.G * wa + b.G * weightB),
            ClampByte(a.B * wa + b.B * weightB));
    }

    private static byte ClampByte(double value) => (byte)Math.Clamp(value, 0, 255);
}
