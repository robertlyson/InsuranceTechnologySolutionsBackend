using System.ComponentModel.DataAnnotations;
using Claims.Application.Covers;

namespace Claims.Controllers.Covers.Dto;

public class CreateCoverDto
{
    [Required] 
    public DateOnly? StartDate { get; set; }
    
    [Required] 
    public DateOnly? EndDate { get; set; }
    
    [Required] 
    public CoverType? CoverType { get; set; }
}