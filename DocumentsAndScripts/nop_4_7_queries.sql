

-- use  [qaonjobsupport47]
-- use [nopcommerce46]
-- use [onjobsupport47]
-- use [onjobsupport]

select * from Customer order by CreatedOnUtc desc
select * from [dbo].[CustomerAttribute]
select * from [dbo].[CustomerAttributeValue]
select * from [SpecificationAttributeOption]


select *,VatNumberStatusId from Customer
where Email='umsateesh@gmail.com'

--update Customer
--set VatNumberStatusId=123456
--where Email='umsateesh@gmail.com'

select * from [dbo].[CustomerAttribute]
Where Id=7

select email,LastLoginDateUtc from Customer 
order by CreatedOnUtc desc


select email,CreatedOnUtc,LastLoginDateUtc,LastActivityDateUtc
from Customer 
where 
--LastLoginDateUtc is null AND
CAST(LastLoginDateUtc AS date)= CAST(LastActivityDateUtc AS date)
AND CAST(LastActivityDateUtc AS date) < '2024-05-01'
order by CreatedOnUtc desc

select email,LastLoginDateUtc, CAST(LastLoginDateUtc as date) as date123
from Customer 
where LastLoginDateUtc is null
order by CreatedOnUtc desc

-- Products tagged with 'teradata'
--update [dbo].[CustomerAttribute]
--set helptext='Provide short description like I have 5 years experaince in XYZ technology OR I am looking to take support on XYZ technologies'
--Where Id=5

select * from Customer
order by CreatedOnUtc desc

select Email,VatNumber,VatNumberStatusId from Customer
order by CreatedOnUtc desc

select * from Customer
Where 
--affiliateid > 0 and 
id = 1394

select * from affiliate
Where 
id = 1394

select * from Customer
Where Email like '%roja.ums@gmail.com%'
-- 20500

select * from Customer
Where id=2542

--Update Customer
--set Active=1
--Where id=2542

--Update Customer
--SET CustomCustomerAttributesXML='<Attributes><CustomerAttribute ID="1"><CustomerAttributeValue><Value>2</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="7"><CustomerAttributeValue><Value>30</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="3"><CustomerAttributeValue><Value>10</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="2"><CustomerAttributeValue><Value>3</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="4"><CustomerAttributeValue><Value>20</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="5"><CustomerAttributeValue><Value>Just joined company with 5 years of work experience, so need support</Value></CustomerAttributeValue></CustomerAttribute><CustomerAttribute ID="6"><CustomerAttributeValue><Value>Need job support for Java spring boot microservice job with 5+ years of experience</Value></CustomerAttributeValue></CustomerAttribute></Attributes>'
--Where id=20500

select * from Customer
Where Email='roja.ums@gmail.com'

-- customer (me) --- I am affiliate means I have an affiliate id
-- store it in customer table

select * from CustomerRole -- 6


select * from [dbo].[Customer_CustomerRole_Mapping]
where customer_id=20500
and CustomerRole_Id=6

--update [Customer_CustomerRole_Mapping]
--set CustomerRole_Id=7
--where customer_id=20500 and CustomerRole_Id=6

select displayorder as displayorder1,* from Category
order by displayorder1 

select * from [dbo].[Product_Category_Mapping]
where ProductId=1439 and CategoryId=1

--update [Product_Category_Mapping]
--set CategoryId=2
--where ProductId=1439 and CategoryId=1

--categoryid=5

-- delete Customer Where Email is null

--delete Forums_PrivateMessage where fromcustomerid in 
--(
--select id from Customer
--Where Email is null
--)

-- EXEC [DeleteACustomer] 'roja.ums@gmail.com', 0


select * from [SpecificationAttributeOption] 
where SpecificationAttributeId=7
order by Name

select Id,Name,displayorder
from [SpecificationAttributeOption]
where SpecificationAttributeId=7
order by displayorder

with cte as 
(
select Id,Name,displayorder,
RANK() over (order by Name) as rank1
from [SpecificationAttributeOption]
where SpecificationAttributeId=7
--order by Name
)

--update cte 
--set DisplayOrder=rank1

sp_helptext DeleteACustomer

