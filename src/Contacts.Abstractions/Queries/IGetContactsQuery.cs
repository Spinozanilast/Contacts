using Contacts.Contracts.Queries;

namespace Contacts.Abstractions.Queries;

public interface IGetContactsQuery
{
    Task<PagedContactsResponse> GetContactsAsync(GetContactsParams @params,
        CancellationToken cancellationToken);
}