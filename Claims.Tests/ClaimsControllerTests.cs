using System.Net.Mime;
using NUnit.Framework;

namespace Claims.Tests
{
    public class Tmp
    {
        [Test]
        public void One()
        {
            
        }
    }
    
    public class ClaimsControllerTests : ApplicationFixture
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
