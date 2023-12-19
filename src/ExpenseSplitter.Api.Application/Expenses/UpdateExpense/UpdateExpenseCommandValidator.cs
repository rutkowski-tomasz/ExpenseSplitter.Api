using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Expenses.UpdateExpense;

[ExcludeFromCodeCoverage]
public sealed class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.Date).LessThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.PayingParticipantId).NotEmpty();

        RuleForEach(x => x.Allocations)
            .SetValidator(new UpdateExpenseCommandAllocationValidator());

        RuleFor(x => x.Allocations)
            .NotEmpty()
            .WithMessage("Must contain at least one allocation");
    }
}

[ExcludeFromCodeCoverage]
public sealed class UpdateExpenseCommandAllocationValidator : AbstractValidator<UpdateExpenseCommandAllocation>
{
    public UpdateExpenseCommandAllocationValidator()
    {
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.ParticipantId).NotEmpty();
    }
}
