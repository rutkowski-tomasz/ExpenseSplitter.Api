﻿using ExpenseSplitter.Domain.Participants;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Infrastructure.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participants");

        builder.HasKey(participant => participant.Id);

        builder.Property(participant => participant.Id)
            .HasConversion(participantId => participantId.Value, value => new ParticipantId(value));

        builder
            .HasOne<Settlement>()
            .WithMany()
            .HasForeignKey(participant => participant.SettlementId);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(participant => participant.UserId);

        builder.Property(participant => participant.Nickname);
    }
}