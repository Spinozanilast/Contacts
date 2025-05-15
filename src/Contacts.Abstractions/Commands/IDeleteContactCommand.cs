using Contacts.Contracts.Commands;

namespace Contacts.Abstractions.Commands;

public interface IDeleteContactCommand
{
    Task ExecuteAsync(DeleteContactDto dto, CancellationToken cancellationToken);
}