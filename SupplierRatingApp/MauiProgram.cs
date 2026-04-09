using Microsoft.Extensions.Logging;
using SupplierRatingApp.Data;
using SupplierRatingApp.Services;
using SupplierRatingApp.ViewModels;
using SupplierRatingApp.Views;

namespace SupplierRatingApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<DatabaseInitializer>();
        builder.Services.AddSingleton<SupplierRatingCalculator>();
        builder.Services.AddSingleton<SuppliersViewModel>();
        builder.Services.AddSingleton<SuppliersPage>();
        builder.Services.AddTransient<SupplierDetailsViewModel>();
        builder.Services.AddTransient<SupplierDetailsPage>();
        builder.Services.AddTransient<SupplierCalculationViewModel>();
        builder.Services.AddTransient<SupplierCalculationPage>();
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}