
use [itjobsupport]

use [onjobsupport]

use nopcommerce46

SELECT * FROM [Customer]
SELECT * FROM [dbo].[CustomerPassword]
SELECT * FROM [dbo].[CustomerRole]
SELECT * FROM [dbo].[Customer_CustomerRole_Mapping] where customer_id=263
SELECT * FROM [dbo].[Customer_CustomerRole_Mapping] where customer_id=1


SELECT * FROM [Discount]


-- exec ProductLoadAllPaged_V5 @ProductIds=N'5,13,14,15,16,17,18',@CustomerId=263,@ProfileTypeId=1

SELECT * FROM [Customer] WHERE Id in (2009,2010)
-- DELETE [Customer] WHERE Id in  (2009,2010)

select * from  [dbo].[Product_SpecificationAttribute_Mapping] Where SpecificationAttributeOptionId in (7)
-- UPDATE [Product_SpecificationAttribute_Mapping] SET AllowFiltering=0 Where SpecificationAttributeOptionId in (1,2)

SELECT * FROM [Category]
SELECT * FROM [product]
SELECT * FROM [UrlRecord] Where EntityName='Product'
SELECT * FROM [UrlRecord] Where EntityName='Category' AND EntityId in (1,2,3)
-- DELETE [UrlRecord] Where EntityName='Product' AND Id in (103)

--DELETE FROM [Category] Where Id in (1,2,3)
--DELETE FROM [product] Where Id in (1,2,3)

SELECT * FROM [CustomerRole]
SELECT * FROM [UrlRecord] Where EntityName='Product'
and slug like '%test%'

SELECT * FROM [dbo].[Customer_CustomerRole_Mapping] where customer_id=263

SELECT  
		Distinct(C.VendorId) AS ProductId, 
        C.Id as CustomerId,
        CRM.CustomerRole_Id AS CustomerRole,
		CAST(CASE 
                  --WHEN OI.ProductId=1 AND O.PaidDateUtc >= GETUTCDATE()-90  THEN 1	 -- 1 Month Subscription
				  WHEN CRM.CustomerRole_Id=9 THEN 1	 -- 6 Month Subscription
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.Id=OI.ProductId
		INNER JOIN [Customer] C ON C.Id=O.CustomerId
		INNER JOIN [Customer_CustomerRole_Mapping] CRM ON C.Id=CRM.Customer_Id   -- paid customer
		--WHERE O.OrderStatusId=30 -- Order Paid
        WHERE CRM.CustomerRole_Id=9 -- paid customer

        select * from  #CustomerOrderTemp
        drop table #CustomerOrderTemp

         SELECT ISNULL((SELECT 1 FROM [Product] WHERE id = 100), 0) res

         SELECT ISNULL((SELECT 1 FROM [Customer_CustomerRole_Mapping] WHERE CustomerRole_Id=9), 0) res

 -- paid customers product ids
    SELECT  
		Distinct(C.VendorId) AS ProductId,
        C.Id as CustomerId,
        CRM.CustomerRole_Id AS CustomerRole,
        CRM.Customer_Id
      --  CAST(CASE WHEN CRM.CustomerRole_Id=9 THEN 1	 -- 6 Month Subscription
				  --ELSE 0 END AS BIT) AS PremiumCustomer
    --INTO #CustomerOrderTemp
		FROM [Customer] C 
		INNER JOIN [Product] P ON P.Id=C.VendorId
		LEFT JOIN [Customer_CustomerRole_Mapping] CRM ON C.Id=CRM.Customer_Id
        WHERE CRM.CustomerRole_Id=9 -- paid customer



        	SELECT  
		Distinct(C.VendorId) AS ProductId,
        C.Id as CustomerId,
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

		 SELECT '#CustomerOrderTemp', * FROM #CustomerOrderTemp
		 -- DROP TABLE #CustomerOrderTemp

        
UPDATE Customer SET CustomerProfileTypeId=0
Where Id=1

UPDATE Customer SET CustomerProfileTypeId=1
Where Id=1

SELECT * From Product WHERE  Vendorid <> null
-- DELETE From Product WHERE Id in (46,47)


--INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[IsActive],[LanguageId]) VALUES('Product','sateesh-um',263,1,0)
Update [UrlRecord] SET [slug]='sateesh-um' where Id=104

