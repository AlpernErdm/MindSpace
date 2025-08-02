using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        // Table name
        builder.ToTable("Bookmarks");

        // Primary Key
        builder.HasKey(b => b.Id);

        // Properties
        builder.Property(b => b.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(b => b.PostId)
            .IsRequired();

        // Foreign Keys
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.Post)
            .WithMany()
            .HasForeignKey(b => b.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(b => b.UserId)
            .HasDatabaseName("IX_Bookmarks_UserId");

        builder.HasIndex(b => b.PostId)
            .HasDatabaseName("IX_Bookmarks_PostId");

        // Unique constraint - bir kullanıcı aynı post'u birden fazla kez bookmark edemez
        builder.HasIndex(b => new { b.UserId, b.PostId })
            .IsUnique()
            .HasDatabaseName("IX_Bookmarks_UserId_PostId");
    }
} 