namespace Contacts.Contracts.Commands;

public record SaveContactDto(string Name, string MobilePhone, string? JobTitle, DateOnly? BirthDate);