using System.Net.Mime;
using NUnit.Framework;

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

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
        }

    }
}
