using Newtonsoft.Json;

namespace Claims.Application.Claims
{
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

    }
}
