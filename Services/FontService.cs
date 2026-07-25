using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

namespace Bloom.Services;

public sealed class FontService
{
    private List<string>? _cache;

    public IReadOnlyList<string> SystemFonts()
    {
        if (_cache is not null)
        {
            return _cache;
        }

        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
        foreach (FontFamily family in Fonts.SystemFontFamilies)
        {
            string? name = ResolveName(family);
            if (!string.IsNullOrWhiteSpace(name))
            {
                names.Add(name);
            }
        }

        _cache = names.OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList();
        return _cache;
    }

    public bool Exists(string family) =>
        SystemFonts().Contains(family, StringComparer.OrdinalIgnoreCase);

    private static string? ResolveName(FontFamily family)
    {
        LanguageSpecificStringDictionary names = family.FamilyNames;
        XmlLanguage current = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);
        if (names.TryGetValue(current, out string? localized) && !string.IsNullOrWhiteSpace(localized))
        {
            return localized;
        }
        XmlLanguage english = XmlLanguage.GetLanguage("en-US");
        if (names.TryGetValue(english, out string? englishName) && !string.IsNullOrWhiteSpace(englishName))
        {
            return englishName;
        }
        return names.Values.FirstOrDefault() ?? family.Source;
    }
}
