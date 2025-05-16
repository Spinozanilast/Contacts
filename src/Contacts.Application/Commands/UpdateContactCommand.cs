using Contacts.Abstractions.Commands;
using Contacts.Application.Exceptions;
using Contacts.Contracts.Commands;
using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Commands;

public class UpdateContactCommand : IUpdateContactCommand
{
    private readonly ILogger<UpdateContactCommand> _logger;
    private readonly ContactsContext _db;

    public UpdateContactCommand(ILogger<UpdateContactCommand> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task ExecuteAsync(Guid contactId, UpdateContactDto dto, CancellationToken cancellationToken)
    {
        var affectedRows = await _db.Contacts
            .Where(c => c.Id == contactId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(c => c.Name, dto.Name)
                .SetProperty(c => c.MobilePhone, dto.MobilePhone)
                .SetProperty(c => c.BirthDate, dto.BirthDate)
                .SetProperty(c => c.JobTitle, dto.JobTitle)
                .SetProperty(c => c.LastModified, DateTimeOffset.UtcNow), cancellationToken);

        if (affectedRows == 0)
        {
            throw new NotFoundException($"Contact with ID {contactId} not found.");
        }
    }
}