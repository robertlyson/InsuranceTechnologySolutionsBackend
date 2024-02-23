using Claims.Application.Covers.Dto;
using Claims.Utils;

namespace Claims.Application.Covers;

public class Cover
{
    public Guid Id { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public CoverType Type { get; init; }
    public decimal Premium { get; init; }

    private Cover(Guid id, CreateCoverDto cover, decimal premium)
    {
        Id = id;
        StartDate = cover.StartDate!.Value;
        EndDate = cover.EndDate!.Value;
        Type = cover.CoverType!.Value;
        Premium = premium;
    }
    
    public static Result<Cover> Create(CreateCoverDto createCoverDto, IPremiumStrategy premiumStrategy)
    {
        if (createCoverDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Result<Cover>.Failure(CoverErrors.StartDateInPast);
        }

        if (!IsWithinOneYear(createCoverDto.StartDate!.Value, createCoverDto.EndDate!.Value))
        {
            return Result<Cover>.Failure(CoverErrors.ExceedOneYear);
        }

        var premium = premiumStrategy.Calculate(createCoverDto.StartDate!.Value, createCoverDto.EndDate!.Value,
            createCoverDto.CoverType!.Value);
        var id = Guid.NewGuid();
        return Result<Cover>.Success(new Cover(id, createCoverDto, premium));
    }
    
    private static bool IsWithinOneYear(DateOnly startDate, DateOnly endDate)
    {
        var duration = new DateTime(endDate.Year, endDate.Month, endDate.Day) -
                       new DateTime(startDate.Year, startDate.Month, startDate.Day);

        var daysInYear = DateTime.IsLeapYear(startDate.Year) ? 366 : 365;

        return duration.TotalDays <= daysInYear;
    }
}