namespace Domain;

public class Claim
{
    public string Name { get; init; }
    public Guid Id { get; init; }
    public ClaimType Type { get; init; }
    public Guid CoverId { get; init; }
    public DateTime Created { get; init; }
    public decimal DamageCost { get; init; }
    
    private Claim(Guid id, CreateClaim claim)
    {
        Id = id;
        Name = claim.Name;
        Type = claim.ClaimType;
        CoverId = claim.CoverId;
        Created = claim.Created;
        DamageCost = claim.DamageCost;
    }

    public static Result<Claim> Create(CreateClaim createClaim, DateOnly coverStartDate, DateOnly coverEndDate)
    {
        if (createClaim.DamageCost > 100_000)
        {
            return Result<Claim>.Failure(ClaimErrors.ExceededDamageCost);
        }

        var created = DateOnly.FromDateTime(createClaim.Created);
        if (created < coverStartDate || created > coverEndDate)
        {
            return Result<Claim>.Failure(ClaimErrors.CreatedDateNotWithinCoverPeriod);
        }
        
        var id = Guid.NewGuid();

        return Result<Claim>.Success(new Claim(id, createClaim));
    }
}