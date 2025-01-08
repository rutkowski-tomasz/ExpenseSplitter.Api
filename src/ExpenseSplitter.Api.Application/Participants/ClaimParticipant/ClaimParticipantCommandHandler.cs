using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Participants.ClaimParticipant;

internal sealed class ClaimParticipantCommandHandler(
    ISettlementUserRepository settlementUserRepository,
    ISettlementRepository settlementRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ClaimParticipantCommand>
{
    public async Task<Result> Handle(ClaimParticipantCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        var participantId = new ParticipantId(request.ParticipantId);

        var settlementUser = await settlementUserRepository.GetBySettlementId(settlementId, cancellationToken);
        if (settlementUser is null)
        {
            return SettlementErrors.Forbidden;
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (!settlement!.IsParticipantInSettlement(participantId))
        {
            return ParticipantErrors.NotFound;
        }

        settlementUser.SetParticipantId(participantId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
