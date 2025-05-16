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

    public async Task<Guid> ExecuteAsync(SaveContactDto dto, CancellationToken cancellationToken)
    {
        var newContactId = Guid.CreateVersion7();

        await _db
            .Contacts
            .AddAsync(new Contact
            {
                Id = newContactId,
                Name = dto.Name,
                MobilePhone = dto.MobilePhone,
                BirthDate = dto.BirthDate,
                JobTitle = dto.JobTitle,
                Created = DateTimeOffset.UtcNow,
            }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return newContactId;
    }
}