SELECT * From Customer Where VendorId=10
DELETE From Customer Where Id=1495


SELECT * From Forums_PrivateMessage
-- DELETE Forums_PrivateMessage
SELECT * From [dbo].[QueuedEmail]
SELECT * From [dbo].[MessageTemplate] Where Name like '%NewPM%'
SELECT * From [dbo].[MessageTemplate] Where Name like '%OrderPlaced.StoreOwnerNotification%'
select * from [dbo].[GenericAttribute] WHERE [KEY]='NotifiedAboutNewPrivateMessages'
And EntityId=263

SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=263 and ProductId=14


-- DELETE From Forums_PrivateMessage Where ToCustomerId=1495
-- DELETE From [ShoppingCartItem] Where ToCustomerId=1495

SELECT * FROM [product] WHERE Id=5
Update [product] SET VendorId=263 where Id=5

SELECT * FROM [Vendor] WHERE Id=5

-- sp_who2 ACTIVE

-- exec ProductLoadAllPaged_V5 @ProductIds=N'4,5',@CustomerId=263,@ProfileTypeId=2

UPDATE [dbo].[Customer_CustomerRole_Mapping] set CustomerRole_Id=1 where customer_id=263
SELECT * FROM [dbo].[CustomerPassword] Where CustomerId=1
Update [dbo].[CustomerPassword] SET [Password]='123' Where CustomerId=1

SELECT * FROM [Customer] where Deleted=1
Update [Customer] SET VendorId=5 where Id=263
SELECT * FROM [Product] Where Id=4
SELECT * FROM  [dbo].[Product_SpecificationAttribute_Mapping] WHERE ProductId=4
select * from [dbo].[GenericAttribute] where entityid=1868 and [Key]='Gender'

select * from [dbo].[GenericAttribute] 

select * from [dbo].[GenericAttribute] where [KeyGroup]='Customer' and entityid=1926

SELECT * FROM [Customer] C
INNER JOIN [GenericAttribute] GA on GA.EntityId=C.Id
WHERE GA.[Key]='CustomCustomerAttributes'

-- UPDATE [Customer] SET VendorId=14 Where id=263

SELECT * FROM [CustomerAttribute] order by DisplayOrder
SELECT * FROM [CustomerAttributeValue]
select * from [SpecificationAttribute] order by DisplayOrder

SELECT * FROM [CustomerAttribute] 
select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption] WHERE SpecificationAttributeId=9

DELETE [SpecificationAttributeOption] WHERE SpecificationAttributeId=6


SELECT * FROM [ProductAttribute] 
SELECT * FROM [ProductAttributeValue] 
SELECT * FROM [PredefinedProductAttributeValue] 


DELETE [CustomerAttribute]  WHERE Id=14

SELECT * FROM [UrlRecord] Where EntityName='Category'

SELECT * FROM [Customer] where Email IS NULL
SELECT VendorId FROM [Customer]

SELECT * FROM [product]

SELECT * FROM [Order] 
WHERE CustomerId=1
SELECT * FROM [dbo].[OrderItem]
SELECT * FROM [dbo].[OrderNote]
--DELETE [Order]

SELECT * FROM  [dbo].[Product_SpecificationAttribute_Mapping] WHERE ProductId=4

-- DELETE [Customer] where id=277

SELECT VendorId,* FROM [product] where id=1402
update [product] SET VendorId=263 WHERE Id in (10)
SELECT VendorId FROM [product] where id=1402
SELECT * FROM [product] where VendorId=263

select * from [SpecificationAttribute] order by DisplayOrder
select * from [SpecificationAttributeOption]
select * from [SpecificationAttributeOption] where SpecificationAttributeId=3
select * from [SpecificationAttributeOption] where id=2

