
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
where Name like 'catalogsettings.tag%';


select * FROM [dbo].[Setting] 
where Name like '%tag%'
and Name like 'catalogsettings.%';


select * FROM CustomerPassword

-- update CustomerPassword set PasswordFormatId=0, Password='India@2024'

select * from Product
select * from UrlRecord
select * from [Product_Category_Mapping]


select * from [Order]
select * from OrderItem


select * from [Log]


