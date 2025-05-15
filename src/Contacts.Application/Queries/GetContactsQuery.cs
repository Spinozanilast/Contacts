using Contacts.Abstractions.Queries;
using Contacts.Contracts.Queries;
using Contacts.DataAccess;
using Contacts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Contacts.Application.Queries;

public class GetContactsQuery : IGetContactsQuery
{
    private readonly ILogger<GetContactsQuery> _logger;
    private readonly ContactsContext _db;

    public GetContactsQuery(ILogger<GetContactsQuery> logger, ContactsContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<PagedContactsResponse> GetContactsAsync(
        GetContactsRequest request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var query = _db.Contacts.AsNoTracking().AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var results = await GetPagedResults(query, request, cancellationToken);

        LogExecution(request, results.Length, totalCount);

        return new PagedContactsResponse(
            results,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }

    private void ValidateRequest(GetContactsRequest request)
    {
        if (request.PageNumber < 1)
            throw new ArgumentException("Page number must be at least 1");

        if (request.PageSize is < 1 or > 100)
            throw new ArgumentException("Page size must be between 1 and 100");
    }

    private async Task<GetContactDto[]> GetPagedResults(
        IQueryable<Contact> query,
        GetContactsRequest request,
        CancellationToken cancellationToken)
    {
        return await query
            .OrderBy(c => c.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new GetContactDto(
                c.Id,
                c.Name,
                c.MobilePhone,
                c.Created,
                c.JobTitle,
                c.BirthDate,
                c.LastModified))
            .ToArrayAsync(cancellationToken);
    }

    private void LogExecution(GetContactsRequest request, int resultCount, int totalCount)
    {
        _logger.LogInformation(
            "Contacts query: Page {Page} (Size {Size}) returned {Results} of {Total} matches",
            request.PageNumber,
            request.PageSize,
            resultCount,
            totalCount);
    }

    public Task<PagedContactsResponse> GetContactsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}