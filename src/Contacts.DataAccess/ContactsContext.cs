using Contacts.DataAccess.Configurations;
using Contacts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Contacts.DataAccess;

public class ContactsContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public ContactsContext()
    {
    }

    public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactsConfiguration());
    }
}