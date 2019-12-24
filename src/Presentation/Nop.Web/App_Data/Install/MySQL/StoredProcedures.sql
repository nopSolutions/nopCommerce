use nop_mysql_db_test111;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` FUNCTION `Check_Exists_FullText_Index`(
	`TableName` 	varchar(200),
	`IndexName` 	varchar(200)
) RETURNS tinyint(1)
BEGIN

RETURN exists(
			select distinct index_name from information_schema.statistics 
			where table_schema = database() 
			and TABLE_NAME = `TableName` and INDEX_TYPE = 'FULLTEXT' and INDEX_NAME like `IndexName`
		);
END$$
DELIMITER ;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `CategoryLoadAllPaged`(
	`ShowHidden`        	bool,
    `Name`              	text,
    `StoreId`           	int,
    `CustomerRoleIds`		text,
    `PageIndex`				int,
	`PageSize`				int,
    OUT `TotalRecords`		int
)
BEGIN
	Set @lengthId = (select CHAR_LENGTH(MAX(Id)) FROM Category);
	Set @lengthOrder = (select CHAR_LENGTH(MAX(DisplayOrder)) FROM Category);
	drop temporary table if exists OrderedCategories;
    create temporary table `OrderedCategories` (
		`id`	int,
        `Order` text
    );
    
    insert into `OrderedCategories`
	with recursive CategoryTree AS
	(
	  SELECT id, cast(concat(LPAD(DisplayOrder, @lengthOrder, '0'), '-' , LPAD(Id, @lengthId, '0'))  as char(500)) as `Order`
		FROM category
		WHERE ParentCategoryId = 0
	  UNION ALL
	  SELECT c.id, concat(sc.`Order`, '|', LPAD(c.DisplayOrder, @lengthOrder, '0'), '-' , LPAD(c.Id, @lengthId, '0')) as `Order`
		FROM CategoryTree AS sc 
		  JOIN category AS c ON sc.id = c.ParentCategoryId
	)
    select *
    from CategoryTree;
    
	select c.`Id`, c.`Name`, ct.`Order` 
		from category c
			inner join `OrderedCategories` as ct on c.Id = ct.Id
		#filter results
		where not c.Deleted
			and (ShowHidden OR c.Published)
            and (COALESCE(`Name`, '') = '' OR c.`Name` LIKE concat('%', `Name`, '%'))
            and (ShowHidden OR COALESCE(`CustomerRoleIds`, '') = '' OR not c.SubjectToAcl
				OR EXISTS (
					select 1 
                    from aclRecord as acl 
                    where find_in_set(acl.CustomerRoleId, CustomerRoleIds) 
						and acl.`EntityId` = c.`Id` AND acl.`EntityName` = 'Category')
			)
            and (not StoreId OR not c.`LimitedToStores`
				OR EXISTS (SELECT 1 FROM storemapping sm
					WHERE sm.`EntityId` = c.`Id` AND sm.`EntityName` = 'Category' AND sm.`StoreId` = StoreId
				)
			)
            and ct.Id > `PageSize` * `PageIndex`
	order by ct.`Order`, 1
    LIMIT `PageSize`;
    
    select count(*) from `OrderedCategories` into `TotalRecords`;
    
    drop temporary table if exists OrderedCategories;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `Create_FullText_Index`(
		`TableName` 	varchar(200),
		`ColumnNames` 	varchar(600),
		`IndexName` 	varchar(200),
    out `Result` 		bool
)
BEGIN
	set `Result` = true;
	select if (
		`Check_Exists_FullText_Index`(`TableName`, `IndexName`)
		,'select false into @stmt_result'
		, concat('CREATE FULLTEXT INDEX ', `IndexName`, ' ON ', `TableName`, '(', `ColumnNames`, ');')) into @a; 
	
	PREPARE stmt1 FROM @a;
	EXECUTE stmt1;
	DEALLOCATE PREPARE stmt1;
    
    Set `Result` = @stmt_result;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteGuests`(
	OnlyWithoutShoppingCart bool,
	CreatedFromUtc datetime,
	CreatedToUtc datetime,
	out TotalRecordsDeleted int
)
BEGIN
	create temporary table tmp_guests (CustomerId int);
    
    INSERT into tmp_guests (CustomerId)
	SELECT c.`Id` 
	FROM `Customer` c
		LEFT JOIN `ShoppingCartItem` sci ON sci.`CustomerId` = c.`Id`
		INNER JOIN (
			#guests only
			SELECT ccrm.`Customer_Id` 
			FROM `Customer_CustomerRole_Mapping` ccrm
				INNER JOIN `CustomerRole` cr ON cr.`Id` = ccrm.`CustomerRole_Id`
			WHERE cr.`SystemName` = 'Guests'
		) g ON g.`Customer_Id` = c.`Id`
		LEFT JOIN `Order` o ON o.`CustomerId` = c.`Id`
		LEFT JOIN `BlogComment` bc ON bc.`CustomerId` = c.`Id`
		LEFT JOIN `NewsComment` nc ON nc.`CustomerId` = c.`Id`
		LEFT JOIN `ProductReview` pr ON pr.`CustomerId` = c.`Id`
		LEFT JOIN `ProductReviewHelpfulness` prh ON prh.`CustomerId` = c.`Id`
		LEFT JOIN `PollVotingRecord` pvr ON pvr.`CustomerId` = c.`Id`
		LEFT JOIN `Forums_Topic` ft ON ft.`CustomerId` = c.`Id`
		LEFT JOIN `Forums_Post` fp ON fp.`CustomerId` = c.`Id`
	WHERE 1 = 1
		#no orders
		AND (o.Id is null)
		#no blog comments
		AND (bc.Id is null)
		#no news comments
		AND (nc.Id is null)
		#no product reviews
		AND (pr.Id is null)
		#no product reviews helpfulness
		AND (prh.Id is null)
		#no poll voting
		AND (pvr.Id is null)
		#no forum topics
		AND (ft.Id is null)
		#no forum topics
		AND (fp.Id is null)
		#no system accounts
		AND (c.IsSystemAccount = 0)
		#created from
		AND ((CreatedFromUtc is null) OR (c.`CreatedOnUtc` > @CreatedFromUtc))
		#created to
		AND ((CreatedToUtc is null) OR (c.`CreatedOnUtc` < @CreatedToUtc))
		#shopping cart items
		AND (OnlyWithoutShoppingCart OR (sci.Id is null));
        
    #delete guests
	DELETE from `Customer` WHERE `Id` IN (SELECT `CustomerId` FROM tmp_guests);
	
	#delete attributes
	DELETE FROM `GenericAttribute` 
    WHERE 
		`EntityId` IN (SELECT `CustomerId` FROM tmp_guests)
		AND (`KeyGroup` = N'Customer');
	
	#total records
	SELECT COUNT(*) FROM tmp_guests into TotalRecordsDeleted;
	
	DROP TEMPORARY TABLE tmp_guests; 
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `Drop_FullText_Index`(
		`TableName` 	varchar(200),
		`IndexName` 	varchar(200),
    out `Result` 		bool
)
BEGIN
	set `Result` = true;
	select if (
		`Check_Exists_FullText_Index`(`TableName`, `IndexName`)
		, concat('drop index ', `IndexName`, ' ON ', `TableName`, ';')
		, 'select false into @stmt_result') into @a; 
	
	PREPARE stmt1 FROM @a;
	EXECUTE stmt1;
	DEALLOCATE PREPARE stmt1;
    
    Set `Result` = @stmt_result;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `FullText_Disable`()
