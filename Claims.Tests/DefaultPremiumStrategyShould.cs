using Claims.Application.Covers;
using Shouldly;

namespace Claims.Tests;

[TestFixture]
public class DefaultPremiumStrategyShould
{
    [Test]
    [TestCase(CoverType.Yacht, 41250)]
    [TestCase(CoverType.PassengerShip, 45000)]
    [TestCase(CoverType.Tanker, 56250)]
    [TestCase(CoverType.BulkCarrier, 48750)]
    [TestCase(CoverType.ContainerShip, 48750)]
    public void CalculateInsuranceForFirst30Days(CoverType type, decimal expected)
    {
        var premium = CalculatePremium(new DateTime(2023, 1, 1), new DateTime(2023, 1, 30), type);
        premium.ShouldBe(expected);
    }

    [Test]
    [TestCase(CoverType.Yacht, 237187.5)]
    [TestCase(CoverType.PassengerShip, 265500)]
    [TestCase(CoverType.Tanker, 331875)]
    [TestCase(CoverType.BulkCarrier, 287625)]
    [TestCase(CoverType.ContainerShip, 287625)]
    public void CalculateInsuranceForNext150Days(CoverType type, decimal expected)
    {
        var premium = CalculatePremium(new DateTime(2023, 1, 1), new DateTime(2023, 6, 29), type);
        premium.ShouldBe(expected);
    }

    [Test]
    [TestCase(CoverType.Yacht, 472477.5)]
    [TestCase(CoverType.PassengerShip, 536130)]
    [TestCase(CoverType.Tanker, 670162.5)]
    [TestCase(CoverType.BulkCarrier, 580807.5)]
    [TestCase(CoverType.ContainerShip, 580807.5)]
    public void CalculateInsuranceForRemainingDays(CoverType type, decimal expected)
    {
        var premium = CalculatePremium(new DateTime(2023, 1, 1), new DateTime(2024, 1, 1), type);
        premium.ShouldBe(expected);
    }
    
    private decimal CalculatePremium(DateTime startDate, DateTime endDate, CoverType type)
    {
        var calculator = new DefaultPremiumStrategy();
        return calculator.Calculate(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), type);
    }
}