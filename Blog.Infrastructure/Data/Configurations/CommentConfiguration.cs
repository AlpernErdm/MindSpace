using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Table name
        builder.ToTable("Comments");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Content)
            .IsRequired()
            .HasColumnType("text");

        // Foreign Keys
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        // Self-referencing relationship - Parent/Child comments
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict); // Parent silinirse child'ları silinmesin

        // Indexes
        builder.HasIndex(c => c.PostId)
            .HasDatabaseName("IX_Comments_PostId");

        builder.HasIndex(c => c.AuthorId)
            .HasDatabaseName("IX_Comments_AuthorId");

        builder.HasIndex(c => c.ParentCommentId)
            .HasDatabaseName("IX_Comments_ParentCommentId");

        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Comments_CreatedAt");

        // Composite index - Post'un yorumlarını tarih sırasına göre getirmek için
        builder.HasIndex(c => new { c.PostId, c.CreatedAt })
            .HasDatabaseName("IX_Comments_PostId_CreatedAt");

        // Default values
        builder.Property(c => c.LikeCount)
            .HasDefaultValue(0);
    }
} 