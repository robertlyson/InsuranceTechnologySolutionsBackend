using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Controllers.Claims.Dto;
using NUnit.Framework;
using VerifyTests;

namespace Claims.Tests
{
    [TestFixture]
    public class ClaimsControllerTests : BaseTest
    {
        [Test]
        public async Task Get_Claims()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();

            var response = await client.GetAsync("/claims");

            response.EnsureSuccessStatusCode();

            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
        }

        [Test]
        public async Task GetClaim()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();

            var payload = new CreateClaimDto
            {
                CoverId = Guid.NewGuid(), 
                Created = DateTime.UtcNow, 
                Name = "EF3EB3FE-8083-4572-B995-7EAF16F5EC70",
                ClaimType = ClaimType.Fire, 
                DamageCost = decimal.One
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);
            var createdClaim = await createResponse.Content.ReadFromJsonAsync<Claim>(SerializerOptions());
            var getResponse = await client.GetAsync($"/claims/{createdClaim!.Id}");
            await Verify(getResponse);
        }

        [Test]
        public async Task DeleteClaim()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();

            var payload = new CreateClaimDto
            {
                CoverId = Guid.NewGuid(), 
                Created = DateTime.UtcNow, 
                Name = "2F2743A9-0D5E-45F1-8134-39487EC6CFE6",
                ClaimType = ClaimType.Fire, 
                DamageCost = decimal.One
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);
            var createdClaim = await createResponse.Content.ReadFromJsonAsync<Claim>(SerializerOptions());
            var getResponse = await client.GetAsync($"/claims/{createdClaim!.Id}");
            var deleteResponse = await client.DeleteAsync($"/claims/{createdClaim!.Id}");

            await Verify(new[] { createResponse, getResponse, deleteResponse });
        }

        [Test]
        public async Task ValidateClaimsPost()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();

            var payload = new CreateClaimDto
            {
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);

            await Verify(createResponse);
        }
    }
}
