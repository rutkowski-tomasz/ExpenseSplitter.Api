using ExpenseSplitter.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await SaveChangesAsync(cancellationToken);

        return result;
    }
}
