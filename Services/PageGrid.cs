using System;
using System.Windows;
using System.Windows.Media;

namespace Bloom.Services;

public static class PageGrid
{
    public const double Pitch = 26;
    public const double PaddingSide = 58;
    public const double PaddingTop = 54;
    public const double PaddingBottom = 58;
    public const double BodyFontSize = 13.5;
    public const double TitleFontSize = 26;
    public const double MarginLineX = 40;
    public const double BaselineBiasRatio = 0.15;

    public static double Rows(int count) => count * Pitch;

    public static double RulePhase(string fontFamily)
    {
        double baseline = Baseline(Pitch, BodyFontSize, fontFamily);
        return Mod(PaddingTop + baseline, Pitch);
    }

    public static double TitleLineY(string fontFamily) =>
        PaddingTop + Baseline(Rows(2), TitleFontSize, fontFamily);

    public static double Baseline(double boxHeight, double fontSize, string fontFamily)
    {
        double emBaseline = 0.9;
        double emHeight = 1.15;
        Typeface typeface = new(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (typeface.TryGetGlyphTypeface(out GlyphTypeface glyph))
        {
            emBaseline = glyph.Baseline;
            emHeight = glyph.Height;
        }
        double natural = emHeight * fontSize;
        double topGap = Math.Max(0, (boxHeight - natural) / 2.0);
        return topGap + (emBaseline + BaselineBiasRatio) * fontSize;
    }

    private static double Mod(double a, double b) => ((a % b) + b) % b;
}
