using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class CreateSettlementCommandHandler : ICommandHandler<CreateSettlementCommand, Guid>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IUserContext userContext;
    private readonly IUnitOfWork unitOfWork;

    private const string InviteCodeChars = "abcdefghjkmnpqrstwxyzABCDEFGHJKLMNOPQRSTWXYZ23456789";
    private const int InviteCodeLength = 8;

    public CreateSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IParticipantRepository participantRepository,
        ISettlementUserRepository settlementUserRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.participantRepository = participantRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.userContext = userContext;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSettlementCommand request, CancellationToken cancellationToken)
    {
        var inviteCode = GenerateInviteCode();
        var settlementResult = Settlement.Create(request.Name, inviteCode, userContext.UserId);

        if (settlementResult.IsFailure)
        {
            return Result.Failure<Guid>(settlementResult.Error);
        }
        
        var settlement = settlementResult.Value;
        settlementRepository.Add(settlement);

        foreach (var participantName in request.ParticipantNames)
        {
            var participantResult = Participant.Create(settlement.Id, participantName);

            if (participantResult.IsFailure)
            {
                return Result.Failure<Guid>(participantResult.Error);
            }

            participantRepository.Add(participantResult.Value);
        }

        var error = CreateSettlementUser(settlement);
        if (error is not null)
        {
            return Result.Failure<Guid>(error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return settlementResult.Value.Id.Value;
    }

    private Error? CreateSettlementUser(Settlement settlement)
    {
        var settlementUserResult = SettlementUser.Create(settlement.Id, userContext.UserId);
        if (settlementUserResult.IsFailure)
        {
            return settlementUserResult.Error;
        }

        var settlementUser = settlementUserResult.Value;
        settlementUserRepository.Add(settlementUser);
        return null;
    }

    private string GenerateInviteCode()
    {
        var random = new Random();

        var inviteCode = string.Join(
            "",
            Enumerable
                .Range(1, InviteCodeLength)
                .Select(_ => InviteCodeChars[random.Next(InviteCodeChars.Length)])
        );

        return inviteCode;
    }
}
