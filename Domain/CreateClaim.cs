namespace Domain;

public record CreateClaim(string Name, DateTime Created, decimal DamageCost, ClaimType ClaimType, Guid CoverId);