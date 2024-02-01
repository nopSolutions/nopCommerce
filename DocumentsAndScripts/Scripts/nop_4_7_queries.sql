
use [nop471]

-- use [nopcommerce46]
-- use [onjobsupport47]
-- use [onjobsupport]

select * from Customer
select * from [dbo].[CustomerAttribute]
select * from [dbo].[CustomerAttributeValue]
select * from [SpecificationAttributeOption]

select * from [dbo].[GenericAttribute] WHERE [KEY]='NotifiedAboutNewPrivateMessages'
select * from [dbo].[GenericAttribute] where [KeyGroup]='Customer' and [Key]='SubscriptionId'

select * from [dbo].[TopicTemplate]
select * from [dbo].[Affiliate]
select * from [dbo].[MessageTemplate]

select * from [dbo].[GenericAttribute] 
where [KeyGroup]='Customer' and [Key] like '%allotted%'
--update [CustomerAttribute]
--SET HelpText='Provide Your mother tongue so that others can contact you <br/> if they chose same mother tongue seeker'
--WHERE Id=4



update [CustomerAttribute]
SET ShowOnRegisterPage=1
WHERE Id=4

update [CustomerAttribute]
SET IsRequired=0
WHERE Id=9


select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption]
select * from [SpecificationAttributeOption] where SpecificationAttributeId=5
select * from [SpecificationAttributeOption] where id=2
select * from [ProductAttribute] where id=2

select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption]
select * from [SpecificationAttributeOption] where SpecificationAttributeId=3
select * from [SpecificationAttributeOption] where id=2

select * from [dbo].[CustomerAddresses]
select * from [dbo].[CustomerRole]

select * from [dbo].[ProductReview]
where ProductId=5



select * from [dbo].[Customer_CustomerRole_Mapping]

use [nopcommerce46]

SELECT * FROM [Customer] C
INNER JOIN [GenericAttribute] GA on GA.EntityId=C.Id
WHERE GA.[Key]='CustomCustomerAttributes'
and c.Id=1

select * from [dbo].[GenericAttribute]
where KeyGroup='Customer' and EntityId=3421
order by [Key]

select * from [dbo].[GenericAttribute]
where KeyGroup='Customer' and EntityId=14
order by [Key]

select * from Customer where id=3421
select * from Product where id=3421


update [Customer] set vendorid=14
Where id=1

UPDATE [dbo].[Setting]
SET Value='Clear' 
where Name = 'customersettings.defaultpasswordformat';
-- Original : Hashed

--UPDATE [dbo].[CustomerPassword]
--SET PasswordFormatId=0
---- Original : 1

--UPDATE [dbo].[CustomerPassword]
--SET [Password]='123'
--WHERE id in (2,3)

select * from CustomerPassword

select * from Topic

select * from [dbo].[Customer_CustomerRole_Mapping]
wHERE Customer_Id=3421

--Insert into [dbo].[Customer_CustomerRole_Mapping]
--values (3421,1)


SELECT
	[t1].[Id],
	[t1].[CustomerAttributeId],
	[t1].[DisplayOrder],
	[t1].[IsPreSelected],
	[t1].[Name]
FROM
	[CustomerAttributeValue] [t1]


select * FROM [dbo].[Setting] 
where Name like 'mediasettings.%';

select * FROM [dbo].[Setting] 
where Name like 'catalogsettings.show%';

select * FROM [dbo].[Setting] 
where value like '%Be the first%';

select * FROM [dbo].[Setting] 
where value like '%Be the first%';

select * from LocaleStringResource
Where ResourceName like 'Admin.Catalog.Products.Fields.%desc%'

select * from LocaleStringResource
Where ResourceName='Account.Login.NewCustomerText'

update LocaleStringResource
SET ResourceValue='<p><strong>Welcome to on job support!</strong><br />Register with us for future convenience:</p><p style="text-align: left;">1.Resgistration is mandatory as we need to show relavent profiles to provide support and take support</p><p style="text-align: left;">2. You can directly contact with people who can provide support thus eliminating middle man</p><p style="text-align: left;">3. Please visit <a title="This Link" href="https://onjobsupport.in" target="_blank" rel="noopener">This Link</a> for further information</p>'
Where ResourceName='Account.Login.NewCustomerText'



