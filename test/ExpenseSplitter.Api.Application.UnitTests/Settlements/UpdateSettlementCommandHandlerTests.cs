using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class UpdateSettlementCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepository;
    private readonly Mock<ISettlementRepository> settlementRepository;
    private readonly Mock<IParticipantRepository> participantRepository;
    private readonly Mock<IDateTimeProvider> dateTimeProvider;
    private readonly UpdateSettlementCommandHandler handler;
    private readonly Settlement settlement;

    public UpdateSettlementCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementUserRepository = new Mock<ISettlementUserRepository>();
        settlementRepository = new Mock<ISettlementRepository>();
        participantRepository = new Mock<IParticipantRepository>();
        dateTimeProvider = new Mock<IDateTimeProvider>();
        Mock<IUnitOfWork> unitOfWork = new();

        settlementUserRepository
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        settlement = fixture.Create<Settlement>();
        settlementRepository
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        participantRepository
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<Participant>().ToList());
        
        handler = new UpdateSettlementCommandHandler(
            settlementUserRepository.Object,
            settlementRepository.Object,
            participantRepository.Object,
            dateTimeProvider.Object,
            unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserHasNoAccessToSettlement()
    {
        settlementUserRepository
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = fixture.Create<UpdateSettlementCommand>();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSettlementDoesNotExist()
    {
        settlementRepository
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Settlement) null!);

        var request = fixture.Create<UpdateSettlementCommand>();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(SettlementErrors.NotFound.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldSetUpdatedOnUtc()
    {
        var now = fixture.Create<DateTime>();
        dateTimeProvider
            .Setup(x => x.UtcNow)
            .Returns(now);

        var request = fixture.Create<UpdateSettlementCommand>();
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        settlement.UpdatedOnUtc.Should().Be(now);
    }
    
    [Fact]
    public async Task Handle_ShouldSetSettlementName()
    {
        var request = fixture.Create<UpdateSettlementCommand>();
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        settlement.Name.Should().Be(request.Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var request = fixture
            .Build<UpdateSettlementCommand>()
            .With(x => x.Name, "")
            .Create();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(SettlementErrors.EmptyName.Type);
    }

    [Fact]
    public async Task Handle_ShouldRemoveParticipants_WhenNotPresentInUpdateCommand()
    {
        var participant1 = fixture.Create<Participant>();
        var participant2 = fixture.Create<Participant>();

        participantRepository
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([participant1, participant2]);

        var request = fixture
            .Build<UpdateSettlementCommand>()
            .With(
                x => x.Participants, 
                new List<UpdateSettlementCommandParticipant>
                {
                    new (participant2.Id.Value, "Andrzej")
                }
            )
            .Create();
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        participantRepository.Verify(x => x.Remove(It.Is<Participant>(y => y.Id == participant1.Id)));
    }

    [Fact]
    public async Task Handle_ShouldAddParticipants_WhenNotPresentInRepository()
    {
        var participant1 = fixture.Create<Participant>();

        participantRepository
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([participant1]);

        var request = fixture
            .Build<UpdateSettlementCommand>()
            .With(
                x => x.Participants, 
                new List<UpdateSettlementCommandParticipant> 
                {
                    new (null, "Krzysztof"),
                    new (participant1.Id.Value, participant1.Nickname)
                }
            )
            .Create();
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        participantRepository.Verify(x => x.Add(It.Is<Participant>(y => y.Nickname == "Krzysztof")), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToUpdateParticipantToEmptyName()
    {
        var participant1 = fixture.Create<Participant>();

        participantRepository
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([participant1]);

        var request = fixture
            .Build<UpdateSettlementCommand>()
            .With(
                x => x.Participants, 
                new List<UpdateSettlementCommandParticipant>
                {
                    new (participant1.Id.Value, "")
                }
            )
            .Create();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ParticipantErrors.NicknameEmpty.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCreatingParticipantWithEmptyNickname()
    {
        var request = fixture
            .Build<UpdateSettlementCommand>()
            .With(
                x => x.Participants, 
                new List<UpdateSettlementCommandParticipant>
                {
                    new (null, "")
                }
            )
            .Create();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ParticipantErrors.NicknameEmpty.Type);
    }
}
