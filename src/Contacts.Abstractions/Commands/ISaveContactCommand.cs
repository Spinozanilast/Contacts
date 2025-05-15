using Contacts.Contracts.Commands;

namespace Contacts.Abstractions.Commands;

public interface ISaveContactCommand
{
    Task ExecuteAsync(SaveContactDto dto, CancellationToken cancellationToken);
}