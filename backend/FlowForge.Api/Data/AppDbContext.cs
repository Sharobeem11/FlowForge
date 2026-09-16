using FlowForge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow> Workflows => Set<Workflow>();
}