select * from Customer
Where Email like '%roja%'

select Id,Published from Product
Where Name like '%roja ums%'


UPDATE [dbo].[LocaleStringResource] 
SET [ResourceValue]='<div class="panel panel-danger"><div class="panel-heading"><h3 class="panel-title">No Results!</h3></div><div class="panel-body">No profiles were found that matched your criteria. Please adjust your filter criteria to see more profiles. </br> (OR) </br> Send an email using Contact us form, our team will try best to get the relavent profiles.</div></div>' 
WHERE resourcename = 'catalog.products.noresult';

Update LocaleStringResource 
SET ResourceValue='<div class="panel panel-danger"><div class="panel-heading"><h3 class="panel-title">Login/Register!</h3></div><div class="panel-body">Please <a href="https://onjobsupport.in/login">Login/Register</a> to see the profiles</br>This helps us to show the relavent profiles</div></div>'  
WHERE ResourceName='Catalog.Products.GuestCustomerResult'


--update LocaleStringResource
--SET ResourceValue='Your registration has been completed. An email containing an activation link has been sent to you. Verify email by clicking on the activation link. 
--If you cannot locate the email in your inbox, please check your spam folder.'
--Where ResourceName='account.register.result.standard'


select * from Category
where id=5

select * from Category
where name like '%jala%'

select * from Category
where id=1012



select * from [dbo].[Product_Category_Mapping]
where categoryid=5

select * from [dbo].[Product_Manufacturer_Mapping]


select * from [dbo].[GenericAttribute] WHERE [KEY]='NotifiedAboutNewPrivateMessages'
select * from [dbo].[GenericAttribute] where [KeyGroup]='Customer' and [Key] like '%Subscription%'

select * from [dbo].[GenericAttribute] 
where [KeyGroup]='Customer' 
AND [Key] like 'Subscription%'
and EntityId=2371

select * from [dbo].[GenericAttribute] 
where [KeyGroup]='Customer' 
AND [Key] like 'Subscription%'

--delete [dbo].[GenericAttribute] 
--where [KeyGroup]='Customer' 
--AND [Key] like '%Subscription%'
and EntityId=2371

select * from [Order]
select * from OrderItem

select * from ActivityLog

select id,customerid,CardType,CardName,CardNumber,MaskedCreditCardNumber,CardCvv2,CardExpirationMonth,CardExpirationYear
from [Order]

--delete [Order]
--delete OrderItem

select * from [dbo].[GenericAttribute] where [KeyGroup]='Customer' 
AND [Key] like '%alert%'

select * from [dbo].[GenericAttribute] WHERE [KEY]='NotifiedAboutNewPrivateMessages'
 and entityid=6

 select * from ActivityLog
 WHERE ActivityLogTypeId=3

  select * from ActivityLog
 WHERE entityid=6

 select * from ActivityLogType
 WHERE systemkeyword like '%Subscription%'
 -- CustomerSubscriptionInfo

 select * from Affiliate
 where FriendlyUrlName like '%sateesh%'

 -- public int SliderAlignmentId { get; set; }

--ALTER TABLE [WidgetZone]
--ADD SliderAlignmentId [int] NULL;


 select * from  [dbo].[WidgetZone]
 select * from [dbo].[WidgetZoneSlide]

  select * from Address
  where Id=44

select * from Customer
Where BillingAddress_Id=44

select * from Customer
Where Id=2630

select * from Customer
where AffiliateId > 0

update Customer
set VatNumberStatusId=1
where Id=6

update Customer
set AffiliateId=1
where Id > 200 and  Id < 300

select CreatedOnUtc from Customer
where Id > 200 and  Id < 300

select * from Product
Where Id in (1273,125,43)

--update Product
--set BasepriceUnitId=0,BasepriceBaseUnitId=0 
--Where Id in (1273,125)


select VendorId,BasepriceUnitId,BasepriceBaseUnitId,* from Product
Where VendorId = 0 
order by Id

select * from Product
Where Name like'%sateesh munagala%'

select c.Id,c.VendorId,c.Email,c.Phone,c.Active,c.CreatedOnUtc,p.Id,p.Name,p.VendorId,p.Published,p.Deleted,p.CreatedOnUtc from Product p
join Customer C on c.VendorId=p.Id
where p.Id=125

