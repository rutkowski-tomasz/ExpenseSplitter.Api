﻿// <auto-generated />
using System;
using ExpenseSplitter.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpenseSplitter.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExpenseSplitter.Domain.ExpenseAllocations.ExpenseAllocation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uuid")
                        .HasColumnName("expense_id");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("participant_id");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_expense_allocations");

                    b.HasIndex("ExpenseId")
                        .HasDatabaseName("ix_expense_allocations_expense_id");

                    b.HasIndex("ParticipantId")
                        .HasDatabaseName("ix_expense_allocations_participant_id");

                    b.ToTable("expense_allocations", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Expenses.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("PayingParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("paying_participant_id");

                    b.Property<Guid>("SettlementId")
                        .HasColumnType("uuid")
                        .HasColumnName("settlement_id");

                    b.HasKey("Id")
                        .HasName("pk_expenses");

                    b.HasIndex("PayingParticipantId")
                        .HasDatabaseName("ix_expenses_paying_participant_id");

                    b.HasIndex("SettlementId")
                        .HasDatabaseName("ix_expenses_settlement_id");

                    b.ToTable("expenses", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Participants.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nickname");

                    b.Property<Guid>("SettlementId")
                        .HasColumnType("uuid")
                        .HasColumnName("settlement_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_participants");

                    b.HasIndex("SettlementId")
                        .HasDatabaseName("ix_participants_settlement_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_participants_user_id");

                    b.ToTable("participants", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Settlements.Settlement", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_settlements");

                    b.ToTable("settlements", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("nickname");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.ExpenseAllocations.ExpenseAllocation", b =>
                {
                    b.HasOne("ExpenseSplitter.Domain.Expenses.Expense", null)
                        .WithMany()
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expense_allocations_expense_expense_temp_id");

                    b.HasOne("ExpenseSplitter.Domain.Participants.Participant", null)
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expense_allocations_participant_participant_temp_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Expenses.Expense", b =>
                {
                    b.HasOne("ExpenseSplitter.Domain.Participants.Participant", null)
                        .WithMany()
                        .HasForeignKey("PayingParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expenses_participant_participant_temp_id");

                    b.HasOne("ExpenseSplitter.Domain.Settlements.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expenses_settlement_settlement_temp_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Domain.Participants.Participant", b =>
                {
                    b.HasOne("ExpenseSplitter.Domain.Settlements.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_participants_settlement_settlement_temp_id");

                    b.HasOne("ExpenseSplitter.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_participants_user_user_temp_id");
                });
#pragma warning restore 612, 618
        }
    }
}
