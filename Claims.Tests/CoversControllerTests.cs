using System.Net.Http.Json;
using System.Text.Json;
using Claims.Application.Covers;
using Claims.Controllers.Covers.Dto;

namespace Claims.Tests;

[TestFixture]
public class CoversControllerTests : BaseTest
{
    [Test]
    public async Task DeleteCover()
    {
        var application = base.Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto
        {
            CoverType = CoverType.Yacht,
            StartDate = DateOnly.FromDateTime(new DateTime(2020, 1, 1)),
            EndDate = DateOnly.FromDateTime(new DateTime(2021, 1, 1)),
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);
        var created = await createResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions());
        var getResponse = await client.GetAsync($"/covers/{created!.Id}");
        var deleteResponse = await client.DeleteAsync($"/covers/{created!.Id}");

        await Verify(new[] { createResponse, getResponse, deleteResponse });
    }

    [Test]
    public async Task GetClaim()
    {
        var application = base.Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto
        {
            CoverType = CoverType.Yacht,
            StartDate = DateOnly.FromDateTime(new DateTime(2024, 1, 1)),
            EndDate = DateOnly.FromDateTime(new DateTime(2025, 1, 1)),
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);
        var createdCover = await createResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions());
        var getResponse = await client.GetAsync($"/covers/{createdCover!.Id}");
        await Verify(getResponse);
    }

    [Test]
    public async Task ValidateCoversPost()
    {
        var application = base.Factory!;

        using var client = application.CreateClient();

        var payload = new CreateCoverDto()
        {
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);

        await Verify(createResponse);
    }
}