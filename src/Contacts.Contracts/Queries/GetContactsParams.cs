namespace Contacts.Contracts.Queries;

public record GetContactsParams(
    int PageNumber = 1,
    int PageSize = 20,
    string? NameSearch = null,
    string? JobTitleSearch = null);