using Contacts.Contracts.Commands;

namespace Contacts.Abstractions.Commands;

public interface IUpdateContactCommand
{
    Task ExecuteAsync(UpdateContactDto dto, CancellationToken cancellationToken);
}