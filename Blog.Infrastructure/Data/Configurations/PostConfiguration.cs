using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// Post entity için EF Core configuration
/// Medium benzeri blog post yapılandırması
/// </summary>
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        // Table name
        builder.ToTable("Posts");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.Content)
            .IsRequired()
            .HasColumnType("text"); // PostgreSQL için text type

        builder.Property(p => p.Excerpt)
            .HasMaxLength(500);

        builder.Property(p => p.MetaDescription)
            .HasMaxLength(160); // SEO için optimal uzunluk

        builder.Property(p => p.MetaKeywords)
            .HasMaxLength(255);

        builder.Property(p => p.FeaturedImageUrl)
            .HasMaxLength(500);

        // Enum configuration
        builder.Property(p => p.Status)
            .HasConversion<int>(); // Enum'u int olarak sakla

        // Foreign Keys
        builder.HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes - Performance için kritik!
        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Posts_Slug");

        builder.HasIndex(p => p.AuthorId)
            .HasDatabaseName("IX_Posts_AuthorId");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Posts_CategoryId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Posts_Status");

        builder.HasIndex(p => p.PublishedAt)
            .HasDatabaseName("IX_Posts_PublishedAt");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Posts_CreatedAt");

        // Composite index - Post listesi için optimize
        builder.HasIndex(p => new { p.Status, p.PublishedAt })
            .HasDatabaseName("IX_Posts_Status_PublishedAt");

        // Default values
        builder.Property(p => p.ViewCount)
            .HasDefaultValue(0);

        builder.Property(p => p.LikeCount)
            .HasDefaultValue(0);

        builder.Property(p => p.CommentCount)
            .HasDefaultValue(0);

        builder.Property(p => p.ReadTimeMinutes)
            .HasDefaultValue(1);
    }
} 