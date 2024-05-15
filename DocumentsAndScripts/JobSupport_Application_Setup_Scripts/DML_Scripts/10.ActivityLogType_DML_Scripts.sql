

-- use onjobsupport47


IF NOT EXISTS (SELECT * FROM [ActivityLogType] WHERE SystemKeyword='PublicStore.ViewContactDetail')
   BEGIN
		INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled]) 
		VALUES('PublicStore.ViewContactDetail','PublicStore.ViewContactDetail',1)
   END

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.EditCustomerAvailabilityToTrue')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES ('PublicStore.EditCustomerAvailabilityToTrue','PublicStore.EditCustomerAvailabilityToTrue',1)
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.CustomerSubscriptionInfo')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES ('PublicStore.CustomerSubscriptionInfo','PublicStore.CustomerSubscriptionInfo',1)
END

GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.InterestSent')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES('PublicStore.InterestSent','Public store. Interest Sent',1)
END

GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.RemoveFromWishlist')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES('PublicStore.RemoveFromWishlist','Public store. Remove From Wishlist',1)
END

GO