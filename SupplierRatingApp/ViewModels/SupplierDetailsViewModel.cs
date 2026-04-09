using System.Collections.ObjectModel;
using SupplierRatingApp.Data;

namespace SupplierRatingApp.ViewModels;

public class SupplierDetailsViewModel : BaseViewModel
{
    private readonly AppDatabase _appDatabase;

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

    private string _supplierNotes = string.Empty;
    public string SupplierNotes
    {
        get => _supplierNotes;
        set => SetProperty(ref _supplierNotes, value);
    }

    private string _lastRatingText = "Нет оценок";
    public string LastRatingText
    {
        get => _lastRatingText;
        set => SetProperty(ref _lastRatingText, value);
    }

    private string _lastCategoryText = "-";
    public string LastCategoryText
    {
        get => _lastCategoryText;
        set => SetProperty(ref _lastCategoryText, value);
    }

    private string _lastCheckDateText = "-";
    public string LastCheckDateText
    {
        get => _lastCheckDateText;
        set => SetProperty(ref _lastCheckDateText, value);
    }

    public ObservableCollection<SupplierRatingHistoryItemViewModel> RatingHistory { get; } = new();

    public SupplierDetailsViewModel(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task LoadAsync(int supplierId)
    {
        SupplierId = supplierId;

        var supplier = await _appDatabase.GetSupplierByIdAsync(supplierId);
        if (supplier is null)
            throw new InvalidOperationException("Поставщик не найден.");

        SupplierName = supplier.Name;
        SupplierNotes = supplier.Notes ?? string.Empty;

        var lastRating = await _appDatabase.GetLastRatingBySupplierIdAsync(supplierId);

        LastRatingText = lastRating is null
            ? "Нет оценок"
            : $"{lastRating.FinalRatingPercent:F2} %";

        LastCategoryText = lastRating is null
            ? "-"
            : $"{lastRating.CategoryCode} — {lastRating.CategoryName}";

        LastCheckDateText = lastRating is null
            ? "-"
            : lastRating.CreatedAt.ToString("dd.MM.yyyy HH:mm");

        RatingHistory.Clear();

        var ratings = await _appDatabase.GetRatingsBySupplierIdAsync(supplierId);

        foreach (var rating in ratings)
        {
            RatingHistory.Add(new SupplierRatingHistoryItemViewModel
            {
                Id = rating.Id,
                CreatedAtText = rating.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                KpiText = $"{rating.KpiPercent:F2} %",
                OtdText = $"{rating.OtdPercent:F2} %",
                FinalRatingText = $"{rating.FinalRatingPercent:F2} %",
                CategoryText = $"{rating.CategoryCode} — {rating.CategoryName}",
                RecommendedActionText = rating.RecommendedAction
            });
        }
    }
}