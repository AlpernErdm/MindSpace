using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// Tag entity için EF Core configuration
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Table name
        builder.ToTable("Tags");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(t => t.Description)
            .HasMaxLength(200);

        builder.Property(t => t.Color)
            .HasMaxLength(7); // #FFFFFF format

        // Indexes
        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Name");

        builder.HasIndex(t => t.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Slug");

        // Default values
        builder.Property(t => t.PostCount)
            .HasDefaultValue(0);

        builder.Property(t => t.FollowerCount)
            .HasDefaultValue(0);
    }
} 