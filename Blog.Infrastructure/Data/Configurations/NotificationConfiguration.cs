using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Data.Configurations;

/// <summary>
/// Notification entity için EF Core configuration
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Table name
        builder.ToTable("Notifications");

        // Primary Key
        builder.HasKey(n => n.Id);

        // Properties
        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.Property(n => n.ActionData)
            .HasColumnType("text");

        // Enum configuration
        builder.Property(n => n.Type)
            .HasConversion<int>();

        // Foreign Keys
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        builder.HasOne(n => n.Actor)
            .WithMany()
            .HasForeignKey(n => n.ActorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Post)
            .WithMany()
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        builder.HasOne(n => n.Comment)
            .WithMany()
            .HasForeignKey(n => n.CommentId)
            .OnDelete(DeleteBehavior.Restrict); // SQL Server cascade path sorunu için

        // Indexes
        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("IX_Notifications_UserId");

        builder.HasIndex(n => n.IsRead)
            .HasDatabaseName("IX_Notifications_IsRead");

        builder.HasIndex(n => n.Type)
            .HasDatabaseName("IX_Notifications_Type");

        builder.HasIndex(n => n.CreatedAt)
            .HasDatabaseName("IX_Notifications_CreatedAt");

        // Composite index - Kullanıcının okunmamış bildirimlerini getirmek için
        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt })
            .HasDatabaseName("IX_Notifications_User_Read_Created");

        // Default values
        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);
    }
} 