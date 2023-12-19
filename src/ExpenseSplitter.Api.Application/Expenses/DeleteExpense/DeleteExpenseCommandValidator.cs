using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Expenses.DeleteExpense;

[ExcludeFromCodeCoverage]
public sealed class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
