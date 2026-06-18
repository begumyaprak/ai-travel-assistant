using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTravelAssistant.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDocumentRepository"/>.
/// </summary>
public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentRepository"/>.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Document>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.Documents
            .AsNoTracking()
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddAsync(Document document, CancellationToken cancellationToken)
    {
        await _context.Documents.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Document document, CancellationToken cancellationToken)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