UPDATE P
SET P.VendorId=C.id
FROM Product P
right outer join Customer C on c.VendorId=p.Id
where p.VendorId=0


select * from Customer
where Email like '%umsateesh@gmail.com%' -- 122

select * from Customer
order by CreatedOnUtc desc

select vendorid from product

select * from [dbo].[GenericAttribute] 
WHERE [KEY]='NotifiedAboutNewPrivateMessages'
and entityid=2371

update [dbo].[GenericAttribute]
SET Value='False'
WHERE [KEY]='NotifiedAboutNewPrivateMessages'
and entityid=2371

select TOP 10 * from [dbo].[Setting] 
WHERE Name like '%Captcha%'


select TOP 10 * from [dbo].[Setting] 
WHERE Name like '%review%'


UPDATE [dbo].[Setting] SET Value='True' WHERE Name = 'forumsettings.showalertforpm';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'messagessettings.usepopupnotifications';


--delete Customer
--where Email is null

select * from CustomerPassword
-- where CustomerId

select * from CustomerPassword CP
INNER JOIN customer C on C.Id=CP.customerid
where 
--C.CustomerProfileTypeId=2 and 
C.Id=2542 and 
C.FirstName like '%roja%'


select * from Product
where Id=4

select * from Customer
where Id in (select Id from Customer Where Active=0)

update Customer
set Active=1
where Id in (
select Id from Customer Where Active=0
)

select * from Customer
Where Active=0
order by CreatedOnUtc desc



-- 18081,18053,18050,17666,17614,17555,17478,17445,17194,17087,17063,16987,16967,16711,16562,15963


select * from Customer
where Id in (18081,18053,18050,17666,17614,17555,17478,17445,17194,17087,17063,16987,16967,16711,16562,15963)

select Id,Email,deleted,CreatedOnUtc,Active from Customer Where Active=0
order by id desc

select * from [dbo].Forums_PrivateMessage
--where FromCustomerId=6
order by createdonutc desc


select pm.FromCustomerId
,C.FirstName as From_Customer
,C.Email as From_Email
,pm.ToCustomerId
,C2.FirstName as To_Customer
,C2.Email as To_Email,
pm.IsSystemGenerated
from [dbo].Forums_PrivateMessage pm
inner join Customer C on C.Id=pm.FromCustomerId
Inner join Customer C2 on C2.Id=pm.ToCustomerId
order by pm.createdonutc desc 

select * from Customer where Id in (14748,14128)

select * from Customer where Id=6
select * from Customer where Id=1385



--select '' <> NUll
--select 1==1



select * from Customer
Where Email='saiprakashchintapalli@gmail.com'


select * from [dbo].[Topic]
select * from [dbo].[TopicTemplate]
select * from [dbo].[Affiliate]


select Id,AffiliateId from Customer
where AffiliateId > 0

select Id,AffiliateId from Customer
where AffiliateId =19

select * from UrlRecord
WHERE EntityName='Topic'

select * from UrlRecord

select * from UrlRecord
WHERE slug='affiliates'


select * from [dbo].[MessageTemplate]
where Name like '%NotifyTargetCustomers%'
order by Name

select * from [dbo].[MessageTemplate]
where BccEmailAddresses is not null

--update[dbo].[MessageTemplate]
--SET BccEmailAddresses=NULL
--where BccEmailAddresses='umsateesh@gmail.com'

select * from QueuedEmail
order by createdonUTC DESC

select * from QueuedEmail A
INNER JOIN Customer C on C.email=A.[To]
where C.Active=0
order by A.SentOnUtc desc

-- NewCustomer.NotifyTargets

select * from [dbo].[GenericAttribute] 
where [KeyGroup]='Customer' and [Key] like '%allotted%'

--update [CustomerAttribute]
--SET HelpText='<p>Provide Your mother tongue so that others can contact you &lt;br/&gt; if they want to take support from same language persons</p>'
--WHERE Id=4

SELECT * FROM MigrationVersionInfo
--SELECT * FROM DistributedCache


--update [CustomerAttribute]
--SET ShowOnRegisterPage=1
--WHERE Id=4

