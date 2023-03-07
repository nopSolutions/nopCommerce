DO $$
	BEGIN
		BEGIN
			ALTER TABLE "MessageTemplate" ADD COLUMN "AllowDirectReply" BOOLEAN NOT NULL DEFAULT FALSE;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column AllowDirectReply already exists in MessageTemplate.';
        END;
    END;
$$;

