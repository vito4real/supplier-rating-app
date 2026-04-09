namespace SupplierRatingApp.ViewModels;

public class SupplierRatingTrendItemViewModel
{
    public string DateText { get; set; } = "-";

    public string RatingText { get; set; } = "-";

    public double RatingValue { get; set; }

    public double BarWidth { get; set; }
}