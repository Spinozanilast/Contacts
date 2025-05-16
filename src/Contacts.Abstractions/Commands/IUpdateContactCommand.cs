using Contacts.Contracts.Commands;

namespace Contacts.Abstractions.Commands;

public interface IUpdateContactCommand
{
    Task ExecuteAsync(Guid contactId, UpdateContactDto dto, CancellationToken cancellationToken);
}