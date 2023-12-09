
use [nop471]

select * from Customer
select * from [dbo].[CustomerAttribute]
select * from [dbo].[CustomerAttributeValue]

select * from [dbo].[CustomerAddresses]

select * from [dbo].[Customer_CustomerRole_Mapping]

use [nopcommerce46]

SELECT * FROM [Customer] C
INNER JOIN [GenericAttribute] GA on GA.EntityId=C.Id
WHERE GA.[Key]='CustomCustomerAttributes'
and c.Id=1

select * from [dbo].[GenericAttribute]
where KeyGroup='Customer' and EntityId=3421
order by [Key]

select * from Customer where id=3421

UPDATE [dbo].[Setting]
SET Value='Clear' 
where Name = 'customersettings.defaultpasswordformat';
-- Original : Hashed

--UPDATE [dbo].[CustomerPassword]
--SET PasswordFormatId=0
---- Original : 1

--UPDATE [dbo].[CustomerPassword]
--SET [Password]='123'

