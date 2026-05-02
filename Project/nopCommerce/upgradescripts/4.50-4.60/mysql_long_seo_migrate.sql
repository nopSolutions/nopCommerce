USE nopcommerce;
SET @schema = 'nopcommerce';

CREATE FUNCTION is_exists (col_name text)
RETURNS boolean DETERMINISTIC
RETURN (SELECT COUNT(*)>0
FROM `information_schema`.`COLUMNS` 
WHERE 
    `TABLE_SCHEMA` = @schema 
	AND `TABLE_NAME` = 'store'
	AND COLUMN_NAME = col_name);

SET @col_name = 'DefaultTitle';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `store` ADD COLUMN `', @col_name, '` LONGTEXT NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'DefaultMetaDescription';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `store` ADD COLUMN `', @col_name, '` LONGTEXT NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'DefaultMetaKeywords';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `store` ADD COLUMN `', @col_name, '` LONGTEXT NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'HomepageDescription';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `store` ADD COLUMN `', @col_name, '` LONGTEXT NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;

SET @col_name = 'HomepageTitle';
SET @query = IF(is_exists(@col_name) <= 0, CONCAT('ALTER TABLE `store` ADD COLUMN `', @col_name, '` LONGTEXT NULL'), 'SET @exists = true');
PREPARE stmt FROM @query;
EXECUTE stmt;


DROP FUNCTION is_exists;