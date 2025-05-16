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
        GetContactsParams @params,
        CancellationToken cancellationToken)
    {
        ValidateRequest(@params);

        var query = _db.Contacts.AsNoTracking().AsQueryable();

        ApplyFilters(ref query, @params);
        var totalCount = await query.CountAsync(cancellationToken);

        var results = await GetPagedResults(query, @params, cancellationToken);

        LogExecution(@params, results.Length, totalCount);

        return new PagedContactsResponse(
            results,
            totalCount,
            @params.PageNumber,
            @params.PageSize);
    }

    private void LogExecution(GetContactsParams @params, int resultCount, int totalCount)
    {
        _logger.LogInformation(
            "Contacts query: Page {Page} (Size {Size}) with {Search} with job of {Job}  returned {Results} of {Total} matches",
            @params.PageNumber,
            @params.PageSize,
            @params.NameSearch,
            @params.JobTitleSearch,
            resultCount,
            totalCount);
    }

    private static async Task<GetContactDto[]> GetPagedResults(
        IQueryable<Contact> query,
        GetContactsParams @params,
        CancellationToken cancellationToken)
    {
        return await query
            .OrderBy(c => c.Name)
            .Skip((@params.PageNumber - 1) * @params.PageSize)
            .Take(@params.PageSize)
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

    private static void ValidateRequest(GetContactsParams @params)
    {
        if (@params.PageNumber < 1)
            throw new ArgumentException("Page number must be at least 1");

        if (@params.PageSize is < 1 or > 100)
            throw new ArgumentException("Page size must be between 1 and 100");
    }

    private static void ApplyFilters(ref IQueryable<Contact> query, GetContactsParams @params)
    {
        if (!string.IsNullOrEmpty(@params.NameSearch))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Name, $"%{@params.NameSearch}%"));
        }

        if (!string.IsNullOrEmpty(@params.JobTitleSearch))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.JobTitle!, $"%{@params.JobTitleSearch}%"));
        }
    }
}