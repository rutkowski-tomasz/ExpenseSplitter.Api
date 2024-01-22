using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.Expenses;

public sealed class Expense : Entity<ExpenseId>
{
    private Expense(
        string title,
        Amount amount,
        DateOnly paymentDate,
        ExpenseId id,
        SettlementId settlementId,
        ParticipantId payingParticipantId
    ) : base(id)
    {
        SettlementId = settlementId;
        Title = title;
        Amount = amount;
        PaymentDate = paymentDate;
        PayingParticipantId = payingParticipantId;
    }

    public SettlementId SettlementId { get; private set; }
    public string Title { get; private set; }
    public Amount Amount { get; private set; }
    public DateOnly PaymentDate { get; private set;  }
    public ParticipantId PayingParticipantId { get; private set; }

    public List<Allocation> Allocations { get; set; } = null!;

    public static Result<Expense> Create(
        string title,
        Amount amount,
        DateOnly date,
        SettlementId settlementId,
        ParticipantId payingParticipantId
    )
    {
        if (string.IsNullOrEmpty(title))
        {
            return Result.Failure<Expense>(ExpenseErrors.EmptyTitle);
        }

        var expense = new Expense(
            title,
            amount,
            date,
            ExpenseId.New(),
            settlementId,
            payingParticipantId
        );

        return expense;
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

    public void SetAmount(Amount amount)
    {
        Amount = amount;
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
