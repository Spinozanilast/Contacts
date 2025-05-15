namespace Contacts.Contracts.Queries;

public record GetContactDto(
    Guid Id,
    string Name,
    string MobilePhone,
    DateTimeOffset Created,
    string? JobTitle,
    DateOnly? BirthDate,
    DateTimeOffset? LastModified);