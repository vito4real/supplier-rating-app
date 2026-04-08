using System.Collections.ObjectModel;
using SupplierRatingApp.Data;
using SupplierRatingApp.Models;

namespace SupplierRatingApp.ViewModels;

public class SuppliersViewModel : BaseViewModel
{
    private readonly AppDatabase _appDatabase;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
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

            Suppliers.Clear();

            var suppliers = await _appDatabase.GetSuppliersAsync();

            foreach (var supplier in suppliers)
            {
                var lastRating = await _appDatabase.GetLastRatingBySupplierIdAsync(supplier.Id);

                Suppliers.Add(new SupplierListItemViewModel
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
}