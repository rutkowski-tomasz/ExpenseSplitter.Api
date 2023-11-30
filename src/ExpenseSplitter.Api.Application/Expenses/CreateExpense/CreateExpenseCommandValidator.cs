using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

[ExcludeFromCodeCoverage]
public class CreateExpenseCommandValidator : AbstractValidator<CreateSettlementCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