--update [CustomerAttribute]
--SET IsRequired=0
--WHERE Id=9

select RewardPointsHistoryEntryId,* from [dbo].[Order]

select * from [dbo].[OrderItem]

select * from [dbo].[RewardPointsHistory]
where customerid=6
order by CreatedOnUtc desc,Id desc

select * from [dbo].[RewardPointsHistory]
where Id=33

update [dbo].[RewardPointsHistory]
set EndDateUtc=DATEADD(DAY,2,CreatedOnUtc)
where Id=33

select DATEADD(DAY,-27,
delete [dbo].[Order]
where RewardPointsHistoryEntryId in (27,28,30)

delete [dbo].[RewardPointsHistory]  
where customerid=6
and Id in (27,28,30)
-- FK_Order_RewardPointsHistoryEntryId_RewardPointsHistory_Id
-- RewardPointsHistoryEntryId

select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption]
select * from [SpecificationAttributeOption] where SpecificationAttributeId=7
select * from [SpecificationAttributeOption] where id=7
select * from [ProductAttribute] where id=2

select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption]
select * from [SpecificationAttributeOption] where SpecificationAttributeId=9

select * from [SpecificationAttributeOption]
where Id=480

UPDATE [SpecificationAttributeOption]
SET [Name]='MS SQL Server'
where Id=480

select * from [SpecificationAttributeOption] where id in (1,2)

select * from Product_SpecificationAttribute_Mapping
Where SpecificationAttributeOptionId in (1,2)

select * from Product where Id=122

select * from Product_SpecificationAttribute_Mapping
where productid=122 and id=923

select * from Product_SpecificationAttribute_Mapping
where id=923

UPDATE Product_SpecificationAttribute_Mapping
SET SpecificationAttributeOptionId=1
where Id=923

select * from Product_SpecificationAttribute_Mapping
Where SpecificationAttributeOptionId in (1,2)
and AllowFiltering=1

select * from Product
where Id=6

--update Product_SpecificationAttribute_Mapping
--set 
--AllowFiltering=0
--where Id=2882


select * from [dbo].[CustomerAddresses]
where Customer_Id=70

select * from [dbo].[ProductReview]
where ProductId=5

select Id,BillingAddress_Id,Email from [dbo].[Customer]
WHERE Email is null

SELECT * FROM Address

SELECT * FROM [dbo].[CustomerAddresses]
where Customer_Id=1221


IF NOT EXISTS (SELECT * FROM [LocaleStringResource] WHERE [ResourceName]='account.register.errors.phonealreadyexists')
   BEGIN
		INSERT INTO [dbo].[LocaleStringResource]([ResourceName],[ResourceValue],[LanguageId]) 
		VALUES('account.register.errors.phonealreadyexists','The specified phone already exists.',1)
   END


SELECT * FROM [dbo].[Affiliate]
WHERE AddressId=44

select * from [dbo].[CustomerRole]

select * from [dbo].[Customer_CustomerRole_Mapping]
where CustomerRole_Id=9


SELECT * FROM [Customer] C
INNER JOIN [GenericAttribute] GA on GA.EntityId=C.Id
WHERE GA.[Key]='CustomCustomerAttributes'
and c.Id=1

select * from [dbo].[GenericAttribute]
order by [Key] desc

select * from [dbo].[GenericAttribute]
where KeyGroup='Customer' and [Key]='CustomCustomerAttributes'
order by [Key]

select * from [dbo].[GenericAttribute]
where KeyGroup='Customer' and EntityId=14
order by [Key]

select * from Customer where id=3421

select * from Product where id=3421

select Name,PageSize,PageSizeOptions from Category
order by PageSizeOptions desc

select Name,PageSize from Category
where Id > 3

Select id,name,displayorder,ShowOnHomepage 
from Category 
where Id not in (1,2,3) and ShowOnHomepage=1
order by displayorder asc
--set PageSize=50

update Category
set ShowOnHomepage=0
where Id in (6,5,4,821,872,719,52)

update Category
set ShowOnHomepage=1
where Id in (799)

