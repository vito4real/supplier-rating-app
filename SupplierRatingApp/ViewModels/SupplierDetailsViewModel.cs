using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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

    private Color _lastCategoryColor = Colors.Gray;
    public Color LastCategoryColor
    {
        get => _lastCategoryColor;
        set => SetProperty(ref _lastCategoryColor, value);
    }

    private string _lastCheckDateText = "-";
    public string LastCheckDateText
    {
        get => _lastCheckDateText;
        set => SetProperty(ref _lastCheckDateText, value);
    }

    private ISeries[] _ratingSeries = Array.Empty<ISeries>();
    public ISeries[] RatingSeries
    {
        get => _ratingSeries;
        set => SetProperty(ref _ratingSeries, value);
    }

    private Axis[] _xAxes = Array.Empty<Axis>();
    public Axis[] XAxes
    {
        get => _xAxes;
        set => SetProperty(ref _xAxes, value);
    }

    private Axis[] _yAxes = Array.Empty<Axis>();
    public Axis[] YAxes
    {
        get => _yAxes;
        set => SetProperty(ref _yAxes, value);
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

        LastCategoryColor = GetCategoryColor(lastRating?.CategoryCode);

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

        var chartRatings = ratings
            .OrderBy(r => r.CreatedAt)
            .ToList();

        var values = chartRatings
            .Select(r => r.FinalRatingPercent)
            .ToArray();

        var labels = chartRatings
            .Select(r => r.CreatedAt.ToString("dd.MM"))
            .ToArray();

        RatingSeries = values.Length == 0
            ? Array.Empty<ISeries>()
            : new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = values,
                    Name = "Final Rating",
                    GeometrySize = 10,
                    LineSmoothness = 0.6
                }
            };

        XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsRotation = 0
            }
        };

        YAxes = new[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                Name = "Rating, %",
                Labeler = value => $"{value:F0}%"
            }
        };
    }

    public async Task UpdateSupplierAsync(string name, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название поставщика не может быть пустым.");

        await _appDatabase.UpdateSupplierAsync(
            SupplierId,
            name.Trim(),
            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim());

        await LoadAsync(SupplierId);
    }

    public async Task DeleteSupplierAsync()
    {
        await _appDatabase.DeleteRatingsBySupplierIdAsync(SupplierId);
        await _appDatabase.DeleteSupplierByIdAsync(SupplierId);
    }

    private Color GetCategoryColor(string? categoryCode)
    {
        return categoryCode switch
        {
            "A" => Colors.Blue,
            "B" => Colors.Green,
            "C" => Colors.Orange,
            "D" => Colors.Red,
            _ => Colors.Gray
        };
    }
}