using Claims.Application.Covers.Dto;

namespace Claims.Application.Covers;

public static class Mappers
{
    public static CoverDto ToDto(CoverCosmosEntity coverCosmosEntity)
    {
        return new CoverDto
        {
            Id = coverCosmosEntity.Id,
            StartDate = coverCosmosEntity.StartDate,
            EndDate = coverCosmosEntity.EndDate,
            CoverType = coverCosmosEntity.Type,
            Premium = coverCosmosEntity.Premium,
        };
    }
    
    public static CoverDto ToDto(Cover cover)
    {
        return new CoverDto
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            CoverType = cover.Type,
            Premium = cover.Premium,
        };
    }
}