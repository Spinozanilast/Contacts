using Contacts.Abstractions.Commands;
using Contacts.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Application.Extensions;

public static class ContactsCommands
{
    public static IServiceCollection AddContactsCommands(this IServiceCollection services)
    {
        services
            .AddScoped<IDeleteContactCommand, DeleteContactCommand>()
            .AddScoped<ISaveContactCommand, SaveContactCommand>()
            .AddScoped<IUpdateContactCommand, UpdateContactCommand>();

        return services;
    }
}