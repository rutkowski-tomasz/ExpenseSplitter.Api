using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Expenses;

public sealed class Expense : AggregateRoot<ExpenseId>
{
    private readonly List<Allocation> allocations;

    private Expense(
        string title,
        DateOnly paymentDate,
        ExpenseId id,
        SettlementId settlementId,
        ParticipantId payingParticipantId,
        Amount totalAmount,
        List<Allocation> expenseAllocations
    ) : base(id)
    {
        SettlementId = settlementId;
        Title = title;
        PaymentDate = paymentDate;
        PayingParticipantId = payingParticipantId;
        Amount = totalAmount;
        allocations = expenseAllocations;
    }

    public SettlementId SettlementId { get; private set; }
    public string Title { get; private set; }
    public Amount Amount { get; private set; }
    public DateOnly PaymentDate { get; private set;  }
    public ParticipantId PayingParticipantId { get; private set; }
    public IReadOnlyList<Allocation> Allocations => allocations.AsReadOnly();

    public static Result<Expense> Create(
        string title,
        DateOnly date,
        SettlementId settlementId,
        ParticipantId payingParticipantId,
        Dictionary<ParticipantId, decimal> expenseAllocations
    )
    {
        if (string.IsNullOrEmpty(title))
        {
            return Result.Failure<Expense>(ExpenseErrors.EmptyTitle);
        }

        var expenseId = ExpenseId.New();

        var totalAmount = Amount.Zero();
        var newAllocations = new List<Allocation>();

        foreach (var (participantId, amount) in expenseAllocations)
        {
            var amountResult = Amount.Create(amount);
            if (amountResult.IsFailure)
            {
                return Result.Failure<Expense>(amountResult.AppError);
            }

            totalAmount += amountResult.Value;

            var newAllocation = Allocation.Create(amountResult.Value, expenseId, participantId);
            newAllocations.Add(newAllocation);
        }

        var expense = new Expense(
            title,
            date,
            expenseId,
            settlementId,
            payingParticipantId,
            totalAmount,
            newAllocations
        );
        
        return expense;
    }

    public Result AddAllocation(Amount amount, ParticipantId participantId)
    {
        var allocation = Allocation.Create(amount, Id, participantId);
        allocations.Add(allocation);
        
        var totalAmount = allocations.Sum(x => x.Amount.Value);
        Amount = Amount.Create(totalAmount).Value;
        
        return Result.Success();
    }

    public Result UpdateAllocation(AllocationId allocationId, Amount newAmount, ParticipantId participantId)
    {
        var allocation = allocations.FirstOrDefault(x => x.Id == allocationId);
        if (allocation is null)
        {
            return Result.Failure(ExpenseErrors.AllocationNotFound);
        }

        allocation.Update(newAmount, participantId);
        
        var totalAmount = allocations.Sum(x => x.Amount.Value);
        Amount = Amount.Create(totalAmount).Value;
        
        return Result.Success();
    }

    public Result RemoveAllocation(AllocationId allocationId)
    {
        var allocation = allocations.FirstOrDefault(x => x.Id == allocationId);
        if (allocation is null)
        {
            return Result.Failure(ExpenseErrors.AllocationNotFound);
        }

        allocations.Remove(allocation);
        
        var totalAmount = allocations.Sum(x => x.Amount.Value);
        Amount = Amount.Create(totalAmount).Value;
        
        return Result.Success();
    }
    
    public Result SetTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return Result.Failure<Expense>(ExpenseErrors.EmptyTitle);
        }

        Title = title;
        return Result.Success();
    }

    public void SetPaymentDate(DateOnly date)
    {
        PaymentDate = date;
    }

    public void SetPayingParticipantId(ParticipantId participantId)
    {
        PayingParticipantId = participantId;
    }
}
