using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.DataAccess.Extensions;

public static class ContactsContextInjector
{
    public static IServiceCollection AddContactsContext(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureOptions = null,
        bool usePooling = true,
        int poolSize = 128,
        string connectionName = "DefaultConnection")
    {
        var connectionString = configuration.GetConnectionString(connectionName);

        if (usePooling)
        {
            services.AddDbContextPool<ContactsContext>(options =>
            {
                configureOptions?.Invoke(options);
                ApplyDefaultConfiguration(options, connectionString);
            }, poolSize);
        }
        else
        {
            services.AddDbContext<ContactsContext>(options =>
            {
                configureOptions?.Invoke(options);
                ApplyDefaultConfiguration(options, connectionString);
            });
        }

        return services;
    }

    private static void ApplyDefaultConfiguration(
        DbContextOptionsBuilder options,
        string connectionString)
    {
        options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }
}