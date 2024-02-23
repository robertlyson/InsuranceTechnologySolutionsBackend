using System.Text.Json.Serialization;
using Claims.Application.Covers;

namespace Claims.Controllers.Covers.Dto;

public class CoverDto
{
    [JsonRequired]
    public string Id { get; set; } = string.Empty;
    [JsonRequired] 
    public DateOnly StartDate { get; set; }
    
    [JsonRequired] 
    public DateOnly EndDate { get; set; }
    
    [JsonRequired] 
    public CoverType CoverType { get; set; }

    [JsonRequired] 
    public decimal Premium { get; set; }
}