BEGIN
    call `Drop_FullText_Index`('Product', 'FT_IX_Product', @drop_result);
    call `Drop_FullText_Index`('LocalizedProperty', 'FT_IX_LocalizedProperty', @drop_result);
    call `Drop_FullText_Index`('ProductTag', 'FT_IX_ProductTag', @drop_result);    
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `FullText_Enable`()
BEGIN
	CALL `nop_mysql_db_test`.`Create_FullText_Index`('Product', 'Name, ShortDescription, FullDescription', 'FT_IX_Product',  @result);
    CALL `nop_mysql_db_test`.`Create_FullText_Index`('LocalizedProperty', 'LocaleValue', 'FT_IX_LocalizedProperty',  @result);
    CALL `nop_mysql_db_test`.`Create_FullText_Index`('ProductTag', 'Name', 'FT_IX_ProductTag',  @result);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `FullText_IsSupported`()
BEGIN
	select true;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `LanguagePackImport`(
	`LanguageId` 				int,
	`XmlPackage` 				longtext,
	`UpdateExistingResources` 	bit
)
BEGIN
	declare `row_index` int unsigned default 0;   
	declare `row_count` int unsigned;  
	declare `path_row` varchar(255); 
        
	IF EXISTS(SELECT * FROM `Language` WHERE Id = `LanguageId`) THEN
		set `row_count`  = extractValue(`XmlPackage`, concat('count(//Language/LocaleResource)')); 
        
        while `row_index` < `row_count` do                
			set `row_index` = `row_index` + 1;        
			set `path_row` = concat('//Language/LocaleResource[', `row_index`, ']');
			
			set @locale_name = extractValue(`XmlPackage`, concat(`path_row`, '/@Name'));
			set @locale_value = extractValue(`XmlPackage`, concat(`path_row`, '/Value'));
            
			IF (EXISTS (SELECT 1 FROM `LocaleStringResource` WHERE LanguageId = `LanguageId` AND ResourceName=@locale_name)) then
				IF (`UpdateExistingResources`) then
					UPDATE `LocaleStringResource` SET ResourceValue = @locale_value
					WHERE LanguageId= `LanguageId` AND ResourceName= @locale_name;
				end if;
			else
				INSERT INTO `LocaleStringResource` (`LanguageId`, `ResourceName`, `ResourceValue`)
				VALUES (`LanguageId`, @locale_name, @locale_value);
			end if;
        
		end while;
    end if;    
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `ProductLoadAllPaged`(
		`CategoryIds`										text,				#a list of category IDs (comma-separated list). e.g. 1,2,3
		`ManufacturerId`									int,
		`StoreId`											int,
		`VendorId`											int,
		`WarehouseId`										int,
		`ProductTypeId`										int, 				#product type identifier, null - load all products
		`VisibleIndividuallyOnly` 							bool, 				#0 - load all products , 1 - "visible indivially" only
		`MarkedAsNewOnly`									bool, 				#0 - load all products , 1 - "marked as new" only
		`ProductTagId`										int,
		`FeaturedProducts`									bool,				#0 featured only , 1 not featured only, null - load all products
		`PriceMin`											decimal(18, 4),
		`PriceMax`											decimal(18, 4),
		`Keywords`											nvarchar(4000),
		`SearchDescriptions` 								bool, 				#a value indicating whether to search by a specified "keyword" in product descriptions
		`SearchManufacturerPartNumber` 						bool, 				# a value indicating whether to search by a specified "keyword" in manufacturer part number
		`SearchSku`											bool, 				#a value indicating whether to search by a specified "keyword" in product SKU
		`SearchProductTags`  								bool, 				#a value indicating whether to search by a specified "keyword" in product tags
		`UseFullTextSearch`  								bool,
		`FullTextMode`										int, 				#0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
		`FilteredSpecs`										text,				#filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
		`LanguageId`										int,
		`OrderBy`											int, 				#0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
		`AllowedCustomerRoleIds`							text,				#a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
		`PageIndex`											int, 
		`PageSize`											int,
		`ShowHidden`										bool,
		`OverridePublished`									bit, 				#null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
		`LoadFilterableSpecificationAttributeOptionIds` 	bool, 				#a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	out	`FilterableSpecificationAttributeOptionIds` 		text, 				#the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	out	`TotalRecords`										int
)
BEGIN

