USE [onjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[ProductLoadAllPaged_V6]    Script Date: 12/7/2023 4:25:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--execution exampls
-- exec ProductLoadAllPaged_V5 @ProductIds=N'1',@CustomerId=1,@ProfileTypeId=1
-- exec ProductLoadAllPaged_V5 @ProductIds=N'40,41,42,49',@CustomerId=1,@ProfileTypeId=2

-- update the "ProductLoadAllPaged" stored procedure
CREATE PROCEDURE [dbo].[ProductLoadAllPaged_V6]
(
	@ProductIds		nvarchar(MAX) = null, --a list of product IDs (comma-separated list). e.g. 1,2,3
	@CustomerId		int = 0, -- customer id
	@ProfileTypeId	int = 0
)
AS
BEGIN	

	DECLARE @ProfileType varchar(100);

	IF @ProfileTypeId=1
		BEGIN
			SET @ProfileType='GiveSupport';
		END;
	ELSE
		BEGIN
			SET @ProfileType='TakeSupport';
		END;

	CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

	INSERT INTO #Products ([ProductId])
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@ProductIds, ',')	

	--SELECT '#PageIndex', * FROM #Products

	CREATE TABLE #ProductSpecs 
	(
		[ProductId] int NOT NULL,
		[PrimaryTechnology] varchar(1000) NULL,
		[SecondaryTechnology] varchar(1000) NULL,
		[CurrentAvalibility] varchar(1000) NULL,
		[ProfileType] varchar(100) NULL,
		[MotherTongue] varchar(100) NULL,
		[WorkExperience] varchar(100) NULL
	)

-- nop 4.6 customization

