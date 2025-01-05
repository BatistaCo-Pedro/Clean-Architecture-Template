namespace Application.Models.Entities;

/// <summary>
/// Abstract entity type that includes audit fields.
/// </summary>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// Column representing who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Column representing when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Column representing who updated the entity.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Column representing when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// A stamp of how many times this entity has been updated.
    /// </summary>
    public int Stamp { get; set; }
}