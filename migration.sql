CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE users (
        id uuid NOT NULL,
        nickname character varying(50) NOT NULL,
        email character varying(200) NOT NULL,
        CONSTRAINT pk_users PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE settlements (
        id uuid NOT NULL,
        name text NOT NULL,
        invite_code character varying(20) NOT NULL,
        creator_user_id uuid NOT NULL,
        CONSTRAINT pk_settlements PRIMARY KEY (id),
        CONSTRAINT fk_settlements_user_user_temp_id FOREIGN KEY (creator_user_id) REFERENCES users (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE participants (
        id uuid NOT NULL,
        settlement_id uuid NOT NULL,
        user_id uuid,
        nickname text NOT NULL,
        CONSTRAINT pk_participants PRIMARY KEY (id),
        CONSTRAINT fk_participants_settlement_settlement_temp_id FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE,
        CONSTRAINT fk_participants_user_user_temp_id FOREIGN KEY (user_id) REFERENCES users (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE expenses (
        id uuid NOT NULL,
        settlement_id uuid NOT NULL,
        title text NOT NULL,
        amount numeric NOT NULL,
        payment_date timestamp with time zone NOT NULL,
        paying_participant_id uuid NOT NULL,
        CONSTRAINT pk_expenses PRIMARY KEY (id),
        CONSTRAINT fk_expenses_participant_participant_temp_id FOREIGN KEY (paying_participant_id) REFERENCES participants (id) ON DELETE CASCADE,
        CONSTRAINT fk_expenses_settlement_settlement_temp_id FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE settlement_users (
        id uuid NOT NULL,
        settlement_id uuid NOT NULL,
        user_id uuid NOT NULL,
        participant_id uuid,
        CONSTRAINT pk_settlement_users PRIMARY KEY (id),
        CONSTRAINT fk_settlement_users_participants_participant_id1 FOREIGN KEY (participant_id) REFERENCES participants (id),
        CONSTRAINT fk_settlement_users_settlements_settlement_id1 FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE,
        CONSTRAINT fk_settlement_users_user_user_temp_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE TABLE expense_allocations (
        id uuid NOT NULL,
        expense_id uuid NOT NULL,
        participant_id uuid NOT NULL,
        amount numeric NOT NULL,
        CONSTRAINT pk_expense_allocations PRIMARY KEY (id),
        CONSTRAINT fk_expense_allocations_expense_expense_temp_id FOREIGN KEY (expense_id) REFERENCES expenses (id) ON DELETE CASCADE,
        CONSTRAINT fk_expense_allocations_participant_participant_temp_id FOREIGN KEY (participant_id) REFERENCES participants (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_expense_allocations_expense_id ON expense_allocations (expense_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_expense_allocations_participant_id ON expense_allocations (participant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_expenses_paying_participant_id ON expenses (paying_participant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_expenses_settlement_id ON expenses (settlement_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_participants_settlement_id ON participants (settlement_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_participants_user_id ON participants (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_settlement_users_participant_id ON settlement_users (participant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_settlement_users_settlement_id ON settlement_users (settlement_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_settlement_users_user_id ON settlement_users (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE INDEX ix_settlements_creator_user_id ON settlements (creator_user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE UNIQUE INDEX ix_settlements_invite_code ON settlements (invite_code);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    CREATE UNIQUE INDEX ix_users_email ON users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231207184247_CreateDatabase') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231207184247_CreateDatabase', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231209125222_RenameToAllocation') THEN
    DROP TABLE expense_allocations;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231209125222_RenameToAllocation') THEN
    CREATE TABLE allocations (
        id uuid NOT NULL,
        expense_id uuid NOT NULL,
        participant_id uuid NOT NULL,
        amount numeric NOT NULL,
        CONSTRAINT pk_allocations PRIMARY KEY (id),
        CONSTRAINT fk_allocations_expense_expense_temp_id FOREIGN KEY (expense_id) REFERENCES expenses (id) ON DELETE CASCADE,
        CONSTRAINT fk_allocations_participant_participant_temp_id FOREIGN KEY (participant_id) REFERENCES participants (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231209125222_RenameToAllocation') THEN
    CREATE INDEX ix_allocations_expense_id ON allocations (expense_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231209125222_RenameToAllocation') THEN
    CREATE INDEX ix_allocations_participant_id ON allocations (participant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231209125222_RenameToAllocation') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231209125222_RenameToAllocation', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE participants DROP CONSTRAINT fk_participants_user_user_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE settlement_users DROP CONSTRAINT fk_settlement_users_user_user_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE settlements DROP CONSTRAINT fk_settlements_user_user_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    DROP INDEX ix_participants_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE participants DROP COLUMN user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE settlement_users ADD CONSTRAINT fk_settlement_users_users_user_temp_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    ALTER TABLE settlements ADD CONSTRAINT fk_settlements_users_user_temp_id FOREIGN KEY (creator_user_id) REFERENCES users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231218195230_RemoveUserIdFromParticipant') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231218195230_RemoveUserIdFromParticipant', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219113413_SettlementDateTracking') THEN
    ALTER TABLE settlements ADD created_on_utc timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219113413_SettlementDateTracking') THEN
    ALTER TABLE settlements ADD updated_on_utc timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219113413_SettlementDateTracking') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231219113413_SettlementDateTracking', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219114708_SettlementAndExpenseVersion') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231219114708_SettlementAndExpenseVersion', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219115450_PaymentOnlyDate') THEN
    ALTER TABLE expenses ALTER COLUMN payment_date TYPE date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219115450_PaymentOnlyDate') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231219115450_PaymentOnlyDate', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219135230_ExpenseAllocationRelation') THEN
    ALTER TABLE allocations DROP CONSTRAINT fk_allocations_expense_expense_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219135230_ExpenseAllocationRelation') THEN
    ALTER TABLE allocations ADD CONSTRAINT fk_allocations_expense_expense_temp_id1 FOREIGN KEY (expense_id) REFERENCES expenses (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20231219135230_ExpenseAllocationRelation') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20231219135230_ExpenseAllocationRelation', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240116145328_IdempotentRequest') THEN
    CREATE TABLE idempotent_requests (
        id uuid NOT NULL,
        name text NOT NULL,
        created_on_utc timestamp with time zone NOT NULL,
        CONSTRAINT pk_idempotent_requests PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240116145328_IdempotentRequest') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20240116145328_IdempotentRequest', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE allocations DROP CONSTRAINT fk_allocations_expense_expense_temp_id1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE allocations DROP CONSTRAINT fk_allocations_participant_participant_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE expenses DROP CONSTRAINT fk_expenses_participant_participant_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE expenses DROP CONSTRAINT fk_expenses_settlement_settlement_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE participants DROP CONSTRAINT fk_participants_settlement_settlement_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users DROP CONSTRAINT fk_settlement_users_participants_participant_id1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users DROP CONSTRAINT fk_settlement_users_settlements_settlement_id1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users DROP CONSTRAINT fk_settlement_users_users_user_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlements DROP CONSTRAINT fk_settlements_users_user_temp_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE users ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlements ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE participants ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE expenses ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE allocations ADD last_modified timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE allocations ADD CONSTRAINT fk_allocations_expenses_expense_id FOREIGN KEY (expense_id) REFERENCES expenses (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE allocations ADD CONSTRAINT fk_allocations_participants_participant_id FOREIGN KEY (participant_id) REFERENCES participants (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE expenses ADD CONSTRAINT fk_expenses_participants_paying_participant_id FOREIGN KEY (paying_participant_id) REFERENCES participants (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE expenses ADD CONSTRAINT fk_expenses_settlements_settlement_id FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE participants ADD CONSTRAINT fk_participants_settlements_settlement_id FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users ADD CONSTRAINT fk_settlement_users_participants_participant_id FOREIGN KEY (participant_id) REFERENCES participants (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users ADD CONSTRAINT fk_settlement_users_settlements_settlement_id FOREIGN KEY (settlement_id) REFERENCES settlements (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlement_users ADD CONSTRAINT fk_settlement_users_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    ALTER TABLE settlements ADD CONSTRAINT fk_settlements_users_creator_user_id FOREIGN KEY (creator_user_id) REFERENCES users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20240704125932_AddLastModifiedForEntities') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20240704125932_AddLastModifiedForEntities', '9.0.0');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20241220142130_AlignRename') THEN
    DROP TABLE idempotent_requests;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20241220142130_AlignRename') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20241220142130_AlignRename', '9.0.0');
    END IF;
END $EF$;
COMMIT;

