namespace Domain;

public record CreateCover(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);