namespace Contacts.Contracts.Queries;

public record ContactsResponse(PagedContactsResponse Contacts, string?[] Jobs);