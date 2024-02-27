using Domain;
using Newtonsoft.Json;

namespace Infrastructure.Covers;

public class CoverCosmosEntity
{
    [JsonProperty(PropertyName = "id")] public Guid Id { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public CoverType Type { get; set; }

    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; set; }

    [JsonProperty(PropertyName = "deleted")]
    public bool Deleted { get; set; }
}