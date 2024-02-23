using Claims.Application.Claims.Dto;
using Claims.Application.Covers;
using Claims.Utils;

namespace Claims.Application.Claims;

public class Claim
{
    public string Name { get; init; }
    public Guid Id { get; init; }
    public ClaimType Type { get; init; }
    public Guid CoverId { get; init; }
    public DateTime Created { get; init; }
    public decimal DamageCost { get; init; }
    
    private Claim(Guid id, CreateClaimDto claim)
    {
        Id = id;
        Name = claim.Name!;
        Type = claim.ClaimType!.Value;
        CoverId = claim.CoverId!.Value;
        Created = claim.Created!.Value;
        DamageCost = claim.DamageCost!.Value;
    }

    public static Result<Claim> Create(CreateClaimDto createClaimDto, CoverCosmosEntity cover)
    {
        if (createClaimDto.DamageCost > 100_000)
        {
            return Result<Claim>.Failure(ClaimErrors.ExceededDamageCost);
        }

        var created = DateOnly.FromDateTime(createClaimDto.Created!.Value);
        if (created < cover.StartDate || created > cover.EndDate)
        {
            return Result<Claim>.Failure(ClaimErrors.CreatedDateNotWithinCoverPeriod);
        }
        
        var id = Guid.NewGuid();

        return Result<Claim>.Success(new Claim(id, createClaimDto));
    }
}