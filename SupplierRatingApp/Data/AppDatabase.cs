using SQLite;
using SupplierRatingApp.Models;

namespace SupplierRatingApp.Data;

public class AppDatabase
{
    private SQLiteAsyncConnection? _database;
    private bool _isInitialized;

    public async Task InitAsync()
    {
        if (_isInitialized && _database is not null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "supplier_rating.db3");

        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<Supplier>();
        await _database.CreateTableAsync<SupplierRating>();

        _isInitialized = true;
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (!_isInitialized || _database is null)
            await InitAsync();

        return _database!;
    }

    // Suppliers

    public async Task<List<Supplier>> GetSuppliersAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Supplier>()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Supplier>()
            .FirstOrDefaultAsync(x => x.Id == supplierId);
    }

    public async Task<int> SaveSupplierAsync(Supplier supplier)
    {
        var db = await GetDatabaseAsync();

        if (supplier.Id == 0)
            return await db.InsertAsync(supplier);

        return await db.UpdateAsync(supplier);
    }

    public async Task<int> DeleteSupplierAsync(Supplier supplier)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(supplier);
    }

    // SupplierRatings

    public async Task<List<SupplierRating>> GetRatingsBySupplierIdAsync(int supplierId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<SupplierRating>()
            .Where(x => x.SupplierId == supplierId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<SupplierRating?> GetLastRatingBySupplierIdAsync(int supplierId)
    {
        var db = await GetDatabaseAsync();
        var ratings = await db.Table<SupplierRating>()
            .Where(x => x.SupplierId == supplierId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return ratings.FirstOrDefault();
    }

    public async Task<int> SaveRatingAsync(SupplierRating rating)
    {
        var db = await GetDatabaseAsync();

        if (rating.Id == 0)
            return await db.InsertAsync(rating);

        return await db.UpdateAsync(rating);
    }

    public async Task<int> DeleteRatingAsync(SupplierRating rating)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(rating);
    }

    public string GetDatabasePath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, "supplier_rating.db3");
    }
}