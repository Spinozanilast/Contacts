using Contacts.Domain.Common;

namespace Contacts.Domain;

public class Contact : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public DateOnly? BirthDate { get; set; }
}