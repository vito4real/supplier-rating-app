using SQLite;

namespace SupplierRatingApp.Models;

public class Supplier
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Notes { get; set; }
}