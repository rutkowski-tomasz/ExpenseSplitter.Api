using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class CreateSettlementCommandHandler(
    ISettlementRepository settlementRepository,
    ISettlementUserRepository settlementUserRepository,
    IUserContext userContext,
    IInviteCodeService inviteCodeService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateSettlementCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSettlementCommand request, CancellationToken cancellationToken)
    {
        var inviteCode = inviteCodeService.GenerateInviteCode();
        var settlementResult = Settlement.Create(request.Name, inviteCode, userContext.UserId, dateTimeProvider.UtcNow);

        if (settlementResult.IsFailure)
        {
            return settlementResult.AppError;
        }
        
        var settlement = settlementResult.Value;
        settlementRepository.Add(settlement);

        foreach (var participantName in request.ParticipantNames)
        {
            var participantResult = settlement.AddParticipant(participantName);
            if (participantResult.IsFailure)
            {
                return participantResult.AppError;
            }
        }

        var settlementUser = SettlementUser.Create(settlement.Id, userContext.UserId);
        settlementUserRepository.Add(settlementUser);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return settlementResult.Value.Id.Value;
    }
}
