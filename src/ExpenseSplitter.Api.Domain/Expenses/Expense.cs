using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.Expenses;

public sealed class Expense : Entity<ExpenseId>
{
    private Expense(
        string title,
        Amount amount,
        DateTime paymentDate,
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
    public DateTime PaymentDate { get; private set;  }
    public ParticipantId PayingParticipantId { get; private set; }

    public static Result<Expense> Create(
        string title,
        Amount amount,
        DateTime date,
        SettlementId settlementId,
        ParticipantId payingParticipantId
    )
    {
        if (string.IsNullOrEmpty(title))
        {
            return Result.Failure<Expense>(ExpenseErrors.EmptyName);
        }

        if (amount.Value <= 0)
        {
            return Result.Failure<Expense>(ExpenseErrors.NonPositiveAmount);
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

    public void SetTitle(string title)
    {
        Title = title;
    }

    public void SetAmount(Amount amount)
    {
        Amount = amount;
    }

    public void SetPaymentDate(DateTime date)
    {
        PaymentDate = date;
    }

    public void SetPayingParticipantId(ParticipantId participantId)
    {
        PayingParticipantId = participantId;
    }
}