select * from  [dbo].[Product_SpecificationAttribute_Mapping] Where SpecificationAttributeOptionId in (1,2)
UPDATE [Product_SpecificationAttribute_Mapping] SET AllowFiltering=0 Where SpecificationAttributeOptionId in (1,2)

SELECT * FROM [Category] where id=29
SELECT PageSize FROM [Category] where id=29
SELECT * FROM [Product]
SELECT * FROM [Product_Category_Mapping] where CategoryId=2
SELECT * FROM [Product_Category_Mapping] where CategoryId=49

SELECT * FROM [Customer]
select * from  [dbo].[Product_SpecificationAttribute_Mapping] where ProductId=34

SELECT * FROM [UrlRecord] WHERE EntityName='product'

SELECT * FROM [Picture]

-- DELETE FROM [Category] where id=29
SELECT * FROM [CategoryTemplate]

update [Category] SET Name='Give Support' WHERE Id=1
update [Category] SET Name='Take Support' WHERE Id=2 
update [Category] SET deleted=1 WHERE Id in (16,17)

SELECT * FROM [Category]
SELECT * FROM [CategoryTemplate]
SELECT * FROM [dbo].[ProductTemplate]

update [product] SET showonhomepage=1 WHERE Id in (8)

SELECT * FROM [dbo].[Product_Picture_Mapping]
WHERE ProductId=49

INSERT INTO [dbo].[Product_Picture_Mapping]([PictureId],[ProductId],[DisplayOrder])
  VALUES (78,49,1)

SELECT * FROM [product] WHERE Id=14
SELECT * FROM [product] WHERE VendorId=273

SELECT * FROM [dbo].[Product_SpecificationAttribute_Mapping]
WHERE ProductId=8

SELECT  * FROM [dbo].[Customer] 
SELECT * FROM [Picture]

-- umsateeshTest@gmail.com

SELECT * FROM [Product_Picture_Mapping] Where productid=49

select * from [dbo].[GenericAttribute] where entityid=1376

select * from [dbo].[GenericAttribute]  where [Key] like '%city%'

select distinct(keygroup) from [dbo].[GenericAttribute] 

--delete [SpecificationAttributeOption] where id=6

-- disable paging 

select * from [dbo].[Setting] WHERE Name like '%gdpr%'

SELECT * FROM .[dbo].[Setting]
where name like '%sendin%'

SELECT * FROM [dbo].[MessageTemplate]

SELECT * FROM .[dbo].[LocaleStringResource]
where resourcename like '%sendin%';

SELECT * FROM .[dbo].[LocaleStringResource]
where resourcename like 'options%';


SELECT * FROM .[dbo].[LocaleStringResource]
where resourcename like '%wishlist.cartisempty%';

SELECT * FROM .[dbo].[LocaleStringResource]
where ResourceValue like '%options%';

UPDATE [dbo].[Setting]
SET Value='False' 
where Name = 'catalogsettings.allowproductviewmodechanging';
-- org: Recently viewed products


---------------------------------------------------------
-- ***  Local String update queries ****
---------------------------------------------------------
UPDATE [dbo].[LocaleStringResource]
SET ResourceValue='Recently viewed profiles' 
where resourcename = 'products.recentlyviewedproducts';
-- org: Recently viewed products
---------------------------------------------------------

SELECT * FROM [itjobsupport].[dbo].[Forums_PrivateMessage]
Delete [itjobsupport].[dbo].[Forums_PrivateMessage] WHERE SenderSubject is null

SELECT * FROM [Topic]
SELECT * FROM [TopicTemplate]

SELECT  * FROM [dbo].[Customer]  WHERE Id >=1619
SELECT  * FROM [dbo].[Customer] WHERE VendorId not in (SELECT  Id FROM [dbo].[Product] )
SELECT  * FROM [dbo].[Product] Where id >=25

--DELETE [dbo].[Customer]  WHERE Id >=1619
--DELETE [dbo].[Product] Where id >=25
--DELETE [dbo].[Forums_PrivateMessage]
--DELETE [dbo].[Order]

