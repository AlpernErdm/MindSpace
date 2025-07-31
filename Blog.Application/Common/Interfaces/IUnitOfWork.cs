namespace Blog.Application.Common.Interfaces;


public interface IUnitOfWork : IDisposable
{
    // Repositories
    IUserRepository Users { get; }
    IPostRepository Posts { get; }
    IRepository<Blog.Domain.Entities.Comment> Comments { get; }
    IRepository<Blog.Domain.Entities.Like> Likes { get; }
    IRepository<Blog.Domain.Entities.Tag> Tags { get; }
    IRepository<Blog.Domain.Entities.Category> Categories { get; }
    IRepository<Blog.Domain.Entities.PostTag> PostTags { get; }
    IRepository<Blog.Domain.Entities.UserFollow> UserFollows { get; }
    IRepository<Blog.Domain.Entities.Notification> Notifications { get; }

    // Transaction operations
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
} 