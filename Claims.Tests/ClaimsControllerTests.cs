using System.Net.Http.Json;
using Claims.Application.Claims;
using Claims.Application.Claims.Dto;
using Claims.Application.Covers;
using Claims.Application.Covers.Dto;

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
            
            var cover = await CreateValidCover(client);

            var payload = new CreateClaimDto
            {
                CoverId = cover.Id, 
                Created = DateTime.UtcNow, 
                Name = "EF3EB3FE-8083-4572-B995-7EAF16F5EC70",
                ClaimType = ClaimType.Fire, 
                DamageCost = decimal.One
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);
            var createdClaim = await createResponse.Content.ReadFromJsonAsync<ClaimDto>(SerializerOptions());
            var getResponse = await client.GetAsync($"/claims/{createdClaim!.Id}");
            await Verify(getResponse);
        }

        [Test]
        public async Task GetNonExistingClaim()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();
            
            var getResponse = await client.GetAsync("/claims/non_existing");
            await Verify(getResponse);
        }

        [Test]
        public async Task DeleteClaim()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();
            
            var cover = await CreateValidCover(client);

            var payload = new CreateClaimDto
            {
                CoverId = cover.Id, 
                Created = DateTime.UtcNow, 
                Name = "2F2743A9-0D5E-45F1-8134-39487EC6CFE6",
                ClaimType = ClaimType.Fire, 
                DamageCost = decimal.One
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);
            var createdClaim = await createResponse.Content.ReadFromJsonAsync<ClaimDto>(SerializerOptions());
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

        [Test]
        public async Task CantCreateClaimWithInvalidDamage()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();
            
            var cover = await CreateValidCover(client);

            var payload = new CreateClaimDto
            {
                CoverId = cover.Id, 
                Created = DateTime.UtcNow, 
                Name = "A2ED5661-7F81-434E-BDBF-56CF44AA1318",
                ClaimType = ClaimType.Fire, 
                DamageCost = 100_001,
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);

            await Verify(createResponse);
        }

        [Test]
        public async Task BadRequestWhenCoverDoesntExist()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();

            var payload = new CreateClaimDto
            {
                CoverId = Guid.NewGuid(), 
                Created = DateTime.UtcNow, 
                Name = "9849F256-63F3-4D2E-8B5A-708A65526B51",
                ClaimType = ClaimType.Fire, 
                DamageCost = 100_001,
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);

            await Verify(createResponse);
        }

        [Test]
        public async Task CreatedDateMustBeWithinThePeriodOfTheRelatedCover()
        {
            var application = base.Factory!;

            using var client = application.CreateClient();
            
            var cover = await CreateValidCover(client);

            var payload = new CreateClaimDto
            {
                CoverId = cover.Id, 
                Created = new DateTime(2000, 1, 1), 
                Name = "9849F256-63F3-4D2E-8B5A-708A65526B51",
                ClaimType = ClaimType.Fire, 
                DamageCost = 100,
            };
            var createResponse = await client.PostAsJsonAsync("/claims", payload);

            await Verify(createResponse);
        }

        private static async Task<CoverDto> CreateValidCover(HttpClient client)
        {
            var coverPayload = new CreateCoverDto
            {
                CoverType = CoverType.Yacht,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            };
            var createCoverResponse = await client.PostAsJsonAsync("/covers", coverPayload);
            var cover = await createCoverResponse.Content.ReadFromJsonAsync<CoverDto>(SerializerOptions()) ?? throw new Exception("Cover could not be created.");
            return cover;
        }
    }
}


