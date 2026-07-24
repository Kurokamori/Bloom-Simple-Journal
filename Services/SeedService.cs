using Bloom.Data.Repositories;
using Bloom.Models;

namespace Bloom.Services;

public sealed class SeedService
{
    private readonly AssetRepository _assets;
    private readonly CreatureRepository _creatures;
    private readonly SettingsRepository _settings;

    public SeedService(AssetRepository assets, CreatureRepository creatures, SettingsRepository settings)
    {
        _assets = assets;
        _creatures = creatures;
        _settings = settings;
    }

    public void EnsureSeeded()
    {
        ArtPaths.EnsureDirectories();

        if (!_settings.GetBool("seed.backgrounds"))
        {
            int order = 0;
            foreach (BackgroundPreset preset in Presets.Backgrounds)
            {
                _assets.InsertBackground(new PageBackground
                {
                    Key = preset.Key,
                    Name = preset.Name,
                    Kind = preset.Kind,
                    Value = preset.Value,
                    IsUnlocked = preset.Free,
                    UnlockCost = preset.Cost,
                    SortOrder = order++
                });
            }
            _settings.SetBool("seed.backgrounds", true);
        }

        if (!_settings.GetBool("seed.stickers"))
        {
            int order = 0;
            foreach (StickerPreset preset in Presets.Stickers)
            {
                _assets.InsertSticker(new Sticker
                {
                    Name = preset.Name,
                    Category = preset.Category,
                    ImagePath = ArtPaths.StickerRelative(preset.Key),
                    IsUnlocked = preset.Free,
                    UnlockCost = preset.Cost,
                    SortOrder = order++
                });
            }
            _settings.SetBool("seed.stickers", true);
        }

        if (!_settings.GetBool("seed.creatures"))
        {
            int order = 0;
            foreach (CreaturePreset preset in Presets.Creatures)
            {
                _creatures.Insert(new Creature
                {
                    Name = preset.Name,
                    Species = preset.Species,
                    Blurb = preset.Blurb,
                    ImagePath = ArtPaths.CreatureRelative(preset.Key),
                    SilhouettePath = ArtPaths.SilhouetteRelative(preset.Key),
                    AccentColor = preset.AccentColor,
                    Rarity = preset.Rarity,
                    UnlockCost = preset.Cost,
                    IsUnlocked = false,
                    SortOrder = order++
                });
            }
            _settings.SetBool("seed.creatures", true);
        }
    }
}
