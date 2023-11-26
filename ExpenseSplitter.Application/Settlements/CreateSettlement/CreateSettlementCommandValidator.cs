using FluentValidation;

namespace ExpenseSplitter.Application.Settlements.CreateSettlement;

public class CreateSettlementCommandValidator : AbstractValidator<CreateSettlementCommand>
{
    public CreateSettlementCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
