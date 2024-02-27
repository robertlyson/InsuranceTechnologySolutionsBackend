namespace Domain;

public class Cover
{
    public Guid Id { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public CoverType Type { get; init; }
    public decimal Premium { get; init; }

    private Cover(Guid id, CreateCover cover, decimal premium)
    {
        Id = id;
        StartDate = cover.StartDate;
        EndDate = cover.EndDate;
        Type = cover.CoverType;
        Premium = premium;
    }
    
    public static Result<Cover> Create(CreateCover createCover, IPremiumStrategy premiumStrategy)
    {
        if (createCover.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Result<Cover>.Failure(CoverErrors.StartDateInPast);
        }

        if (!IsWithinOneYear(createCover.StartDate, createCover.EndDate))
        {
            return Result<Cover>.Failure(CoverErrors.ExceedOneYear);
        }

        var premium = premiumStrategy.Calculate(createCover.StartDate, createCover.EndDate,
            createCover.CoverType);
        var id = Guid.NewGuid();
        return Result<Cover>.Success(new Cover(id, createCover, premium));
    }
    
    private static bool IsWithinOneYear(DateOnly startDate, DateOnly endDate)
    {
        var duration = new DateTime(endDate.Year, endDate.Month, endDate.Day) -
                       new DateTime(startDate.Year, startDate.Month, startDate.Day);

        var daysInYear = DateTime.IsLeapYear(startDate.Year) ? 366 : 365;

        return duration.TotalDays <= daysInYear;
    }
}