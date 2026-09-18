using System.ComponentModel.DataAnnotations;
namespace FlowForge.Api.Dtos;
    
public class CreateWorkflowRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}