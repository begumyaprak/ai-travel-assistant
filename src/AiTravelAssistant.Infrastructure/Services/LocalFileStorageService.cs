using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IDocumentStorageService"/> by reading and writing files on the local filesystem.
/// </summary>
/// <remarks>
/// Intended for local development; replace with an Azure Blob Storage implementation for production deployments.
/// </remarks>
public class LocalFileStorageService : IDocumentStorageService
{
    private readonly string _basePath;

    /// <summary>
    /// Initializes a new instance of <see cref="LocalFileStorageService"/> with the configured base path.
    /// </summary>
    /// <param name="options">The storage settings containing the base directory path.</param>
    public LocalFileStorageService(IOptions<StorageSettings> options)
    {
        _basePath = options.Value.BasePath;
    }

    /// <summary>
    /// Saves the file stream to the local filesystem under a unique name and returns the full file path.
    /// </summary>
    /// <param name="content">A readable stream of the file bytes to persist.</param>
    /// <param name="fileName">The original file name used as a suffix in the generated unique name.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The absolute file path where the file was saved.</returns>
    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_basePath);

        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_basePath, uniqueName);

        await using var fs = File.Create(filePath);
        await content.CopyToAsync(fs, cancellationToken);

        return filePath;
    }

    /// <summary>
    /// Opens and returns a read-only stream for the file at the specified path.
    /// </summary>
    /// <param name="storagePath">The path returned by a prior <see cref="SaveAsync"/> call.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used for this synchronous operation).</param>
    /// <returns>A stream for reading the stored file.</returns>
    public Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        Stream stream = File.OpenRead(storagePath);
        return Task.FromResult(stream);
    }
}
