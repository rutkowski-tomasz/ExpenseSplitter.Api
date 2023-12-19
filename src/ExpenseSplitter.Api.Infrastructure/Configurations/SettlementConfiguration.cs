using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("settlements");

        builder.HasKey(settlement => settlement.Id);

        builder.Property(settlement => settlement.Id)
            .HasConversion(settlementId => settlementId.Value, value => new SettlementId(value));

        builder.Property(settlement => settlement.Name);

        builder.Property(settlement => settlement.InviteCode).HasMaxLength(20);

        builder.HasIndex(settlement => settlement.InviteCode).IsUnique();

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(settlement => settlement.CreatorUserId);

        builder.Property(settlement => settlement.CreatedOnUtc);

        builder.Property(settlement => settlement.UpdatedOnUtc);
    }
}