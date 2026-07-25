using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Bloom.Common;
using Bloom.Models;
using Bloom.ViewModels.Journal;
using Condition = Bloom.Models.Condition;

namespace Bloom.Services;

public sealed class ComposedDocument
{
    public required FlowDocument Document { get; init; }
    public required DocumentPaginator Paginator { get; init; }
    public required int PageCount { get; init; }
    public required double PageWidth { get; init; }
    public required double PageHeight { get; init; }
}

public sealed class PageComposer
{
    private readonly AppServices _services;

    public PageComposer(AppServices services) => _services = services;

    public ComposedDocument Build(string dateKey, string fontFamily, Color ink, double width = 794, double height = 1123)
    {
        Color accent = Blend(ink, Colors.Gray, 0.15);
        FlowDocument document = new()
        {
            PageWidth = width,
            PageHeight = height,
            ColumnWidth = width,
            PagePadding = new Thickness(PageGrid.PaddingSide, PageGrid.PaddingTop, PageGrid.PaddingSide, PageGrid.PaddingBottom),
            FontFamily = new FontFamily(fontFamily),
            FontSize = 13.5,
            Foreground = new SolidColorBrush(ink),
            IsColumnWidthFlexible = false
        };

        Dates.TryParse(dateKey, out DateOnly date);
        JournalBody body = _services.Journal.GetBody(dateKey);

        Paragraph header = GridParagraph(rows: 2);
        header.Inlines.Add(new Run(Dates.Pretty(date))
        {
            FontSize = 26,
            FontWeight = FontWeights.Bold
        });
        document.Blocks.Add(header);

        if (!string.IsNullOrWhiteSpace(body.Title))
        {
            Paragraph subtitle = GridParagraph(rows: 1);
            subtitle.Inlines.Add(new Run(body.Title) { FontSize = 16, FontStyle = FontStyles.Italic, Foreground = new SolidColorBrush(accent) });
            document.Blocks.Add(subtitle);
        }
        else
        {
            document.Blocks.Add(GridParagraph(rows: 1));
        }

        IReadOnlyList<Checkin> checkins = _services.Journal.Checkins(dateKey);
        if (checkins.Count > 0)
        {
            AddHeader(document, "☀  Check-ins", accent);
            foreach (Checkin c in checkins)
            {
                string note = string.IsNullOrWhiteSpace(c.Note) ? string.Empty : $"  — {c.Note}";
                AddLine(document, $"{MoodGlyphs.ForValue(c.Mood)}  Mood {c.Mood} · Energy {c.Energy} · Productivity {c.Productivity}{note}");
            }
        }

        IReadOnlyList<WotCheckin> wots = _services.Journal.WotCheckins(dateKey);
        if (wots.Count > 0)
        {
            AddHeader(document, "🌿  Window of tolerance", accent);
            foreach (WotCheckin w in wots)
            {
                AddLine(document, $"{ZoneInfo.Glyph(w.Zone)}  {ZoneInfo.Label(w.Zone)} · intensity {w.Intensity}");
                if (!string.IsNullOrWhiteSpace(w.Cause))
                {
                    AddSub(document, $"cause: {w.Cause}", accent);
                }
                if (!string.IsNullOrWhiteSpace(w.Helped))
                {
                    AddSub(document, $"helped: {w.Helped}", accent);
                }
                if (!string.IsNullOrWhiteSpace(w.Note))
                {
                    AddSub(document, w.Note!, accent);
                }
            }
        }

        List<Condition> conditions = _services.Conditions.All().ToList();
        bool anySymptom = false;
        foreach (Condition condition in conditions)
        {
            List<(Symptom Symptom, SymptomLog Log)> logged = new();
            foreach (Symptom symptom in _services.Conditions.Symptoms(condition.Id))
            {
                SymptomLog? log = _services.SymptomLogs.ForSymptomOnDate(symptom.Id, dateKey);
                if (log is not null && (log.ScaleValue is not null || !string.IsNullOrWhiteSpace(log.TextValue)))
                {
                    logged.Add((symptom, log));
                }
            }
            if (logged.Count == 0)
            {
                continue;
            }
            if (!anySymptom)
            {
                AddHeader(document, "🩷  Symptoms", accent);
                anySymptom = true;
            }
            AddLine(document, $"{condition.Icon}  {condition.Name}", bold: true);
            foreach ((Symptom symptom, SymptomLog log) in logged)
            {
                string scale = log.ScaleValue is not null ? $"{log.ScaleValue}/{symptom.ScaleMax}" : string.Empty;
                string text = string.IsNullOrWhiteSpace(log.TextValue) ? string.Empty : $" — {log.TextValue}";
                AddSub(document, $"{symptom.Name}: {scale}{text}", accent);
            }
        }

        IReadOnlyList<FrontEvent> fronts = _services.Alters.EventsForDate(dateKey);
        if (fronts.Count > 0)
        {
            Dictionary<long, Alter> lookup = _services.Alters.All(includeArchived: true).ToDictionary(a => a.Id);
            AddHeader(document, "🪞  System", accent);
            foreach (FrontEvent f in fronts)
            {
                string name = lookup.TryGetValue(f.AlterId, out Alter? alter) ? alter.Name : "Alter";
                string status = f.Fronted ? (f.CoConscious ? "fronted, co-conscious" : "fronted") : "present";
                AddLine(document, $"{name} — {status}", bold: true);
                if (!string.IsNullOrWhiteSpace(f.Opinion))
                {
                    AddSub(document, $"opinion: {f.Opinion}", accent);
                }
                if (!string.IsNullOrWhiteSpace(f.Note))
                {
                    AddSub(document, f.Note!, accent);
                }
            }
        }

        IReadOnlyList<FoodLog> foods = _services.Journal.Foods(dateKey);
        if (foods.Count > 0)
        {
            AddHeader(document, "🍽  Nourishment", accent);
            foreach (FoodLog food in foods)
            {
                string meal = string.IsNullOrWhiteSpace(food.MealTime) ? string.Empty : $"[{food.MealTime}] ";
                string note = string.IsNullOrWhiteSpace(food.Note) ? string.Empty : $" — {food.Note}";
                AddLine(document, $"{meal}{food.Name}{note}");
            }
        }

        if (!string.IsNullOrWhiteSpace(body.Content))
        {
            AddHeader(document, "🌙  Journal", accent);
            foreach (string line in body.Content.Replace("\r\n", "\n").Split('\n'))
            {
                Paragraph paragraph = GridParagraph(rows: 1);
                paragraph.Inlines.Add(new Run(line));
                document.Blocks.Add(paragraph);
            }
        }

        DocumentPaginator paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
        paginator.PageSize = new Size(width, height);
        paginator.ComputePageCount();
        int count = Math.Max(1, paginator.PageCount);

        return new ComposedDocument
        {
            Document = document,
            Paginator = paginator,
            PageCount = count,
            PageWidth = width,
            PageHeight = height
        };
    }

