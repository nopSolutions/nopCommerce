-- =============================================================================
-- Author:		Dave Farinelli
-- Create date: 2020-10-22
-- Description:	Updates both ABC Promos and Promo Product Mappings from ISAM
-- =============================================================================

CREATE PROCEDURE dbo.UpdateAbcPromos 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -----------------------------------------------------------
	-- Populates both ABC Promos and ABC Promo-Product Mappings
	-----------------------------------------------------------

	IF OBJECT_ID('tempdb..##tmpErpPromos') IS NOT NULL DROP TABLE ##tmpErpPromos;
	IF OBJECT_ID('tempdb..#tmpPromos') IS NOT NULL DROP TABLE #tmpPromos;
	IF OBJECT_ID('tempdb..#tmpPromoProductMappings') IS NOT NULL DROP TABLE #tmpPromoProductMappings;

	DECLARE @TSQL varchar(8000), @CurrentMonth varchar(8), @PreviousMonth varchar(8)

	-- 2 weeks ahead to get upcoming promos
	SELECT @CurrentMonth = FORMAT(DATEADD(week, 2, GETDATE()), 'yyyyMMdd')
	-- one month behind to get expired promos
	SELECT @PreviousMonth = FORMAT(DATEADD(month, -1, GETDATE()), 'yyyyMMdd')

	SELECT @TSQL =
	'
	SELECT * INTO ##tmpErpPromos FROM OPENQUERY(
		ERP,
		''
		SELECT
		  pm.KEY_BUYERID + ''''_'''' + pm.KEY_DEPT + ''''_'''' + pm.KEY_PROMO_CODE as Name,
		  pm.PROMO_DESC,
		  pm.START_DATE,
		  pm.END_DATE,
		  im.ITEM_NUMBER
		FROM DA1_PROMO_MASTER pm
		JOIN DA1_PROMO_ITEMS_INCLUDE pii
		  ON pm.KEY_BUYERID = pii.KEY_BUYERID
		  AND pm.KEY_DEPT = pii.KEY_DEPT
		  AND pm.KEY_PROMO_CODE = pii.KEY_PROMO_CODE
		LEFT JOIN DA1_INVENTORY_MASTER im
		  ON pii.KEY_ITEM_NUMBER = im.ITEM_NUMBER
		WHERE
		  pm.START_DATE <= ''''' + @CurrentMonth + ''''' AND ''''' + @PreviousMonth + ''''' <= pm.END_DATE
		  AND im.ITEM_NUMBER IS NOT NULL

		UNION

		SELECT
		  pm.KEY_BUYERID + ''''_'''' + pm.KEY_DEPT + ''''_'''' + pm.KEY_PROMO_CODE as Name,
		  pm.PROMO_DESC,
		  pm.START_DATE,
		  pm.END_DATE,
		  im.ITEM_NUMBER
		FROM DA1_PROMO_MASTER pm
		JOIN DA1_INVENTORY_MASTER im
		  ON pm.INC_BRAND_1 = im.BRAND
		  OR pm.INC_BRAND_2 = im.BRAND
		  OR pm.INC_BRAND_3 = im.BRAND
		  OR pm.INC_BRAND_4 = im.BRAND
		  OR pm.INC_BRAND_5 = im.BRAND
		  OR pm.INC_BRAND_6 = im.BRAND
		  OR pm.INC_BRAND_7 = im.BRAND
		  OR pm.INC_BRAND_8 = im.BRAND
		  OR pm.INC_BRAND_9 = im.BRAND
		  OR pm.INC_BRAND_10 = im.BRAND
		  OR pm.INC_PROD_1 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_2 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_3 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_4 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_5 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_6 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_7 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_8 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_9 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_10 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_11 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_12 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_13 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_14 = im.PRODUCT_TYPE
		  OR pm.INC_PROD_15 = im.PRODUCT_TYPE
		LEFT JOIN DA1_PROMO_ITEMS_EXCLUDE pie
		  ON pm.KEY_BUYERID = pie.KEY_BUYERID
		  AND pm.KEY_DEPT = pie.KEY_DEPT
		  AND pm.KEY_PROMO_CODE = pie.KEY_PROMO_CODE
		  AND im.ITEM_NUMBER = pie.KEY_ITEM_NUMBER
		WHERE
		  pm.START_DATE <= ''''' + @CurrentMonth + ''''' AND ''''' + @PreviousMonth + ''''' <= pm.END_DATE
		  AND pie.KEY_BUYERID IS NULL
		  AND pie.KEY_DEPT IS NULL
		  AND pie.KEY_PROMO_CODE IS NULL
		  AND pie.KEY_ITEM_NUMBER IS NULL
		  AND im.ITEM_NUMBER IS NOT NULL
		  AND 
		  (
			((pm.INC_BRAND_1 IS NULL OR pm.INC_PROD_1 IS NULL) AND pm.KEY_DEPT = im.DEPARTMENT)
			OR
			(pm.INC_BRAND_1 IS NOT NULL AND pm.INC_PROD_1 IS NOT NULL) AND
			  (
			  (pm.INC_BRAND_1 = im.BRAND
			  OR pm.INC_BRAND_2 = im.BRAND
			  OR pm.INC_BRAND_3 = im.BRAND
			  OR pm.INC_BRAND_4 = im.BRAND
			  OR pm.INC_BRAND_5 = im.BRAND
			  OR pm.INC_BRAND_6 = im.BRAND
			  OR pm.INC_BRAND_7 = im.BRAND
			  OR pm.INC_BRAND_8 = im.BRAND
			  OR pm.INC_BRAND_9 = im.BRAND
			  OR pm.INC_BRAND_10 = im.BRAND) AND
			  (pm.INC_PROD_1 = im.PRODUCT_TYPE
				OR pm.INC_PROD_2 = im.PRODUCT_TYPE
				OR pm.INC_PROD_3 = im.PRODUCT_TYPE
				OR pm.INC_PROD_4 = im.PRODUCT_TYPE
				OR pm.INC_PROD_5 = im.PRODUCT_TYPE
				OR pm.INC_PROD_6 = im.PRODUCT_TYPE
				OR pm.INC_PROD_7 = im.PRODUCT_TYPE
				OR pm.INC_PROD_8 = im.PRODUCT_TYPE
				OR pm.INC_PROD_9 = im.PRODUCT_TYPE
				OR pm.INC_PROD_10 = im.PRODUCT_TYPE
				OR pm.INC_PROD_11 = im.PRODUCT_TYPE
				OR pm.INC_PROD_12 = im.PRODUCT_TYPE
				OR pm.INC_PROD_13 = im.PRODUCT_TYPE
				OR pm.INC_PROD_14 = im.PRODUCT_TYPE
				OR pm.INC_PROD_15 = im.PRODUCT_TYPE)
			  )
			)
		''
		)
	'

	EXEC (@TSQL)

	--------------------
	-- Update ABC Promos
	--------------------

	SELECT DISTINCT RTRIM(LTRIM(tep.NAME)) as NAME, tep.PROMO_DESC, tep.START_DATE, tep.END_DATE
	INTO #tmpPromos 
	FROM ##tmpErpPromos tep

	MERGE AbcPromo t USING #tmpPromos s
	ON (t.Name COLLATE SQL_Latin1_General_CP1_CI_AS = s.NAME COLLATE SQL_Latin1_General_CP1_CI_AS)
	WHEN MATCHED
		AND t.StartDate <> s.START_DATE OR t.EndDate <> s.END_DATE OR t.Description COLLATE SQL_Latin1_General_CP1_CI_AS <> s.PROMO_DESC
		THEN UPDATE SET
			t.Name = s.NAME,
			t.StartDate = s.START_DATE,
			t.EndDate = s.END_DATE,
			t.Description = s.PROMO_DESC
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (Name, Description, StartDate, EndDate) VALUES (s.NAME, s.PROMO_DESC, s.START_DATE, s.END_DATE)
	WHEN NOT MATCHED BY SOURCE THEN
		DELETE;
	--OUTPUT
	--   $action,
	--   CASE WHEN $action = 'INSERT' THEN Inserted.Name
	--		ELSE Deleted.Name
	--   END As Name,
	--   CASE WHEN $action = 'INSERT' THEN Inserted.StartDate
	--		ELSE Deleted.StartDate
	--   END As StartDate,
	--   CASE WHEN $action = 'INSERT' THEN Inserted.EndDate
	--		ELSE Deleted.EndDate
	--   END As EndDate;

	------------------------------------
	-- Update ABC Promo-Product Mappings
	------------------------------------

	SELECT ap.Id as Id, pad.Product_Id
	INTO #tmpPromoProductMappings
	FROM ##tmpErpPromos tep
	JOIN AbcPromo ap on ap.Name = RTRIM(LTRIM(tep.NAME)) COLLATE SQL_Latin1_General_CP1_CI_AS
	JOIN ProductAbcDescriptions pad on pad.AbcItemNumber = tep.ITEM_NUMBER COLLATE SQL_Latin1_General_CP1_CI_AS

	-- mattresses
	IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AbcMattressModel'))
	BEGIN
		INSERT INTO #tmpPromoProductMappings
			select distinct ap.Id, amm.ProductId as Product_Id
			  FROM ##tmpErpPromos tep
				JOIN AbcPromo ap on ap.Name = RTRIM(LTRIM(tep.NAME)) COLLATE SQL_Latin1_General_CP1_CI_AS
				JOIN AbcMattressEntry ame on ame.ItemNo = tep.ITEM_NUMBER COLLATE SQL_Latin1_General_CP1_CI_AS
				JOIN AbcMattressModel amm on ame.AbcMattressModelId = amm.Id
				JOIN AbcMattressPackage amp on amp.AbcMattressEntryId = ame.Id
	END

	MERGE ProductAbcPromo t USING #tmpPromoProductMappings s
	ON (t.AbcPromoId = s.Id AND t.ProductId = s.Product_Id)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (AbcPromoId, ProductId) VALUES (s.Id, s.Product_Id)
	WHEN NOT MATCHED BY SOURCE THEN
		DELETE;
	--OUTPUT
	--   $action,
	--   CASE WHEN $action = 'INSERT' THEN Inserted.AbcPromoId
	--		ELSE Deleted.AbcPromoId
	--   END As AbcPromoId,
	--   CASE WHEN $action = 'INSERT' THEN Inserted.ProductId
	--		ELSE Deleted.ProductId
	--   END As ProductId;

	------------------------------
	-- Update ABC Promo Quick Tabs
	------------------------------
	DELETE FROM SS_MAP_EntityMapping WHERE EntityId in (SELECT id FROM SS_QT_Tab WHERE Description LIKE '%Promotion Image%')
	DELETE FROM SS_QT_Tab WHERE Description LIKE '%Promotion Image%'

	INSERT INTO SS_QT_Tab (SystemName, DisplayName, Description, LimitedToStores, TabMode, DisplayOrder)
	SELECT TOP 1 WITH TIES
		'Rebate/Promo - Sku: ' + p.Sku as SystemName,
		'Rebates/Promos' as DisplayName,
		STUFF(
			(SELECT
				'<br /><a href="javascript:win = window.open(''/promotion_forms/' + ap2.Name + '.pdf'', ''Promotion Image'', ''height=500,width=750,top=25,left=25,resizable=yes''); win.focus()">' +
				ap2.Description + ' (Expires ' + convert(nvarchar(max), ap2.EndDate, 101) + ')</a>'
			FROM AbcPromo ap2
			JOIN ProductAbcPromo pap2 on pap2.AbcPromoId = ap2.Id and ap2.StartDate <= GETDATE() AND GETDATE() < ap2.EndDate
			WHERE pap2.ProductId = pap.ProductId
			FOR XML PATH(''), TYPE).value('.','varchar(max)'),1,6, '') AS Description,
		 0 as LimitedToStores,
		 5 as TabMode,
		 0 as DisplayOrder
	FROM ProductAbcPromo pap
	JOIN AbcPromo ap ON ap.Id = pap.AbcPromoId and ap.StartDate <= GETDATE() AND GETDATE() < ap.EndDate
	JOIN Product p ON p.Id = pap.ProductId
	ORDER BY ROW_NUMBER() OVER(PARTITION BY pap.ProductId ORDER BY pap.ProductId DESC);

	INSERT INTO SS_MAP_EntityMapping (EntityType, EntityId, MappedEntityId, DisplayOrder, MappingType)
	SELECT TOP 1 WITH TIES
	'10' AS EntityType, ssqt.Id AS EntityId, p.Id AS MappedEntityId, 0 AS DisplayOrder, 2 AS MappingType
	FROM SS_QT_Tab ssqt
	JOIN Product p ON p.Sku COLLATE SQL_Latin1_General_CP1_CI_AS = RIGHT(ssqt.SystemName, LEN(ssqt.SystemName) - CHARINDEX('Sku: ', ssqt.SystemName) - 4)
	WHERE Description LIKE '%Promotion%'
	ORDER BY ROW_NUMBER() OVER(PARTITION BY p.Id ORDER BY p.Id DESC);

	-- cleanup
	IF OBJECT_ID('tempdb..##tmpErpPromos') IS NOT NULL DROP TABLE ##tmpErpPromos;
	IF OBJECT_ID('tempdb..#tmpPromos') IS NOT NULL DROP TABLE #tmpPromos;
	IF OBJECT_ID('tempdb..#tmpPromoProductMappings') IS NOT NULL DROP TABLE #tmpPromoProductMappings;
END
