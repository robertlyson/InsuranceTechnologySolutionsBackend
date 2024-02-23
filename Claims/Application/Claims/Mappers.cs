using Claims.Application.Claims.Dto;

namespace Claims.Application.Claims;

public class Mappers
{
    public static ClaimDto ToDto(Claim item)
    {
        return new ClaimDto
        {
            Id = Guid.Parse(item.Id),
            CoverId = Guid.Parse(item.CoverId),
            ClaimType = item.Type,
            Created = item.Created,
            DamageCost = item.DamageCost,
            Name = item.Name
        };
    }
}