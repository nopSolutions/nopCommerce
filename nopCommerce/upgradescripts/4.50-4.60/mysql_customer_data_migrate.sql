USE nopcommerce;
SET @schema = 'nopcommerce';

CREATE FUNCTION is_exists (col_name text)
RETURNS boolean DETERMINISTIC
RETURN (SELECT COUNT(*)>0
FROM `information_schema`.`COLUMNS` 
WHERE 
    `TABLE_SCHEMA` = @schema 
	AND `TABLE_NAME` = 'Customer'
	AND COLUMN_NAME = col_name);

SET @col_name = 'FirstName';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'LastName';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'Gender';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'Company';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'StreetAddress';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'StreetAddress2';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'ZipPostalCode';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'TimeZoneId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'City';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'County';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'Phone';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'Fax';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'VatNumber';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` nvarchar(1000) NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'CustomCustomerAttributesXML';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` longtext NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'CountryId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NOT NULL DEFAULT(0)'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'StateProvinceId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NOT NULL DEFAULT(0)'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'VatNumberStatusId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NOT NULL DEFAULT(0)'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'CurrencyId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'LanguageId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'TaxDisplayTypeId';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` int NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'DateOfBirth';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `Customer` ADD COLUMN `', @col_name, '` datetime NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

DROP FUNCTION is_exists;

SET @fk_name = 'FK_Customer_CurrencyId_Currency_Id';
SELECT COUNT(*)>0
INTO @exists
FROM `information_schema`.`REFERENTIAL_CONSTRAINTS`
WHERE `CONSTRAINT_SCHEMA` = @schema
AND `TABLE_NAME` = 'Customer'
AND `REFERENCED_TABLE_NAME` = 'Currency'
AND `CONSTRAINT_NAME` = @fk_name;

SET @query = IF(@exists <= 0, CONCAT('ALTER TABLE `Customer` ADD CONSTRAINT `', @fk_name,'` FOREIGN KEY(`CurrencyId`) REFERENCES `Currency` (`Id`)'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @fk_name = 'FK_Customer_LanguageId_Language_Id';
SELECT COUNT(*)>0
INTO @exists
FROM `information_schema`.`REFERENTIAL_CONSTRAINTS`
WHERE `CONSTRAINT_SCHEMA` = @schema
AND `TABLE_NAME` = 'Customer'
AND `REFERENCED_TABLE_NAME` = 'Language'
AND `CONSTRAINT_NAME` = @fk_name;

SET @query = IF(@exists <= 0, CONCAT('ALTER TABLE `Customer` ADD CONSTRAINT `', @fk_name,'` FOREIGN KEY(`LanguageId`) REFERENCES `Language` (`Id`)'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

-- delete duplicate entities
With `cte_duplicates` AS 
(SELECT `Id`, `KeyGroup`, `Key`, `EntityId`, row_number() 
	OVER(PARTITION BY `KeyGroup`, `Key`, `EntityId` order by `KeyGroup`, `Key`, `EntityId`) AS rownumber  
	FROM `GenericAttribute` 
	WHERE `KeyGroup` = 'Customer' AND 
	`Key` IN ('FirstName', 'LastName', 'Gender', 'Company', 
		'StreetAddress', 'StreetAddress2', 'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 
		'TimeZoneId', 'CustomCustomerAttributes', 'CountryId', 'StateProvinceId', 'VatNumberStatusId', 
		'CurrencyId', 'LanguageId', 'TaxDisplayTypeId', 'DateOfBirth'))
DELETE FROM `GenericAttribute` WHERE `Id` IN (SELECT `Id` FROM `cte_duplicates` WHERE `rownumber` != 1);

-- delete invalid country ids
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'CountryId' AND `Value` NOT IN (SELECT `Id` FROM `Country`);

-- delete invalid language ids
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'LanguageId' AND `Value` NOT IN (SELECT `Id` FROM `Language`);

-- truncate if length is more than 1000
UPDATE `GenericAttribute` SET `Value` = SUBSTRING(`Value`, 1, 1000) WHERE `KeyGroup` = 'Customer' AND 
 `Key` IN ('FirstName', 'LastName', 'Gender', 'Company', 'StreetAddress', 'StreetAddress2',
  'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 'TimeZoneId');
  
-- data type cast
UPDATE `GenericAttribute` SET `Value` = CAST(`Value` AS unsigned) WHERE `KeyGroup` = 'Customer' AND 
 `Key` IN ('CountryId', 'StateProvinceId', 'VatNumberStatusId');
UPDATE `GenericAttribute` SET `Value` = case CAST(`Value` AS unsigned) WHEN 0 THEN NULL ELSE CAST(`Value` AS unsigned) END WHERE `KeyGroup` = 'Customer' AND 
 `Key` IN ('CurrencyId', 'LanguageId', 'TaxDisplayTypeId');
UPDATE `GenericAttribute` SET `Value` = STR_TO_DATE(`Value`,'%Y-%m-%dT%H:%i:%s') WHERE `KeyGroup` = 'Customer' AND
 `Key` = 'DateOfBirth';
 
-- Move data FROM GA to `Customer` table
-- FirstName
UPDATE `Customer` AS `c` SET `FirstName` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'FirstName') WHERE `FirstName` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'FirstName';

-- LastName
UPDATE `Customer` AS `c` SET `LastName` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'LastName') WHERE `LastName` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'LastName';

-- Gender
UPDATE `Customer` AS `c` SET `Gender` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'Gender') WHERE `Gender` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'Gender';

-- Company
UPDATE `Customer` AS `c` SET `Company` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'Company') WHERE `Company` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'Company';

-- StreetAddress
UPDATE `Customer` AS `c` SET `StreetAddress` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'StreetAddress') WHERE `StreetAddress` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'StreetAddress';

