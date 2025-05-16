namespace Contacts.Contracts.Commands;

public record UpdateContactDto(string Name, string MobilePhone, string? JobTitle, DateOnly? BirthDate);