using SupplierRatingApp.Constants;
using SupplierRatingApp.Models;

namespace SupplierRatingApp.Services;

public class SupplierRatingCalculator
{
    public SupplierRatingResult Calculate(
        int totalParts,
        int goodParts,
        int totalDeliveries,
        int onTimeDeliveries)
    {
        Validate(totalParts, goodParts, totalDeliveries, onTimeDeliveries);

        double kpi = (double)goodParts / totalParts * 100.0;
        double otd = (double)onTimeDeliveries / totalDeliveries * 100.0;
        double finalRating = (kpi * 0.7) + (otd * 0.3);

        var (categoryCode, categoryName, recommendedAction) = DetermineCategory(finalRating);

        return new SupplierRatingResult
        {
            KpiPercent = Math.Round(kpi, 2),
            OtdPercent = Math.Round(otd, 2),
            FinalRatingPercent = Math.Round(finalRating, 2),
            CategoryCode = categoryCode,
            CategoryName = categoryName,
            RecommendedAction = recommendedAction
        };
    }

    private static void Validate(
        int totalParts,
        int goodParts,
        int totalDeliveries,
        int onTimeDeliveries)
    {
        if (totalParts <= 0)
            throw new ArgumentException("Общее количество деталей должно быть больше 0.");

        if (goodParts < 0)
            throw new ArgumentException("Количество годных деталей не может быть отрицательным.");

        if (goodParts > totalParts)
            throw new ArgumentException("Количество годных деталей не может быть больше общего количества деталей.");

        if (totalDeliveries <= 0)
            throw new ArgumentException("Общее количество партий должно быть больше 0.");

        if (onTimeDeliveries < 0)
            throw new ArgumentException("Количество партий, пришедших в срок, не может быть отрицательным.");

        if (onTimeDeliveries > totalDeliveries)
            throw new ArgumentException("Количество партий, пришедших в срок, не может быть больше общего количества партий.");
    }

    private static (string CategoryCode, string CategoryName, string RecommendedAction) DetermineCategory(double finalRating)
    {
        if (finalRating >= 90)
        {
            return (
                SupplierRatingCategories.CategoryA,
                "Элитный поставщик",
                "Увеличивать объёмы заказов, работать по предоплате."
            );
        }

        if (finalRating >= 75)
        {
            return (
                SupplierRatingCategories.CategoryB,
                "Стабильный поставщик",
                "Стандартный входной контроль, периодический мониторинг."
            );
        }

        if (finalRating >= 50)
        {
            return (
                SupplierRatingCategories.CategoryC,
                "Проблемный поставщик",
                "Выставить официальную претензию, перевести на оплату по факту приёмки, требовать отчет о корректирующих действиях."
            );
        }

        return (
            SupplierRatingCategories.CategoryD,
            "Недопустимый поставщик",
            "Прекратить размещение заказов."
        );
    }
}