-- StreetAddress2
UPDATE `Customer` AS `c` SET `StreetAddress2` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'StreetAddress2') WHERE `StreetAddress2` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'StreetAddress2';

-- ZipPostalCode
UPDATE `Customer` AS `c` SET `ZipPostalCode` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'ZipPostalCode') WHERE `ZipPostalCode` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'ZipPostalCode';

-- City
UPDATE `Customer` AS `c` SET `City` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'City') WHERE `City` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'City';

-- County
UPDATE `Customer` AS `c` SET `County` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'County') WHERE `County` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'County';

-- Phone
UPDATE `Customer` AS `c` SET `Phone` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'Phone') WHERE `Phone` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'Phone';

-- Fax
UPDATE `Customer` AS `c` SET `Fax` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'Fax') WHERE `Fax` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'Fax';

-- VatNumber
UPDATE `Customer` AS `c` SET `VatNumber` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'VatNumber') WHERE `VatNumber` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'VatNumber';

-- TimeZoneId
UPDATE `Customer` AS `c` SET `TimeZoneId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'TimeZoneId') WHERE `TimeZoneId` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'TimeZoneId';

-- CustomCustomerAttributesXML
UPDATE `Customer` AS `c` SET `CustomCustomerAttributesXML` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'CustomCustomerAttributes') WHERE `CustomCustomerAttributesXML` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'CustomCustomerAttributes';

-- CountryId
UPDATE `Customer` AS `c` SET `CountryId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'CountryId') WHERE `CountryId` = 0 AND EXISTS (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'CountryId');
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'CountryId';

-- StateProvinceId
UPDATE `Customer` AS `c` SET `StateProvinceId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'StateProvinceId') WHERE `StateProvinceId` = 0 AND EXISTS (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'StateProvinceId');
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'StateProvinceId';

-- VatNumberStatusId
UPDATE `Customer` AS `c` SET `VatNumberStatusId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'VatNumberStatusId') WHERE `VatNumberStatusId` = 0 AND EXISTS (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'VatNumberStatusId');
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'VatNumberStatusId';

-- CurrencyId
UPDATE `Customer` AS `c` SET `CurrencyId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'CurrencyId') WHERE `CurrencyId` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'CurrencyId';

-- LanguageId
UPDATE `Customer` AS `c` SET `LanguageId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'LanguageId') WHERE `LanguageId` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'LanguageId';

-- TaxDisplayTypeId
UPDATE `Customer` AS `c` SET `TaxDisplayTypeId` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'TaxDisplayTypeId') WHERE `TaxDisplayTypeId` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'TaxDisplayTypeId';

-- DateOfBirth
UPDATE `Customer` AS `c` SET `DateOfBirth` = (SELECT `Value` FROM `GenericAttribute` AS `ga` WHERE `ga`.`EntityId` = `c`.`Id` AND `ga`.`KeyGroup` = 'Customer' AND `ga`.`Key` = 'DateOfBirth') WHERE `DateOfBirth` is NULL;
DELETE FROM `GenericAttribute` WHERE `KeyGroup` = 'Customer' AND `Key` = 'DateOfBirth';