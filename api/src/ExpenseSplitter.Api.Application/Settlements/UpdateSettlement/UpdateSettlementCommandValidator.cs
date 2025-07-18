using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

[ExcludeFromCodeCoverage]
public sealed class UpdateSettlementCommandValidator : AbstractValidator<UpdateSettlementCommand>
{
    public UpdateSettlementCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();

        RuleForEach(x => x.Participants)
            .SetValidator(new UpdateSettlementCommandParticipantValidator());

        RuleFor(x => x.Participants).NotEmpty().WithMessage("Must contain at least one participant");
    }
}

[ExcludeFromCodeCoverage]
public sealed class UpdateSettlementCommandParticipantValidator : AbstractValidator<UpdateSettlementCommandParticipant>
{
    public UpdateSettlementCommandParticipantValidator()
    {
        RuleFor(x => x.Nickname).NotEmpty();
    }
}
