using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        // Table name
        builder.ToTable("PostTags");

        // Composite Primary Key
        builder.HasKey(pt => new { pt.PostId, pt.TagId });

        // Foreign Keys
        builder.HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pt => pt.PostId)
            .HasDatabaseName("IX_PostTags_PostId");

        builder.HasIndex(pt => pt.TagId)
            .HasDatabaseName("IX_PostTags_TagId");
    }
} 