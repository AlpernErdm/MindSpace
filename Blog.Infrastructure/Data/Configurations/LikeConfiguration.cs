using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// Like entity için EF Core configuration
/// Post ve Comment beğenileri için ortak yapı
/// </summary>
public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        // Table name
        builder.ToTable("Likes");

        // Primary Key
        builder.HasKey(l => l.Id);

        // Enum configuration
        builder.Property(l => l.Type)
            .HasConversion<int>();

        // Foreign Keys
        builder.HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        builder.HasOne(l => l.Comment)
            .WithMany(c => c.Likes)
            .HasForeignKey(l => l.CommentId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        // Indexes
        builder.HasIndex(l => l.UserId)
            .HasDatabaseName("IX_Likes_UserId");

        builder.HasIndex(l => l.PostId)
            .HasDatabaseName("IX_Likes_PostId");

        builder.HasIndex(l => l.CommentId)
            .HasDatabaseName("IX_Likes_CommentId");

        builder.HasIndex(l => l.Type)
            .HasDatabaseName("IX_Likes_Type");

        // Unique constraints - Bir kullanıcı aynı içeriği sadece bir kez beğenebilir
        builder.HasIndex(l => new { l.UserId, l.PostId })
            .IsUnique()
            .HasDatabaseName("IX_Likes_User_Post")
            .HasFilter("[PostId] IS NOT NULL"); // SQL Server syntax

        builder.HasIndex(l => new { l.UserId, l.CommentId })
            .IsUnique()
            .HasDatabaseName("IX_Likes_User_Comment")
            .HasFilter("[CommentId] IS NOT NULL"); // SQL Server syntax

        // Check constraint - PostId veya CommentId'den biri null olmak zorunda
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Likes_PostOrComment", 
            "([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL)"));
    }
} 