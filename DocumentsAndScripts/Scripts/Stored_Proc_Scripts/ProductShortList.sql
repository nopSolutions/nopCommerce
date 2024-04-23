USE [onjobsupport47]
GO

/****** Object:  StoredProcedure [dbo].[ProductShortList]    Script Modified Date: 23-04-2024 10:34:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- I short Listed profiles
-- exec [ProductShortList] 5, 2, 6 ,0,0,100
-- exec [ProductShortList] NULL, 2, 6 ,0,0,100
-- exec [ProductShortList] NULL, 3, 6 ,0,0,100
-- product id : 6 for customer id:3627
-- The below should give who short listed me (product id)
-- exec [ProductShortList] 6,2,3627,0,0,100

ALTER PROCEDURE [dbo].[ProductShortList]
(
	@ProductIds			nvarchar(MAX) = null,--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ShoppingCartTypeId	int = 0, -- shortlist = 2, ShortListedMe =3, InterestSent = 4, InterestReceived = 5,
	@CustomerId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN	

	SET NOCOUNT ON
	
	CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

	-- nop 4.7 customization
	-- shopping cart items 
	IF @ShoppingCartTypeId=2 -- shortlisted by me. I.e I have short listed
		BEGIN
			-- get the product ids that I have shortlisted and insert into #Products table
			INSERT INTO #Products ([ProductId])
				SELECT Distinct(ProductId) 
				FROM ShoppingCartItem AS a
				WHERE CustomerId=@CustomerId AND ShoppingCartTypeId=2
				-- get only published products and not unpublished products i.e in-active customers
				AND Exists (
                    Select * FROM Product AS p 
                    WHERE p.Id=a.ProductId 
                    AND p.Published=1
                )
		END

	IF @ShoppingCartTypeId=3 -- Who shortlisted me
		BEGIN
			-- get customer ids that have shortlisted me (shortlsted my product id)
			-- get my product id from customer id
			DECLARE @myProductId Int;
			Select @myProductId=VendorId FROM Customer WHERE Id=@CustomerId
			--print(@myProductId)

			--SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=@myProductId

			INSERT INTO #Products ([ProductId])
				SELECT Id 
				FROM Product 
				WHERE VendorId in (SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=@myProductId)
				-- get only published products and not unpublished products i.e in-active customers
				AND Published=1
		END 


	-- SELECT '#Products',* FROM #Products;

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

	-- SELECT * FROM #ProductSpecs

	-- nop 4.6 customization
	SELECT 
		 C.Id AS CustomerId,
		 C.FirstName,
		 C.LastName,
		 c.Phone,
		 c.Gender,
		 c.Company,
		 c.CountryId,
		 c.StateProvinceId,
		 c.LanguageId,
		 c.TimeZoneId,
		 (SELECT [value] from [GenericAttribute] WHERE KeyGroup='Customer' AND [Key]='AvatarPictureId' AND EntityId=c.Id) As 'AvatarPictureId',
		 c.CustomerProfileTypeId,
		 c.City,
		 C.LastLoginDateUtc,
		 C.LastActivityDateUtc,
		 P.* 
	INTO #CustomerTemp
		FROM #Products [pi]
		INNER JOIN Product p on p.Id = [pi].[ProductId]
		LEFT JOIN Customer C on P.Id = [C].VendorId 

	 -- SELECT '#CustomerTemp',* FROM #CustomerTemp
	 -- exec [ProductShortList_v1] NULL,2,263,0,0,1000

	SELECT 
		p.Id as Id,
		p.FirstName,
		p.LastName,
		p.Phone,
		p.Gender,
		p.Company,
		p.CountryId,
		p.StateProvinceId,
		p.City,
		p.LanguageId,
		p.TimeZoneId,
		p.AvatarPictureId,
		p.CustomerProfileTypeId,
		ps.PrimaryTechnology,
		ps.SecondaryTechnology,
		ps.CurrentAvalibility,
		ps.WorkExperience,
		--p.*
		--ps.*,
		C.Name AS Country,
		sp.Name AS StateProvince,
		la.Name AS Language,
		url.Slug AS Slug,
		P.LastLoginDateUtc,
		P.LastActivityDateUtc,
		CAST(CASE WHEN sci.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS ProfileShortListed, 
		CAST(CASE WHEN sc.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS InterestSent,
		CAST(CASE 
				WHEN CRM.CustomerRole_Id IS NULL THEN 0 
				WHEN CRM.CustomerRole_Id=9 THEN 1 
				ELSE 0 END AS BIT) AS PremiumCustomer,
		(Select VendorId FROM Product WHERE Id=p.Id) AS VendorId
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
    LEFT JOIN [Customer_CustomerRole_Mapping] CRM ON P.VendorId=CRM.Customer_Id AND CRM.CustomerRole_Id=9 -- newly added on 11/28/22

    --WHERE sc.CustomerId=@CustomerId


	-- I short Listed profiles
	-- exec [ProductShortList_v2] null,2,61,0,0,100

	-- SELECT * FROM #FinalProductsTemp

	--return products
	SELECT 
		P.*
	FROM
		#FinalProducts P

	DROP TABLE #Products
	DROP TABLE #ProductSpecs
	DROP TABLE #FinalProducts
	DROP TABLE #CustomerTemp
END
