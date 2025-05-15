using Contacts.Abstractions.Commands;
using Contacts.Contracts.Commands;
using Contacts.DataAccess;
using Contacts.Domain;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Commands;

public class SaveContactCommand : ISaveContactCommand
{
    private readonly ILogger<SaveContactCommand> _logger;
    private readonly ContactsContext _db;

    public SaveContactCommand(ILogger<SaveContactCommand> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task ExecuteAsync(SaveContactDto dto, CancellationToken cancellationToken)
    {
        await _db
            .Contacts
            .AddAsync(new Contact
            {
                Id = Guid.CreateVersion7(),
                Name = dto.Name,
                MobilePhone = dto.MobilePhone,
                BirthDate = dto.BirthDate,
                JobTitle = dto.JobTitle,
                Created = DateTimeOffset.UtcNow,
            }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}