﻿using Claims.Controllers.Covers.Dto;

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
}