INSERT INTO #ProductSpecs ([ProductId],[PrimaryTechnology],[SecondaryTechnology],[CurrentAvalibility],[ProfileType],[MotherTongue],[WorkExperience])
		SELECT 
		p.Id as ProductId,
		(SELECT STRING_AGG(sao.Name, ',') AS 'Primary Technology'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=7 and ps.ProductId=p.Id) AS 'PrimaryTechnology',
		(SELECT STRING_AGG(sao.Name, ',') AS 'Secondary Technology'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=8 and ps.ProductId=p.Id) AS 'SecondaryTechnology',
		(SELECT STRING_AGG(sao.Name, ',') AS 'Current Avalibility'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=2 and ps.ProductId=p.Id) AS 'CurrentAvalibility',
		(SELECT STRING_AGG(sao.Name, ',') AS 'ProfileType'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=1 and ps.ProductId=p.Id) AS 'ProfileType',
		(SELECT STRING_AGG(sao.Name, ',') AS 'Mother Tongue'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=4 and ps.ProductId=p.Id) AS 'MotherTongue',
		(SELECT STRING_AGG(sao.Name, ',') AS 'Relavent Experiance'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=3 and ps.ProductId=p.Id) AS 'WorkExperience'
		FROM [Product] p
		order by P.Id asc
	
	-- SELECT '#ProductSpecs', * FROM #ProductSpecs
	-- SELECT * FROM [SpecificationAttribute]
	-- SELECT * FROM [SpecificationAttributeOption]
	-- SELECT * FROM [Product]
	-- SELECT * FROM [Product_SpecificationAttribute_Mapping]

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'1,3,4,5,6,7,8',@CustomerId=1,@ProfileTypeId=2

 
	-- nop 4.6 customization
	SELECT 
		-- C.Id AS CustomerId,
		 C.FirstName,
		 C.LastName,
		 c.Phone,
		 c.Gender,
		 c.Company,
		 c.CountryId,
		 c.StateProvinceId,
		 c.LanguageId,
		 c.TimeZoneId,
		 (SELECT [value] from [GenericAttribute] WHERE KeyGroup='Customer' AND [Key]='AvatarPictureId' AND EntityId=c.Id) 
			As 'AvatarPictureId',
		 c.CustomerProfileTypeId,
		 c.City,
		 C.LastLoginDateUtc,
		 C.LastActivityDateUtc,
		 P.* 
	INTO #CustomerTemp
		FROM #Products [pi]
		INNER JOIN Product p on p.Id = [pi].[ProductId]
		LEFT JOIN Customer C on P.Id = [C].VendorId 
		
	--select * from Customer
	--update customer set VendorId=18 where Id=1
	--select * from Product
	--select * from [GenericAttribute]
	--WHERE KeyGroup='Customer' and [Key]='AvatarPictureId'

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'1,18',@CustomerId=1,@ProfileTypeId=2

   -- SELECT '#CustomerTemp', * FROM #CustomerTemp

	-- create CustomerOrderTemp table
	SELECT  
		Distinct(C.VendorId) AS ProductId,
		CAST(CASE WHEN OI.ProductId=1 AND O.PaidDateUtc >= GETUTCDATE()-90  THEN 1	 -- 1 Month Subscription
				  WHEN OI.ProductId=2 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1	 -- 6 Month Subscription
				  WHEN OI.ProductId=3 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1	 -- 1 Year subscription
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.Id=OI.ProductId
		INNER JOIN [Customer] C ON C.Id=O.CustomerId
		WHERE O.OrderStatusId=30 -- Order Paid

 -- comment date : 11/27/22
 -- nop 4.6 customization :Need to work on it
 -- Action Items : Modify/remove logic of above #CustomerOrderTemp table
 -- Add Customer Id to #CustomerTemp table


  -- SELECT '#CustomerOrderTemp', * FROM #CustomerOrderTemp
  -- exec ProductLoadAllPaged_V5 @ProductIds=N'1,2,3,4,5,6,7,8',@CustomerId=1,@ProfileTypeId=1
  -- exec ProductLoadAllPaged_V5 @ProductIds=N'1,2,18',@CustomerId=1,@ProfileTypeId=1

	SELECT DISTINCT
		p.*,
		ps.*,
		C.Name AS Country,
		sp.Name AS StateProvince,
		la.Name AS Language,
		url.Slug AS Slug,
		CAST(CASE WHEN sci.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS ProfileShortListed, 
		CAST(CASE WHEN sc.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS InterestSent
		--CAST(CASE WHEN co.PremiumCustomer=1 THEN 1 ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #FinalProducts
	FROM
		#CustomerTemp P
	LEFT JOIN #ProductSpecs ps on p.id=ps.ProductId
	LEFT JOIN [country] C on C.Id=p.CountryId
	LEFT JOIN [StateProvince] sp on sp.Id=p.StateProvinceId
	LEFT JOIN [Language] la on la.Id=p.LanguageId
	LEFT JOIN [UrlRecord] url on url.EntityId=p.Id AND url.EntityName='Product'
	LEFT JOIN [ShoppingCartItem] sci on sci.ProductId=p.Id 
									AND sci.ShoppingCartTypeId=2 -- shortlisted
									AND sci.CustomerId =@CustomerId
	LEFT JOIN [ShoppingCartItem] sc on sc.ProductId=p.Id  -- InterestSent
									AND sc.ShoppingCartTypeId=4
									AND sc.CustomerId =@CustomerId
    --LEFT JOIN #CustomerOrderTemp co	on co.ProductId	= ps.ProductId
    LEFT JOIN [Customer_CustomerRole_Mapping] CRM ON P.VendorId=CRM.Customer_Id AND CRM.CustomerRole_Id=9 -- newly added on 11/28/22
	

	-- SELECT '#FinalProducts Table', * FROM #FinalProducts

	-- exec ProductLoadAllPaged_V6 @ProductIds=N'18',@CustomerId=1,@ProfileTypeId=2

	--return all products
	SELECT 
		P.*
	FROM
		#FinalProducts P
	--WHERE REPLACE(P.ProfileType,' ','')= @ProfileType

	DROP TABLE #Products
	DROP TABLE #ProductSpecs
	DROP TABLE #FinalProducts
	DROP TABLE #CustomerTemp
	DROP TABLE #CustomerOrderTemp
	

END


GO


