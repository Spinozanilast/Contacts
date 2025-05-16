using Contacts.Abstractions.Queries;
using Contacts.Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Application.Extensions;

public static class ContactsQueries
{
    public static IServiceCollection AddContactsQueries(this IServiceCollection services)
    {
        services
            .AddScoped<IGetContactsQuery, GetContactsQuery>()
            .AddScoped<IGetContactByIdQuery, GetContactByIdQuery>()
            .AddScoped<IGetContactsJobsQuery, GetContactsJobsQuery>();

        return services;
    }
}