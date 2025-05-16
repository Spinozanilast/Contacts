namespace Contacts.Abstractions.Queries;

public interface IGetContactsJobsQuery
{
    Task<string?[]> GetContactsJobsAsync(string? nameSearch, CancellationToken cancellationToken);
}