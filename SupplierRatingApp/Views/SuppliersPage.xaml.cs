using Microsoft.Extensions.DependencyInjection;
using SupplierRatingApp.ViewModels;

namespace SupplierRatingApp.Views;

public partial class SuppliersPage : ContentPage
{
    private readonly SuppliersViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public SuppliersPage(SuppliersViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.ToString(), "OK");
        }
    }

    private async void AddSupplierButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            var supplierName = await DisplayPromptAsync(
                "Новый поставщик",
                "Введите название поставщика:",
                accept: "Сохранить",
                cancel: "Отмена");

            if (string.IsNullOrWhiteSpace(supplierName))
                return;

            await _viewModel.AddSupplierAsync(supplierName);
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.ToString(), "OK");
        }
    }

    private async void RefreshButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.ToString(), "OK");
        }
    }

    private async void SuppliersCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as SupplierListItemViewModel;
            if (selectedItem is null)
                return;

            var page = _serviceProvider.GetRequiredService<SupplierCalculationPage>();
            await page.InitializeAsync(selectedItem.Id);

            if (sender is CollectionView collectionView)
                collectionView.SelectedItem = null;

            await Shell.Current.Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.ToString(), "OK");
        }
    }
}