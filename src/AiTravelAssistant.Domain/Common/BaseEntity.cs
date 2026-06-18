namespace AiTravelAssistant.Domain.Common;

/// <summary>
/// Base class for all domain entities, providing common identity and audit timestamp properties.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Gets the unique identifier of the entity.</summary>
    public Guid Id { get; protected set; }

    /// <summary>Gets the UTC timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>Gets the UTC timestamp when the entity was last updated.</summary>
    public DateTime UpdatedAt { get; protected set; }
}
