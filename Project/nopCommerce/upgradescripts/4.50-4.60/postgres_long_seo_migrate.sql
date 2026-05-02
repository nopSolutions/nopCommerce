DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Store" ADD COLUMN "DefaultTitle" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column DefaultTitle already exists in Store.';
        END;
    END;
$$;

DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Store" ADD COLUMN "DefaultMetaDescription" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column DefaultMetaDescription already exists in Store.';
        END;
    END;
$$;

DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Store" ADD COLUMN "DefaultMetaKeywords" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column DefaultMetaKeywords already exists in Store.';
        END;
    END;
$$;

DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Store" ADD COLUMN "HomepageDescription" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column HomepageDescription already exists in Store.';
        END;
    END;
$$;

DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Store" ADD COLUMN "HomepageTitle" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column HomepageTitle already exists in Store.';
        END;
    END;
$$;