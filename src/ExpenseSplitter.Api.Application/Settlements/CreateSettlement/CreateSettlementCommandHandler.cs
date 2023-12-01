using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class CreateSettlementCommandHandler : ICommandHandler<CreateSettlementCommand, Guid>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly IUnitOfWork unitOfWork;

    private const string InviteCodeChars = "abcdefghjkmnpqrstwxyzABCDEFGHJKLMNOPQRSTWXYZ23456789";
    private const int InviteCodeLength = 8;

    public CreateSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSettlementCommand request, CancellationToken cancellationToken)
    {
        var inviteCode = GenerateInviteCode();
        var settlementResult = Settlement.Create(request.Name, inviteCode);

        if (settlementResult.IsFailure)
        {
            return Result.Failure<Guid>(settlementResult.Error);
        }
        
        settlementRepository.Add(settlementResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return settlementResult.Value.Id.Value;
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