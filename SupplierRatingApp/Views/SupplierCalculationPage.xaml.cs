using SupplierRatingApp.ViewModels;

namespace SupplierRatingApp.Views;

public partial class SupplierCalculationPage : ContentPage
{
    private readonly SupplierCalculationViewModel _viewModel;

    public SupplierCalculationPage(SupplierCalculationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }

    public async Task InitializeAsync(int supplierId)
    {
        await _viewModel.LoadSupplierAsync(supplierId);
        SupplierNameLabel.Text = $"Расчет рейтинга: {_viewModel.SupplierName}";
    }

    private async void CalculateButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            _viewModel.TotalParts = TotalPartsEntry.Text ?? string.Empty;
            _viewModel.GoodParts = GoodPartsEntry.Text ?? string.Empty;
            _viewModel.TotalDeliveries = TotalDeliveriesEntry.Text ?? string.Empty;
            _viewModel.OnTimeDeliveries = OnTimeDeliveriesEntry.Text ?? string.Empty;

            _viewModel.Calculate();

            KpiLabel.Text = $"KPI: {_viewModel.KpiText}";
            OtdLabel.Text = $"OTD: {_viewModel.OtdText}";
            FinalRatingLabel.Text = $"Итоговый рейтинг: {_viewModel.FinalRatingText}";
            CategoryLabel.Text = $"Категория: {_viewModel.CategoryText}";
            ActionLabel.Text = $"Действие: {_viewModel.RecommendedActionText}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }

    private async void SaveButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            _viewModel.TotalParts = TotalPartsEntry.Text ?? string.Empty;
            _viewModel.GoodParts = GoodPartsEntry.Text ?? string.Empty;
            _viewModel.TotalDeliveries = TotalDeliveriesEntry.Text ?? string.Empty;
            _viewModel.OnTimeDeliveries = OnTimeDeliveriesEntry.Text ?? string.Empty;

            await _viewModel.SaveAsync();

            await DisplayAlert("Успех", "Рейтинг сохранен.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }
}