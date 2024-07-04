﻿// <auto-generated />
using System;
using ExpenseSplitter.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240704125932_AddLastModifiedForEntities")]
    partial class AddLastModifiedForEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Allocations.Allocation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uuid")
                        .HasColumnName("expense_id");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("participant_id");

                    b.HasKey("Id")
                        .HasName("pk_allocations");

                    b.HasIndex("ExpenseId")
                        .HasDatabaseName("ix_allocations_expense_id");

                    b.HasIndex("ParticipantId")
                        .HasDatabaseName("ix_allocations_participant_id");

                    b.ToTable("allocations", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Expenses.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<Guid>("PayingParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("paying_participant_id");

                    b.Property<DateOnly>("PaymentDate")
                        .HasColumnType("date")
                        .HasColumnName("payment_date");

                    b.Property<Guid>("SettlementId")
                        .HasColumnType("uuid")
                        .HasColumnName("settlement_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_expenses");

                    b.HasIndex("PayingParticipantId")
                        .HasDatabaseName("ix_expenses_paying_participant_id");

                    b.HasIndex("SettlementId")
                        .HasDatabaseName("ix_expenses_settlement_id");

                    b.ToTable("expenses", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Participants.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nickname");

                    b.Property<Guid>("SettlementId")
                        .HasColumnType("uuid")
                        .HasColumnName("settlement_id");

                    b.HasKey("Id")
                        .HasName("pk_participants");

                    b.HasIndex("SettlementId")
                        .HasDatabaseName("ix_participants_settlement_id");

                    b.ToTable("participants", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.SettlementUsers.SettlementUser", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<Guid?>("ParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("participant_id");

                    b.Property<Guid>("SettlementId")
                        .HasColumnType("uuid")
                        .HasColumnName("settlement_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_settlement_users");

                    b.HasIndex("ParticipantId")
                        .HasDatabaseName("ix_settlement_users_participant_id");

                    b.HasIndex("SettlementId")
                        .HasDatabaseName("ix_settlement_users_settlement_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_settlement_users_user_id");

                    b.ToTable("settlement_users", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Settlements.Settlement", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on_utc");

                    b.Property<Guid>("CreatorUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("creator_user_id");

                    b.Property<string>("InviteCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("invite_code");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on_utc");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_settlements");

                    b.HasIndex("CreatorUserId")
                        .HasDatabaseName("ix_settlements_creator_user_id");

                    b.HasIndex("InviteCode")
                        .IsUnique()
                        .HasDatabaseName("ix_settlements_invite_code");

                    b.ToTable("settlements", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("nickname");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Infrastructure.Idempotency.IdempotentRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on_utc");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_idempotent_requests");

                    b.ToTable("idempotent_requests", (string)null);
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Allocations.Allocation", b =>
                {
                    b.HasOne("ExpenseSplitter.Api.Domain.Expenses.Expense", "Expense")
                        .WithMany("Allocations")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_allocations_expenses_expense_id");

                    b.HasOne("ExpenseSplitter.Api.Domain.Participants.Participant", null)
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_allocations_participants_participant_id");

                    b.Navigation("Expense");
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Expenses.Expense", b =>
                {
                    b.HasOne("ExpenseSplitter.Api.Domain.Participants.Participant", null)
                        .WithMany()
                        .HasForeignKey("PayingParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expenses_participants_paying_participant_id");

                    b.HasOne("ExpenseSplitter.Api.Domain.Settlements.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expenses_settlements_settlement_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Participants.Participant", b =>
                {
                    b.HasOne("ExpenseSplitter.Api.Domain.Settlements.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_participants_settlements_settlement_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.SettlementUsers.SettlementUser", b =>
                {
                    b.HasOne("ExpenseSplitter.Api.Domain.Participants.Participant", null)
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .HasConstraintName("fk_settlement_users_participants_participant_id");

                    b.HasOne("ExpenseSplitter.Api.Domain.Settlements.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_settlement_users_settlements_settlement_id");

                    b.HasOne("ExpenseSplitter.Api.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_settlement_users_users_user_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Settlements.Settlement", b =>
                {
                    b.HasOne("ExpenseSplitter.Api.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_settlements_users_creator_user_id");
                });

            modelBuilder.Entity("ExpenseSplitter.Api.Domain.Expenses.Expense", b =>
                {
                    b.Navigation("Allocations");
                });
#pragma warning restore 612, 618
        }
    }
}
