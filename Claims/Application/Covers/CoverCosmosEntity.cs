using Newtonsoft.Json;

namespace Claims.Application.Covers;

public class CoverCosmosEntity
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; set; }
    
    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; set; }
    
    [JsonProperty(PropertyName = "claimType")]
    public CoverType Type { get; set; }

    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; set; }
}