using FlowForge.Api.Models;
namespace FlowForge.Api.Dtos

{
    public class WorkflowResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkflowStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}