using Claims.Application.Covers;
using Claims.Application.Covers.Dto;
using Shouldly;

namespace Claims.Tests;

[TestFixture]
public class CoverShould
{
    [Test]
    public void BeAbleToCreateCover()
    {
        var actual = Cover.Create(
            new CreateCoverDto
            {
                CoverType = CoverType.Tanker, 
                StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                EndDate = DateOnly.FromDateTime(DateTime.Now).AddYears(1)
            },
            new DefaultPremiumStrategy());
        
        actual.IsSuccess.ShouldBe(true);
    }
    
    [Test]
    public void NotCreateCoverWithDateInPast()
    {
        var actual = Cover.Create(
            new CreateCoverDto
            {
                CoverType = CoverType.Tanker, StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
                EndDate = DateOnly.FromDateTime(DateTime.Now).AddYears(1)
            },
            new DefaultPremiumStrategy());
        
        actual.Error.ShouldBe(CoverErrors.StartDateInPast);
    }
    
    [Test]
    public void NotCreateCoverWithInsurancePeriodExceedOneYear()
    {
        var actual = Cover.Create(
            new CreateCoverDto
            {
                CoverType = CoverType.Tanker, StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                EndDate = DateOnly.FromDateTime(DateTime.Now).AddYears(2)
            },
            new DefaultPremiumStrategy());
        
        actual.Error.ShouldBe(CoverErrors.ExceedOneYear);
    }
}