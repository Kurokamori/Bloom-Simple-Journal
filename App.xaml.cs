using System.IO;
using System.Windows;
using System.Windows.Threading;
using Bloom.Services;
using Bloom.Views;

namespace Bloom;

public partial class App : Application
{
    private static AppServices? _services;

    public static AppServices Services =>
        _services ?? throw new InvalidOperationException("Services are not initialized yet.");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += (_, args) => Log(args.ExceptionObject as Exception);

        try
        {
            _services = new AppServices();
            _services.Theme.ApplySaved();
            ShowRootWindow();
        }
        catch (Exception ex)
        {
            Log(ex);
            throw;
        }
    }

    public void ShowRootWindow()
    {
        Window? previous = MainWindow;
        Window window = Services.OnboardingComplete ? new MainWindow() : new OnboardingWindow();

        MainWindow = window;
        window.Show();

        if (previous is not null && !ReferenceEquals(previous, window))
        {
            previous.Close();
        }
    }

    public void RestartShell() => ShowRootWindow();

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log(e.Exception);
        MessageBox.Show(
            $"Something went wrong:\n\n{e.Exception.Message}",
            "Bloom", MessageBoxButton.OK, MessageBoxImage.Warning);
        e.Handled = true;
    }

    private static void Log(Exception? ex)
    {
        try
        {
            string dir = _services?.Database.DataDirectory
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bloom");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "startup.log"), ex?.ToString() ?? "unknown");
        }
        catch
        {
        }
    }
}
