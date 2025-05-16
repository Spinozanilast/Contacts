using Bogus;
using Contacts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.DataAccess.Extensions;

public static class DbContextSeeder
{
    public static async Task SeedContacts(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactsContext>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Contacts.AnyAsync())
        {
            var contacts = GenerateFakeContacts(100);
            await context.Contacts.AddRangeAsync(contacts);
            await context.SaveChangesAsync();
        }
    }

    private static List<Contact> GenerateFakeContacts(int count)
    {
        var faker = new Faker<Contact>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.MobilePhone, f => f.Phone.PhoneNumber("+############"))
            .RuleFor(c => c.JobTitle, f => f.Name.JobTitle())
            .RuleFor(c => c.BirthDate, f => f.Date.BetweenDateOnly(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-65)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-18))))
            .RuleFor(c => c.Created, f => DateTimeOffset.UtcNow)
            .RuleFor(c => c.LastModified, f => DateTimeOffset.UtcNow);

        return faker.Generate(count);
    }
}