UPDATE [dbo].[Product] SET OrderMaximumQuantity=100;

SELECT  [Name],[Sku],[ProductTypeId],[ParentGroupedProductId],[VisibleIndividually]
		   ,[ShortDescription],[FullDescription],[ProductTemplateId],[IsTaxExempt]
           ,[StockQuantity],[OrderMinimumQuantity],[OrderMaximumQuantity],[DisableBuyButton]
		   ,[Price],[OldPrice],[DisplayOrder],[Published],[Deleted],[CreatedOnUtc],[UpdatedOnUtc]
FROM [dbo].[Product] Where Id in (16,17,18)



SELECT  * FROM [dbo].[Customer]
Update [dbo].[Product] SET VendorId=1402 where  Id=10

SELECT  VendorId FROM [dbo].[Customer]

SELECT  VendorId FROM [dbo].[Customer]
SELECT  VendorId FROM [dbo].[Product]
SELECT  * FROM [dbo].[Customer]
SELECT  * FROM [dbo].[Customer] WHERE Id=263
-- 14 Product Id

SELECT *FROM .[dbo].[ShoppingCartItem]
-- DELETE [dbo].[ShoppingCartItem]


SELECT  * FROM [dbo].[CustomerAddresses]
SELECT  * FROM [dbo].[Address]

select * from tmp_guestsToDelete


SELECT *FROM [dbo].[ShoppingCartItem] WHERE ShoppingCartTypeId=1 -- ShoppingCart
SELECT *FROM [dbo].[ShoppingCartItem] WHERE ShoppingCartTypeId=2 -- shortlisted
SELECT * FROM [dbo].[ShoppingCartItem] WHERE ShoppingCartTypeId=4 -- InterestSent
SELECT * FROM [dbo].[ShoppingCartItem] WHERE ShoppingCartTypeId=4 -- InterestSent
-- delete [dbo].[ShoppingCartItem] WHERE ShoppingCartTypeId=4 -- InterestSent


SELECT  * FROM [dbo].[Customer] Where Id=1542
SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=1542 and ProductId=14
SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=263 and ShoppingCartTypeId=2
SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=263 and ShoppingCartTypeId=4

SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=263 and ProductId=14
UPDATE [dbo].[ShoppingCartItem] SET ShoppingCartTypeId=3 where CustomerId=263 and ProductId=14

--UPDATE [SpecificationAttributeOption] SET Name='Male' WHERE Id=29
--UPDATE [SpecificationAttributeOption] SET Name='Fe Male' WHERE Id=30

select * from store
select * from Warehouse

select * from  Product

select * from  UrlRecord
where entityid=57


SELECT 
		 G.[Key]
		,G.[Value]
		,P.* 
		,C.LastLoginDateUtc
		,C.LastActivityDateUtc
	--INTO #CustomerTemp
		FROM Product [p]
		--with (NOLOCK) on p.Id = [pi].[ProductId]
		LEFT JOIN Customer C with (NOLOCK) on P.Id = [C].[VendorId]
		LEFT JOIN  [GenericAttribute] G on G.EntityId= C.Id
		WHERE G.[KEY] in ('FirstName','LastName','Phone','Gender','Company',
						  'CountryId','StateProvinceId','LanguageId','TimeZoneId',
						  'AvatarPictureId','CustomerProfileTypeId')


SELECT  G.[Key],
		G.[Value],
		C.*
		FROM Customer [C]
		LEFT JOIN  [GenericAttribute] G on G.EntityId= C.Id AND C.Id=263
		WHERE G.[KEY] in ('FirstName','LastName','Phone','Gender','Company',
						  'CountryId','StateProvinceId','LanguageId','TimeZoneId',
						  'AvatarPictureId','MotherTounge')
		AND C.Id=263


	SELECT 
		p.id as ProductId,
		sa.Name,
		--sao.Id as SpecificationAttributeOptionId,
		Technology = STRING_AGG(sao.Name,' , ')
	FROM [Product_SpecificationAttribute_Mapping] PS
		JOIN [Product] p on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
		--WHERE sao.Id <> 2
		GROUP BY p.id,sa.name
		--,sao.Id
		Order By p.id 


