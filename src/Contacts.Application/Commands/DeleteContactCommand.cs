using Contacts.Abstractions.Commands;
using Contacts.Contracts.Commands;
using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Commands;

public class DeleteContactCommand : IDeleteContactCommand
{
    private readonly ILogger<DeleteContactCommand> _logger;
    private readonly ContactsContext _db;

    public DeleteContactCommand(ILogger<DeleteContactCommand> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task ExecuteAsync(DeleteContactDto dto, CancellationToken cancellationToken)
    {
        await _db
            .Contacts
            .Where(c => c.Id == dto.Id).ExecuteDeleteAsync(cancellationToken);
    }
}