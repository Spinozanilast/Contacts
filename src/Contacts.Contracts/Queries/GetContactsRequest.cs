namespace Contacts.Contracts.Queries;

public record GetContactsRequest(
    int PageNumber = 1,
    int PageSize = 20);