SELECT * FROM [Order]
SELECT * FROM [dbo].[OrderItem]
SELECT * FROM [dbo].[OrderNote]

SELECT Distinct(P.Id) as ProductId,
		--OI.ProductId AS ProductSubscriptionId,
		--O.Id AS OrderId,
		--O.CustomerId,
		--O.OrderStatusId,
		--O.PaidDateUtc,
		CAST(CASE WHEN OI.ProductId=16 AND O.PaidDateUtc >= GETUTCDATE()-90 THEN 1
				  WHEN OI.ProductId=17 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1
				  WHEN OI.ProductId=18 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1
				  ELSE 0 END AS BIT) AS PremiumCustomer
FROM [Order] O 
INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
INNER JOIN [Product] P ON P.VendorId=O.CustomerId
WHERE  O.OrderStatusId=30 -- Order Paid status
	 

SELECT  CAST(CASE WHEN O.PaidDateUtc >= GETUTCDATE() THEN 1 ELSE 0 END AS BIT) AS PremiumCustomer
FROM [Order] O

SELECT O.PaidDateUtc,
	   GETUTCDATE()-1 AS [EndDate],
	   CAST(CASE WHEN O.PaidDateUtc >= GETUTCDATE()-1 THEN 1 ELSE 0 END AS BIT) AS PremiumCustomer
FROM [Order] O

SELECT * FROM [dbo].[QueuedEmail]
SELECT * FROM [dbo].[ScheduleTask]
SELECT * FROM [dbo].[Customer] WHERE Id=263
SELECT * FROM [dbo].[Customer] WHERE VendorId=10 -- 1337
SELECT * FROM [dbo].[Product] WHERE Id=10 -- 1337
SELECT * FROM [dbo].[Product] WHERE VendorId=263 -- 1337



	SELECT  
		--Distinct(P.Id) as PricingProductId,
		--P.VendorId, 
		--O.CustomerId,
		Distinct(C.VendorId) AS ProductId,
		CAST(CASE WHEN OI.ProductId=16 AND O.PaidDateUtc >= GETUTCDATE()-90 THEN 1
				  WHEN OI.ProductId=17 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1
				  WHEN OI.ProductId=18 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.Id=OI.ProductId
		INNER JOIN [Customer] C ON C.Id=O.CustomerId
		WHERE O.OrderStatusId=30 -- Order Paid

SELECT * FROM #CustomerOrderTemp
DROP TABLE #CustomerOrderTemp


SELECT o.Id,o.CustomerId,OI.OrderId,OI.ProductId,o.PaymentStatusId FROM [Order] O
INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id
WHERE O.OrderStatusId=30 -- Order Paid

SELECT * FROM [dbo].[Product] WHERE VendorId=0
SELECT * FROM [dbo].[Customer] WHERE VendorId=14
SELECT * FROM [dbo].[Customer] WHERE Id in (1337)

SELECT * FROM [dbo].[Customer] WHERE id in (1337)
SELECT * FROM [dbo].[Order] where CustomerId in (263)
SELECT * FROM [dbo].[OrderItem] where OrderId in (12)
SELECT * FROM [dbo].[Product] WHERE Id in (14)
SELECT * FROM [dbo].[Product] WHERE VendorId in (1337)
SELECT * FROM [dbo].[ShoppingCartItem] WHERE Id in (118)

UPDATE [dbo].[Product] SET Name='Sateesh UM',Sku='SKU_Sateesh_UM' WHERE VendorId in (263)
UPDATE [dbo].[Customer] SET VendorId=30 where Id in (1337)

	SELECT C.VendorId as ProductId
	FROM [ShoppingCartItem] sci
	INNER JOIN [Customer] C on C.Id=sci.CustomerId
	WHERE 
		sci.ProductId=10
		AND sci.ShoppingCartTypeId=2

    SELECT * FROM [Discount]

SELECT * FROM [dbo].[Country] where [Name] like '%United states%'
SELECT * FROM [dbo].[Country] where Published=1
SELECT * FROM [dbo].[Setting] where [Name] like '%Featured%';
SELECT * FROM [dbo].[Setting] where [Name] like '%You can edit%';
SELECT * FROM [dbo].[Setting] where [Value] like '%Featured%';

SELECT * FROM [dbo].[Customer]
SELECT * FROM [dbo].[CustomerRole]

SELECT * FROM [dbo].[Customer_CustomerRole_Mapping]
WHERE Customer_Id=1 AND CustomerRole_Id=9

--DELETE  [dbo].[Customer_CustomerRole_Mapping]
--WHERE Customer_Id=1 AND CustomerRole_Id=9

-- DELETE [Order] WHERE CustomerId=1
SELECT * FROM [Order] WHERE CustomerId=1

SELECT * FROM [Order]
SELECT * FROM [dbo].[OrderItem]cc

SELECT * FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=133
SELECT * FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=163 
SELECT * FROM [dbo].[ActivityLogType] WHERE Id=163
SELECT * FROM [dbo].[ActivityLogType] WHERE SystemKeyword like '%PublicStore.EditCustomerAvailabilityToTrue%'

SELECT distinct(EntityName) FROM [dbo].[ActivityLog]
SELECT * FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=133 AND CustomerId=263 and EntityName='Product'
SELECT * FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=133 and EntityName='Customer'

 -- PublicStore.ViewContactDetail

 SELECT * FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=154 AND CustomerId=263 and EntityName='Product'

  SELECT distinct(entityid) FROM [dbo].[ActivityLog] WHERE ActivityLogTypeId=133 AND CustomerId=263 and EntityName='Product'

  SELECT * FROM [dbo].[EmailAccount]



select [Name],[PageSizeOptions],[CategoryTemplateId],[PageSize],[ShowOnHomepage]
		   ,[IncludeInTopMenu],[SubjectToAcl],[LimitedToStores],[Published]
		   ,[Deleted],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc],[PriceRangeFiltering] from Category


