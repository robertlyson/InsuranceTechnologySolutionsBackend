namespace Claims.Application.Covers;

public interface IPremiumStrategy
{
    decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType);
}