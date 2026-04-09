using Microsoft.Extensions.DependencyInjection;
using SupplierRatingApp.ViewModels;

namespace SupplierRatingApp.Views;

public partial class SupplierDetailsPage : ContentPage
{
    private readonly SupplierDetailsViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public SupplierDetailsPage(SupplierDetailsViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync(int supplierId)
    {
        await _viewModel.LoadAsync(supplierId);
        Render();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.SupplierId > 0)
        {
            await _viewModel.LoadAsync(_viewModel.SupplierId);
            Render();
        }
    }

    private void Render()
    {
        SupplierNameLabel.Text = _viewModel.SupplierName;
        SupplierNotesLabel.Text = $"Комментарий: {_viewModel.SupplierNotes}";
        LastRatingLabel.Text = $"Последний рейтинг: {_viewModel.LastRatingText}";
        LastCategoryLabel.Text = $"Категория: {_viewModel.LastCategoryText}";
        LastCheckDateLabel.Text = $"Последняя оценка: {_viewModel.LastCheckDateText}";
        HistoryCollectionView.ItemsSource = _viewModel.RatingHistory;
    }

    private async void NewCalculationButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            var page = _serviceProvider.GetRequiredService<SupplierCalculationPage>();
            await page.InitializeAsync(_viewModel.SupplierId);
            await Shell.Current.Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }
}