using JetBrains.Annotations;

namespace Domain;

[UsedImplicitly]
public class CoverErrors
{
    public static readonly Error StartDateInPast = new(
        "Cover.StartDateInPast", "StartDate cannot be in the past.");

    public static readonly Error ExceedOneYear = new(
        "Cover.ExceedOneYear", "Total insurance period cannot exceed 1 year.");
}