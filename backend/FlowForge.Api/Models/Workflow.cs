namespace FlowForge.Api.Models;

public class Workflow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}