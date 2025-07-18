using AutoFixture.Dsl;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class UpdateSettlementCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateSettlementCommandHandler handler;
    private readonly Settlement settlement;
    private readonly DateTime someDateTime;

    public UpdateSettlementCommandHandlerTests()
    {
        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());

        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);
        
        someDateTime = fixture.Create<DateTime>();
        dateTimeProvider.UtcNow.Returns(someDateTime);

        handler = new UpdateSettlementCommandHandler(
            settlementUserRepository,
            settlementRepository,
            dateTimeProvider,
            unitOfWork
        );
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserHasNoAccessToSettlement()
    {
        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        var command = ComposeCommand().Create();
        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSettlementDoesNotExist()
    {
        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns((Settlement) default);

        var command = ComposeCommand().Create();
        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(SettlementErrors.NotFound.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldSetUpdatedOnUtc()
    {
        var command = ComposeCommand().Create();

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        settlement.UpdatedOnUtc.Should().Be(someDateTime);
    }
    
    [Fact]
    public async Task Handle_ShouldSetSettlementName()
    {
        var command = ComposeCommand().Create();
        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        settlement.Name.Should().Be(command.Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var command = ComposeCommand()
            .With(x => x.Name, "")
            .Create();
        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(SettlementErrors.EmptyName.Type);
    }

    [Fact]
    public async Task Handle_ShouldRemoveParticipants_WhenNotPresentInUpdateCommand()
    {
        var command = ComposeCommand()
            .With(x => x.Participants, [
                new UpdateSettlementCommandParticipant(settlement.Participants[1].Id.Value, "Andrzej")
            ])
            .Create();

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        settlement.Participants.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldAddParticipants_WhenNotPresentInRepository()
    {
        var firstParticipantId = settlement.Participants[0].Id;
        var secondParticipantName = settlement.Participants[1].Nickname;

        var command = ComposeCommand()
            .With(
                x => x.Participants, 
                new List<UpdateSettlementCommandParticipant> 
                {
                    new (null, "Krzysztof"),
                    new (settlement.Participants[1].Id.Value, settlement.Participants[1].Nickname)
                }
            )
            .Create();

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        settlement.Participants.Should().HaveCount(2);
        settlement.Participants.Should().NotContain(x => x.Id == firstParticipantId);
        settlement.Participants.Should().Contain(x => x.Nickname == secondParticipantName);
        settlement.Participants.Should().Contain(x => x.Nickname == "Krzysztof");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToUpdateParticipantToEmptyName()
    {
        var command = ComposeCommand()
            .With(x => x.Participants, [
                new UpdateSettlementCommandParticipant(settlement.Participants[1].Id.Value, "")
            ])
            .Create();

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(ParticipantErrors.NicknameEmpty.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCreatingParticipantWithEmptyNickname()
    {
        var command = ComposeCommand()
            .With(x => x.Participants, [
                new UpdateSettlementCommandParticipant(null, "")
            ])
            .Create();

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(ParticipantErrors.NicknameEmpty.Type);
    }
    
    private IPostprocessComposer<UpdateSettlementCommand> ComposeCommand()
    {
        var participants = settlement.Participants.Select(x => new UpdateSettlementCommandParticipant(
            x.Id.Value,
            fixture.Create<string>())
        ).ToList();

        return fixture
            .Build<UpdateSettlementCommand>()
            .With(x => x.Id, settlement.Id.Value)
            .With(x => x.Participants, participants);
    }
}
