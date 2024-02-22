using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Claims;

public class Cover
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("startDate")]
    public DateOnly StartDate { get; set; }
    
    [JsonPropertyName("endDate")]
    public DateOnly EndDate { get; set; }
    
    [JsonPropertyName("claimType")]
    public CoverType Type { get; set; }

    [JsonPropertyName("premium")]
    public decimal Premium { get; set; }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}