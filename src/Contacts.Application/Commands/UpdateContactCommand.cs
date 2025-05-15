using Contacts.Abstractions.Commands;
using Contacts.Application.Helpers;
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

    public async Task ExecuteAsync(UpdateContactDto dto, CancellationToken cancellationToken)
    {
        await _db.Contacts
            .Where(c => c.Id == dto.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetIfNotNull(c => c.Name, dto.Name)
                .SetIfNotNull(c => c.MobilePhone, dto.MobilePhone)
                .SetIfNotNull(c => c.JobTitle, dto.JobTitle)
                .SetIfNotNull(c => c.BirthDate, dto.BirthDate)
                .SetProperty(c => c.LastModified, DateTimeOffset.UtcNow), cancellationToken);
    }
}