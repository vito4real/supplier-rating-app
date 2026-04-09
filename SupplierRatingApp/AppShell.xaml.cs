using SupplierRatingApp.Views;

namespace SupplierRatingApp;

public partial class AppShell : Shell
{
    public AppShell(SuppliersPage suppliersPage)
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(SupplierDetailsPage), typeof(SupplierDetailsPage));
        Routing.RegisterRoute(nameof(SupplierCalculationPage), typeof(SupplierCalculationPage));

        Items.Add(new ShellContent
        {
            Title = "Поставщики",
            Content = suppliersPage,
            Route = "SuppliersPage"
        });
    }
}