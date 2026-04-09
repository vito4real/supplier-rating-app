using System.Collections.ObjectModel;
using SupplierRatingApp.Data;
using SupplierRatingApp.Models;

namespace SupplierRatingApp.ViewModels;

public class SuppliersViewModel : BaseViewModel
{
    private readonly AppDatabase _appDatabase;

    private readonly List<SupplierListItemViewModel> _allSuppliers = new();

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public ObservableCollection<SupplierListItemViewModel> Suppliers { get; } = new();

    public SuppliersViewModel(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _allSuppliers.Clear();

            var suppliers = await _appDatabase.GetSuppliersAsync();

            foreach (var supplier in suppliers)
            {
                var lastRating = await _appDatabase.GetLastRatingBySupplierIdAsync(supplier.Id);

                _allSuppliers.Add(new SupplierListItemViewModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Notes = supplier.Notes ?? string.Empty,
                    LastRatingText = lastRating is null
                        ? "Нет оценок"
                        : $"{lastRating.FinalRatingPercent:F2} %",
                    CategoryText = lastRating is null
                        ? "-"
                        : $"{lastRating.CategoryCode} — {lastRating.CategoryName}",
                    LastCheckDateText = lastRating is null
                        ? "-"
                        : lastRating.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                });
            }

            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task AddSupplierAsync(string name, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название поставщика не может быть пустым.");

        var supplier = new Supplier
        {
            Name = name.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await _appDatabase.SaveSupplierAsync(supplier);
    }

    private void ApplyFilter()
    {
        Suppliers.Clear();

        IEnumerable<SupplierListItemViewModel> filtered = _allSuppliers;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim();

            filtered = filtered.Where(x =>
                x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.Notes.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var supplier in filtered)
        {
            Suppliers.Add(supplier);
        }
    }
}