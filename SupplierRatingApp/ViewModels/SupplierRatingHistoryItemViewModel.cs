namespace SupplierRatingApp.ViewModels;

public class SupplierRatingHistoryItemViewModel
{
    public int Id { get; set; }

    public string CreatedAtText { get; set; } = "-";

    public string KpiText { get; set; } = "-";

    public string OtdText { get; set; } = "-";

    public string FinalRatingText { get; set; } = "-";

    public string CategoryText { get; set; } = "-";

    public string RecommendedActionText { get; set; } = "-";
}