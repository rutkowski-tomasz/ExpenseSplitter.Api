using ExpenseSplitter.Api.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal abstract class Repository<TEntity, TEntityId>(ApplicationDbContext dbContext)
    where TEntity : AggregateRoot<TEntityId>
    where TEntityId : class
{
    protected readonly ApplicationDbContext DbContext = dbContext;

    public Task<TEntity?> GetById(
        TEntityId id,
        CancellationToken cancellationToken
    )
    {
        return DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        DbContext.Remove(entity);
    }
}
