using Claims.Application.Covers;
using Domain;
using Shouldly;

namespace Claims.Tests;

[TestFixture]
public class CoverShould
{
    [Test]
    public void BeAbleToCreateCover()
    {
        var actual = Cover.Create(
            new CreateCover(DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddYears(1), CoverType.Tanker),
            new DefaultPremiumStrategy());
        
        actual.IsSuccess.ShouldBe(true);
    }
    
    [Test]
    public void NotCreateCoverWithDateInPast()
    {
        var actual = Cover.Create(
            new CreateCover(DateOnly.FromDateTime(DateTime.Now).AddDays(-1), DateOnly.FromDateTime(DateTime.Now).AddYears(1), CoverType.Tanker),
            new DefaultPremiumStrategy());
        
        actual.Error.ShouldBe(CoverErrors.StartDateInPast);
    }
    
    [Test]
    public void NotCreateCoverWithInsurancePeriodExceedOneYear()
    {
        var actual = Cover.Create(
            new CreateCover(DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddYears(2), CoverType.Tanker),
            new DefaultPremiumStrategy());
            new DefaultPremiumStrategy();
        
        actual.Error.ShouldBe(CoverErrors.ExceedOneYear);
    }
}