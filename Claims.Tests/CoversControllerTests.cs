using System.Net.Http.Json;
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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
        };
        var createResponse = await client.PostAsJsonAsync("/covers", payload);
        var created = await createResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions());
        var getResponse = await client.GetAsync($"/covers/{created!.Id}");
        var deleteResponse = await client.DeleteAsync($"/covers/{created!.Id}");

        await Verify(new[] { createResponse, getResponse, deleteResponse });
    }
}