namespace Claims.Application.Covers;

public class DefaultPremiumStrategy : IPremiumStrategy
{
    private const decimal BASE_RATE = 1250m;

    public decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var cumulativePremium = 0m;
        var multiplier = Multiplier(coverType);

        var premiumPerDay = BASE_RATE * multiplier;
        var insurancePeriod = (new DateTime(endDate.Year, endDate.Month, endDate.Day) -
                               new DateTime(startDate.Year, startDate.Month, startDate.Day)).Days + 1;

        var firstPeriod = Math.Min(insurancePeriod, 30);
        var secondPeriod = Math.Min(Math.Max(insurancePeriod - 30, 0), 150);
        var thirdPeriod = Math.Max(insurancePeriod - 180, 0);

        // First 30 days
        cumulativePremium += firstPeriod * premiumPerDay;

        // Next 150 days
        var discountForSecondPeriod = coverType == CoverType.Yacht ? 0.05m : 0.02m;
        cumulativePremium += secondPeriod * premiumPerDay * (1 - discountForSecondPeriod);

        // Remaining days
        var discountForThirdPeriod = coverType == CoverType.Yacht ? 0.03m : 0.01m;
        cumulativePremium += thirdPeriod * premiumPerDay * (1 - discountForSecondPeriod - discountForThirdPeriod);

        return cumulativePremium;
    }

    private static decimal Multiplier(CoverType coverType)
    {
        var multiplier = 1.3m;

        switch (coverType)
        {
            case CoverType.Yacht:
                multiplier = 1.1m;
                break;
            case CoverType.PassengerShip:
                multiplier = 1.2m;
                break;
            case CoverType.Tanker:
                multiplier = 1.5m;
                break;
            default:
                multiplier = 1.3m;
                break;
        }

        return multiplier;
    }
}