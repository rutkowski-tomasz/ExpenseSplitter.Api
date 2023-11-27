using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

[ExcludeFromCodeCoverage]
public class CreateSettlementCommandValidator : AbstractValidator<CreateSettlementCommand>
{
    public CreateSettlementCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
