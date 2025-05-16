using Contacts.Abstractions.Queries;
using Contacts.Application.Exceptions;
using Contacts.Contracts.Queries;
using Contacts.DataAccess;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Queries;

public class GetContactByIdQuery : IGetContactByIdQuery
{
    private readonly ILogger<GetContactByIdQuery> _logger;
    private readonly ContactsContext _db;

    public GetContactByIdQuery(ILogger<GetContactByIdQuery> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<GetContactDto?> GetContactAsync(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _db.Contacts.FindAsync(id, cancellationToken);
        if (contact is null)
        {
            throw new NotFoundException($"Contact with id {id} was not found");
        }

        return new GetContactDto(
            id,
            contact.Name,
            contact.MobilePhone,
            contact.Created,
            contact.JobTitle,
            contact.BirthDate,
            contact.LastModified
        );
    }
}