select * from LocaleStringResource
Where ResourceName like '%locale%'

select * from LocaleStringResource
Where ResourceName='media.category.imagelinktitleformat'

select * FROM [dbo].[Setting] 
where Name like '%tag%'
and Name like 'catalogsettings.show%';


select * FROM Customer
select * FROM CustomerRole

select * FROM [dbo].[Customer_CustomerRole_Mapping]
-- 6

-- Insert Into Customer_CustomerRole_Mapping values(6,2)

-- update CustomerPassword set PasswordFormatId=0, Password='India@2024'

select * from Product
select * from UrlRecord
select * from [Product_Category_Mapping]

select * FROM Customer where VendorId=5

-- hi-IN (India)

select * from UrlRecord
WHERE EntityName='Topic'

select * from [Forums_Topic]
select * from [dbo].[Forums_Forum]


select * from [Order]
select * from OrderItem

select * from [Log]

select * from [picture]
select * from [pictureBinary]

select * from [dbo].Forums_PrivateMessage

 --update Category set ShowOnHomepage=1
 --Where Id in (4,5)


 select * from shoppingcartitem
 -- delete shoppingcartitem

  select * from ScheduleTask

 select * from shoppingcartitem
 where ProductId=4

 select * from Product where VendorId=14

 select * from ActivityLog
 -- delete ActivityLog

 	SELECT p.Id
	FROM
		Product p with (NOLOCK)
		INNER JOIN ShoppingCartItem sci with (NOLOCK)
				ON p.Id = sci.ProductId
		WHERE CustomerId=6
		AND ShoppingCartTypeId=2
		AND p.Deleted = 0
		AND p.Id IN (4)


        select Vendorid FROM Customer

 SELECT * FROM  [GenericAttribute]
 Where EntityId=14

 -- customer 6

 select * from Customer where Id=6 
 select VendorId as productid from Customer where Id=6
 -- 4
 select * from SpecificationAttribute
 select * from SpecificationAttributeOption
 where SpecificationAttributeId in (7,4,9)

 select * from RelatedProduct

 select * from Product_SpecificationAttribute_Mapping psm
 JOIN SpecificationAttributeOption sao on sao.Id=psm.SpecificationAttributeOptionId
 where ProductId=(Select VendorId as productid from Customer where Id=6)
 and sao.SpecificationAttributeId in (7,4,9)
 --Order By SpecificationAttributeId
 

 select sao.Id from Product_SpecificationAttribute_Mapping psm
 JOIN SpecificationAttributeOption sao on sao.Id=psm.SpecificationAttributeOptionId
 where ProductId=(Select VendorId as productid from Customer where Id=6)
 and sao.SpecificationAttributeId in (7)
 --Order By SpecificationAttributeId


 select distinct(ProductId) FROM  Product_SpecificationAttribute_Mapping
 WHERE SpecificationAttributeOptionId in 
 (
      select sao.Id from Product_SpecificationAttribute_Mapping psm
     JOIN SpecificationAttributeOption sao on sao.Id=psm.SpecificationAttributeOptionId
     where ProductId=(Select VendorId as productid from Customer where Id=6)
     and sao.SpecificationAttributeId in (7,4,9)
 )
 AND ProductId != (Select VendorId as productid from Customer where Id=6)


 SELECT * FROM ShoppingCartItem WHERE CustomerId=6 and ShoppingCartTypeId=2
 

 SELECT Distinct(ProductId) FROM ShoppingCartItem WHERE CustomerId=6
 SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=4

 SELECT Id FROM Product WHERE VendorId in (SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=4)

 select * from [dbo].[MessageTemplate]
 WHERE Body like '%product %'
 --and Body not like '%store.URL%'

 select * from [dbo].[MessageTemplate]
 WHERE Name in ('Product.ProductReview','ProductReview.Reply.CustomerNotification')


 --UPDATE [dbo].[MessageTemplate]
 --SET Subject=REPLACE(Subject,'product','profile')
 --WHERE Name in ('Product.ProductReview','ProductReview.Reply.CustomerNotification')

