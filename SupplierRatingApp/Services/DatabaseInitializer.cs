using SupplierRatingApp.Data;

namespace SupplierRatingApp.Services;

public class DatabaseInitializer
{
    private readonly AppDatabase _appDatabase;

    public DatabaseInitializer(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task InitializeAsync()
    {
        await _appDatabase.InitAsync();
    }
}