﻿using System.Net.Http.Json;
using Domain;
using Infrastructure.Dto;

namespace Claims.Tests;

[TestFixture]
public class CoversControllerTests : BaseTest
{
    [Test]
    public async Task DeleteCover()
    {
        var application = Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto
        {
            CoverType = CoverType.Yacht,
            StartDate = DateOnly.FromDateTime(new DateTime(2030, 1, 1)),
            EndDate = DateOnly.FromDateTime(new DateTime(2030, 1, 1).AddMonths(1))
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);
        var created = await createResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions());
        var getResponse = await client.GetAsync($"/covers/{created!.Id}");
        var deleteResponse = await client.DeleteAsync($"/covers/{created!.Id}");
        var getDeletedResponse = await client.GetAsync($"/covers/{created!.Id}");

        await Verify(new[] { createResponse, getResponse, deleteResponse, getDeletedResponse });
    }

    [Test]
    public async Task GetClaim()
    {
        var application = Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto
        {
            CoverType = CoverType.Yacht,
            StartDate = DateOnly.FromDateTime(new DateTime(2030, 1, 1)),
            EndDate = DateOnly.FromDateTime(new DateTime(2030, 1, 1).AddMonths(1))
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);
        var createdCover = await createResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions());
        var getResponse = await client.GetAsync($"/covers/{createdCover!.Id}");
        await Verify(getResponse);
    }

    [Test]
    public async Task ValidateCoversPost()
    {
        var application = Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto();
        var createResponse = await client.PostAsJsonAsync("/covers", payload);

        await Verify(createResponse);
    }
}