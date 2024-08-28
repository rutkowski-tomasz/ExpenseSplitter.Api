using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Abstraction.Clock;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExpenseSplitter.Api.Infrastructure;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users { get; set; }

    private readonly IPublisher publisher;
    private readonly IDateTimeProvider dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions options,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider
    ) : base(options)
    {
        this.publisher = publisher;
        this.dateTimeProvider = dateTimeProvider;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateLastModifiedDateForChangedEntries();

            var result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEvents();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    private void UpdateLastModifiedDateForChangedEntries()
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added)
            .Select(x => x.Entity as IEntity)
            .Where(x => x is not null);

        foreach (var entity in entities)
        {
            entity!.LastModified = dateTimeProvider.UtcNow;
        }
    }

    private async Task PublishDomainEvents()
    {
        var domainEvents = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}
