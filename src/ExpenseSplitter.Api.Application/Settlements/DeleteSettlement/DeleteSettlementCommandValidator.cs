using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;

[ExcludeFromCodeCoverage]
public sealed class DeleteSettlementCommandValidator : AbstractValidator<DeleteSettlementCommand>
{
    public DeleteSettlementCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
