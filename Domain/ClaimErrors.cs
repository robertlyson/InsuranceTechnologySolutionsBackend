using System.Globalization;
using JetBrains.Annotations;

namespace Domain;

[UsedImplicitly]
public class ClaimErrors
{
    public static readonly Error ExceededDamageCost = new(
        "Claim.ExceededDamageCost", $"DamageCost cannot exceed {new decimal(100_000).ToString(CultureInfo.InvariantCulture)}.");
    
    public static readonly Error CreatedDateNotWithinCoverPeriod = new(
        "Claim.CreatedDateNotWithinCoverPeriod", "Created date must be within the period of the related Cover.");
    
    public static readonly Error CoverNotFound = new(
        "Claim.CoverNotFound", "The related Cover could not be found.");
}