using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.DataAccess.Extensions;

public static class MigrationsApplier
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbcontext = scope.ServiceProvider.GetRequiredService<ContactsContext>();
        await dbcontext.Database.MigrateAsync();
    }
}