    private static Paragraph GridParagraph(int rows, double leftIndent = 0)
    {
        return new Paragraph
        {
            Margin = new Thickness(leftIndent, 0, 0, 0),
            LineHeight = PageGrid.Rows(rows),
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight
        };
    }

    private static void AddHeader(FlowDocument document, string text, Color accent)
    {
        document.Blocks.Add(GridParagraph(rows: 1));

        Paragraph paragraph = GridParagraph(rows: 1);
        paragraph.Inlines.Add(new Run(text) { FontSize = 15.5, FontWeight = FontWeights.Bold });
        document.Blocks.Add(paragraph);

        Grid ruleHost = new() { Height = PageGrid.Rows(1) };
        ruleHost.Children.Add(new Rectangle
        {
            Height = 1.4,
            Fill = new SolidColorBrush(Blend(accent, Colors.Gray, 0.4)),
            Opacity = 0.5,
            VerticalAlignment = VerticalAlignment.Center
        });
        document.Blocks.Add(new BlockUIContainer(ruleHost) { Margin = new Thickness(0) });
    }

    private static void AddLine(FlowDocument document, string text, bool bold = false)
    {
        Paragraph paragraph = GridParagraph(rows: 1);
        paragraph.Inlines.Add(new Run(text) { FontWeight = bold ? FontWeights.SemiBold : FontWeights.Normal });
        document.Blocks.Add(paragraph);
    }

    private static void AddSub(FlowDocument document, string text, Color accent)
    {
        Paragraph paragraph = GridParagraph(rows: 1, leftIndent: 16);
        paragraph.Inlines.Add(new Run(text) { FontSize = 12.5, Foreground = new SolidColorBrush(accent) });
        document.Blocks.Add(paragraph);
    }

    private static Color Blend(Color a, Color b, double t) => Color.FromRgb(
        (byte)(a.R * (1 - t) + b.R * t),
        (byte)(a.G * (1 - t) + b.G * t),
        (byte)(a.B * (1 - t) + b.B * t));
}
