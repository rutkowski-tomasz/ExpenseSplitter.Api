using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Participants;

public class ClaimParticipantCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;
    private readonly ClaimParticipantCommandHandler handler;

    public ClaimParticipantCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        participantRepositoryMock = new Mock<IParticipantRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<SettlementUser>());

        participantRepositoryMock
            .Setup(x => x.IsParticipantInSettlement(It.IsAny<SettlementId>(), It.IsAny<ParticipantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        handler = new ClaimParticipantCommandHandler(
            settlementUserRepositoryMock.Object,
            participantRepositoryMock.Object,
            unitOfWorkMock.Object
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
        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementUser)null!);

        var command = fixture.Create<ClaimParticipantCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenParticipantIdIsNotInSettlement()
    {
        participantRepositoryMock
            .Setup(x => x.IsParticipantInSettlement(It.IsAny<SettlementId>(), It.IsAny<ParticipantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = fixture.Create<ClaimParticipantCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(ParticipantErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShoulSuccess()
    {
        var command = fixture.Create<ClaimParticipantCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
