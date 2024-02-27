using System.Text.Json.Serialization;
using Domain;

namespace Infrastructure.Dto;

public class ClaimDto
{
    public Guid Id { get; set; }
    [JsonRequired]
    public Guid CoverId { get; set; }
    [JsonRequired]
    public DateTime Created { get; set; }

    [JsonRequired] public string Name { get; set; } = string.Empty;
    [JsonRequired]
    public ClaimType ClaimType { get; set; }
    [JsonRequired]
    public decimal DamageCost { get; set; }
}