END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `ProductTagCountLoadAll`(
	`StoreId` 					int,
	`AllowedCustomerRoleIds`	text	#a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
)
BEGIN
	#filter by customer role IDs (access control list)	
	SELECT pt.Id as `ProductTagId`, COUNT(p.Id) as `ProductCount`
	FROM ProductTag pt
	LEFT JOIN Product_ProductTag_Mapping pptm ON pt.`Id` = pptm.`ProductTag_Id`
	LEFT JOIN Product p ON pptm.`Product_Id` = p.`Id`
	WHERE
		not p.`Deleted`
		AND p.Published
		AND (`StoreId` = 0 or (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM `StoreMapping` sm 
			WHERE `sm`.EntityId = p.Id AND `sm`.EntityName = 'Product' and `sm`.StoreId=`StoreId`
			)))
		AND (length(`AllowedCustomerRoleIds`) = 0 or (not p.SubjectToAcl 
			OR EXISTS (
					select 1 
                    from aclRecord as acl 
                    where find_in_set(acl.CustomerRoleId, `AllowedCustomerRoleIds`) 
						and acl.`EntityId` = p.`Id` AND acl.`EntityName` = 'Product')
			))
	GROUP BY pt.Id
	ORDER BY pt.Id;
END$$
DELIMITER ;
