using Dapper;
using Bloom.Data;
using Bloom.Data.Repositories;

namespace Bloom.Services;

public sealed class AppServices
{
    public Database Database { get; }

    public SettingsRepository Settings { get; }
    public EconomyRepository EconomyRepo { get; }
    public ConditionRepository Conditions { get; }
    public SymptomLogRepository SymptomLogs { get; }
    public AlterRepository Alters { get; }
    public JournalRepository Journal { get; }
    public HabitRepository Habits { get; }
    public TaskRepository Tasks { get; }
    public RewardRepository Rewards { get; }
    public CreatureRepository Creatures { get; }
    public AssetRepository Assets { get; }

    public EconomyService Economy { get; }
    public ThemeService Theme { get; }
    public FontService Fonts { get; }
    public BackupService Backup { get; }
    public SeedService Seed { get; }
    public ExportService Export { get; }

    public AppServices(string? dataDirectory = null)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        Database = new Database(dataDirectory);
        Database.Initialize();

        Settings = new SettingsRepository(Database);
        EconomyRepo = new EconomyRepository(Database);
        Conditions = new ConditionRepository(Database);
        SymptomLogs = new SymptomLogRepository(Database);
        Alters = new AlterRepository(Database);
        Journal = new JournalRepository(Database);
        Habits = new HabitRepository(Database);
        Tasks = new TaskRepository(Database);
        Rewards = new RewardRepository(Database);
        Creatures = new CreatureRepository(Database);
        Assets = new AssetRepository(Database);

        Economy = new EconomyService(EconomyRepo, Settings);
        Theme = new ThemeService(Settings);
        Fonts = new FontService();
        Backup = new BackupService(Database);
        Seed = new SeedService(Assets, Creatures, Settings);
        Export = new ExportService();

        Seed.EnsureSeeded();
    }

    public bool OnboardingComplete
    {
        get => Settings.GetBool("onboarding.complete");
        set => Settings.SetBool("onboarding.complete", value);
    }
}
