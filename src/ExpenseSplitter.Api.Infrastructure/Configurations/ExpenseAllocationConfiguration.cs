using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
internal sealed class ExpenseAllocationConfiguration : IEntityTypeConfiguration<ExpenseAllocation>
{
    public void Configure(EntityTypeBuilder<ExpenseAllocation> builder)
    {
        builder.ToTable("expense_allocations");
        
        builder.HasKey(expenseAllocation => expenseAllocation.Id);

        builder.Property(expenseAllocation => expenseAllocation.Id)
            .HasConversion(expenseAllocationId => expenseAllocationId.Value, value => new ExpenseAllocationId(value));

        builder.Property(expenseAllocation => expenseAllocation.Value);

        builder
            .HasOne<Expense>()
            .WithMany()
            .HasForeignKey(expenseAllocation => expenseAllocation.ExpenseId);

        builder
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(expenseAllocation => expenseAllocation.ParticipantId);
    }
}