using Bloom.Models;

namespace Bloom.Services;

public sealed record SymptomPreset(string Name, bool Scale, bool Text, SymptomWidget Widget = SymptomWidget.Standard);

public sealed record ConditionPreset(string Name, string Icon, string Color, IReadOnlyList<SymptomPreset> Symptoms);

public sealed record RewardPreset(string Name, string Icon, int Cost, RewardCategory Category, string Description);

public sealed record CreaturePreset(
    string Name, string Species, string Blurb, string AccentColor,
    CreatureRarity Rarity, int Cost, string Key);

public sealed record StickerPreset(string Name, StickerCategory Category, bool Free, int Cost, string Key);

public sealed record BackgroundPreset(string Key, string Name, string Kind, string Value, bool Free, int Cost);

public static class Presets
{
    public static IReadOnlyList<ConditionPreset> Conditions { get; } = new List<ConditionPreset>
    {
        new("Bipolar Disorder", "🌗", "#B7A6E0", new List<SymptomPreset>
        {
            new("Mania", true, true, SymptomWidget.Mood),
            new("Depression", true, true, SymptomWidget.Mood),
            new("Irritability", true, false),
            new("Racing thoughts", true, true),
            new("Sleep changes", false, true)
        }),
        new("Lupus", "🦋", "#8FB8DE", new List<SymptomPreset>
        {
            new("Fatigue", true, true),
            new("Joint pain", true, true, SymptomWidget.Pain),
            new("Brain fog", true, false),
            new("Flare / rash", true, true),
            new("Photosensitivity", true, false)
        }),
        new("Anxiety", "🌊", "#8FD3C7", new List<SymptomPreset>
        {
            new("Worry", true, true),
            new("Panic attacks", true, true),
            new("Restlessness", true, false),
            new("Physical tension", true, false)
        }),
        new("Depression", "🌧", "#9AA7C7", new List<SymptomPreset>
        {
            new("Low mood", true, true, SymptomWidget.Mood),
            new("Anhedonia", true, false),
            new("Fatigue", true, false),
            new("Hopelessness", true, true)
        }),
        new("PTSD / C-PTSD", "🕯", "#D8A7B1", new List<SymptomPreset>
        {
            new("Flashbacks", true, true),
            new("Nightmares", false, true),
            new("Hypervigilance", true, false),
            new("Dissociation", true, false, SymptomWidget.Dissociation)
        }),
        new("DID / OSDD", "🪞", "#A7C7E7", new List<SymptomPreset>
        {
            new("Dissociation", true, true, SymptomWidget.Dissociation),
            new("Amnesia / time loss", true, true),
            new("Switching frequency", true, true),
            new("Internal communication", true, false)
        }),
        new("ADHD", "⚡", "#F2C879", new List<SymptomPreset>
        {
            new("Focus", true, false),
            new("Impulsivity", true, false),
            new("Overwhelm", true, true),
            new("Restlessness", true, false)
        }),
        new("Fibromyalgia", "🌡", "#E7A7C7", new List<SymptomPreset>
        {
            new("Pain level", true, true, SymptomWidget.Pain),
            new("Stiffness", true, false),
            new("Fatigue", true, false),
            new("Tender points", true, true)
        }),
        new("Migraine", "🌙", "#A7A7D8", new List<SymptomPreset>
        {
            new("Headache", true, true, SymptomWidget.Pain),
            new("Aura", false, true),
            new("Nausea", true, false),
            new("Light sensitivity", true, false)
        }),
        new("POTS / Dysautonomia", "💫", "#8FD3C7", new List<SymptomPreset>
        {
            new("Dizziness", true, false),
            new("Heart racing", true, true),
            new("Fatigue", true, false),
            new("Temperature swings", true, false)
        }),
        new("EDS / Hypermobility", "🧩", "#C7B7E7", new List<SymptomPreset>
        {
            new("Joint pain", true, true, SymptomWidget.Pain),
            new("Subluxations", false, true),
            new("Fatigue", true, false),
            new("Skin fragility", true, false)
        }),
        new("Chronic Pain", "🌷", "#E7B7A7", new List<SymptomPreset>
        {
            new("Pain level", true, true, SymptomWidget.Pain),
            new("Mobility", true, false),
            new("Pain location", false, true)
        })
    };

