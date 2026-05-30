DO $$ 
    BEGIN
        BEGIN
            ALTER TABLE "Customer" ADD COLUMN "FirstName" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column FirstName already exists in Customer.';
        END;
    END;
$$;


DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "LastName" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column LastName already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "Gender" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column Gender already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "Company" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column Company already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "StreetAddress" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column StreetAddress already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "StreetAddress2" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column StreetAddress2 already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "ZipPostalCode" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column ZipPostalCode already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "TimeZoneId" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column TimeZoneId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "City" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column City already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "County" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column County already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "Phone" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column Phone already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "Fax" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column Fax already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "VatNumber" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column VatNumber already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "CustomCustomerAttributesXML" citext NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column CustomCustomerAttributesXML already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "CountryId" integer NOT NULL default(0);
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column CountryId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "StateProvinceId" integer NOT NULL default(0);
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column StateProvinceId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "VatNumberStatusId" integer NOT NULL default(0);
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column VatNumberStatusId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "CurrencyId" integer NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column CurrencyId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "LanguageId" integer NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column LanguageId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "TaxDisplayTypeId" integer NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column TaxDisplayTypeId already exists in Customer.';
        END;
    END;
$$;

DO $$
	BEGIN
		BEGIN
			ALTER TABLE "Customer" ADD COLUMN "DateOfBirth" timestamp without time zone NULL;
        EXCEPTION
            WHEN duplicate_column THEN RAISE NOTICE 'column DateOfBirth already exists in Customer.';
        END;
    END;
$$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Customer_CurrencyId_Currency_Id') THEN
        ALTER TABLE "Customer"
            ADD CONSTRAINT "FK_Customer_CurrencyId_Currency_Id"
            FOREIGN KEY ("CurrencyId") REFERENCES "Currency"("Id");
	ELSE
		RAISE NOTICE 'FK FK_Customer_CurrencyId_Currency_Id already exists.';
    END IF;
END;
$$;


DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Customer_LanguageId_Language_Id') THEN
        ALTER TABLE "Customer"
            ADD CONSTRAINT "FK_Customer_LanguageId_Language_Id"
            FOREIGN KEY ("LanguageId") REFERENCES "Language"("Id");
	ELSE
		RAISE NOTICE 'FK FK_Customer_LanguageId_Language_Id already exists.';
    END IF;
END;
$$;

-- delete duplicate entities
With "cte_duplicates" AS 
(SELECT "Id", "KeyGroup", "Key", "EntityId", row_number() 
	OVER(PARTITION BY "KeyGroup", "Key", "EntityId" order by "KeyGroup", "Key", "EntityId") AS rownumber  
	FROM "GenericAttribute" 
	WHERE "KeyGroup" = 'Customer' AND 
	"Key" IN ('FirstName', 'LastName', 'Gender', 'Company', 
		'StreetAddress', 'StreetAddress2', 'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 
		'TimeZoneId', 'CustomCustomerAttributes', 'CountryId', 'StateProvinceId', 'VatNumberStatusId', 
		'CurrencyId', 'LanguageId', 'TaxDisplayTypeId', 'DateOfBirth'))
DELETE FROM "GenericAttribute" WHERE "Id" IN (SELECT "Id" FROM "cte_duplicates" WHERE "rownumber" != 1);

-- delete invalid country ids
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'CountryId' AND "Value" NOT IN (SELECT CAST("Id" AS citext) FROM "Country");

-- delete invalid language ids
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'LanguageId' AND "Value" NOT IN (SELECT CAST("Id" AS citext) FROM "Language");

-- truncate if length is more than 1000
UPDATE "GenericAttribute" SET "Value" = SUBSTRING("Value", 1, 1000) WHERE "KeyGroup" = 'Customer' AND 
 "Key" IN ('FirstName', 'LastName', 'Gender', 'Company', 'StreetAddress', 'StreetAddress2',
  'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 'TimeZoneId');
  
-- data type cast
UPDATE "GenericAttribute" SET "Value" = CAST("Value" AS integer) WHERE "KeyGroup" = 'Customer' AND 
 "Key" IN ('CountryId', 'StateProvinceId', 'VatNumberStatusId');
UPDATE "GenericAttribute" SET "Value" = case CAST("Value" AS integer) WHEN 0 THEN NULL ELSE CAST("Value" AS integer) END WHERE "KeyGroup" = 'Customer' AND 
 "Key" IN ('CurrencyId', 'LanguageId', 'TaxDisplayTypeId');
UPDATE "GenericAttribute" SET "Value" = TO_TIMESTAMP("Value", '%Y-%m-%dT%H:%i:%s') WHERE "KeyGroup" = 'Customer' AND
 "Key" = 'DateOfBirth';
 
-- Move data FROM GA to "Customer" table
-- FirstName
UPDATE "Customer" AS "c" SET "FirstName" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'FirstName') WHERE "FirstName" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'FirstName';

