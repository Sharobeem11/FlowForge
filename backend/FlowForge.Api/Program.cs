using System.ComponentModel.DataAnnotations;
using FlowForge.Api.Data;
using FlowForge.Api.Models;
using FlowForge.Api.Dtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => {
    return "FlowForge API is running!";
});

app.MapGet("/workflows", async (AppDbContext db) => {
    var workflows = await db.Workflows.ToListAsync();
    
    var response = workflows.Select(workflow => new WorkflowResponse
    {
        Id = workflow.Id,
        Name = workflow.Name,
        Description = workflow.Description,
        Status = workflow.Status,
        CreatedAt = workflow.CreatedAt
    });

    return Results.Ok(response);
});

app.MapGet("/workflows/{id}", async (Guid id, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }

    var response = new WorkflowResponse
    {
        Id = workflow.Id,
        Name = workflow.Name,
        Description = workflow.Description,
        Status = workflow.Status,
        CreatedAt = workflow.CreatedAt
    };

    return Results.Ok(response);
});

app.MapPost("/workflows", async (CreateWorkflowRequest request, AppDbContext db) => {
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(
        request,
        validationContext,
        validationResults,
        validateAllProperties: true
    );

    if (!isValid)
    {
        return Results.BadRequest(new
        {
            errors = validationResults.Select(result => result.ErrorMessage)
        });
    }
    
    var workflow = new Workflow
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Description = request.Description,
        CreatedAt = DateTime.UtcNow
    };
    db.Workflows.Add(workflow);
    await db.SaveChangesAsync();

    var response = new WorkflowResponse
    {
        Id = workflow.Id,
        Name = workflow.Name,
        Description = workflow.Description,
        Status = workflow.Status,
        CreatedAt = workflow.CreatedAt
    };

    return Results.Created($"/workflows/{workflow.Id}", response);
});

app.MapPut("/workflows/{id}", async (Guid id, UpdateWorkflowRequest request, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }

    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(
        request,
        validationContext,
        validationResults,
        validateAllProperties: true
    );

    if (!isValid)
    {
        return Results.BadRequest(new
        {
            errors = validationResults.Select(result => result.ErrorMessage)
        });
    }

    workflow.Name = request.Name;
    workflow.Description = request.Description;
    workflow.Status = request.Status;
    
    await db.SaveChangesAsync();

    var response = new WorkflowResponse
    {
        Id = workflow.Id,
        Name = workflow.Name,
        Description = workflow.Description,
        Status = workflow.Status,
        CreatedAt = workflow.CreatedAt
    };

    return Results.Ok(response);
});

app.MapDelete("/workflows/{id}", async (Guid id, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }
    db.Workflows.Remove(workflow);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();