    public static IReadOnlyList<RewardPreset> Rewards { get; } = new List<RewardPreset>
    {
        new("Cozy movie night", "🎬", 40, RewardCategory.Activity, "Blanket, snacks, and a film you love."),
        new("Favorite snack", "🍫", 20, RewardCategory.Treat, "Something sweet, just because."),
        new("Bubble bath", "🛁", 30, RewardCategory.Rest, "Warm water and quiet."),
        new("One episode", "📺", 15, RewardCategory.Activity, "Watch the next episode guilt-free."),
        new("A small treat", "🛍", 60, RewardCategory.Purchase, "Buy yourself a little something."),
        new("Gaming hour", "🎮", 35, RewardCategory.Activity, "An hour of play, earned."),
        new("Order takeout", "🍜", 100, RewardCategory.Purchase, "No cooking tonight."),
        new("A new book", "📚", 120, RewardCategory.Purchase, "Add to the to-be-read pile."),
        new("Guilt-free nap", "😴", 25, RewardCategory.Rest, "Rest is productive too."),
        new("Call a friend", "📞", 15, RewardCategory.Social, "Reach out to someone kind."),
        new("Fresh flowers", "💐", 80, RewardCategory.Purchase, "Brighten your space."),
        new("Skincare pamper", "🧖", 45, RewardCategory.Rest, "The full routine tonight."),
        new("Sleep in", "🌅", 30, RewardCategory.Rest, "No alarm tomorrow."),
        new("Craft time", "🎨", 35, RewardCategory.Activity, "Make something with your hands."),
        new("Coffee shop trip", "☕", 50, RewardCategory.Treat, "Your favorite order, out.")
    };

    public static IReadOnlyList<CreaturePreset> Creatures { get; } = new List<CreaturePreset>
    {
        new("Mochi", "Mochimal", "A soft little dumpling spirit who hums when content. Squishes flat when very happy.", "#F6C6D8", CreatureRarity.Common, 80, "mochi"),
        new("Pip", "Sproutling", "Grew from a hopeful seed. Follows sunlight and small victories.", "#A7D7C5", CreatureRarity.Common, 90, "pip"),
        new("Pebble", "Stonelet", "Quiet and steady. Warms in your palm like a river stone.", "#C4C0B6", CreatureRarity.Common, 100, "pebble"),
        new("Luna", "Moonkit", "Naps by day, glows faintly by night. Purrs in Morse code.", "#C9B8F0", CreatureRarity.Uncommon, 150, "luna"),
        new("Ember", "Emberpup", "A warm little flame with a wagging tail. Never burns, only comforts.", "#F0A587", CreatureRarity.Uncommon, 160, "ember"),
        new("Bubbles", "Fizzfin", "Half fish, half soda. Blows bubbles that smell like your favorite drink.", "#8FD3C7", CreatureRarity.Uncommon, 180, "bubbles"),
        new("Tansy", "Petalpup", "Sheds flower petals when it shakes. Blooms brighter with company.", "#F2D479", CreatureRarity.Uncommon, 200, "tansy"),
        new("Sol", "Sunbun", "A rabbit made of afternoon light. Its ears catch the warmth.", "#F4C95D", CreatureRarity.Rare, 260, "sol"),
        new("Willow", "Wispdeer", "Steps without sound through soft places. Antlers bud in spring.", "#A9C7A0", CreatureRarity.Rare, 300, "willow"),
        new("Coral", "Reeflet", "A tidepool sprite who collects sea glass and gentle words.", "#F2A9C0", CreatureRarity.Rare, 320, "coral"),
        new("Nimbus", "Cloudcat", "Floats an inch off the ground. Rains only when you need watering.", "#A7C7E7", CreatureRarity.Legendary, 500, "nimbus"),
        new("Aurora", "Starmoth", "Wings painted with northern lights. Visits only the well-rested.", "#B7A6E0", CreatureRarity.Legendary, 600, "aurora")
    };

