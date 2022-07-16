
-- Data Cleaning Scripts 
-- nop 4.4.0 version

use [onjobsupport]
use [itjobsupport]

----------------------------------------------------
SELECT * FROM [Forums_PrivateMessage] 

-- DELETE [Forums_PrivateMessage]
DBCC CHECKIDENT ('Forums_PrivateMessage', NORESEED);
DBCC CHECKIDENT ('Forums_PrivateMessage', RESEED, 0);
----------------------------------------------------
SELECT * FROM [Customer] WHERE Id >263

-- DELETE [Customer] WHERE Id >263
DBCC CHECKIDENT ('Customer', NORESEED);
DBCC CHECKIDENT ('Customer', RESEED, 263);
----------------------------------------------------
SELECT * FROM [CustomerAttribute] Order By DisplayOrder

DELETE [CustomerAttribute]
DBCC CHECKIDENT ('CustomerAttribute', NORESEED);
DBCC CHECKIDENT ('CustomerAttribute', RESEED, 0);
----------------------------------------------------
SELECT * FROM [SpecificationAttribute]

DELETE [SpecificationAttribute]
DBCC CHECKIDENT ('SpecificationAttribute', NORESEED);
DBCC CHECKIDENT ('SpecificationAttribute', RESEED, 0);
----------------------------------------------------
SELECT * FROM [Category]

DELETE [Category]
DBCC CHECKIDENT ('Category', NORESEED);
DBCC CHECKIDENT ('Category', RESEED, 0);
----------------------------------------------------
SELECT * FROM [UrlRecord] Where EntityName='Category' And EntityId NOT IN (1,2,3)

-- DELETE [UrlRecord] Where EntityName='Category' And EntityId NOT IN (1,2,3)
DBCC CHECKIDENT ('UrlRecord', NORESEED);
--DBCC CHECKIDENT ('UrlRecord', RESEED, 87);
----------------------------------------------------
SELECT * FROM [Product]

DELETE [Product]
DBCC CHECKIDENT ('Product', NORESEED);
DBCC CHECKIDENT ('Product', RESEED, 0);
----------------------------------------------------
SELECT * FROM [UrlRecord] Where EntityName='Product' -- And EntityId NOT IN (1,2,3)

-- DELETE [UrlRecord] Where EntityName='Product' --And EntityId NOT IN (1,2,3)
DBCC CHECKIDENT ('UrlRecord', NORESEED);
--DBCC CHECKIDENT ('UrlRecord', RESEED, 87);
----------------------------------------------------
SELECT * FROM [Product_Category_Mapping]

DELETE [Product_Category_Mapping]
DBCC CHECKIDENT ('Product_Category_Mapping', NORESEED);
DBCC CHECKIDENT ('Product_Category_Mapping', RESEED, 0);
----------------------------------------------------
SELECT * FROM  [ProductTemplate]

DBCC CHECKIDENT ('Product', NORESEED);
DBCC CHECKIDENT ('Product', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('Forums_Post', NORESEED);
DBCC CHECKIDENT ('Forums_Post', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('Forums_Subscription', NORESEED);
DBCC CHECKIDENT ('Forums_Subscription', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('GenericAttribute', NORESEED);
DBCC CHECKIDENT ('GenericAttribute', RESEED, 2352);
----------------------------------------------------

DBCC CHECKIDENT ('QueuedEmail', NORESEED);
DBCC CHECKIDENT ('QueuedEmail', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('Product_Category_Mapping', NORESEED);
DBCC CHECKIDENT ('Product_Category_Mapping', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('Product_Category_Mapping', NORESEED);
DBCC CHECKIDENT ('Product_Category_Mapping', RESEED, 0);
----------------------------------------------------

DBCC CHECKIDENT ('Product_Category_Mapping', NORESEED);
DBCC CHECKIDENT ('Product_Category_Mapping', RESEED, 0);
----------------------------------------------------

-- Delete private messages from generic attribute table
-- This is used for UI notifications

select * from [dbo].[GenericAttribute]
WHERE KeyGroup='Customer' 
AND [KEY]='NotifiedAboutNewPrivateMessages'
And EntityId NOT IN (SELECT ID FROM Customer)


DELETE FROM [dbo].[GenericAttribute]
WHERE KeyGroup='Customer' 
AND [KEY]='NotifiedAboutNewPrivateMessages'
And EntityId NOT IN (SELECT ID FROM Customer)

-----------------------------------
-- DELETE Forums_PrivateMessage
-- DELETE  [Order]
-- DELETE Customer Where Id > 1402
SELECT * FROM Customer
-----------------------------------
DELETE [UrlRecord] WHERE EntityName='Category'
SELECT * FROM [UrlRecord] WHERE EntityName='Category'

SELECT * FROM [UrlRecord] WHERE EntityName='Product'

--DELETE [Category]
SELECT * FROM [Category]
SELECT * FROM [Category]
-----------------------------------
SELECT * FROM  Product
DELETE Product

-----------------------------------
SELECT * FROM  Topic
SELECT * FROM [SpecificationAttribute]
SELECT * FROM [SpecificationAttributeOption] WHERE SpecificationAttributeId=6
SELECT * FROM [Product_SpecificationAttribute_Mapping]
WHERE SpecificationAttributeOptionId in 
(
11,
12,
13,
14,
15,
16
)


SELECT * FROM [Forums_Subscription]
--DELETE [Forums_Subscription]

SELECT * FROM [Customer]

SELECT * FROM [dbo].[GenericAttribute] WHERE KeyGroup='Customer' And EntityId=1

SELECT * FROM [dbo].[GenericAttribute] WHERE KeyGroup='Customer'
And EntityId not in (select Id from Customer)

DELETE [dbo].[GenericAttribute] WHERE KeyGroup='Customer'
And EntityId not in (select Id from Customer)


SELECT * FROM [QueuedEmail]
DELETE [QueuedEmail]

SELECT * FROM [Forums_Subscription]
--DELETE [Forums_Subscription]

SELECT * FROM [Forums_Subscription]
--DELETE [Forums_Subscription]

SELECT * FROM [Forums_Subscription]
--DELETE [Forums_Subscription]


SELECT * FROM [Product_Category_Mapping]

select * from Product where VendorId=276

select * from Product where VendorId=Null
DELETE Product where VendorId=Null

select * from Customer where VendorId is null
--DELETE Customer where VendorId IS NULL

SELECT * FROM [Category] where Deleted=1
--DELETE [Category] where Deleted=1

--disable pagesize on categories
UPDATE [Category] SET AllowCustomersToSelectPageSize=0

SELECT * FROM [Category]
SELECT * FROM [UrlRecord]
SELECT * FROM [Category]
SELECT * FROM [Category]
SELECT * FROM [Category] where Deleted=0

SELECT * FROM [MessageTemplate] 

SELECT * FROM [UrlRecord] WHERE EntityName='Category'

SELECT * FROM [product]

