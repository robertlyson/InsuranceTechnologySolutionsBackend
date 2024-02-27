using Domain;
using Newtonsoft.Json;

namespace Infrastructure.Claims;

public class ClaimCosmosEntity
{
    [JsonProperty(PropertyName = "id")] public string Id { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "coverId")]
    public Guid CoverId { get; set; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; set; }

    [JsonProperty(PropertyName = "name")] public string Name { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "claimType")]
    public ClaimType Type { get; set; }

    [JsonProperty(PropertyName = "damageCost")]
    public decimal DamageCost { get; set; }

    [JsonProperty(PropertyName = "deleted")]
    public bool Deleted { get; set; }
}