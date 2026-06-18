using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTravelAssistant.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDocumentChunkRepository"/>.
/// </summary>
public class DocumentChunkRepository : IDocumentChunkRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentChunkRepository"/>.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    public DocumentChunkRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken)
    {
        await _context.DocumentChunks.AddRangeAsync(chunks, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DocumentChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken)
        => await _context.DocumentChunks
            .AsNoTracking()
            .Where(c => c.DocumentId == documentId)
            .OrderBy(c => c.ChunkIndex)
            .ToListAsync(cancellationToken);
}
