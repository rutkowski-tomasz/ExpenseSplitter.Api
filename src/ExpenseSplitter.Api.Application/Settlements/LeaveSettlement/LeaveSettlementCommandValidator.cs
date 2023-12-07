using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;

[ExcludeFromCodeCoverage]
public class LeaveSettlementCommandValidator : AbstractValidator<LeaveSettlementCommand>
{
    public LeaveSettlementCommandValidator()
    {
        RuleFor(x => x.SettlemetId).NotEmpty();
    }
}
