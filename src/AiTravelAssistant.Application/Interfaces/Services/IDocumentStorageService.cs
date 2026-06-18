namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Provides read and write access to the underlying document file storage.
/// </summary>
public interface IDocumentStorageService
{
    /// <summary>
    /// Persists the file content to storage and returns the path at which it was saved.
    /// </summary>
    /// <param name="content">A readable stream of the file bytes to store.</param>
    /// <param name="fileName">The original file name, used to derive a unique storage name.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The storage path where the file was saved.</returns>
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken);

    /// <summary>
    /// Opens and returns a readable stream for the file at the specified storage path.
    /// </summary>
    /// <param name="storagePath">The path returned by a prior <see cref="SaveAsync"/> call.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A stream containing the raw file bytes.</returns>
    Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken);
}
