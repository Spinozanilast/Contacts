namespace Contacts.Domain.Common;

public abstract class BaseAuditableEntity<T> : BaseEntity<T> where T : struct, IEquatable<T>, IComparable<T>
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}