    public static IReadOnlyList<StickerPreset> Stickers { get; } = new List<StickerPreset>
    {
        new("Heart", StickerCategory.Feelings, true, 0, "heart"),
        new("Star", StickerCategory.Decor, true, 0, "star"),
        new("Sparkle", StickerCategory.Decor, true, 0, "sparkle"),
        new("Sun", StickerCategory.Weather, true, 0, "sun"),
        new("Cloud", StickerCategory.Weather, true, 0, "cloud"),
        new("Rainbow", StickerCategory.Weather, true, 0, "rainbow"),
        new("Flower", StickerCategory.Nature, true, 0, "flower"),
        new("Leaf", StickerCategory.Nature, true, 0, "leaf"),
        new("Mushroom", StickerCategory.Nature, true, 0, "mushroom"),
        new("Cat", StickerCategory.Cute, true, 0, "cat"),
        new("Bunny", StickerCategory.Cute, true, 0, "bunny"),
        new("Bear", StickerCategory.Cute, true, 0, "bear"),
        new("Coffee", StickerCategory.Food, true, 0, "coffee"),
        new("Strawberry", StickerCategory.Food, true, 0, "strawberry"),
        new("Cupcake", StickerCategory.Food, true, 0, "cupcake"),
        new("Moon", StickerCategory.Weather, true, 0, "moon"),
        new("Smile", StickerCategory.Feelings, true, 0, "smile"),
        new("Teardrop", StickerCategory.Feelings, true, 0, "teardrop"),
        new("Bow", StickerCategory.Decor, false, 15, "bow"),
        new("Crown", StickerCategory.Decor, false, 20, "crown"),
        new("Butterfly", StickerCategory.Nature, false, 18, "butterfly"),
        new("Ghost", StickerCategory.Cute, false, 18, "ghost"),
        new("Planet", StickerCategory.Decor, false, 22, "planet"),
        new("Candle", StickerCategory.Decor, false, 16, "candle"),
        new("Tea", StickerCategory.Food, false, 14, "tea"),
        new("Frog", StickerCategory.Cute, false, 20, "frog"),
        new("Snail", StickerCategory.Nature, false, 16, "snail"),
        new("Crescent", StickerCategory.Weather, false, 14, "crescent"),
        new("Gem", StickerCategory.Decor, false, 25, "gem"),
        new("Paw", StickerCategory.Cute, false, 12, "paw")
    };

    public static IReadOnlyList<BackgroundPreset> Backgrounds { get; } = new List<BackgroundPreset>
    {
        new("paper-cream", "Cream Paper", "color", "#FBF3EA", true, 0),
        new("blush", "Blush", "color", "#FCE7EF", true, 0),
        new("mint", "Mint", "color", "#E6F5EE", true, 0),
        new("lavender-mist", "Lavender Mist", "color", "#EFE8FA", true, 0),
        new("sky", "Soft Sky", "color", "#E7F0FB", true, 0),
        new("butter", "Butter", "color", "#FBF3D9", true, 0),
        new("dotted", "Dotted Grid", "grid", "#FBF7F0", true, 0),
        new("lined", "Notebook Lines", "lined", "#FDFBF6", true, 0),
        new("graph", "Graph Paper", "graph", "#F6FAF8", false, 30),
        new("rose-grid", "Rose Grid", "grid", "#FDEEF3", false, 30),
        new("night", "Starry Night", "color", "#2B2740", false, 60),
        new("forest", "Deep Forest", "color", "#26332B", false, 60)
    };
}