Select C.Id ,C.Name,COUNT(pc.Id) as Count from Category C
	Join Product_Category_Mapping pc
	on pc.CategoryId=C.Id
	Where C.Id not in (1,2,3)
	group by C.Id,C.Name
	order by Count desc,C.Name

--update Category
--SET 
--MetaTitle=CONCAT(Name,' Proxy Support. ',Name,' Job Support from India')
--, MetaDescription=CONCAT(Name,' Proxy Support. ',Name,' IT Job Support from India | Proxy Support Services USA/UK/India | Proxy Interview Support | No Mediator Platform | Technical Proxy Support')
--where Id > 3

--Adeptia Etl Proxy Support. Adeptia Etl Job Support from India
--Adeptia Etl Proxy SupportAdeptia Etl Job Support from India | Proxy support services | Proxy Interview Support | No Mediator Platform

select * from Product_Category_Mapping
select * from [dbo].[Picture]
select * from [dbo].[Product_Picture_Mapping]


select C.Id,C.Name,COUNT(pc.Id) as cnt from Category C
Join Product_Category_Mapping pc
on pc.CategoryId=C.Id
group by C.Id,C.Name

SELECT compatibility_level  
FROM sys.databases WHERE name = 'AdventureWorks2022';  

SELECT value
FROM GENERATE_SERIES(1, 10);

select C.Id as CategoryId,C.Name,COUNT(pc.Id) as Count from Category C
Join Product_Category_Mapping pc
on pc.CategoryId=C.Id
group by C.Id,C.Name
order by Count desc

Select id from Category CA
Where CA.Id=
(select A.Id from 
    (
        select C.Id,C.Name,COUNT(pc.Id) as cnt from Category C
        Join Product_Category_Mapping pc
        on pc.CategoryId=C.Id
        group by C.Id,C.Name
    )  A
WHERE A.Id=CA.Id
)

select * from 
(
select C.Id,C.Name,COUNT(pc.Id) as cnt from Category C
Join Product_Category_Mapping pc
on pc.CategoryId=C.Id
group by C.Id,C.Name
)  A
order by A.cnt desc
-- technologies with zero supporters

select C.Id as CategoryId,C.Name,COUNT(pc.Id) as Count from Category C
right Join Product_Category_Mapping pc
on pc.CategoryId=C.Id
group by C.Id,C.Name
order by Count desc

select C.Id as CategoryId,C.Name,0 As Count
from Category C
Where C.Id not in (select CategoryId from Product_Category_Mapping)
order by Name Asc


select *,UpdatedOnUtc as up from Product 
--where id=3421
order by up desc


select OrderMaximumQuantity from Product where id=3421

select Top 100  * from setting 
where Name like '%nivo%' 
--where Name like 'catalogsettings.%' 
--and Name like '%pagesize%'
order by Name asc

--update Product set OrderMaximumQuantity=1000
--Where id=1

--UPDATE [dbo].[Setting]
--SET Value='Clear' 
--where Name = 'customersettings.defaultpasswordformat';
-- Original : Hashed

--UPDATE [dbo].[CustomerPassword]
--SET PasswordFormatId=0
---- Original : 1

--UPDATE [dbo].[CustomerPassword]
--SET [Password]='123'
--WHERE id in (2,3)

select * from CustomerPassword
order by createdonutc desc

select * from Topic
where id=14

update topic 
SET SystemName='LoginPageNewCustomerText'
where id=14

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
where Name like '%subscription%';

select * FROM [dbo].[Setting] 
where Name like '%activation%';

select * FROM [dbo].[Setting] 
where value like '%Account activation%';

select * FROM [dbo].[Setting] 
where value like '%Be the first%';

select * from LocaleStringResource
Where ResourceName like '%Admin.Customers.Customers.Fields.%'
order by ResourceName

select * from LocaleStringResource
Where ResourceValue like '%Viewed a product details page %'

-- UPDATE [dbo].[Setting] SET Value='True' WHERE Name = 'forumsettings.showalertforpm';

select * from LocaleStringResource
Where ResourceValue like '%successfully processed%'

--update LocaleStringResource
--SET ResourceValue='Your registration has been successfully processed.An email containing an activation link has been sent to you. Kindly click on the activation link to activate your account. If you cannot locate the email in your inbox, please check your spam folder.'
--Where ResourceName='account.register.result.standard'

