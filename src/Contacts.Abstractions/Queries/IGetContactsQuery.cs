using Contacts.Contracts.Queries;

namespace Contacts.Abstractions.Queries;

public interface IGetContactsQuery
{
    Task<PagedContactsResponse> GetContactsAsync(GetContactsRequest request,
        CancellationToken cancellationToken);
}