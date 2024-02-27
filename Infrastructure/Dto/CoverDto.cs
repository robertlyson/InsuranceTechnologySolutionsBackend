using System.Text.Json.Serialization;
using Domain;

namespace Infrastructure.Dto;

public class CoverDto
{
    [JsonRequired]
    public Guid Id { get; set; }
    [JsonRequired] 
    public DateOnly StartDate { get; set; }
    
    [JsonRequired] 
    public DateOnly EndDate { get; set; }
    
    [JsonRequired] 
    public CoverType CoverType { get; set; }

    [JsonRequired] 
    public decimal Premium { get; set; }
}