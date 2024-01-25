

-- use onjobsupport47

---------------- Customer Roles  GiveSupport & TakeSupport -----------------

-- These roles are used for showing different topic content to different customers
-- Guest cusotmers see one topic , Give support customers see another topic and same with take support role 
SET IDENTITY_INSERT [dbo].[CustomerRole] ON

INSERT [dbo].[CustomerRole] ([Id], [Name], [SystemName], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [EnablePasswordLifetime], [OverrideTaxDisplayType], [DefaultTaxDisplayTypeId], [PurchasedWithProductId])
VALUES (6, N'Give Support', N'GiveSupport', 1, 1, 1, 0, 0, 0, 0, 0)
INSERT [dbo].[CustomerRole] ([Id], [Name], [SystemName], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [EnablePasswordLifetime], [OverrideTaxDisplayType], [DefaultTaxDisplayTypeId], [PurchasedWithProductId]) 
VALUES (7, N'Take Support', N'TakeSupport', 1, 1, 1, 0, 0, 1, 0, 0)

SET IDENTITY_INSERT [dbo].[CustomerRole] OFF



IF NOT EXISTS (SELECT * FROM [CustomerRole] WHERE [Name]='GiveSupport-Paid')
   BEGIN
    INSERT INTO [dbo].[CustomerRole]([Name],[SystemName],[FreeShipping],[TaxExempt],[Active],[IsSystemRole],[EnablePasswordLifetime],[OverrideTaxDisplayType],[DefaultTaxDisplayTypeId],[PurchasedWithProductId])
    VALUES ('GiveSupport-Paid','GiveSupport-Paid',1,0,1,0,0,0,0,0)
  END

IF NOT EXISTS (SELECT * FROM [CustomerRole] WHERE [Name]='PaidCustomer')
   BEGIN
    INSERT INTO [dbo].[CustomerRole]([Name],[SystemName],[FreeShipping],[TaxExempt],[Active],[IsSystemRole],[EnablePasswordLifetime],[OverrideTaxDisplayType],[DefaultTaxDisplayTypeId],[PurchasedWithProductId])
    VALUES ('PaidCustomer','PaidCustomer',1,0,1,0,0,0,0,0)
 END
