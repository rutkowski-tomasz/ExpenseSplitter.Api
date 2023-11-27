using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("settlements");

        builder.HasKey(settlement => settlement.Id);

        builder.Property(settlement => settlement.Id)
            .HasConversion(settlementId => settlementId.Value, value => new SettlementId(value));

        builder.Property(settlement => settlement.Name);
    }
}