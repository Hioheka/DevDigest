using BackApp.Entities;

namespace BackApp.DataAccess.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<BlogPost> BlogPosts { get; }
    IGenericRepository<Comment> Comments { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