-- LastName
UPDATE "Customer" AS "c" SET "LastName" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'LastName') WHERE "LastName" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'LastName';

-- Gender
UPDATE "Customer" AS "c" SET "Gender" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'Gender') WHERE "Gender" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'Gender';

-- Company
UPDATE "Customer" AS "c" SET "Company" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'Company') WHERE "Company" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'Company';

-- StreetAddress
UPDATE "Customer" AS "c" SET "StreetAddress" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'StreetAddress') WHERE "StreetAddress" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'StreetAddress';

-- StreetAddress2
UPDATE "Customer" AS "c" SET "StreetAddress2" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'StreetAddress2') WHERE "StreetAddress2" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'StreetAddress2';

-- ZipPostalCode
UPDATE "Customer" AS "c" SET "ZipPostalCode" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'ZipPostalCode') WHERE "ZipPostalCode" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'ZipPostalCode';

-- City
UPDATE "Customer" AS "c" SET "City" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'City') WHERE "City" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'City';

-- County
UPDATE "Customer" AS "c" SET "County" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'County') WHERE "County" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'County';

-- Phone
UPDATE "Customer" AS "c" SET "Phone" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'Phone') WHERE "Phone" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'Phone';

-- Fax
UPDATE "Customer" AS "c" SET "Fax" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'Fax') WHERE "Fax" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'Fax';

-- VatNumber
UPDATE "Customer" AS "c" SET "VatNumber" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'VatNumber') WHERE "VatNumber" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'VatNumber';

-- TimeZoneId
UPDATE "Customer" AS "c" SET "TimeZoneId" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'TimeZoneId') WHERE "TimeZoneId" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'TimeZoneId';

-- CustomCustomerAttributesXML
UPDATE "Customer" AS "c" SET "CustomCustomerAttributesXML" = (SELECT "Value" FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'CustomCustomerAttributes') WHERE "CustomCustomerAttributesXML" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'CustomCustomerAttributes';

-- CountryId
UPDATE "Customer" AS "c" SET "CountryId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'CountryId') WHERE "CountryId" = 0 AND EXISTS (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'CountryId');
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'CountryId';

-- StateProvinceId
UPDATE "Customer" AS "c" SET "StateProvinceId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'StateProvinceId') WHERE "StateProvinceId" = 0 AND EXISTS (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'StateProvinceId');
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'StateProvinceId';

-- VatNumberStatusId
UPDATE "Customer" AS "c" SET "VatNumberStatusId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'VatNumberStatusId') WHERE "VatNumberStatusId" = 0 AND EXISTS (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'VatNumberStatusId');
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'VatNumberStatusId';

-- CurrencyId
UPDATE "Customer" AS "c" SET "CurrencyId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'CurrencyId') WHERE "CurrencyId" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'CurrencyId';

-- LanguageId
UPDATE "Customer" AS "c" SET "LanguageId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'LanguageId') WHERE "LanguageId" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'LanguageId';

-- TaxDisplayTypeId
UPDATE "Customer" AS "c" SET "TaxDisplayTypeId" = (SELECT CAST("Value" as integer) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'TaxDisplayTypeId') WHERE "TaxDisplayTypeId" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'TaxDisplayTypeId';

-- DateOfBirth
UPDATE "Customer" AS "c" SET "DateOfBirth" = (SELECT CAST("Value" as timestamp without time zone) FROM "GenericAttribute" AS "ga" WHERE "ga"."EntityId" = "c"."Id" AND "ga"."KeyGroup" = 'Customer' AND "ga"."Key" = 'DateOfBirth') WHERE "DateOfBirth" is NULL;
DELETE FROM "GenericAttribute" WHERE "KeyGroup" = 'Customer' AND "Key" = 'DateOfBirth';
