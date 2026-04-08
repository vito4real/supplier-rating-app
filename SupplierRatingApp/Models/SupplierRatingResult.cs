namespace SupplierRatingApp.Models;

public class SupplierRatingResult
{
    public double KpiPercent { get; set; }

    public double OtdPercent { get; set; }

    public double FinalRatingPercent { get; set; }

    public string CategoryCode { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string RecommendedAction { get; set; } = string.Empty;
}