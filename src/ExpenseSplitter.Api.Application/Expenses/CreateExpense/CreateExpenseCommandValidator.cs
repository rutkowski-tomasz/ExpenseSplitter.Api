using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

[ExcludeFromCodeCoverage]
public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.Date).LessThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.SettlementId).NotEmpty();

        RuleFor(x => x.PayingParticipantId).NotEmpty();

        RuleForEach(x => x.Allocations)
            .SetValidator(new CreateExpenseCommandAllocationValidator());

        RuleFor(x => x.Allocations)
            .NotEmpty()
            .WithMessage("Must contain at least one element");
    }
}

[ExcludeFromCodeCoverage]
public class CreateExpenseCommandAllocationValidator : AbstractValidator<CreateExpenseCommandAllocation>
{
    public CreateExpenseCommandAllocationValidator()
    {
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.ParticipantId).NotEmpty();
    }
}
