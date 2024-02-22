using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Claims;

public class CreateClaimDto
{
    [Required]
    public Guid? CoverId { get; set; }
    [Required]
    public DateTime? Created { get; set; }

    [Required] public string? Name { get; set; }
    [Required]
    public ClaimType? ClaimType { get; set; }
    [Required]
    public decimal? DamageCost { get; set; }
}