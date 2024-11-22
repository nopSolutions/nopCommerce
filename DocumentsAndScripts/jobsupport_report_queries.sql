
-- use  [qaonjobsupport47]
-- use [nopcommerce46]
-- use [onjobsupport47]
-- use [onjobsupport]

------------------------------------------------------------------
        -- ** Private messages  ** --
------------------------------------------------------------------
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

--------------------------------------------------------------------------
        -- ** Customer Count By Date and customer profile type   ** --
--------------------------------------------------------------------------

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

--------------------------------------------------------------------------

SELECT * 
FROM Customer
WHERE CAST(CreatedOnUtc as date)='2024-05-16'

SELECT TOP 100 * FROM onjobsupport47.dbo.Customer

-----------------------------------------------------------

 DELETE ActivityLog
 WHERE CustomerId=6
