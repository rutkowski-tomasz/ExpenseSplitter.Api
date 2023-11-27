using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");

        builder.HasKey(expense => expense.Id);

        builder.Property(expense => expense.Id)
            .HasConversion(expenseId => expenseId.Value, value => new ExpenseId(value));

        builder
            .HasOne<Settlement>()
            .WithMany()
            .HasForeignKey(expense => expense.SettlementId);

        builder.Property(expense => expense.Name);

        builder
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(expense => expense.PayingParticipantId);
    }
}