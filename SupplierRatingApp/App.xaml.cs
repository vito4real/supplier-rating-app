using SupplierRatingApp.Services;

namespace SupplierRatingApp;

public partial class App : Application
{
    private readonly DatabaseInitializer _databaseInitializer;
    private readonly AppShell _appShell;

    public App(DatabaseInitializer databaseInitializer, AppShell appShell)
    {
        InitializeComponent();

        _databaseInitializer = databaseInitializer;
        _appShell = appShell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Task.Run(async () =>
        {
            await _databaseInitializer.InitializeAsync();
        });

        return new Window(_appShell);
    }
}