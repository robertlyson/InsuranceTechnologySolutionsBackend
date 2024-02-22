using System.Text.Json.Serialization;

namespace Claims
{
    public class Claim
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("coverId")]
        public string CoverId { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("claimType")]
        public ClaimType Type { get; set; }

        [JsonPropertyName("damageCost")]
        public decimal DamageCost { get; set; }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
