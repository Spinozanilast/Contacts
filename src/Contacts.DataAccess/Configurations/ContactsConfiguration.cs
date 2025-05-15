using Contacts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contacts.DataAccess.Configurations;

public class ContactsConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder
            .Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasIndex(c => c.Name)
            .HasDatabaseName("idx_contact_name");
    }
}