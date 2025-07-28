using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// UserFollow junction table configuration - Many-to-Many user follow sistemi
/// </summary>
public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        // Table name
        builder.ToTable("UserFollows");

        // Composite Primary Key - FollowerId + FollowingId
        builder.HasKey(uf => new { uf.FollowerId, uf.FollowingId });

        // Foreign Key - Follower
        builder.HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Key - Following  
        builder.HasOne(uf => uf.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.NoAction); // Self-reference cascade sorununu önlemek için

        // Indexes
        builder.HasIndex(uf => uf.FollowerId)
            .HasDatabaseName("IX_UserFollows_FollowerId");

        builder.HasIndex(uf => uf.FollowingId)
            .HasDatabaseName("IX_UserFollows_FollowingId");
    }
} 