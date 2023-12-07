USE [onjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[NotifyCustomersWhenNewCustomerRegisters]    Script Date: 12/7/2023 4:26:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--execution exampls
-- exec [NotifyCustomersWhenNewCustomerRegisters] @CustomerId=263
-- exec [NotifyCustomersWhenNewCustomerRegisters] @CustomerId=263


CREATE PROCEDURE [dbo].[NotifyCustomersWhenNewCustomerRegisters]
(
	@CustomerId		int = 0 -- customer id
)
AS
BEGIN	

	DECLARE @ProfileType varchar(100);

	-- get newly registered customer profile type
    SET @ProfileType=( SELECT CustomerProfileTypeId FROM Customer WHERE Id=@CustomerId)
    --SET @ProfileType=( SELECT CustomerProfileTypeId FROM Customer WHERE Id=263)

	-- get target customers to whom we need send notifications about new registration

	CREATE TABLE #TargetCustomer 
	(
		[CustomerId] int NOT NULL,
		[EmailId] varchar(200) NULL
	)

	INSERT INTO #TargetCustomer ([CustomerId],[EmailId])
		SELECT C.Id,C.Email FROM Customer C 
		INNER JOIN [Customer_CustomerRole_Mapping] CR ON C.Id=CR.Customer_Id
		WHERE C.Deleted=0 AND C.Active=1 AND C.CustomerProfileTypeId=IIF(@ProfileType=1, 2, 1)
		AND CR.CustomerRole_Id=9; -- Get only paid customers

	SELECT * FROM #TargetCustomer

	-- insert into message queue table

	DROP TABLE #TargetCustomer

		
	

END
GO


