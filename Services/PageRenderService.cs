using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bloom.Common;
using Bloom.Controls;
using Bloom.Models;

namespace Bloom.Services;

public sealed class PageRenderService
{
    private const double StickerBox = 78;

    private readonly AppServices _services;
    private readonly ExportService _export = new();

    public PageRenderService(AppServices services) => _services = services;

    public PageComposition LoadComposition(string dateKey)
    {
        JournalPage? saved = _services.Journal.Pages(dateKey).FirstOrDefault(p => p.PageIndex == 0);
        if (saved is not null)
        {
            return new PageComposition
            {
                BackgroundKey = saved.BackgroundId,
                FontFamily = saved.FontFamily,
                Items = Json.Read<List<DecorItem>>(saved.DecorJson) ?? new List<DecorItem>()
            };
        }
        return new PageComposition
        {
            BackgroundKey = "paper-cream",
            FontFamily = _services.Settings.Get("page.font.default", "Segoe UI"),
            Items = new List<DecorItem>()
        };
    }

    public List<BitmapSource> RenderSavedDay(string dateKey, double dpi = ExportService.DefaultDpi)
    {
        PageComposition composition = LoadComposition(dateKey);
        List<PageBackground> backgrounds = _services.Assets.Backgrounds().ToList();
        return RenderComposition(dateKey, composition, backgrounds, dpi);
    }

    public List<BitmapSource> RenderComposition(
        string dateKey, PageComposition composition, IReadOnlyList<PageBackground> backgrounds, double dpi = ExportService.DefaultDpi)
    {
        PageBackground background = backgrounds.FirstOrDefault(b => b.Key == composition.BackgroundKey)
            ?? new PageBackground { Key = "paper-cream", Value = "#FBF3EA", Kind = "color" };
        Color ink = InkFor(background);

        ComposedDocument composed = new PageComposer(_services).Build(dateKey, composition.FontFamily, ink);
        List<BitmapSource> results = new();
        for (int i = 0; i < composed.PageCount; i++)
        {
            FrameworkElement element = BuildPage(i, composed, composition, background);
            results.Add(_export.Render(element, dpi));
        }
        return results;
    }

    private FrameworkElement BuildPage(int index, ComposedDocument composed, PageComposition composition, PageBackground background)
    {
        Grid root = new()
        {
            Width = composed.PageWidth,
            Height = composed.PageHeight,
            ClipToBounds = true
        };
        root.Children.Add(new Border { Background = BackgroundBrushFactory.Create(background, PageGrid.Pitch, PageGrid.RulePhase(composition.FontFamily)) });
        if (background.Kind == "lined")
        {
            root.Children.Add(PageFurniture.Build(composed.PageWidth, composed.PageHeight, background, composition.FontFamily));
        }
        root.Children.Add(new VisualHost { Child = composed.Paginator.GetPage(index).Visual });

        Canvas canvas = new() { Width = composed.PageWidth, Height = composed.PageHeight };
        foreach (DecorItem item in composition.Items.Where(i => i.Page == index).OrderBy(i => i.Z))
        {
            canvas.Children.Add(BuildSticker(item));
        }
        root.Children.Add(canvas);
        return root;
    }

    private static Grid BuildSticker(DecorItem item)
    {
        Grid grid = new()
        {
            Width = StickerBox,
            Height = StickerBox,
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        if (item.Ref.StartsWith("emoji:", StringComparison.Ordinal))
        {
            grid.Children.Add(new TextBlock
            {
                Text = item.Ref["emoji:".Length..],
                FontSize = 46,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
        }
        else
        {
            BitmapImage? bitmap = Images.LoadFrozen(ArtPaths.Resolve(item.Ref));
            if (bitmap is not null)
            {
                grid.Children.Add(new Image { Source = bitmap, Stretch = Stretch.Uniform });
            }
            else
            {
                grid.Children.Add(new TextBlock { Text = "⭐", FontSize = 46, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            }
        }

        TransformGroup transform = new();
        transform.Children.Add(new ScaleTransform(item.Scale, item.Scale));
        transform.Children.Add(new RotateTransform(item.Rotation));
        grid.RenderTransform = transform;

        Canvas.SetLeft(grid, item.X - StickerBox / 2);
        Canvas.SetTop(grid, item.Y - StickerBox / 2);
        Panel.SetZIndex(grid, item.Z);
        return grid;
    }

    public static Color InkFor(PageBackground background)
    {
        Color baseColor = BackgroundBrushFactory.ParseColor(background.Value, Colors.White);
        double luminance = (0.299 * baseColor.R + 0.587 * baseColor.G + 0.114 * baseColor.B) / 255.0;
        return luminance > 0.5
            ? Color.FromRgb(0x46, 0x3B, 0x54)
            : Color.FromRgb(0xF1, 0xEC, 0xFA);
    }
}
