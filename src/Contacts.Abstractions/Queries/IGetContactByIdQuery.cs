using Contacts.Contracts.Queries;

namespace Contacts.Abstractions.Queries;

public interface IGetContactByIdQuery
{
    Task<GetContactDto?> GetContactAsync(Guid id, CancellationToken cancellationToken);
}