--input currentproductid,currentcustomerid,currentproductid

--currentproductid Get -- specificationattributeoptionids
--Search products with same specificationattributeoptionids

select * from  [dbo].[Product] Where VendorId=263
select * from  [dbo].[Product] Where VendorId=263

select * from  [dbo].[Product_SpecificationAttribute_Mapping] WHERE ProductId=10

select SpecificationAttributeOptionId from  [dbo].[Product_SpecificationAttribute_Mapping] 
WHERE ProductId=10 --AND AllowFiltering=1

select *  from  [dbo].[Product_SpecificationAttribute_Mapping] 
WHERE ProductId=10 AND AllowFiltering=1

select *  from [SpecificationAttribute]
select *  from [SpecificationAttributeOption]
WHERE SpecificationAttributeId=7


SELECT *
	FROM [Product] P
		JOIN [Product_SpecificationAttribute_Mapping] ps on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
WHERE 
				sao.Id in (select SpecificationAttributeOptionId from  [dbo].[Product_SpecificationAttribute_Mapping] 
				WHERE ProductId=10)
		order by p.id	
		 --AND sa.Id in (5)
		 

--The INSERT statement conflicted with the FOREIGN KEY constraint "FK_Product_SpecificationAttribute_Mapping_ProductId_Product_Id". 
--The conflict occurred in database "nopcommerce46", table "dbo.Product", column 'Id'. The statement has been terminated.

INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES (' AI', 7, 0);


select * from  [dbo].[Product] 

select *  from  [dbo].[Product_SpecificationAttribute_Mapping] 
WHERE ProductId=10 AND AllowFiltering=1

SELECT Vendorid,* FROM [dbo].[Customer] --18 --56 
select * from  [dbo].[Product] Where VendorId=1 

UPDATE [dbo].[Product] SET VendorId=1 Where Id=56 
UPDATE [dbo].[Customer] SET VendorId=56 where Id in (1)


