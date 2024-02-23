using System.ComponentModel.DataAnnotations;

namespace Claims.Infrastructure;

public class CosmosDbOption
{
    [Required]
    public string? Account { get; set; }
    [Required]
    public string? Key { get; set; }
    [Required]
    public string? DatabaseName { get; set; }
    [Required]
    public string? ClaimsContainerName { get; set; }
    [Required]
    public string? CoversContainerName { get; set; }
}