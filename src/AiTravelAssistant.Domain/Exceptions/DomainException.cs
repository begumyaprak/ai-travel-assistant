namespace AiTravelAssistant.Domain.Exceptions;

/// <summary>
/// Base exception type for all domain rule violations, carrying a machine-readable error code.
/// </summary>
public class DomainException : Exception
{
    /// <summary>Gets the domain-specific error code identifying the type of violation.</summary>
    public string Code { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DomainException"/> with the specified error code and message.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description of the error.</param>
    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
