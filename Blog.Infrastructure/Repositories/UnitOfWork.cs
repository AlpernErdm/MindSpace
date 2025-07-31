using Blog.Application.Common.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Blog.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BlogDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances - lazy initialization
    private IUserRepository? _users;
    private IPostRepository? _posts;
    private IRepository<Comment>? _comments;
    private IRepository<Like>? _likes;
    private IRepository<Tag>? _tags;
    private IRepository<Category>? _categories;
    private IRepository<PostTag>? _postTags;
    private IRepository<UserFollow>? _userFollows;
    private IRepository<Notification>? _notifications;

    public UnitOfWork(BlogDbContext context)
    {
        _context = context;
    }

    // Repository Properties - Lazy initialization pattern
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IPostRepository Posts => _posts ??= new PostRepository(_context);
    public IRepository<Comment> Comments => _comments ??= new Repository<Comment>(_context);
    public IRepository<Like> Likes => _likes ??= new Repository<Like>(_context);
    public IRepository<Tag> Tags => _tags ??= new Repository<Tag>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<PostTag> PostTags => _postTags ??= new Repository<PostTag>(_context);
    public IRepository<UserFollow> UserFollows => _userFollows ??= new Repository<UserFollow>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction started");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction started");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
} 