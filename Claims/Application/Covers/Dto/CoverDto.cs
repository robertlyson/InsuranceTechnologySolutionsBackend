using System.Text.Json.Serialization;

namespace Claims.Application.Covers.Dto;

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