using System.ComponentModel.DataAnnotations;
using Domain;

namespace Infrastructure.Dto;

public class CreateCoverDto
{
    [Required] 
    public DateOnly? StartDate { get; set; }
    
    [Required] 
    public DateOnly? EndDate { get; set; }
    
    [Required] 
    public CoverType? CoverType { get; set; }
}