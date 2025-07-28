using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// User entity için EF Core configuration
/// ASP.NET Identity ekstra alanları
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name already set in DbContext

        // Properties
        builder.Property(u => u.FirstName)
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .HasMaxLength(50);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Website)
            .HasMaxLength(200);

        builder.Property(u => u.TwitterHandle)
            .HasMaxLength(50);

        builder.Property(u => u.LinkedInUrl)
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserName");

        builder.HasIndex(u => u.JoinDate)
            .HasDatabaseName("IX_Users_JoinDate");

        // Default values
        builder.Property(u => u.FollowerCount)
            .HasDefaultValue(0);

        builder.Property(u => u.FollowingCount)
            .HasDefaultValue(0);

        builder.Property(u => u.IsVerified)
            .HasDefaultValue(false);

        // UserFollow relationships are now configured in UserFollowConfiguration.cs
    }
} 