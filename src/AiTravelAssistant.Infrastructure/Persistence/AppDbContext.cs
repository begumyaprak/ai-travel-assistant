using AiTravelAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTravelAssistant.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the AI Travel Assistant, exposing sets for all persisted domain entities.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Gets or sets the <see cref="Document"/> table.</summary>
    public DbSet<Document> Documents { get; set; } = default!;

    /// <summary>Gets or sets the <see cref="DocumentChunk"/> table.</summary>
    public DbSet<DocumentChunk> DocumentChunks { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="AppDbContext"/> with the provided options.
    /// </summary>
    /// <param name="options">The EF Core options for this context instance.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
