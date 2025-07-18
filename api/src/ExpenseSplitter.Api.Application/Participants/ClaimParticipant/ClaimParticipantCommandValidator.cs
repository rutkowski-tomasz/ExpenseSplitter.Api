using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Participants.ClaimParticipant;

[ExcludeFromCodeCoverage]
public sealed class ClaimParticipantCommandValidator : AbstractValidator<ClaimParticipantCommand>
{
    public ClaimParticipantCommandValidator()
    {
        RuleFor(x => x.SettlementId).NotEmpty();

        RuleFor(x => x.ParticipantId).NotEmpty();
    }
}
