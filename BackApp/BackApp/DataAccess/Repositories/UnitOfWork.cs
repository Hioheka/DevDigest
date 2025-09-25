using BackApp.Entities;

namespace BackApp.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Categories = new GenericRepository<Category>(_context);
        BlogPosts = new GenericRepository<BlogPost>(_context);
        Comments = new GenericRepository<Comment>(_context);
    }

    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<BlogPost> BlogPosts { get; }
    public IGenericRepository<Comment> Comments { get; }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}