--update LocaleStringResource
--SET ResourceValue='Your subscription has been successfully processed!'
--Where ResourceName='checkout.yourorderhasbeensuccessfullyprocessed'


select * from LocaleStringResource
Where ResourceName like 'account.register.result%'

select * from LocaleStringResource
Where ResourceValue like '%Your registration completed%'
-- account.register.result.standard

select * from LocaleStringResource
Where ResourceName='media.category.imagelinktitleformat'

select * FROM [dbo].[Setting] 
where 
Name like 'catalogsettings.%homepage%';


select * FROM Customer
select * FROM CustomerRole

select * FROM [dbo].[Customer_CustomerRole_Mapping]
-- 6

-- Insert Into Customer_CustomerRole_Mapping values(6,2)

-- update CustomerPassword set PasswordFormatId=0, Password='India@2024'

select * from Product
select * from UrlRecord
select * from [Product_Category_Mapping]

select * FROM Customer where id=204
select * FROM Customer where VendorId=5

-- hi-IN (India)

select * from UrlRecord
WHERE EntityName='Topic'

select * from [Forums_Topic]
select * from [dbo].[Forums_Forum]



--delete [Order]
--delete OrderItem

-- delete [Log]

select * from [Log]
where ShortMessage='A bot detected. Honeypot.'

select distinct(shortmessage) from [Log]
where shortmessage like 'Nop.Web.Models%'
order by shortmessage

select * from [Log]
where LogLevelId=20
order by createdonutc desc



select * from [picture]
select * from [pictureBinary]



 --update Category set ShowOnHomepage=1
 --Where Id in (4,5)


 select * from shoppingcartitem
 -- delete shoppingcartitem

  select * from ScheduleTask

 select * from shoppingcartitem
 where ProductId=4

 select * from Product where VendorId=14

 select * from ActivityLog
 WHERE ActivityLogTypeId=149

 DELETE ActivityLog
 WHERE IpAddress='127.0.0.1'

 DELETE ActivityLog
 WHERE CustomerId=6

 select * from ActivityLog
 WHERE CustomerId=6

 select * from ActivityLog
 WHERE Comment like '%Deleted system log%'

 select * from ActivityLog
 WHERE ActivityLogTypeId=145

 select * from ActivityLogType
 where Id=143

 select * from ActivityLogType
 where Name like '%Public store. View a product%'

 Select * from ActivityLogType
 WHERE Id=168

 update ActivityLogType
 set Name='Public store. Remove From Wishlist'
 WHERE Id=168

 
Public store. Removed From Wishlist

 --update ActivityLogType set Enabled=1
 --where SystemKeyword like '%public%'

 select * from ActivityLogType
 Where Id=36

 select * from ActivityLogType
 where Name like '%message%'

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

 SELECT * FROM  [GenericAttribute]
 Where KeyGroup='Customer'
 AND [Key] like '%sub%'

  SELECT * FROM  [GenericAttribute]
 Where KeyGroup='Customer'
 AND [Key] like '%PublicStore.last%'
 order by EntityId asc

   SELECT * FROM  [GenericAttribute]
 Where KeyGroup='Customer'
 AND [Key] like '%LastVisitedPage%'
 order by CreatedOrUpdatedDateUTC desc

