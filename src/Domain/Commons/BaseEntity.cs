namespace Domain.Commons;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
