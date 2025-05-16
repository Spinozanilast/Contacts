namespace Contacts.Contracts.Queries;

public record PagedContactsResponse(
    GetContactDto[] Data,
    int TotalCount,
    int PageNumber,
    int PageSize);