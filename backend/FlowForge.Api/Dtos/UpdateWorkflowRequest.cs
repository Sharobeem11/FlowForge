using System.ComponentModel.DataAnnotations;
namespace FlowForge.Api.Dtos;
    
public class UpdateWorkflowRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;
}