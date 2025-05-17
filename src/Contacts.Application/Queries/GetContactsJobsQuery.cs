using Contacts.Abstractions.Queries;
using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Queries;

public class GetContactsJobsQuery : IGetContactsJobsQuery
{
    private readonly ILogger<GetContactsJobsQuery> _logger;
    private readonly ContactsContext _db;

    public GetContactsJobsQuery(ILogger<GetContactsJobsQuery> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<string?[]> GetContactsJobsAsync(string? nameSearch, CancellationToken cancellationToken)
    {
        var contactsQuery = _db.Contacts
            .AsNoTracking().AsQueryable();

        if (nameSearch is not null)
        {
            contactsQuery = contactsQuery.Where(c => EF.Functions.Like(c.Name, $"%{nameSearch}%"));
        }

        return await contactsQuery.Select(c => c.JobTitle).Distinct()
            .ToArrayAsync(cancellationToken: cancellationToken);
    }
}