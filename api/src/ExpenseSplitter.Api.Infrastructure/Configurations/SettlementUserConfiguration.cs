using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public class SettlementUserConfiguration : IEntityTypeConfiguration<SettlementUser>
{
    public void Configure(EntityTypeBuilder<SettlementUser> builder)
    {
        builder.ToTable("settlement_users");

        builder.HasKey(settlementUser => settlementUser.Id);

        builder.Property(settlementUser => settlementUser.Id)
            .HasConversion(settlementUserId => settlementUserId.Value, value => new SettlementUserId(value));

        builder
            .HasOne<Settlement>()
            .WithMany()
            .HasForeignKey(settlementUser => settlementUser.SettlementId);

        builder
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(settlementUser => settlementUser.ParticipantId);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(settlementUser => settlementUser.UserId);
    }
}