--update  [GenericAttribute]
--set StoreId=1
-- Where KeyGroup='Customer'
-- AND [Key] like '%PublicStore.ViewContactDetail%'



 SELECT * FROM  [GenericAttribute]
 Where Id=73
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
 WHERE Body like '% store %'
 --and Body not like '%store.URL%'

 select * from [dbo].[MessageTemplate]
 WHERE Name in ('Product.ProductReview','ProductReview.Reply.CustomerNotification')

  select Id,Name,ShowOnHomepage,DisplayOrder from [dbo].[Category]
  order by displayorder asc

  update [dbo].[Category]
  set DisplayOrder=100
  Where Id>7

  --update [dbo].[Category]
  --set ShowOnHomepage=0
  --Where DisplayOrder >= 100



  --UPDATE [dbo].[Category]
  --SET Description='<table style="border-collapse: collapse; width: 99.9%; height: 136px; background-color: #f9f9f9; border-color: #ffa500; border-style: solid;" border="1" cellspacing="5" cellpadding="5"><caption>&nbsp;</caption>  <tbody>  <tr>  <td style="width: 100%;">  <ul>  <li><span style="font-size: 10pt;">Below Profiles are interested to take support from you.</span></li>  <li><span style="font-size: 10pt;">You can filter further by using left filter to match your skillset.</span></li>  <li><span style="font-size: 10pt;">You can shortlist the profiles that you like. Shortlisted profiles will appear in My Account page for easy access in future.</span></li>  <li><span style="font-size: 10pt;">You can send interest to the profiles that you like. If they also interested about your profile they may contact you.</span></li>  </ul>  </td>  </tr>  </tbody>  </table>'
  --Where Id=2

  --UPDATE [dbo].[Category]
  --SET Description='<table style="border-collapse: collapse; width: 99.9%; height: 136px; background-color: #f9f9f9; border-color: #ffa500; border-style: solid;" border="1" cellspacing="5" cellpadding="5"><caption>&nbsp;</caption>  <tbody>  <tr>  <td style="width: 100%;">  <ul>  <li><span style="font-size: 10pt;">Below Profiles are interested to provide support.</span></li>  <li><span style="font-size: 10pt;">You can filter further by using left filter to match your skillset.</span></li>  <li><span style="font-size: 10pt;">You can shortlist the profiles that you like. Shortlisted profiles will appear in My Account page for easy access in future.</span></li>  <li><span style="font-size: 10pt;">You can send interest to the profiles that you like. If they also interested about your profile they may contact you.</span></li>  </ul>  </td>  </tr>  </tbody>  </table>'
  --Where Id=1


  select * from EmailAccount

 --UPDATE [dbo].[MessageTemplate]
 --SET Body=REPLACE(Body,' store ',' ')
 --WHERE Name in ('ProductReview.Reply.CustomerNotification')

 SELECT * FROM [dbo].[MessageTemplate]
 --SET Subject=REPLACE(Subject,' store ','')
 WHERE Name in ('ProductReview.Reply.CustomerNotification')

  -- use onjobsupport47

 select * from [dbo].[LocaleStringResource]
 WHERE  ResourceName like 'Account.Register.Errors.%'


 -- Added a product to wishlist ('{0}')

  select * from [dbo].[LocaleStringResource]
 WHERE  ResourceName like '%sendpm%'
 -- activitylog.publicstore.sendpm


 select * from [dbo].[LocaleStringResource]
 WHERE  ResourceName like 'common.wait%'

  select * from [dbo].[LocaleStringResource]
 WHERE  ResourceValue like '%profiles tagged with%'

 select * from [dbo].[LocaleStringResource]
 WHERE  ResourceValue like '%product%'

  select * from QueuedEmail
  -- delete QueuedEmail


   select * from [qaonjobsupport].[dbo].[LocaleStringResource]
   select * from [onjobsupport47].[dbo].[LocaleStringResource]

   --delete [onjobsupport47].[dbo].[LocaleStringResource]
   --DBCC CHECKIDENT ([LocaleStringResource], reseed, 0)

   select * from [onjobsupport47].[dbo].[LocaleStringResource]
   WHERE ResourceName Not in (select ResourceName from [qaonjobsupport].[dbo].[LocaleStringResource])

SELECT * FROM [SpecificationAttributeOption] 
WHERE Id > 28
AND DisplayOrder = 0

SELECT * FROM [Category] 
SELECT * FROM [UrlRecord] 
WHERE EntityName='Category'


-- Insert INTO [Category] 
--SELECT 


--INSERT INTO [dbo].[Category] ( [Name],[CategoryTemplateId],[ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize],
--[ShowOnHomepage], [IncludeInTopMenu], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], 
--[CreatedOnUtc], [UpdatedOnUtc], [PriceRangeFiltering], [PriceFrom], [PriceTo], [ManuallyPriceRange])
--SELECT [Name],1,0,0,20,0,1,0,0,0,1,0,0,GETUTCDATE(), GETUTCDATE(),0,CAST(0.0000 AS Decimal(18, 4)), CAST(10000.0000 AS Decimal(18, 4)),0
--FROM [SpecificationAttributeOption]

--INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[IsActive],[LanguageId])
--SELECT 'Category',REPLACE(Name,' ','-'),Id,1,0 FROM [Category] 

SELECT * FROM ShoppingCartItem

SELECT * FROM UrlRecord
WHERE EntityId=

SELECT * FROM [Product]
where Id=51

SELECT Distinct(ProductId) 
				FROM ShoppingCartItem AS a
				WHERE CustomerId=12937 AND ShoppingCartTypeId=2
				AND Exists (
                    Select * FROM Product AS p 
                    WHERE p.Id=a.ProductId 
                    AND p.Published=1
                )


   Select * FROM Product AS p 
                    WHERE p.Id=51
                    AND p.Published=1

DECLARE @myProductId Int;
Select @myProductId=VendorId FROM Customer WHERE Id=12937

select @myProductId
SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=@myProductId

SELECT Id 
FROM Product
WHERE VendorId in (SELECT CustomerId FROM ShoppingCartItem WHERE ProductId=217)
AND Published=1

select * from Customer where Id=14328
--VendorId
--1238

--update Customer
--set Active=1
--where Id in (
--select Id from Customer Where Active=0
--)

Update [LocaleStringResource] SET ResourceValue='Popular Technologies' WHERE ResourceName='categories'

SELECT STRING_AGG(sao.Name, ',') AS 'Primary Technology'
                FROM [Product_SpecificationAttribute_Mapping] PS
				JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
				JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
				WHERE sa.Id=7 and ps.ProductId=10
                ) AS 'PrimaryTechnology'



CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

DECLARE @tags NVARCHAR(400) = '1,2,3,4,5'

INSERT INTO #Products ([ProductId])
SELECT value
FROM STRING_SPLIT(@tags, ',')
WHERE TRIM(value) <> '';

select * from #Products

drop table #Productsc


select * from [dbo].[Manufacturer]
select * from [dbo].[ManufacturerTemplate]



-----------------------------------------------------------------------------------
   -- ******** Reporting Queries  ******** 
-----------------------------------------------------------------------------------

-- Query 1
-- Registered customers per day by profile type

select * from Customer 
where customerprofiletypeid <> 0
order by CreatedOnUtc desc

-- 748 - by septemeber end need to take to 1500
-- per month 1000 calls

-- -- Registered Customers Per Day By Profile Type
select CAST(CreatedOnUtc as date) as Date , 
SUM(CASE 
    When customerprofiletypeid = 1 Then 1 Else 0 
    END) as 'Give-Support Count',
SUM(CASE 
    When customerprofiletypeid = 2 Then 1 Else 0 
    END) as 'Take-Support Count',
COUNT(*) as [Total Count] from Customer 
where customerprofiletypeid <> 0
group by CAST(CreatedOnUtc as date)
order by Date desc

SELECT CAST(CreatedOnUtc as date) as [Date],
SUM(
    CASE 
    WHEN CustomerProfileTypeId=1 Then 1
    ELSE 0
    END
) as [TakeSupport Count]
, SUM(
    CASE  
    WHEN CustomerProfileTypeId=2 Then 1
    ELSE 0
    END
) as [GiveSupport Count]
FROM Customer
Group By CAST(CreatedOnUtc as date)
Order by CAST(CreatedOnUtc as date) DESC


-- Registered customers Per Month By Profile Type
select YEAR(CreatedOnUtc) as [Year], Month(CreatedOnUtc) as [Month] , 
SUM(CASE 
    When customerprofiletypeid = 1 Then 1 Else 0 
    END) as 'Give-Support Count',
SUM(CASE 
    When customerprofiletypeid = 2 Then 1 Else 0 
    END) as 'Take-Support Count',
COUNT(*) as [Total Count] from Customer 
where customerprofiletypeid <> 0
group by YEAR(CreatedOnUtc), Month(CreatedOnUtc)
--as [Month]
order by [Month] ASC


sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'Ole Automation Procedures', 1;
GO
RECONFIGURE;
GO

