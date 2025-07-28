using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// Category entity için EF Core configuration
/// Hiyerarşik kategori yapısı destekler
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(c => c.Description)
            .HasMaxLength(300);

        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Color)
            .HasMaxLength(7); // #FFFFFF format

        // Self-referencing relationship
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Name");

        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Slug");

        builder.HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_ParentCategoryId");

        // Default values
        builder.Property(c => c.PostCount)
            .HasDefaultValue(0);
    }
} 