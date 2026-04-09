namespace SupplierRatingApp.ViewModels;

public class SupplierListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string LastRatingText { get; set; } = "Нет оценок";

    public string CategoryText { get; set; } = "-";

    public string LastCheckDateText { get; set; } = "-";

    public Color CategoryColor { get; set; } = Colors.Gray;
}