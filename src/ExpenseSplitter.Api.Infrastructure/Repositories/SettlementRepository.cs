﻿using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class SettlementRepository : Repository<Settlement, SettlementId>, ISettlementRepository
{
    public SettlementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Settlement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Settlement>()
            .ToListAsync(cancellationToken);
    }
}