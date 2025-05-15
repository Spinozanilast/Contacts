namespace Contacts.Domain.Common;

public class BaseEntity<T> where T : struct, IEquatable<T>, IComparable<T>
{
    public T Id { get; set; }
}