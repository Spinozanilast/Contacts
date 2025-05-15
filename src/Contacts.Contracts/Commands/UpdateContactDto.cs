namespace Contacts.Contracts.Commands;

public record UpdateContactDto(Guid Id, string? Name, string? MobilePhone, string? JobTitle, DateOnly? BirthDate);