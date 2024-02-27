using Domain;
using Infrastructure.Dto;

namespace Infrastructure.Claims;

public class Mappers
{
    public static ClaimDto ToDto(ClaimCosmosEntity item)
    {
        return new ClaimDto
        {
            Id = Guid.Parse(item.Id),
            CoverId = item.CoverId,
            ClaimType = item.Type,
            Created = item.Created,
            DamageCost = item.DamageCost,
            Name = item.Name
        };
    }

    public static ClaimDto ToDto(Claim item)
    {
        return new ClaimDto
        {
            Id = item.Id,
            CoverId = item.CoverId,
            ClaimType = item.Type,
            Created = item.Created,
            DamageCost = item.DamageCost,
            Name = item.Name
        };
    }
}