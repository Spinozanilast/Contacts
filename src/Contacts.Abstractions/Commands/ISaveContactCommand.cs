using Contacts.Contracts.Commands;

namespace Contacts.Abstractions.Commands;

public interface ISaveContactCommand
{
    Task<Guid> ExecuteAsync(SaveContactDto dto, CancellationToken cancellationToken);
}