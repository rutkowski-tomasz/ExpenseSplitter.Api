using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Participants;

public class ClaimParticipantCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ClaimParticipantCommandHandler handler;
    private readonly Settlement settlement;

    public ClaimParticipantCommandHandlerTests()
    {
        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());

        settlementUserRepository
            .GetBySettlementId(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(fixture.Create<SettlementUser>());

        settlementRepository
            .GetById(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(settlement);

        handler = new ClaimParticipantCommandHandler(
            settlementUserRepository,
            settlementRepository,
            unitOfWork
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenSettlementIdIsEmpty()
    {
        var command = fixture
            .Build<ClaimParticipantCommand>()
            .With(x => x.SettlementId, Guid.Empty)
            .Create();

        var validator = new ClaimParticipantCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.SettlementId));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenParticipantIdIsEmpty()
    {
        var command = fixture
            .Build<ClaimParticipantCommand>()
            .With(x => x.ParticipantId, Guid.Empty)
            .Create();

        var validator = new ClaimParticipantCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.ParticipantId));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementUserDoesNotExist()
    {
        settlementUserRepository
            .GetBySettlementId(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns((SettlementUser)default);

        var command = fixture.Create<ClaimParticipantCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenParticipantIdIsNotInSettlement()
    {
        var command = fixture.Create<ClaimParticipantCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(ParticipantErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = fixture
            .Build<ClaimParticipantCommand>()
            .With(x => x.ParticipantId, settlement.Participants[0].Id.Value)
            .With(x => x.SettlementId, settlement.Id.Value)
            .Create();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
