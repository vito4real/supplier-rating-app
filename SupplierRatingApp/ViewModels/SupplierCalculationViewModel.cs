using SupplierRatingApp.Data;
using SupplierRatingApp.Models;
using SupplierRatingApp.Services;

namespace SupplierRatingApp.ViewModels;

public class SupplierCalculationViewModel : BaseViewModel
{
    private readonly AppDatabase _appDatabase;
    private readonly SupplierRatingCalculator _calculator;

    private int _supplierId;
    public int SupplierId
    {
        get => _supplierId;
        set => SetProperty(ref _supplierId, value);
    }

    private string _supplierName = string.Empty;
    public string SupplierName
    {
        get => _supplierName;
        set => SetProperty(ref _supplierName, value);
    }

    private string _totalParts = string.Empty;
    public string TotalParts
    {
        get => _totalParts;
        set => SetProperty(ref _totalParts, value);
    }

    private string _goodParts = string.Empty;
    public string GoodParts
    {
        get => _goodParts;
        set => SetProperty(ref _goodParts, value);
    }

    private string _totalDeliveries = string.Empty;
    public string TotalDeliveries
    {
        get => _totalDeliveries;
        set => SetProperty(ref _totalDeliveries, value);
    }

    private string _onTimeDeliveries = string.Empty;
    public string OnTimeDeliveries
    {
        get => _onTimeDeliveries;
        set => SetProperty(ref _onTimeDeliveries, value);
    }

    private string _kpiText = "-";
    public string KpiText
    {
        get => _kpiText;
        set => SetProperty(ref _kpiText, value);
    }

    private string _otdText = "-";
    public string OtdText
    {
        get => _otdText;
        set => SetProperty(ref _otdText, value);
    }

    private string _finalRatingText = "-";
    public string FinalRatingText
    {
        get => _finalRatingText;
        set => SetProperty(ref _finalRatingText, value);
    }

    private string _categoryText = "-";
    public string CategoryText
    {
        get => _categoryText;
        set => SetProperty(ref _categoryText, value);
    }

    private string _recommendedActionText = "-";
    public string RecommendedActionText
    {
        get => _recommendedActionText;
        set => SetProperty(ref _recommendedActionText, value);
    }

    public SupplierRatingResult? LastCalculationResult { get; private set; }

    public SupplierCalculationViewModel(AppDatabase appDatabase, SupplierRatingCalculator calculator)
    {
        _appDatabase = appDatabase;
        _calculator = calculator;
    }

    public async Task LoadSupplierAsync(int supplierId)
    {
        var supplier = await _appDatabase.GetSupplierByIdAsync(supplierId);

        if (supplier is null)
            throw new InvalidOperationException("Поставщик не найден.");

        SupplierId = supplier.Id;
        SupplierName = supplier.Name;
    }

    public void Calculate()
    {
        if (!int.TryParse(TotalParts, out var totalParts))
            throw new ArgumentException("Введите корректное общее количество деталей.");

        if (!int.TryParse(GoodParts, out var goodParts))
            throw new ArgumentException("Введите корректное количество годных деталей.");

        if (!int.TryParse(TotalDeliveries, out var totalDeliveries))
            throw new ArgumentException("Введите корректное общее количество партий.");

        if (!int.TryParse(OnTimeDeliveries, out var onTimeDeliveries))
            throw new ArgumentException("Введите корректное количество партий, пришедших в срок.");

        var result = _calculator.Calculate(totalParts, goodParts, totalDeliveries, onTimeDeliveries);
        LastCalculationResult = result;

        KpiText = $"{result.KpiPercent:F2} %";
        OtdText = $"{result.OtdPercent:F2} %";
        FinalRatingText = $"{result.FinalRatingPercent:F2} %";
        CategoryText = $"{result.CategoryCode} — {result.CategoryName}";
        RecommendedActionText = result.RecommendedAction;
    }

    public async Task SaveAsync()
    {
        if (LastCalculationResult is null)
            throw new InvalidOperationException("Сначала выполните расчет.");

        if (!int.TryParse(TotalParts, out var totalParts))
            throw new ArgumentException("Введите корректное общее количество деталей.");

        if (!int.TryParse(GoodParts, out var goodParts))
            throw new ArgumentException("Введите корректное количество годных деталей.");

        if (!int.TryParse(TotalDeliveries, out var totalDeliveries))
            throw new ArgumentException("Введите корректное общее количество партий.");

        if (!int.TryParse(OnTimeDeliveries, out var onTimeDeliveries))
            throw new ArgumentException("Введите корректное количество партий, пришедших в срок.");

        var rating = new SupplierRating
        {
            SupplierId = SupplierId,
            TotalParts = totalParts,
            GoodParts = goodParts,
            TotalDeliveries = totalDeliveries,
            OnTimeDeliveries = onTimeDeliveries,
            KpiPercent = LastCalculationResult.KpiPercent,
            OtdPercent = LastCalculationResult.OtdPercent,
            FinalRatingPercent = LastCalculationResult.FinalRatingPercent,
            CategoryCode = LastCalculationResult.CategoryCode,
            CategoryName = LastCalculationResult.CategoryName,
            RecommendedAction = LastCalculationResult.RecommendedAction,
            CreatedAt = DateTime.Now
        };

        await _appDatabase.SaveRatingAsync(rating);
    }
}