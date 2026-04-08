using SQLite;

namespace SupplierRatingApp.Models;

public class SupplierRating
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public int TotalParts { get; set; }

    public int GoodParts { get; set; }

    public int TotalDeliveries { get; set; }

    public int OnTimeDeliveries { get; set; }

    public double KpiPercent { get; set; }

    public double OtdPercent { get; set; }

    public double FinalRatingPercent { get; set; }

    [MaxLength(10)]
    public string CategoryCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public string RecommendedAction { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}