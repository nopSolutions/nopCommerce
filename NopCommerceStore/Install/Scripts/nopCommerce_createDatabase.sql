CREATE TABLE [dbo].[Nop_CustomerRole](
	[CustomerRoleID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[FreeShipping] [bit] NOT NULL,
	[TaxExempt] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Nop_CustomerRole] PRIMARY KEY CLUSTERED 
(
	[CustomerRoleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Warehouse](
	[WarehouseID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[FaxNumber] [nvarchar](50) NOT NULL,
	[Address1] [nvarchar](100) NOT NULL,
	[Address2] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[StateProvince] [nvarchar](100) NOT NULL,
	[ZipPostalCode] [nvarchar](30) NOT NULL,
	[CountryId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK__Nop_Warehouse__0B5CAFEA] PRIMARY KEY CLUSTERED 
(
	[WarehouseID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Country](
	[CountryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[AllowsRegistration] [bit] NOT NULL,
	[AllowsBilling] [bit] NOT NULL,
	[AllowsShipping] [bit] NOT NULL,
	[TwoLetterISOCode] [nvarchar](2) NOT NULL,
	[ThreeLetterISOCode] [nvarchar](3) NOT NULL,
	[NumericISOCode] [int] NOT NULL,
	[SubjectToVAT] [bit] NOT NULL CONSTRAINT [DF_Nop_Country_SubjectToVAT]  DEFAULT ((0)),
	[Published] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Nop_Country_DisplayOrder]  DEFAULT ((1)),
 CONSTRAINT [PK_Nop_Country] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Country_DisplayOrder] ON [dbo].[Nop_Country] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Affiliate](
	[AffiliateID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[FaxNumber] [nvarchar](50) NOT NULL,
	[Company] [nvarchar](100) NOT NULL,
	[Address1] [nvarchar](100) NOT NULL,
	[Address2] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[StateProvince] [nvarchar](100) NOT NULL,
	[ZipPostalCode] [nvarchar](30) NOT NULL,
	[CountryId] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Nop_Affiliate_Deleted]  DEFAULT ((0)),
 CONSTRAINT [PK_Nop_Affiliate] PRIMARY KEY CLUSTERED 
(
	[AffiliateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingRateComputationMethod](
	[ShippingRateComputationMethodID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[ConfigureTemplatePath] [nvarchar](500) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Nop_ShippingRateComputationMethod_IsActive]  DEFAULT ((0)),
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_ShippingRateComputationMethod] PRIMARY KEY CLUSTERED 
(
	[ShippingRateComputationMethodID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Log](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[LogTypeID] [int] NOT NULL,
	[Severity] [int] NOT NULL,
	[Message] [nvarchar](1000) NOT NULL,
	[Exception] [nvarchar](4000) NOT NULL,
	[IPAddress] [nvarchar](100) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[PageURL] [nvarchar](100) NOT NULL,
	[ReferrerURL] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Log_ReferrerURL]  DEFAULT (''),
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Log_CreatedOn] ON [dbo].[Nop_Log] 
(
	[CreatedOn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_TaxCategory](
	[TaxCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DisplayOrder] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_TaxCategory] PRIMARY KEY CLUSTERED 
(
	[TaxCategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Nop_Download](
	[DownloadID] [int] IDENTITY(1,1) NOT NULL,
	[UseDownloadURL] [bit] NOT NULL CONSTRAINT [DF_Nop_Nop_Download_UseDownloadURL]  DEFAULT ((0)),
	[DownloadURL] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_Download_DownloadURL]  DEFAULT (''),
	[DownloadBinary] [varbinary](max) NULL,
	[ContentType] [nvarchar](20) NOT NULL CONSTRAINT [DF_Nop_Download_ContentType]  DEFAULT (N''),
	[Filename] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Download_Filename]  DEFAULT (''),
	[Extension] [nvarchar](20) NOT NULL,
	[IsNew] [bit] NOT NULL,
 CONSTRAINT [Nop_Download_PK] PRIMARY KEY CLUSTERED 
(
	[DownloadID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Nop_ProductAttribute](
	[ProductAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](400) NOT NULL,
 CONSTRAINT [PK_Nop_ProductAttribute] PRIMARY KEY CLUSTERED 
(
	[ProductAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerGUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Nop_Customer_CustomerGUID]  DEFAULT (newid()),
	[Email] [nvarchar](255) NOT NULL CONSTRAINT [DF_Nop_Customer_Email]  DEFAULT (''),
	[Username] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Customer_Username]  DEFAULT (''),
	[PasswordHash] [nvarchar](255) NOT NULL CONSTRAINT [DF_Nop_Customer_Password]  DEFAULT (''),
	[SaltKey] [nvarchar](255) NOT NULL CONSTRAINT [DF_Nop_Customer_SaltKey]  DEFAULT ((0)),
	[AffiliateID] [int] NOT NULL,
	[BillingAddressID] [int] NOT NULL,
	[ShippingAddressID] [int] NOT NULL,
	[LastPaymentMethodID] [int] NOT NULL,
	[LastAppliedCouponCode] [nvarchar](100) NOT NULL,
	[GiftCardCouponCodes] [xml] NOT NULL CONSTRAINT [DFNop_Customer_GiftCardCouponCodes]  DEFAULT (''),
	[CheckoutAttributes] [xml] NOT NULL CONSTRAINT [DF_Nop_Customer_CheckoutAttributes]  DEFAULT (''),
	[LanguageID] [int] NOT NULL,
	[CurrencyID] [int] NOT NULL,
	[TaxDisplayTypeID] [int] NOT NULL CONSTRAINT [DF_Nop_Customer_TaxDisplayTypeID]  DEFAULT ((1)),
	[IsTaxExempt] [bit] NOT NULL CONSTRAINT [DF_Nop_Customer_IsTaxExempt]  DEFAULT ((0)),
	[IsAdmin] [bit] NOT NULL,
	[IsGuest] [bit] NOT NULL CONSTRAINT [DF_Nop_Customer_IsGuest]  DEFAULT ((0)),
	[IsForumModerator] [bit] NOT NULL CONSTRAINT [DF_Nop_Customer_IsForumModerator]  DEFAULT ((0)),
	[TotalForumPosts] [int] NOT NULL CONSTRAINT [DF_Nop_Customer_TotalForumPosts]  DEFAULT ((0)),
	[Signature] [nvarchar](300) NOT NULL CONSTRAINT [DF_Nop_Customer_Signature]  DEFAULT (''),
	[AdminComment] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_Customer_AdminComment]  DEFAULT (''),
	[Active] [bit] NOT NULL CONSTRAINT [DF_Nop_Customer_Active]  DEFAULT ((1)),
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Nop_Customer_Deleted]  DEFAULT ((0)),
	[RegistrationDate] [datetime] NOT NULL,
	[TimeZoneID] [nvarchar](200) NOT NULL CONSTRAINT [DF_Nop_Customer_TimeZoneID]  DEFAULT (''),
	[AvatarID] [int] NOT NULL CONSTRAINT [DF_Nop_Customer_AvatarID]  DEFAULT ((0)),
	[DateOfBirth] [datetime] NULL,
 CONSTRAINT [PK_Nop_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Customer_AffiliateID] ON [dbo].[Nop_Customer] 
(
	[AffiliateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Customer_Email] ON [dbo].[Nop_Customer] 
(
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Customer_Username] ON [dbo].[Nop_Customer] 
(
	[Username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_TaxProvider](
	[TaxProviderID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[ConfigureTemplatePath] [nvarchar](500) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_TaxProvider] PRIMARY KEY CLUSTERED 
(
	[TaxProviderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_QueuedEmail](
	[QueuedEmailID] [int] IDENTITY(1,1) NOT NULL,
	[Priority] [int] NOT NULL,
	[From] [nvarchar](500) NOT NULL,
	[FromName] [nvarchar](500) NOT NULL,
	[To] [nvarchar](500) NOT NULL,
	[ToName] [nvarchar](500) NOT NULL,
	[Cc] [nvarchar](500) NOT NULL,
	[Bcc] [nvarchar](500) NOT NULL,
	[Subject] [nvarchar](500) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[SendTries] [int] NOT NULL,
	[SentOn] [datetime] NULL,
	[EmailAccountId] [int] NOT NULL CONSTRAINT [DF_Nop_QueuedEmail_EmailAccountId]  DEFAULT ((0)),
 CONSTRAINT [PK_Nop_QueuedEmail] PRIMARY KEY CLUSTERED 
(
	[QueuedEmailID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_QueuedEmail_CreatedOn] ON [dbo].[Nop_QueuedEmail] 
(
	[CreatedOn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_BannedIpAddress](
	[BannedIpAddressID] [int] IDENTITY(1,1) NOT NULL,
	[Address] [nvarchar](50) NOT NULL,
	[Comment] [nvarchar](500) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_BannedIpAddress] PRIMARY KEY CLUSTERED 
(
	[BannedIpAddressID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SMSProvider](
	[SMSProviderId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[SystemKeyword] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SMSProvider] PRIMARY KEY CLUSTERED 
(
	[SMSProviderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_MeasureDimension](
	[MeasureDimensionID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemKeyword] [nvarchar](100) NOT NULL,
	[Ratio] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Nop_MeasureDimension_Ratio]  DEFAULT ((1)),
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_MeasureDimention] PRIMARY KEY CLUSTERED 
(
	[MeasureDimensionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_MeasureWeight](
	[MeasureWeightID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemKeyword] [nvarchar](100) NOT NULL,
	[Ratio] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Nop_MeasureWeight_Ratio]  DEFAULT ((1)),
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_MeasureWeight] PRIMARY KEY CLUSTERED 
(
	[MeasureWeightID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_BannedIpNetwork](
	[BannedIpNetworkID] [int] IDENTITY(1,1) NOT NULL,
	[StartAddress] [nvarchar](50) NOT NULL,
	[EndAddress] [nvarchar](50) NOT NULL,
	[Comment] [nvarchar](500) NULL,
	[IpException] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_BannedNetworkIP] PRIMARY KEY CLUSTERED 
(
	[BannedIpNetworkID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_QBEntity](
	[EntityId] [int] IDENTITY(1,1) NOT NULL,
	[QBEntityId] [nvarchar](50) NOT NULL,
	[EntityTypeId] [int] NOT NULL,
	[NopEntityId] [int] NOT NULL,
	[SynStateId] [int] NOT NULL,
	[SeqNum] [nvarchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_QBEntity] PRIMARY KEY CLUSTERED 
(
	[EntityId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_MessageTemplate](
	[MessageTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Nop_MessageTemplate] PRIMARY KEY CLUSTERED 
(
	[MessageTemplateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_MessageTemplate_Name] ON [dbo].[Nop_MessageTemplate] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ActivityLogType](
	[ActivityLogTypeID] [int] IDENTITY(1,1) NOT NULL,
	[SystemKeyword] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Nop_ActivityLogType] PRIMARY KEY CLUSTERED 
(
	[ActivityLogTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CreditCardType](
	[CreditCardTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemKeyword] [nvarchar](100) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Nop_CreditCardType] PRIMARY KEY CLUSTERED 
(
	[CreditCardTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Setting](
	[SettingID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](2000) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Nop_Setting] PRIMARY KEY CLUSTERED 
(
	[SettingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_Setting] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Order](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[OrderGUID] [uniqueidentifier] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CustomerLanguageID] [int] NOT NULL,
	[CustomerTaxDisplayTypeID] [int] NOT NULL CONSTRAINT [DF_Nop_Order_CustomerTaxDisplayTypeID]  DEFAULT ((1)),
	[CustomerIP] [nvarchar](50) NOT NULL CONSTRAINT [DF_Nop_Order_CustomerIP]  DEFAULT (''),
	[OrderSubtotalInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubtotalInclTax]  DEFAULT ((0)),
	[OrderSubtotalExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubtotalExclTax]  DEFAULT ((0)),
	[OrderSubTotalDiscountInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTax]  DEFAULT ((0)),
	[OrderSubTotalDiscountExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTax]  DEFAULT ((0)),
	[OrderShippingInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderShippingInclTax]  DEFAULT ((0)),
	[OrderShippingExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderShippingExclTax]  DEFAULT ((0)),
	[PaymentMethodAdditionalFeeInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_PaymentMethodAdditionalFeeInclTax]  DEFAULT ((0)),
	[PaymentMethodAdditionalFeeExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_Order_PaymentMethodAdditionalFeeExclTax]  DEFAULT ((0)),
	[TaxRates] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRates]  DEFAULT (''),
	[OrderTax] [money] NOT NULL,
	[OrderTotal] [money] NOT NULL,
	[RefundedAmount] [money] NOT NULL CONSTRAINT [DF_Nop_Order_RefundedAmount]  DEFAULT ((0)),
	[OrderDiscount] [money] NOT NULL,
	[OrderSubtotalInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubtotalInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[OrderSubtotalExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubtotalExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[OrderSubTotalDiscountInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[OrderSubTotalDiscountExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[OrderShippingInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderShippingInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[OrderShippingExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderShippingExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[PaymentMethodAdditionalFeeInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_PaymentMethodAdditionalFeeInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[PaymentMethodAdditionalFeeExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_PaymentMethodAdditionalFeeExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[TaxRatesInCustomerCurrency] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRatesInCustomerCurrency]  DEFAULT (''),
	[OrderTaxInCustomerCurrency] [money] NOT NULL,
	[OrderTotalInCustomerCurrency] [money] NOT NULL,
	[OrderDiscountInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_Order_OrderDiscountInCustomerCurrency]  DEFAULT ((0)),
	[CustomerCurrencyCode] [nvarchar](5) NOT NULL,
	[CheckoutAttributeDescription] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_Order_CheckoutAttributeDescription]  DEFAULT (''),
	[CheckoutAttributesXML] [xml] NOT NULL CONSTRAINT [DF_Nop_Order_CheckoutAttributesXML]  DEFAULT (''),
	[OrderWeight] [decimal](18, 4) NOT NULL,
	[AffiliateID] [int] NOT NULL,
	[OrderStatusID] [int] NOT NULL,
	[AllowStoringCreditCardNumber] [bit] NOT NULL CONSTRAINT [DF_Nop_Order_AllowStoringCreditCardNumber]  DEFAULT ((0)),
	[CardType] [nvarchar](100) NOT NULL,
	[CardName] [nvarchar](1000) NOT NULL,
	[CardNumber] [nvarchar](100) NOT NULL,
	[MaskedCreditCardNumber] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Order_MaskedCreditCardNumber]  DEFAULT (''),
	[CardCVV2] [nvarchar](100) NOT NULL,
	[CardExpirationMonth] [nvarchar](100) NOT NULL,
	[CardExpirationYear] [nvarchar](100) NOT NULL,
	[PaymentMethodID] [int] NOT NULL,
	[PaymentMethodName] [nvarchar](100) NOT NULL,
	[AuthorizationTransactionID] [nvarchar](4000) NOT NULL,
	[AuthorizationTransactionCode] [nvarchar](4000) NOT NULL,
	[AuthorizationTransactionResult] [nvarchar](4000) NOT NULL,
	[CaptureTransactionID] [nvarchar](4000) NOT NULL,
	[CaptureTransactionResult] [nvarchar](4000) NOT NULL,
	[SubscriptionTransactionID] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_Order_SubscriptionTransactionID]  DEFAULT (''),
	[PurchaseOrderNumber] [nvarchar](100) NOT NULL,
	[PaymentStatusID] [int] NOT NULL,
	[PaidDate] [datetime] NULL,
	[BillingFirstName] [nvarchar](100) NOT NULL,
	[BillingLastName] [nvarchar](100) NOT NULL,
	[BillingPhoneNumber] [nvarchar](50) NOT NULL,
	[BillingEmail] [nvarchar](255) NOT NULL,
	[BillingFaxNumber] [nvarchar](50) NOT NULL,
	[BillingCompany] [nvarchar](100) NOT NULL,
	[BillingAddress1] [nvarchar](100) NOT NULL,
	[BillingAddress2] [nvarchar](100) NOT NULL,
	[BillingCity] [nvarchar](100) NOT NULL,
	[BillingStateProvince] [nvarchar](100) NOT NULL,
	[BillingStateProvinceID] [int] NOT NULL,
	[BillingZipPostalCode] [nvarchar](30) NOT NULL,
	[BillingCountry] [nvarchar](100) NOT NULL,
	[BillingCountryID] [int] NOT NULL,
	[ShippingStatusID] [int] NOT NULL,
	[ShippingFirstName] [nvarchar](100) NOT NULL,
	[ShippingLastName] [nvarchar](100) NOT NULL,
	[ShippingPhoneNumber] [nvarchar](50) NOT NULL,
	[ShippingEmail] [nvarchar](255) NOT NULL,
	[ShippingFaxNumber] [nvarchar](50) NOT NULL,
	[ShippingCompany] [nvarchar](100) NOT NULL,
	[ShippingAddress1] [nvarchar](100) NOT NULL,
	[ShippingAddress2] [nvarchar](100) NOT NULL,
	[ShippingCity] [nvarchar](100) NOT NULL,
	[ShippingStateProvince] [nvarchar](100) NOT NULL,
	[ShippingStateProvinceID] [int] NOT NULL,
	[ShippingZipPostalCode] [nvarchar](30) NOT NULL,
	[ShippingCountry] [nvarchar](100) NOT NULL,
	[ShippingCountryID] [int] NOT NULL,
	[ShippingMethod] [nvarchar](100) NOT NULL,
	[ShippingRateComputationMethodID] [int] NOT NULL,
	[ShippedDate] [datetime] NULL,
	[TrackingNumber] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Order_TrackingNumber]  DEFAULT (''),
	[DeliveryDate] [datetime] NULL,
	[VatNumber] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_Order_VatNumber]  DEFAULT (''),
	[Deleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK__Nop_Order__035179CE] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Order_AffiliateID] ON [dbo].[Nop_Order] 
(
	[AffiliateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Order_CreatedOn] ON [dbo].[Nop_Order] 
(
	[CreatedOn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Order_CustomerID] ON [dbo].[Nop_Order] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_EmailAccount](
	[EmailAccountId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[Host] [nvarchar](255) NOT NULL,
	[Port] [int] NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[EnableSSL] [bit] NOT NULL,
	[UseDefaultCredentials] [bit] NOT NULL,
 CONSTRAINT [PK_EmailAccount] PRIMARY KEY CLUSTERED 
(
	[EmailAccountId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_PaymentMethod](
	[PaymentMethodID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[VisibleName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[ConfigureTemplatePath] [nvarchar](500) NOT NULL,
	[UserTemplatePath] [nvarchar](500) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[SystemKeyword] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_PaymentMethod] PRIMARY KEY CLUSTERED 
(
	[PaymentMethodID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingMethod](
	[ShippingMethodID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_ShippingMethod] PRIMARY KEY CLUSTERED 
(
	[ShippingMethodID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductTemplate](
	[ProductTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TemplatePath] [nvarchar](200) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_ProductTemplate_PK] PRIMARY KEY CLUSTERED 
(
	[ProductTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ManufacturerTemplate](
	[ManufacturerTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TemplatePath] [nvarchar](200) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_ManufacturerTemplate_PK] PRIMARY KEY CLUSTERED 
(
	[ManufacturerTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CategoryTemplate](
	[CategoryTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TemplatePath] [nvarchar](200) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_CategoryTemplate_PK] PRIMARY KEY CLUSTERED 
(
	[CategoryTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Topic](
	[TopicID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[IsPasswordProtected] [bit] NOT NULL CONSTRAINT [DF_Nop_Topic_IsPasswordProtected]  DEFAULT ((0)),
	[Password] [nvarchar](200) NOT NULL CONSTRAINT [DF_Nop_Topic_Password]  DEFAULT (''),
	[IncludeInSitemap] [bit] NOT NULL CONSTRAINT [DF_Nop_Topic_IncludeInSitemap]  DEFAULT ((0)),
 CONSTRAINT [PK_Nop_Topic] PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Topic_Name] ON [dbo].[Nop_Topic] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Campaign](
	[CampaignID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Subject] [nvarchar](200) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Campaign] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Nop_Picture](
	[PictureID] [int] IDENTITY(1,1) NOT NULL,
	[PictureBinary] [varbinary](max) NOT NULL,
	[IsNew] [bit] NOT NULL,
	[MimeType] [nvarchar](20) NOT NULL CONSTRAINT [DF_Nop_Picture_MimeType]  DEFAULT (''),
 CONSTRAINT [Nop_Picture_PK] PRIMARY KEY CLUSTERED 
(
	[PictureID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Nop_Forums_Group](
	[ForumGroupID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_Group] PRIMARY KEY CLUSTERED 
(
	[ForumGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Group_DisplayOrder] ON [dbo].[Nop_Forums_Group] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Forums_Subscription](
	[SubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionGUID] [uniqueidentifier] NOT NULL,
	[UserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
	[TopicID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_Subscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Subscription_ForumID] ON [dbo].[Nop_Forums_Subscription] 
(
	[ForumID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Subscription_TopicID] ON [dbo].[Nop_Forums_Subscription] 
(
	[TopicID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Currency](
	[CurrencyID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CurrencyCode] [nvarchar](5) NOT NULL,
	[DisplayLocale] [nvarchar](50) NULL,
	[Rate] [decimal](18, 4) NOT NULL,
	[CustomFormatting] [nvarchar](50) NOT NULL,
	[Published] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_Currency_PK] PRIMARY KEY CLUSTERED 
(
	[CurrencyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Currency_CurrencyCode] ON [dbo].[Nop_Currency] 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Currency_DisplayOrder] ON [dbo].[Nop_Currency] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Forums_PrivateMessage](
	[PrivateMessageID] [int] IDENTITY(1,1) NOT NULL,
	[FromUserID] [int] NOT NULL,
	[ToUserID] [int] NOT NULL,
	[Subject] [nvarchar](450) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[IsDeletedByAuthor] [bit] NOT NULL CONSTRAINT [DF_Nop_Forums_PrivateMessage_IsDeletedByAuthor]  DEFAULT ((0)),
	[IsDeletedByRecipient] [bit] NOT NULL CONSTRAINT [DF_Nop_Forums_PrivateMessage_IsDeletedByRecipient]  DEFAULT ((0)),
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_PrivateMessage] PRIMARY KEY CLUSTERED 
(
	[PrivateMessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Discount](
	[DiscountID] [int] IDENTITY(1,1) NOT NULL,
	[DiscountTypeID] [int] NOT NULL,
	[DiscountRequirementID] [int] NOT NULL,
	[RequirementSpentAmount] [money] NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementSpentAmount]  DEFAULT ((0)),
	[RequirementBillingCountryIs] [int] NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementBillingCountryIs]  DEFAULT ((0)),
	[RequirementShippingCountryIs] [int] NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementShippingCountryIs]  DEFAULT ((0)),
	[DiscountLimitationID] [int] NOT NULL CONSTRAINT [DF_Nop_Discount_DiscountLimitationID]  DEFAULT ((0)),
	[LimitationTimes] [int] NOT NULL CONSTRAINT [DF_Nop_Discount_LimitationTimes]  DEFAULT ((1)),
	[Name] [nvarchar](100) NOT NULL,
	[UsePercentage] [bit] NOT NULL,
	[DiscountPercentage] [decimal](18, 4) NOT NULL,
	[DiscountAmount] [decimal](18, 4) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[RequiresCouponCode] [bit] NOT NULL,
	[CouponCode] [nvarchar](100) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [Nop_Discount_PK] PRIMARY KEY CLUSTERED 
(
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductTag](
	[ProductTagID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ProductCount] [int] NOT NULL,
 CONSTRAINT [PK_Nop_ProductTag] PRIMARY KEY CLUSTERED 
(
	[ProductTagID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CustomerAction](
	[CustomerActionID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemKeyword] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](1000) NOT NULL,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Nop_CustomerAction_DisplayOrder]  DEFAULT ((1)),
 CONSTRAINT [Nop_CustomerAction_PK] PRIMARY KEY CLUSTERED 
(
	[CustomerActionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Nop_Language](
	[LanguageId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[LanguageCulture] [varchar](20) NOT NULL,
	[FlagImageFileName] [nvarchar](50) NOT NULL CONSTRAINT [DF_Nop_Language_FlagImageFileName]  DEFAULT (''),
	[Published] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [Nop_Language_PK] PRIMARY KEY CLUSTERED 
(
	[LanguageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Nop_Language_DisplayOrder] ON [dbo].[Nop_Language] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_RecurringPayment](
	[RecurringPaymentID] [int] IDENTITY(1,1) NOT NULL,
	[InitialOrderID] [int] NOT NULL,
	[CycleLength] [int] NOT NULL,
	[CyclePeriod] [int] NOT NULL,
	[TotalCycles] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_RecurringPayment_PK] PRIMARY KEY CLUSTERED 
(
	[RecurringPaymentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CustomerSession](
	[CustomerSessionGUID] [uniqueidentifier] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[LastAccessed] [datetime] NOT NULL,
	[IsExpired] [bit] NOT NULL,
 CONSTRAINT [PK_Nop_CustomerSession] PRIMARY KEY CLUSTERED 
(
	[CustomerSessionGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_CustomerSession_CustomerID] ON [dbo].[Nop_CustomerSession] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SearchLog](
	[SearchLogID] [int] IDENTITY(1,1) NOT NULL,
	[SearchTerm] [nvarchar](100) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_SearchLog] PRIMARY KEY CLUSTERED 
(
	[SearchLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SpecificationAttribute](
	[SpecificationAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Nop_SpecificationAttribute_DisplayOrder]  DEFAULT ((1)),
 CONSTRAINT [PK_Nop_SpecificationAttribute] PRIMARY KEY CLUSTERED 
(
	[SpecificationAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Pricelist](
	[PricelistID] [int] IDENTITY(1,1) NOT NULL,
	[ExportModeID] [int] NOT NULL,
	[ExportTypeID] [int] NOT NULL,
	[AffiliateID] [int] NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[ShortName] [nvarchar](20) NOT NULL,
	[PricelistGuid] [nvarchar](40) NOT NULL,
	[CacheTime] [int] NOT NULL,
	[FormatLocalization] [nvarchar](5) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[AdminNotes] [nvarchar](500) NOT NULL,
	[Header] [nvarchar](500) NOT NULL,
	[Body] [nvarchar](500) NOT NULL,
	[Footer] [nvarchar](500) NOT NULL,
	[PriceAdjustmentTypeID] [int] NOT NULL CONSTRAINT [DF_Nop_Pricelist_PriceAdjustmentTypeID]  DEFAULT ((0)),
	[PriceAdjustment] [money] NOT NULL CONSTRAINT [DF_Nop_Pricelist_PriceAdjustment]  DEFAULT ((0)),
	[OverrideIndivAdjustment] [bit] NOT NULL CONSTRAINT [DF_Nop_Pricelist_OverrideIndivAdjustment]  DEFAULT ((0)),
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Pricelist] PRIMARY KEY CLUSTERED 
(
	[PricelistID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CheckoutAttribute](
	[CheckoutAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TextPrompt] [nvarchar](300) NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[ShippableProductRequired] [bit] NOT NULL,
	[IsTaxExempt] [bit] NOT NULL,
	[TaxCategoryID] [int] NOT NULL,
	[AttributeControlTypeID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_CheckoutAttribute] PRIMARY KEY CLUSTERED 
(
	[CheckoutAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_NewsLetterSubscription](
	[NewsLetterSubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[NewsLetterSubscriptionGuid] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_NewsLetterSubscription] PRIMARY KEY CLUSTERED 
(
	[NewsLetterSubscriptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CustomerRole_Discount_Mapping](
	[CustomerRoleID] [int] NOT NULL,
	[DiscountID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_CustomerRole_Discount_Mapping] PRIMARY KEY CLUSTERED 
(
	[CustomerRoleID] ASC,
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ACL](
	[ACLID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerActionID] [int] NOT NULL,
	[CustomerRoleID] [int] NOT NULL,
	[Allow] [bit] NOT NULL,
 CONSTRAINT [Nop_ACL_PK] PRIMARY KEY CLUSTERED 
(
	[ACLID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ACL_Unique] UNIQUE NONCLUSTERED 
(
	[CustomerActionID] ASC,
	[CustomerRoleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CustomerRole_ProductPrice](
	[CustomerRoleProductPriceID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerRoleID] [int] NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_Nop_CustomerRole_ProductPrice] PRIMARY KEY CLUSTERED 
(
	[CustomerRoleProductPriceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ACLPerObject](
	[ACLPerObjectId] [int] IDENTITY(1,1) NOT NULL,
	[ObjectId] [int] NOT NULL,
	[ObjectTypeId] [int] NOT NULL,
	[CustomerRoleId] [int] NOT NULL,
	[Deny] [bit] NOT NULL,
 CONSTRAINT [PK_ACLPerObject] PRIMARY KEY CLUSTERED 
(
	[ACLPerObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Customer_CustomerRole_Mapping](
	[CustomerID] [int] NOT NULL,
	[CustomerRoleID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_Customer_CustomerRole_Mapping] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[CustomerRoleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_StateProvince](
	[StateProvinceID] [int] IDENTITY(1,1) NOT NULL,
	[CountryID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Abbreviation] [nvarchar](30) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_StateProvince] PRIMARY KEY CLUSTERED 
(
	[StateProvinceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_StateProvince_CountryID] ON [dbo].[Nop_StateProvince] 
(
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries](
	[PaymentMethodID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_PaymentMethod_RestrictedCountries] PRIMARY KEY CLUSTERED 
(
	[PaymentMethodID] ASC,
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_TaxRate](
	[TaxRateID] [int] IDENTITY(1,1) NOT NULL,
	[TaxCategoryID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
	[StateProvinceID] [int] NOT NULL,
	[Zip] [nvarchar](50) NOT NULL,
	[Percentage] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_Nop_TaxRate] PRIMARY KEY CLUSTERED 
(
	[TaxRateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingByWeightAndCountry](
	[ShippingByWeightAndCountryID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingMethodID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
	[From] [decimal](18, 2) NOT NULL,
	[To] [decimal](18, 2) NOT NULL,
	[UsePercentage] [bit] NOT NULL,
	[ShippingChargePercentage] [decimal](18, 2) NOT NULL,
	[ShippingChargeAmount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Nop_ShippingByWeightAndCountry] PRIMARY KEY CLUSTERED 
(
	[ShippingByWeightAndCountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingMethod_RestrictedCountries](
	[ShippingMethodID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_ShippingMethod_RestrictedCountries] PRIMARY KEY CLUSTERED 
(
	[ShippingMethodID] ASC,
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Product_SpecificationAttribute_Mapping](
	[ProductSpecificationAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[SpecificationAttributeOptionID] [int] NOT NULL,
	[AllowFiltering] [bit] NOT NULL CONSTRAINT [DF_Nop_Product_SpecificationAttribute_Mapping_AllowFiltering]  DEFAULT ((0)),
	[ShowOnProductPage] [bit] NOT NULL CONSTRAINT [DF_Nop_Product_SpecificationAttribute_Mapping_ShowOnProductPage]  DEFAULT ((1)),
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Nop_Product_SpecificationAttribute_Mapping_DisplayOrder]  DEFAULT ((1)),
 CONSTRAINT [PK_Nop_Product_SpecificationAttribute_Mapping] PRIMARY KEY CLUSTERED 
(
	[ProductSpecificationAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SpecificationAttributeOptionLocalized](
	[SpecificationAttributeOptionLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[SpecificationAttributeOptionID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Nop_SpecificationAttributeOptionLocalized] PRIMARY KEY CLUSTERED 
(
	[SpecificationAttributeOptionLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_SpecificationAttributeOptionLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[SpecificationAttributeOptionID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductAttributeLocalized](
	[ProductAttributeLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[ProductAttributeID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](400) NOT NULL,
 CONSTRAINT [PK_Nop_ProductAttributeLocalized] PRIMARY KEY CLUSTERED 
(
	[ProductAttributeLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ProductAttributeLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[ProductAttributeID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariant_ProductAttribute_Mapping](
	[ProductVariantAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[ProductAttributeID] [int] NOT NULL,
	[TextPrompt] [nvarchar](200) NOT NULL CONSTRAINT [DF_Nop_ProductVariant_ProductAttribute_Mapping_Attributes]  DEFAULT (''),
	[IsRequired] [bit] NOT NULL,
	[AttributeControlTypeID] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_ProductAttribute_Mapping_ControlTypeID]  DEFAULT ((1)),
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_ProductVariant_ProductAttribute_Mapping] PRIMARY KEY CLUSTERED 
(
	[ProductVariantAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ProductVariant_ProductAttribute_Mapping_ProductVariantID] ON [dbo].[Nop_ProductVariant_ProductAttribute_Mapping] 
(
	[ProductVariantID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CustomerAttribute](
	[CustomerAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_Nop_CustomerAttribute] PRIMARY KEY CLUSTERED 
(
	[CustomerAttributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_CustomerAttribute_Unique] UNIQUE NONCLUSTERED 
(
	[CustomerId] ASC,
	[Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_CustomerAttribute_CustomerId] ON [dbo].[Nop_CustomerAttribute] 
(
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_RewardPointsHistory](
	[RewardPointsHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[Points] [int] NOT NULL,
	[PointsBalance] [int] NOT NULL,
	[UsedAmount] [money] NOT NULL,
	[UsedAmountInCustomerCurrency] [money] NOT NULL,
	[CustomerCurrencyCode] [nvarchar](5) NOT NULL,
	[Message] [nvarchar](1000) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_RewardPointsHistory_PK] PRIMARY KEY CLUSTERED 
(
	[RewardPointsHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Address](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[IsBillingAddress] [bit] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[FaxNumber] [nvarchar](50) NOT NULL,
	[Company] [nvarchar](100) NOT NULL,
	[Address1] [nvarchar](100) NOT NULL,
	[Address2] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[StateProvinceID] [int] NOT NULL,
	[ZipPostalCode] [nvarchar](30) NOT NULL,
	[CountryID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Nop_Address_UpdatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [Nop_Address_PK] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Address_CustomerID] ON [dbo].[Nop_Address] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ActivityLog](
	[ActivityLogID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityLogTypeID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Comment] [nvarchar](4000) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ActivityLog] PRIMARY KEY CLUSTERED 
(
	[ActivityLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_BlogPost](
	[BlogPostID] [int] IDENTITY(1,1) NOT NULL,
	[LanguageID] [int] NOT NULL,
	[BlogPostTitle] [nvarchar](200) NOT NULL,
	[BlogPostBody] [nvarchar](max) NOT NULL,
	[BlogPostAllowComments] [bit] NOT NULL,
	[Tags] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_BlogPost_Tags]  DEFAULT (''),
	[CreatedByID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_BlogPost] PRIMARY KEY CLUSTERED 
(
	[BlogPostID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_BlogPost_LanguageID] ON [dbo].[Nop_BlogPost] 
(
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_GiftCardUsageHistory](
	[GiftCardUsageHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[GiftCardID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[UsedValue] [money] NOT NULL,
	[UsedValueInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_GiftCardUsageHistory_UsedValueInCustomerCurrency]  DEFAULT ((0)),
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_GiftCardUsageHistory_PK] PRIMARY KEY CLUSTERED 
(
	[GiftCardUsageHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ReturnRequest](
	[ReturnRequestId] [int] IDENTITY(1,1) NOT NULL,
	[OrderProductVariantId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ReasonForReturn] [nvarchar](400) NOT NULL,
	[RequestedAction] [nvarchar](400) NOT NULL,
	[CustomerComments] [nvarchar](max) NOT NULL,
	[StaffNotes] [nvarchar](max) NOT NULL,
	[ReturnStatusId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ReturnRequest] PRIMARY KEY CLUSTERED 
(
	[ReturnRequestId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_DiscountUsageHistory](
	[DiscountUsageHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[DiscountID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_DiscountUsageHistory_PK] PRIMARY KEY CLUSTERED 
(
	[DiscountUsageHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductRating](
	[ProductRatingID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[RatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ProductRating] PRIMARY KEY CLUSTERED 
(
	[ProductRatingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductReview](
	[ProductReviewID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[IPAddress] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_ProductReview_IPAddress]  DEFAULT (''),
	[Title] [nvarchar](1000) NOT NULL,
	[ReviewText] [nvarchar](max) NOT NULL,
	[Rating] [int] NOT NULL,
	[HelpfulYesTotal] [int] NOT NULL,
	[HelpfulNoTotal] [int] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ProductReview] PRIMARY KEY CLUSTERED 
(
	[ProductReviewID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ProductReview_ProductID] ON [dbo].[Nop_ProductReview] 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_PollVotingRecord](
	[PollVotingRecordID] [int] IDENTITY(1,1) NOT NULL,
	[PollAnswerID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_PollVotingRecord] PRIMARY KEY CLUSTERED 
(
	[PollVotingRecordID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_BlogComment](
	[BlogCommentID] [int] IDENTITY(1,1) NOT NULL,
	[BlogPostID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[IPAddress] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_BlogComment_IPAddress]  DEFAULT (''),
	[CommentText] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_BlogComment] PRIMARY KEY CLUSTERED 
(
	[BlogCommentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_BlogComment_BlogPostID] ON [dbo].[Nop_BlogComment] 
(
	[BlogPostID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariantAttributeValueLocalized](
	[ProductVariantAttributeValueLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantAttributeValueID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_ProductVariantAttributeValueLocalized] PRIMARY KEY CLUSTERED 
(
	[ProductVariantAttributeValueLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ProductVariantAttributeValueLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[ProductVariantAttributeValueID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Forums_Post](
	[PostID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[IPAddress] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_Post] PRIMARY KEY CLUSTERED 
(
	[PostID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Post_TopicID] ON [dbo].[Nop_Forums_Post] 
(
	[TopicID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Post_UserID] ON [dbo].[Nop_Forums_Post] 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CheckoutAttributeValueLocalized](
	[CheckoutAttributeValueLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutAttributeValueID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_CheckoutAttributeValueLocalized] PRIMARY KEY CLUSTERED 
(
	[CheckoutAttributeValueLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_CheckoutAttributeValueLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[CheckoutAttributeValueID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_GiftCard](
	[GiftCardID] [int] IDENTITY(1,1) NOT NULL,
	[PurchasedOrderProductVariantID] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[IsGiftCardActivated] [bit] NOT NULL,
	[GiftCardCouponCode] [nvarchar](100) NOT NULL,
	[RecipientName] [nvarchar](100) NOT NULL,
	[RecipientEmail] [nvarchar](100) NOT NULL,
	[SenderName] [nvarchar](100) NOT NULL,
	[SenderEmail] [nvarchar](100) NOT NULL,
	[Message] [nvarchar](4000) NOT NULL,
	[IsRecipientNotified] [bit] NOT NULL CONSTRAINT [DF_Nop_GiftCard_IsRecipientNotified]  DEFAULT ((0)),
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_GiftCard_PK] PRIMARY KEY CLUSTERED 
(
	[GiftCardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_MessageTemplateLocalized](
	[MessageTemplateLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[MessageTemplateID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[BCCEmailAddresses] [nvarchar](200) NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_BCCEmailAddresses]  DEFAULT (''),
	[Subject] [nvarchar](200) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_IsActive]  DEFAULT ((1)),
	[EmailAccountId] [int] NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_EmailAccountId]  DEFAULT ((0)),
 CONSTRAINT [PK_Nop_MessageTemplateLocalized] PRIMARY KEY CLUSTERED 
(
	[MessageTemplateLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Nop_MessageTemplateLocalized] ON [dbo].[Nop_MessageTemplateLocalized] 
(
	[LanguageID] ASC,
	[MessageTemplateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_MessageTemplateLocalized_LanguageID] ON [dbo].[Nop_MessageTemplateLocalized] 
(
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_OrderProductVariant](
	[OrderProductVariantID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[UnitPriceInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_UnitPriceInclTax]  DEFAULT ((0)),
	[UnitPriceExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_UnitPriceExclTax]  DEFAULT ((0)),
	[PriceInclTax] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_PriceInclTax]  DEFAULT ((0)),
	[PriceExclTax] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_PriceExclTax]  DEFAULT ((0)),
	[UnitPriceInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_UnitPriceInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[UnitPriceExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_UnitPriceExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[PriceInclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_PriceInclTaxInCustomerCurrency]  DEFAULT ((0)),
	[PriceExclTaxInCustomerCurrency] [money] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_PriceExclTaxInCustomerCurrency]  DEFAULT ((0)),
	[AttributeDescription] [nvarchar](4000) NOT NULL,
	[AttributesXML] [xml] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_AttributesXML]  DEFAULT (''),
	[Quantity] [int] NOT NULL,
	[DiscountAmountInclTax] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_DiscountAmountInclTax]  DEFAULT ((0)),
	[DiscountAmountExclTax] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_DiscountAmountExclTax]  DEFAULT ((0)),
	[DownloadCount] [int] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_DownloadCount]  DEFAULT ((0)),
	[OrderProductVariantGUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_OrderProductVariantGUID]  DEFAULT (newid()),
	[IsDownloadActivated] [bit] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_IsDownloadActivated]  DEFAULT ((0)),
	[LicenseDownloadID] [int] NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_LicenseDownloadID]  DEFAULT ((0)),
 CONSTRAINT [Nop_OrderProductVariant_PK] PRIMARY KEY CLUSTERED 
(
	[OrderProductVariantID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_OrderProductVariant_OrderID] ON [dbo].[Nop_OrderProductVariant] 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_OrderNote](
	[OrderNoteID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[Note] [nvarchar](4000) NOT NULL,
	[DisplayToCustomer] [bit] NOT NULL CONSTRAINT [DF_Nop_OrderNote_DisplayToCustomer]  DEFAULT ((0)),
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_OrderNote] PRIMARY KEY CLUSTERED 
(
	[OrderNoteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_OrderNote_OrderID] ON [dbo].[Nop_OrderNote] 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariant_Discount_Mapping](
	[ProductVariantID] [int] NOT NULL,
	[DiscountID] [int] NOT NULL,
 CONSTRAINT [PKNop_ProductVariant_Discount_Mapping] PRIMARY KEY CLUSTERED 
(
	[ProductVariantID] ASC,
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariantAttributeCombination](
	[ProductVariantAttributeCombinationID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[AttributesXML] [xml] NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[AllowOutOfStockOrders] [bit] NOT NULL,
 CONSTRAINT [Nop_ProductVariantAttributeCombination_PK] PRIMARY KEY CLUSTERED 
(
	[ProductVariantAttributeCombinationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_DiscountRestriction](
	[ProductVariantID] [int] NOT NULL,
	[DiscountID] [int] NOT NULL,
 CONSTRAINT [Nop_DiscountRestriction_PK] PRIMARY KEY CLUSTERED 
(
	[ProductVariantID] ASC,
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariant_Pricelist_Mapping](
	[ProductVariantPricelistID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[PricelistID] [int] NOT NULL,
	[PriceAdjustmentTypeID] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_Pricelist_Mapping_PriceAdjustmentTypeID]  DEFAULT ((0)),
	[PriceAdjustment] [money] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_Pricelist_Mapping_PriceAdjustment]  DEFAULT ((0)),
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ProductVariant_Pricelist_Mapping] PRIMARY KEY CLUSTERED 
(
	[ProductVariantPricelistID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_TierPrice](
	[TierPriceID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [Nop_TierPrice_PK] PRIMARY KEY CLUSTERED 
(
	[TierPriceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_TierPrice_ProductVariantID] ON [dbo].[Nop_TierPrice] 
(
	[ProductVariantID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariantLocalized](
	[ProductVariantLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Nop_ProductVariantLocalized] PRIMARY KEY CLUSTERED 
(
	[ProductVariantLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ProductVariantLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[ProductVariantID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShoppingCartItem](
	[ShoppingCartItemID] [int] IDENTITY(1,1) NOT NULL,
	[ShoppingCartTypeID] [int] NOT NULL,
	[CustomerSessionGUID] [uniqueidentifier] NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[AttributesXML] [xml] NOT NULL CONSTRAINT [DF_Nop_ShoppingCartItem_AttributesXML]  DEFAULT (''),
	[CustomerEnteredPrice] [money] NOT NULL CONSTRAINT [DF_Nop_ShoppingCartItem_CustomerEnteredPrice]  DEFAULT ((0)),
	[Quantity] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_ShoppingCart_PK] PRIMARY KEY CLUSTERED 
(
	[ShoppingCartItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ShoppingCartItem_ShoppingCartTypeID_CustomerSessionGUID] ON [dbo].[Nop_ShoppingCartItem] 
(
	[ShoppingCartTypeID] ASC,
	[CustomerSessionGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CrossSellProduct](
	[CrossSellProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId1] [int] NOT NULL,
	[ProductId2] [int] NOT NULL,
 CONSTRAINT [PK_CrossSellProduct] PRIMARY KEY CLUSTERED 
(
	[CrossSellProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_CrossSellProduct_Unique] UNIQUE NONCLUSTERED 
(
	[ProductId1] ASC,
	[ProductId2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductTag_Product_Mapping](
	[ProductTagID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
 CONSTRAINT [Nop_ProductTag_Product_Mapping_PK] PRIMARY KEY CLUSTERED 
(
	[ProductTagID] ASC,
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductLocalized](
	[ProductLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ShortDescription] [nvarchar](max) NOT NULL,
	[FullDescription] [nvarchar](max) NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_ProductLocalized] PRIMARY KEY CLUSTERED 
(
	[ProductLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ProductLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[ProductID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_RelatedProduct](
	[RelatedProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID1] [int] NOT NULL,
	[ProductID2] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_RelatedProduct] PRIMARY KEY CLUSTERED 
(
	[RelatedProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_RelatedProduct_Unique] UNIQUE NONCLUSTERED 
(
	[ProductID1] ASC,
	[ProductID2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_RelatedProduct_ProductID1] ON [dbo].[Nop_RelatedProduct] 
(
	[ProductID1] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Product_Category_Mapping](
	[ProductCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[IsFeaturedProduct] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_Product_Category_Mapping_1] PRIMARY KEY CLUSTERED 
(
	[ProductCategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_Product_Category_Mapping_Unique] UNIQUE NONCLUSTERED 
(
	[ProductID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariant](
	[ProductVariantId] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[SKU] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[AdminComment] [nvarchar](4000) NOT NULL,
	[ManufacturerPartNumber] [nvarchar](100) NOT NULL,
	[IsGiftCard] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_IsGiftCard]  DEFAULT ((0)),
	[GiftCardType] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_GiftCardType]  DEFAULT ((0)),
	[IsDownload] [bit] NOT NULL,
	[DownloadID] [int] NOT NULL,
	[UnlimitedDownloads] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_UnlimitedDownloads]  DEFAULT ((1)),
	[MaxNumberOfDownloads] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_MaxNumberOfDownloads]  DEFAULT ((0)),
	[DownloadExpirationDays] [int] NULL,
	[DownloadActivationType] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DownloadActivationType]  DEFAULT ((1)),
	[HasSampleDownload] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_HasSampleDownload]  DEFAULT ((0)),
	[SampleDownloadID] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_SampleDownloadID]  DEFAULT ((0)),
	[HasUserAgreement] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_HasUserAgreement]  DEFAULT ((0)),
	[UserAgreementText] [nvarchar](max) NOT NULL CONSTRAINT [DF_Nop_ProductVariant_UserAgreementText]  DEFAULT (''),
	[IsRecurring] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_IsRecurring]  DEFAULT ((0)),
	[CycleLength] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CycleLength]  DEFAULT ((1)),
	[CyclePeriod] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CyclePeriod]  DEFAULT ((0)),
	[TotalCycles] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_TotalCycles]  DEFAULT ((1)),
	[IsShipEnabled] [bit] NOT NULL,
	[IsFreeShipping] [bit] NOT NULL,
	[AdditionalShippingCharge] [money] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_AdditionalShippingCharge]  DEFAULT ((0)),
	[IsTaxExempt] [bit] NOT NULL,
	[TaxCategoryID] [int] NOT NULL,
	[ManageInventory] [int] NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[DisplayStockAvailability] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DisplayStockAvailability]  DEFAULT ((1)),
	[DisplayStockQuantity] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DisplayStockQuantity]  DEFAULT ((0)),
	[MinStockQuantity] [int] NOT NULL,
	[LowStockActivityID] [int] NOT NULL,
	[NotifyAdminForQuantityBelow] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_NotifyAdminForQuantityBelow]  DEFAULT ((1)),
	[Backorders] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_Backorders]  DEFAULT ((0)),
	[OrderMinimumQuantity] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_OrderMinimumQuantity]  DEFAULT ((1)),
	[OrderMaximumQuantity] [int] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_OrderMaximumQuantity]  DEFAULT ((10000)),
	[WarehouseID] [int] NOT NULL,
	[DisableBuyButton] [bit] NOT NULL,
	[CallForPrice] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CallForPrice]  DEFAULT ((0)),
	[Price] [money] NOT NULL,
	[OldPrice] [money] NOT NULL,
	[ProductCost] [money] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_ProductCost]  DEFAULT ((0)),
	[CustomerEntersPrice] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CustomerEntersPrice]  DEFAULT ((0)),
	[MinimumCustomerEnteredPrice] [money] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_MinimumCustomerEnteredPrice]  DEFAULT ((0)),
	[MaximumCustomerEnteredPrice] [money] NOT NULL CONSTRAINT [DF_Nop_ProductVariant_MaximumCustomerEnteredPrice]  DEFAULT ((1000)),
	[Weight] [decimal](18, 4) NOT NULL,
	[Length] [decimal](18, 4) NOT NULL,
	[Width] [decimal](18, 4) NOT NULL,
	[Height] [decimal](18, 4) NOT NULL,
	[PictureID] [int] NOT NULL,
	[AvailableStartDateTime] [datetime] NULL,
	[AvailableEndDateTime] [datetime] NULL,
	[Published] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_ProductVariant_PK] PRIMARY KEY CLUSTERED 
(
	[ProductVariantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ProductVariant_DisplayOrder] ON [dbo].[Nop_ProductVariant] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ProductVariant_ProductID] ON [dbo].[Nop_ProductVariant] 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductPicture](
	[ProductPictureID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[PictureID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_ProductPicture] PRIMARY KEY CLUSTERED 
(
	[ProductPictureID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Product_Manufacturer_Mapping](
	[ProductManufacturerID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[ManufacturerID] [int] NOT NULL,
	[IsFeaturedProduct] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_Product_Manufacturer_Mapping_1] PRIMARY KEY CLUSTERED 
(
	[ProductManufacturerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_Product_Manufacturer_Mapping_Unique] UNIQUE NONCLUSTERED 
(
	[ProductID] ASC,
	[ManufacturerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ManufacturerLocalized](
	[ManufacturerLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[ManufacturerID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_ManufacturerLocalized] PRIMARY KEY CLUSTERED 
(
	[ManufacturerLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_ManufacturerLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[ManufacturerID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingByTotal](
	[ShippingByTotalID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingMethodID] [int] NOT NULL,
	[From] [decimal](18, 2) NOT NULL,
	[To] [decimal](18, 2) NOT NULL,
	[UsePercentage] [bit] NOT NULL,
	[ShippingChargePercentage] [decimal](18, 2) NOT NULL,
	[ShippingChargeAmount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Nop_ShippingByTotal] PRIMARY KEY CLUSTERED 
(
	[ShippingByTotalID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ShippingByWeight](
	[ShippingByWeightID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingMethodID] [int] NOT NULL,
	[From] [decimal](18, 2) NOT NULL,
	[To] [decimal](18, 2) NOT NULL,
	[UsePercentage] [bit] NOT NULL,
	[ShippingChargePercentage] [decimal](18, 2) NOT NULL,
	[ShippingChargeAmount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Nop_ShippingByWeight] PRIMARY KEY CLUSTERED 
(
	[ShippingByWeightID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ShortDescription] [nvarchar](max) NOT NULL,
	[FullDescription] [nvarchar](max) NOT NULL,
	[AdminComment] [nvarchar](max) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[ShowOnHomePage] [bit] NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
	[AllowCustomerReviews] [bit] NOT NULL,
	[AllowCustomerRatings] [bit] NOT NULL CONSTRAINT [DF_Nop_Product_AllowCustomerRatings]  DEFAULT ((1)),
	[RatingSum] [int] NOT NULL,
	[TotalRatingVotes] [int] NOT NULL,
	[Published] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_Product_PK] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CategoryLocalized](
	[CategoryLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_CategoryLocalized] PRIMARY KEY CLUSTERED 
(
	[CategoryLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_CategoryLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[CategoryID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Category_Discount_Mapping](
	[CategoryID] [int] NOT NULL,
	[DiscountID] [int] NOT NULL,
 CONSTRAINT [PK_Nop_Category_Discount_Mapping] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC,
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Manufacturer](
	[ManufacturerID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
	[PictureID] [int] NOT NULL,
	[PageSize] [int] NOT NULL CONSTRAINT [DF_Nop_Manufacturer_PageSize]  DEFAULT ((10)),
	[PriceRanges] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_Manufacturer_PriceRanges]  DEFAULT (''),
	[Published] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Manufacturer] PRIMARY KEY CLUSTERED 
(
	[ManufacturerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Manufacturer_DisplayOrder] ON [dbo].[Nop_Manufacturer] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Category](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[MetaKeywords] [nvarchar](400) NOT NULL,
	[MetaDescription] [nvarchar](4000) NOT NULL,
	[MetaTitle] [nvarchar](400) NOT NULL,
	[SEName] [nvarchar](100) NOT NULL,
	[ParentCategoryID] [int] NOT NULL,
	[PictureID] [int] NOT NULL,
	[PageSize] [int] NOT NULL CONSTRAINT [DF_Nop_Category_PageSize]  DEFAULT ((10)),
	[PriceRanges] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_Category_PriceRanges]  DEFAULT (''),
	[ShowOnHomePage] [bit] NOT NULL CONSTRAINT [DF_Nop_Category_ShowOnHomePage]  DEFAULT ((0)),
	[Published] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Category_DisplayOrder] ON [dbo].[Nop_Category] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Category_ParentCategoryID] ON [dbo].[Nop_Category] 
(
	[ParentCategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_TopicLocalized](
	[TopicLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_CreatedOn]  DEFAULT (getutcdate()),
	[UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_UpdatedOn]  DEFAULT (getutcdate()),
	[MetaTitle] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaTitle]  DEFAULT (''),
	[MetaKeywords] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaKeywords]  DEFAULT (''),
	[MetaDescription] [nvarchar](4000) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaDescription]  DEFAULT (''),
 CONSTRAINT [PK_Nop_TopicLocalized] PRIMARY KEY CLUSTERED 
(
	[TopicLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_TopicLocalized_LanguageID] ON [dbo].[Nop_TopicLocalized] 
(
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Forums_Forum](
	[ForumID] [int] IDENTITY(1,1) NOT NULL,
	[ForumGroupID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[NumTopics] [int] NOT NULL,
	[NumPosts] [int] NOT NULL,
	[LastTopicID] [int] NOT NULL,
	[LastPostID] [int] NOT NULL,
	[LastPostUserID] [int] NOT NULL,
	[LastPostTime] [datetime] NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_Forum] PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Forum_DisplayOrder] ON [dbo].[Nop_Forums_Forum] 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Forum_ForumGroupID] ON [dbo].[Nop_Forums_Forum] 
(
	[ForumGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Forums_Topic](
	[TopicID] [int] IDENTITY(1,1) NOT NULL,
	[ForumID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[TopicTypeID] [int] NOT NULL,
	[Subject] [nvarchar](450) NOT NULL,
	[NumPosts] [int] NOT NULL,
	[Views] [int] NOT NULL,
	[LastPostID] [int] NOT NULL,
	[LastPostUserID] [int] NOT NULL,
	[LastPostTime] [datetime] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_Forums_Topic] PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_Forums_Topic_ForumID] ON [dbo].[Nop_Forums_Topic] 
(
	[ForumID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductReviewHelpfulness](
	[ProductReviewHelpfulnessID] [int] IDENTITY(1,1) NOT NULL,
	[ProductReviewID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[WasHelpful] [bit] NOT NULL,
 CONSTRAINT [PK_Nop_ProductReviewHelpfulness] PRIMARY KEY CLUSTERED 
(
	[ProductReviewHelpfulnessID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_Poll](
	[PollID] [int] IDENTITY(1,1) NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[SystemKeyword] [nvarchar](400) NOT NULL CONSTRAINT [DF_Nop_Poll_SystemKeyword]  DEFAULT (''),
	[Published] [bit] NOT NULL,
	[ShowOnHomePage] [bit] NOT NULL CONSTRAINT [DF_Nop_Poll_ShowOnHomePage]  DEFAULT ((0)),
	[DisplayOrder] [int] NOT NULL,
	[StartDate] [datetime] NULL CONSTRAINT [DF_Nop_Poll_StartDate]  DEFAULT (NULL),
	[EndDate] [datetime] NULL CONSTRAINT [DF_Nop_Poll_EndDate]  DEFAULT (NULL),
 CONSTRAINT [PK_Nop_Poll] PRIMARY KEY CLUSTERED 
(
	[PollID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_News](
	[NewsID] [int] IDENTITY(1,1) NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Title] [nvarchar](1000) NOT NULL,
	[Short] [nvarchar](4000) NOT NULL,
	[Full] [nvarchar](max) NOT NULL,
	[Published] [bit] NOT NULL,
	[AllowComments] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_News] PRIMARY KEY CLUSTERED 
(
	[NewsID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_News_LanguageID] ON [dbo].[Nop_News] 
(
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_LocaleStringResource](
	[LocaleStringResourceID] [int] IDENTITY(1,1) NOT NULL,
	[LanguageID] [int] NOT NULL,
	[ResourceName] [nvarchar](200) NOT NULL,
	[ResourceValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Nop_LocaleStringResource] PRIMARY KEY CLUSTERED 
(
	[LocaleStringResourceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Nop_LocaleStringResource] ON [dbo].[Nop_LocaleStringResource] 
(
	[LanguageID] ASC,
	[ResourceName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_LocaleStringResource_LanguageID] ON [dbo].[Nop_LocaleStringResource] 
(
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SpecificationAttributeLocalized](
	[SpecificationAttributeLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[SpecificationAttributeID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Nop_SpecificationAttributeLocalized] PRIMARY KEY CLUSTERED 
(
	[SpecificationAttributeLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_SpecificationAttributeLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[SpecificationAttributeID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CheckoutAttributeLocalized](
	[CheckoutAttributeLocalizedID] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutAttributeID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TextPrompt] [nvarchar](300) NOT NULL,
 CONSTRAINT [PK_Nop_CheckoutAttributeLocalized] PRIMARY KEY CLUSTERED 
(
	[CheckoutAttributeLocalizedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Nop_CheckoutAttributeLocalized_Unique1] UNIQUE NONCLUSTERED 
(
	[CheckoutAttributeID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_PollAnswer](
	[PollAnswerID] [int] IDENTITY(1,1) NOT NULL,
	[PollID] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Count] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_PollAnswers] PRIMARY KEY CLUSTERED 
(
	[PollAnswerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_PollAnswer_PollID] ON [dbo].[Nop_PollAnswer] 
(
	[PollID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_RecurringPaymentHistory](
	[RecurringPaymentHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[RecurringPaymentID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_RecurringPaymentHistory_PK] PRIMARY KEY CLUSTERED 
(
	[RecurringPaymentHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_SpecificationAttributeOption](
	[SpecificationAttributeOptionID] [int] IDENTITY(1,1) NOT NULL,
	[SpecificationAttributeID] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Nop_SpecificationAttributeOption_DisplayOrder]  DEFAULT ((1)),
 CONSTRAINT [PK_Nop_SpecificationAttributeOption] PRIMARY KEY CLUSTERED 
(
	[SpecificationAttributeOptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_NewsComment](
	[NewsCommentID] [int] IDENTITY(1,1) NOT NULL,
	[NewsID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[IPAddress] [nvarchar](100) NOT NULL CONSTRAINT [DF_Nop_NewsComment_IPAddress]  DEFAULT (''),
	[Title] [nvarchar](1000) NOT NULL,
	[Comment] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_NewsComment] PRIMARY KEY CLUSTERED 
(
	[NewsCommentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_NewsComment_NewsID] ON [dbo].[Nop_NewsComment] 
(
	[NewsID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_CheckoutAttributeValue](
	[CheckoutAttributeValueID] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutAttributeID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[PriceAdjustment] [money] NOT NULL,
	[WeightAdjustment] [decimal](18, 4) NOT NULL,
	[IsPreSelected] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_CheckoutAttributeValue] PRIMARY KEY CLUSTERED 
(
	[CheckoutAttributeValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Nop_ProductVariantAttributeValue](
	[ProductVariantAttributeValueID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantAttributeID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[PriceAdjustment] [money] NOT NULL,
	[WeightAdjustment] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Nop_ProductVariantAttributeValue_WeightAdjustment]  DEFAULT ((0)),
	[IsPreSelected] [bit] NOT NULL CONSTRAINT [DF_Nop_ProductVariantAttributeValue_IsPreSelected]  DEFAULT ((0)),
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Nop_ProductVariantAttributeValue] PRIMARY KEY CLUSTERED 
(
	[ProductVariantAttributeValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Nop_ProductVariantAttributeValue_ProductVariantAttributeID] ON [dbo].[Nop_ProductVariantAttributeValue] 
(
	[ProductVariantAttributeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem]  WITH CHECK ADD  CONSTRAINT [CK_Nop_ShoppingCart_Quantity] CHECK  (([quantity]>(0)))
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem] CHECK CONSTRAINT [CK_Nop_ShoppingCart_Quantity]
GO
ALTER TABLE [dbo].[Nop_ACL]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACL_Nop_CustomerAction] FOREIGN KEY([CustomerActionID])
REFERENCES [dbo].[Nop_CustomerAction] ([CustomerActionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ACL] CHECK CONSTRAINT [FK_Nop_ACL_Nop_CustomerAction]
GO
ALTER TABLE [dbo].[Nop_ACL]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACL_Nop_CustomerRole] FOREIGN KEY([CustomerRoleID])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ACL] CHECK CONSTRAINT [FK_Nop_ACL_Nop_CustomerRole]
GO
ALTER TABLE [dbo].[Nop_ACLPerObject]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACLPerObject_Nop_CustomerRole] FOREIGN KEY([CustomerRoleId])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ACLPerObject] CHECK CONSTRAINT [FK_Nop_ACLPerObject_Nop_CustomerRole]
GO
ALTER TABLE [dbo].[Nop_ActivityLog]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ActivityLog_Nop_ActivityLogType] FOREIGN KEY([ActivityLogTypeID])
REFERENCES [dbo].[Nop_ActivityLogType] ([ActivityLogTypeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ActivityLog] CHECK CONSTRAINT [FK_Nop_ActivityLog_Nop_ActivityLogType]
GO
ALTER TABLE [dbo].[Nop_ActivityLog]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ActivityLog_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ActivityLog] CHECK CONSTRAINT [FK_Nop_ActivityLog_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_Address]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Address_Nop_Customer1] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Address] CHECK CONSTRAINT [FK_Nop_Address_Nop_Customer1]
GO
ALTER TABLE [dbo].[Nop_BlogComment]  WITH CHECK ADD  CONSTRAINT [FK_Nop_BlogComment_Nop_BlogPost] FOREIGN KEY([BlogPostID])
REFERENCES [dbo].[Nop_BlogPost] ([BlogPostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_BlogComment] CHECK CONSTRAINT [FK_Nop_BlogComment_Nop_BlogPost]
GO
ALTER TABLE [dbo].[Nop_BlogPost]  WITH CHECK ADD  CONSTRAINT [FK_Nop_BlogPost_Nop_Customer] FOREIGN KEY([CreatedByID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_BlogPost] CHECK CONSTRAINT [FK_Nop_BlogPost_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_BlogPost]  WITH CHECK ADD  CONSTRAINT [FK_Nop_BlogPost_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_BlogPost] CHECK CONSTRAINT [FK_Nop_BlogPost_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_Category]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Category_Nop_CategoryTemplate] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[Nop_CategoryTemplate] ([CategoryTemplateId])
GO
ALTER TABLE [dbo].[Nop_Category] CHECK CONSTRAINT [FK_Nop_Category_Nop_CategoryTemplate]
GO
ALTER TABLE [dbo].[Nop_Category_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Category_Discount_Mapping_Nop_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Nop_Category] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Category_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_Category_Discount_Mapping_Nop_Category]
GO
ALTER TABLE [dbo].[Nop_Category_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Category_Discount_Mapping_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Category_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_Category_Discount_Mapping_Nop_Discount]
GO
ALTER TABLE [dbo].[Nop_CategoryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CategoryLocalized_Nop_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Nop_Category] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CategoryLocalized] CHECK CONSTRAINT [FK_Nop_CategoryLocalized_Nop_Category]
GO
ALTER TABLE [dbo].[Nop_CategoryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CategoryLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CategoryLocalized] CHECK CONSTRAINT [FK_Nop_CategoryLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CheckoutAttributeLocalized_Nop_CheckoutAttribute] FOREIGN KEY([CheckoutAttributeID])
REFERENCES [dbo].[Nop_CheckoutAttribute] ([CheckoutAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeLocalized] CHECK CONSTRAINT [FK_Nop_CheckoutAttributeLocalized_Nop_CheckoutAttribute]
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CheckoutAttributeLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeLocalized] CHECK CONSTRAINT [FK_Nop_CheckoutAttributeLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValue]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CheckoutAttributeValue_Nop_CheckoutAttribute] FOREIGN KEY([CheckoutAttributeID])
REFERENCES [dbo].[Nop_CheckoutAttribute] ([CheckoutAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValue] CHECK CONSTRAINT [FK_Nop_CheckoutAttributeValue_Nop_CheckoutAttribute]
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValueLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CheckoutAttributeValueLocalized_Nop_CheckoutAttributeValue] FOREIGN KEY([CheckoutAttributeValueID])
REFERENCES [dbo].[Nop_CheckoutAttributeValue] ([CheckoutAttributeValueID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValueLocalized] CHECK CONSTRAINT [FK_Nop_CheckoutAttributeValueLocalized_Nop_CheckoutAttributeValue]
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValueLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CheckoutAttributeValueLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CheckoutAttributeValueLocalized] CHECK CONSTRAINT [FK_Nop_CheckoutAttributeValueLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CrossSellProduct_Nop_Product] FOREIGN KEY([ProductId1])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct] CHECK CONSTRAINT [FK_Nop_CrossSellProduct_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_Customer_CustomerRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Customer_CustomerRole_Mapping_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Customer_CustomerRole_Mapping] CHECK CONSTRAINT [FK_Nop_Customer_CustomerRole_Mapping_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_Customer_CustomerRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Customer_CustomerRole_Mapping_Nop_CustomerRole] FOREIGN KEY([CustomerRoleID])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Customer_CustomerRole_Mapping] CHECK CONSTRAINT [FK_Nop_Customer_CustomerRole_Mapping_Nop_CustomerRole]
GO
ALTER TABLE [dbo].[Nop_CustomerAttribute]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CustomerAttribute_Nop_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CustomerAttribute] CHECK CONSTRAINT [FK_Nop_CustomerAttribute_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_CustomerRole_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CustomerRole_Discount_Mapping_Nop_CustomerRole] FOREIGN KEY([CustomerRoleID])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CustomerRole_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_CustomerRole_Discount_Mapping_Nop_CustomerRole]
GO
ALTER TABLE [dbo].[Nop_CustomerRole_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CustomerRole_Discount_Mapping_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CustomerRole_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_CustomerRole_Discount_Mapping_Nop_Discount]
GO
ALTER TABLE [dbo].[Nop_CustomerRole_ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CustomerRole_ProductPrice_Nop_CustomerRole] FOREIGN KEY([CustomerRoleID])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CustomerRole_ProductPrice] CHECK CONSTRAINT [FK_Nop_CustomerRole_ProductPrice_Nop_CustomerRole]
GO
ALTER TABLE [dbo].[Nop_CustomerRole_ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CustomerRole_ProductPrice_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_CustomerRole_ProductPrice] CHECK CONSTRAINT [FK_Nop_CustomerRole_ProductPrice_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountRestriction_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction] CHECK CONSTRAINT [FK_Nop_DiscountRestriction_Nop_Discount]
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountRestriction_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction] CHECK CONSTRAINT [FK_Nop_DiscountRestriction_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory] CHECK CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory] CHECK CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Discount]
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Nop_Order] ([OrderID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory] CHECK CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Order]
GO
ALTER TABLE [dbo].[Nop_Forums_Forum]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Forums_Forum_Nop_Forums_Group] FOREIGN KEY([ForumGroupID])
REFERENCES [dbo].[Nop_Forums_Group] ([ForumGroupID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Forums_Forum] CHECK CONSTRAINT [FK_Nop_Forums_Forum_Nop_Forums_Group]
GO
ALTER TABLE [dbo].[Nop_Forums_Post]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Forums_Post_Nop_Forums_Topic] FOREIGN KEY([TopicID])
REFERENCES [dbo].[Nop_Forums_Topic] ([TopicID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Forums_Post] CHECK CONSTRAINT [FK_Nop_Forums_Post_Nop_Forums_Topic]
GO
ALTER TABLE [dbo].[Nop_Forums_Topic]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Forums_Topic_Nop_Forums_Forum] FOREIGN KEY([ForumID])
REFERENCES [dbo].[Nop_Forums_Forum] ([ForumID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Forums_Topic] CHECK CONSTRAINT [FK_Nop_Forums_Topic_Nop_Forums_Forum]
GO
ALTER TABLE [dbo].[Nop_GiftCard]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCard_Nop_OrderProductVariant] FOREIGN KEY([PurchasedOrderProductVariantID])
REFERENCES [dbo].[Nop_OrderProductVariant] ([OrderProductVariantID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_GiftCard] CHECK CONSTRAINT [FK_Nop_GiftCard_Nop_OrderProductVariant]
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory] CHECK CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_GiftCard] FOREIGN KEY([GiftCardID])
REFERENCES [dbo].[Nop_GiftCard] ([GiftCardID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory] CHECK CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_GiftCard]
GO
ALTER TABLE [dbo].[Nop_LocaleStringResource]  WITH CHECK ADD  CONSTRAINT [FK_Nop_LocaleStringResource_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_LocaleStringResource] CHECK CONSTRAINT [FK_Nop_LocaleStringResource_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_Manufacturer]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Manufacturer_Nop_ManufacturerTemplate] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[Nop_ManufacturerTemplate] ([ManufacturerTemplateId])
GO
ALTER TABLE [dbo].[Nop_Manufacturer] CHECK CONSTRAINT [FK_Nop_Manufacturer_Nop_ManufacturerTemplate]
GO
ALTER TABLE [dbo].[Nop_ManufacturerLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ManufacturerLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ManufacturerLocalized] CHECK CONSTRAINT [FK_Nop_ManufacturerLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_ManufacturerLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ManufacturerLocalized_Nop_Manufacturer] FOREIGN KEY([ManufacturerID])
REFERENCES [dbo].[Nop_Manufacturer] ([ManufacturerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ManufacturerLocalized] CHECK CONSTRAINT [FK_Nop_ManufacturerLocalized_Nop_Manufacturer]
GO
ALTER TABLE [dbo].[Nop_MessageTemplateLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_MessageTemplateLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_MessageTemplateLocalized] CHECK CONSTRAINT [FK_Nop_MessageTemplateLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_MessageTemplateLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_MessageTemplateLocalized_Nop_MessageTemplate] FOREIGN KEY([MessageTemplateID])
REFERENCES [dbo].[Nop_MessageTemplate] ([MessageTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_MessageTemplateLocalized] CHECK CONSTRAINT [FK_Nop_MessageTemplateLocalized_Nop_MessageTemplate]
GO
ALTER TABLE [dbo].[Nop_News]  WITH CHECK ADD  CONSTRAINT [FK_Nop_News_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_News] CHECK CONSTRAINT [FK_Nop_News_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_NewsComment]  WITH CHECK ADD  CONSTRAINT [FK_Nop_NewsComment_Nop_News] FOREIGN KEY([NewsID])
REFERENCES [dbo].[Nop_News] ([NewsID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_NewsComment] CHECK CONSTRAINT [FK_Nop_NewsComment_Nop_News]
GO
ALTER TABLE [dbo].[Nop_OrderNote]  WITH CHECK ADD  CONSTRAINT [FK_Nop_OrderNote_Nop_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Nop_Order] ([OrderID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_OrderNote] CHECK CONSTRAINT [FK_Nop_OrderNote_Nop_Order]
GO
ALTER TABLE [dbo].[Nop_OrderProductVariant]  WITH CHECK ADD  CONSTRAINT [FK_Nop_OrderProductVariant_Nop_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Nop_Order] ([OrderID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_OrderProductVariant] CHECK CONSTRAINT [FK_Nop_OrderProductVariant_Nop_Order]
GO
ALTER TABLE [dbo].[Nop_OrderProductVariant]  WITH CHECK ADD  CONSTRAINT [FK_Nop_OrderProductVariant_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
GO
ALTER TABLE [dbo].[Nop_OrderProductVariant] CHECK CONSTRAINT [FK_Nop_OrderProductVariant_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod] FOREIGN KEY([PaymentMethodID])
REFERENCES [dbo].[Nop_PaymentMethod] ([PaymentMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod]
GO
ALTER TABLE [dbo].[Nop_Poll]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Poll_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Poll] CHECK CONSTRAINT [FK_Nop_Poll_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_PollAnswer]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PollAnswer_Nop_Poll] FOREIGN KEY([PollID])
REFERENCES [dbo].[Nop_Poll] ([PollID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PollAnswer] CHECK CONSTRAINT [FK_Nop_PollAnswer_Nop_Poll]
GO
ALTER TABLE [dbo].[Nop_PollVotingRecord]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PollVotingRecord_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PollVotingRecord] CHECK CONSTRAINT [FK_Nop_PollVotingRecord_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_PollVotingRecord]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PollVotingRecord_Nop_PollAnswer] FOREIGN KEY([PollAnswerID])
REFERENCES [dbo].[Nop_PollAnswer] ([PollAnswerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PollVotingRecord] CHECK CONSTRAINT [FK_Nop_PollVotingRecord_Nop_PollAnswer]
GO
ALTER TABLE [dbo].[Nop_Product]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_Nop_ProductTemplate] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[Nop_ProductTemplate] ([ProductTemplateId])
GO
ALTER TABLE [dbo].[Nop_Product] CHECK CONSTRAINT [FK_Nop_Product_Nop_ProductTemplate]
GO
ALTER TABLE [dbo].[Nop_Product_Category_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_Category_Mapping_Nop_Category1] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Nop_Category] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_Category_Mapping] CHECK CONSTRAINT [FK_Nop_Product_Category_Mapping_Nop_Category1]
GO
ALTER TABLE [dbo].[Nop_Product_Category_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_Category_Mapping_Nop_Product1] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_Category_Mapping] CHECK CONSTRAINT [FK_Nop_Product_Category_Mapping_Nop_Product1]
GO
ALTER TABLE [dbo].[Nop_Product_Manufacturer_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_Manufacturer_Mapping_Nop_Manufacturer1] FOREIGN KEY([ManufacturerID])
REFERENCES [dbo].[Nop_Manufacturer] ([ManufacturerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_Manufacturer_Mapping] CHECK CONSTRAINT [FK_Nop_Product_Manufacturer_Mapping_Nop_Manufacturer1]
GO
ALTER TABLE [dbo].[Nop_Product_Manufacturer_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_Manufacturer_Mapping_Nop_Product1] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_Manufacturer_Mapping] CHECK CONSTRAINT [FK_Nop_Product_Manufacturer_Mapping_Nop_Product1]
GO
ALTER TABLE [dbo].[Nop_Product_SpecificationAttribute_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_SpecificationAttribute_Mapping_Nop_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_SpecificationAttribute_Mapping] CHECK CONSTRAINT [FK_Nop_Product_SpecificationAttribute_Mapping_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_Product_SpecificationAttribute_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Product_SpecificationAttribute_Mapping_Nop_SpecificationAttributeOption] FOREIGN KEY([SpecificationAttributeOptionID])
REFERENCES [dbo].[Nop_SpecificationAttributeOption] ([SpecificationAttributeOptionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_Product_SpecificationAttribute_Mapping] CHECK CONSTRAINT [FK_Nop_Product_SpecificationAttribute_Mapping_Nop_SpecificationAttributeOption]
GO
ALTER TABLE [dbo].[Nop_ProductAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductAttributeLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductAttributeLocalized] CHECK CONSTRAINT [FK_Nop_ProductAttributeLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_ProductAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductAttributeLocalized_Nop_ProductAttribute] FOREIGN KEY([ProductAttributeID])
REFERENCES [dbo].[Nop_ProductAttribute] ([ProductAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductAttributeLocalized] CHECK CONSTRAINT [FK_Nop_ProductAttributeLocalized_Nop_ProductAttribute]
GO
ALTER TABLE [dbo].[Nop_ProductLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductLocalized] CHECK CONSTRAINT [FK_Nop_ProductLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_ProductLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductLocalized_Nop_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductLocalized] CHECK CONSTRAINT [FK_Nop_ProductLocalized_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_ProductPicture]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductPicture_Nop_Picture] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Nop_Picture] ([PictureID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductPicture] CHECK CONSTRAINT [FK_Nop_ProductPicture_Nop_Picture]
GO
ALTER TABLE [dbo].[Nop_ProductPicture]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductPicture_Nop_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductPicture] CHECK CONSTRAINT [FK_Nop_ProductPicture_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_ProductRating]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductRating_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductRating] CHECK CONSTRAINT [FK_Nop_ProductRating_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_ProductRating]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductRating_Nop_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductRating] CHECK CONSTRAINT [FK_Nop_ProductRating_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_ProductReview]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductReview_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductReview] CHECK CONSTRAINT [FK_Nop_ProductReview_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_ProductReview]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductReview_Nop_Product1] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductReview] CHECK CONSTRAINT [FK_Nop_ProductReview_Nop_Product1]
GO
ALTER TABLE [dbo].[Nop_ProductReviewHelpfulness]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductReviewHelpfulness_Nop_ProductReview] FOREIGN KEY([ProductReviewID])
REFERENCES [dbo].[Nop_ProductReview] ([ProductReviewID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductReviewHelpfulness] CHECK CONSTRAINT [FK_Nop_ProductReviewHelpfulness_Nop_ProductReview]
GO
ALTER TABLE [dbo].[Nop_ProductTag_Product_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductTag_Product_Mapping_Nop_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductTag_Product_Mapping] CHECK CONSTRAINT [FK_Nop_ProductTag_Product_Mapping_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_ProductTag_Product_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductTag_Product_Mapping_Nop_ProductTag] FOREIGN KEY([ProductTagID])
REFERENCES [dbo].[Nop_ProductTag] ([ProductTagID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductTag_Product_Mapping] CHECK CONSTRAINT [FK_Nop_ProductTag_Product_Mapping_Nop_ProductTag]
GO
ALTER TABLE [dbo].[Nop_ProductVariant]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_Nop_Product1] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant] CHECK CONSTRAINT [FK_Nop_ProductVariant_Nop_Product1]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_Discount_Mapping_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_Discount_Mapping_Nop_Discount]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Discount_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_Discount_Mapping_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Discount_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_Discount_Mapping_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Pricelist_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_Pricelist_Mapping_Nop_Pricelist] FOREIGN KEY([PricelistID])
REFERENCES [dbo].[Nop_Pricelist] ([PricelistID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Pricelist_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_Pricelist_Mapping_Nop_Pricelist]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Pricelist_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_Pricelist_Mapping_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_Pricelist_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_Pricelist_Mapping_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_ProductAttribute_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_ProductAttribute_Mapping_Nop_ProductAttribute] FOREIGN KEY([ProductAttributeID])
REFERENCES [dbo].[Nop_ProductAttribute] ([ProductAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_ProductAttribute_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_ProductAttribute_Mapping_Nop_ProductAttribute]
GO
ALTER TABLE [dbo].[Nop_ProductVariant_ProductAttribute_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariant_ProductAttribute_Mapping_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariant_ProductAttribute_Mapping] CHECK CONSTRAINT [FK_Nop_ProductVariant_ProductAttribute_Mapping_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeCombination]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantAttributeCombination_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeCombination] CHECK CONSTRAINT [FK_Nop_ProductVariantAttributeCombination_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValue]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantAttributeValue_Nop_ProductVariant_ProductAttribute_Mapping] FOREIGN KEY([ProductVariantAttributeID])
REFERENCES [dbo].[Nop_ProductVariant_ProductAttribute_Mapping] ([ProductVariantAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValue] CHECK CONSTRAINT [FK_Nop_ProductVariantAttributeValue_Nop_ProductVariant_ProductAttribute_Mapping]
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValueLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantAttributeValueLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValueLocalized] CHECK CONSTRAINT [FK_Nop_ProductVariantAttributeValueLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValueLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantAttributeValueLocalized_Nop_ProductVariantAttributeValue] FOREIGN KEY([ProductVariantAttributeValueID])
REFERENCES [dbo].[Nop_ProductVariantAttributeValue] ([ProductVariantAttributeValueID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeValueLocalized] CHECK CONSTRAINT [FK_Nop_ProductVariantAttributeValueLocalized_Nop_ProductVariantAttributeValue]
GO
ALTER TABLE [dbo].[Nop_ProductVariantLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantLocalized] CHECK CONSTRAINT [FK_Nop_ProductVariantLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_ProductVariantLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantLocalized_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ProductVariantLocalized] CHECK CONSTRAINT [FK_Nop_ProductVariantLocalized_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_RecurringPaymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_RecurringPaymentHistory_Nop_RecurringPayment] FOREIGN KEY([RecurringPaymentID])
REFERENCES [dbo].[Nop_RecurringPayment] ([RecurringPaymentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_RecurringPaymentHistory] CHECK CONSTRAINT [FK_Nop_RecurringPaymentHistory_Nop_RecurringPayment]
GO
ALTER TABLE [dbo].[Nop_RelatedProduct]  WITH CHECK ADD  CONSTRAINT [FK_Nop_RelatedProduct_Nop_Product] FOREIGN KEY([ProductID1])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_RelatedProduct] CHECK CONSTRAINT [FK_Nop_RelatedProduct_Nop_Product]
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ReturnRequest] CHECK CONSTRAINT [FK_Nop_ReturnRequest_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_OrderProductVariant] FOREIGN KEY([OrderProductVariantId])
REFERENCES [dbo].[Nop_OrderProductVariant] ([OrderProductVariantID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ReturnRequest] CHECK CONSTRAINT [FK_Nop_ReturnRequest_Nop_OrderProductVariant]
GO
ALTER TABLE [dbo].[Nop_RewardPointsHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_RewardPointsHistory_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_RewardPointsHistory] CHECK CONSTRAINT [FK_Nop_RewardPointsHistory_Nop_Customer]
GO
ALTER TABLE [dbo].[Nop_ShippingByTotal]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingByTotal_Nop_ShippingMethod] FOREIGN KEY([ShippingMethodID])
REFERENCES [dbo].[Nop_ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingByTotal] CHECK CONSTRAINT [FK_Nop_ShippingByTotal_Nop_ShippingMethod]
GO
ALTER TABLE [dbo].[Nop_ShippingByWeight]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingByWeight_Nop_ShippingMethod] FOREIGN KEY([ShippingMethodID])
REFERENCES [dbo].[Nop_ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingByWeight] CHECK CONSTRAINT [FK_Nop_ShippingByWeight_Nop_ShippingMethod]
GO
ALTER TABLE [dbo].[Nop_ShippingByWeightAndCountry]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingByWeightAndCountry_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingByWeightAndCountry] CHECK CONSTRAINT [FK_Nop_ShippingByWeightAndCountry_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_ShippingByWeightAndCountry]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingByWeightAndCountry_Nop_ShippingMethod] FOREIGN KEY([ShippingMethodID])
REFERENCES [dbo].[Nop_ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingByWeightAndCountry] CHECK CONSTRAINT [FK_Nop_ShippingByWeightAndCountry_Nop_ShippingMethod]
GO
ALTER TABLE [dbo].[Nop_ShippingMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingMethod_RestrictedCountries_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_ShippingMethod_RestrictedCountries_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_ShippingMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShippingMethod_RestrictedCountries_Nop_ShippingMethod] FOREIGN KEY([ShippingMethodID])
REFERENCES [dbo].[Nop_ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShippingMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_ShippingMethod_RestrictedCountries_Nop_ShippingMethod]
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShoppingCart_Nop_CustomerSession] FOREIGN KEY([CustomerSessionGUID])
REFERENCES [dbo].[Nop_CustomerSession] ([CustomerSessionGUID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem] CHECK CONSTRAINT [FK_Nop_ShoppingCart_Nop_CustomerSession]
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ShoppingCart_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ShoppingCartItem] CHECK CONSTRAINT [FK_Nop_ShoppingCart_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_SpecificationAttributeLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeLocalized] CHECK CONSTRAINT [FK_Nop_SpecificationAttributeLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_SpecificationAttributeLocalized_Nop_SpecificationAttribute] FOREIGN KEY([SpecificationAttributeID])
REFERENCES [dbo].[Nop_SpecificationAttribute] ([SpecificationAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeLocalized] CHECK CONSTRAINT [FK_Nop_SpecificationAttributeLocalized_Nop_SpecificationAttribute]
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOption]  WITH CHECK ADD  CONSTRAINT [FK_Nop_SpecificationAttributeOption_Nop_SpecificationAttribute] FOREIGN KEY([SpecificationAttributeID])
REFERENCES [dbo].[Nop_SpecificationAttribute] ([SpecificationAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOption] CHECK CONSTRAINT [FK_Nop_SpecificationAttributeOption_Nop_SpecificationAttribute]
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOptionLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_SpecificationAttributeOptionLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOptionLocalized] CHECK CONSTRAINT [FK_Nop_SpecificationAttributeOptionLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOptionLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_SpecificationAttributeOptionLocalized_Nop_SpecificationAttributeOption] FOREIGN KEY([SpecificationAttributeOptionID])
REFERENCES [dbo].[Nop_SpecificationAttributeOption] ([SpecificationAttributeOptionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_SpecificationAttributeOptionLocalized] CHECK CONSTRAINT [FK_Nop_SpecificationAttributeOptionLocalized_Nop_SpecificationAttributeOption]
GO
ALTER TABLE [dbo].[Nop_StateProvince]  WITH CHECK ADD  CONSTRAINT [FK_Nop_StateProvince_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_StateProvince] CHECK CONSTRAINT [FK_Nop_StateProvince_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_TaxRate]  WITH CHECK ADD  CONSTRAINT [FK_Nop_TaxRate_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_TaxRate] CHECK CONSTRAINT [FK_Nop_TaxRate_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_TaxRate]  WITH CHECK ADD  CONSTRAINT [FK_Nop_TaxRate_Nop_TaxCategory] FOREIGN KEY([TaxCategoryID])
REFERENCES [dbo].[Nop_TaxCategory] ([TaxCategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_TaxRate] CHECK CONSTRAINT [FK_Nop_TaxRate_Nop_TaxCategory]
GO
ALTER TABLE [dbo].[Nop_TierPrice]  WITH CHECK ADD  CONSTRAINT [FK_Nop_TierPrice_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_TierPrice] CHECK CONSTRAINT [FK_Nop_TierPrice_Nop_ProductVariant]
GO
ALTER TABLE [dbo].[Nop_TopicLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_TopicLocalized_Nop_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Nop_Language] ([LanguageId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_TopicLocalized] CHECK CONSTRAINT [FK_Nop_TopicLocalized_Nop_Language]
GO
ALTER TABLE [dbo].[Nop_TopicLocalized]  WITH CHECK ADD  CONSTRAINT [FK_Nop_TopicLocalized_Nop_Topic] FOREIGN KEY([TopicID])
REFERENCES [dbo].[Nop_Topic] ([TopicID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_TopicLocalized] CHECK CONSTRAINT [FK_Nop_TopicLocalized_Nop_Topic]
GO










CREATE FUNCTION [dbo].[NOP_splitstring_to_table]
(
    @string NVARCHAR(1000),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(256)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO

CREATE FUNCTION [dbo].[NOP_getnotnullnotempty]
(
    @p1 nvarchar(max) = null, 
    @p2 nvarchar(max) = null
)
RETURNS nvarchar(max)
AS
BEGIN
    IF @p1 IS NULL
        return @p2
    IF @p1 =''
        return @p2

    return @p1
END
GO


CREATE FUNCTION [dbo].[NOP_getcustomerattributevalue]
(
    @CustomerID int, 
    @AttributeKey nvarchar(100)
)
RETURNS nvarchar(1000)
AS
BEGIN
	
	DECLARE @AttributeValue nvarchar(1000)
	SET @AttributeValue = N''

	IF (EXISTS (SELECT ca.[Value] FROM [Nop_CustomerAttribute] ca 
				WHERE ca.CustomerID = @CustomerID AND
					  ca.[Key] = @AttributeKey))
	BEGIN
		SELECT @AttributeValue = ca.[Value] FROM [Nop_CustomerAttribute] ca 
				WHERE ca.CustomerID = @CustomerID AND
					  ca.[Key] = @AttributeKey	
	END
	  
	return @AttributeValue  
END
GO



CREATE PROCEDURE [dbo].[Nop_Maintenance_ReindexTables]
AS
BEGIN
	--indexing
	DECLARE @TableName sysname
	DECLARE cur_reindex CURSOR FOR
	SELECT table_name
	FROM information_schema.tables
	WHERE table_type = 'base table'
	OPEN cur_reindex
	FETCH NEXT FROM cur_reindex INTO @TableName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT 'Reindexing ' + @TableName + ' table'
		DBCC DBREINDEX (@TableName, ' ', 80)
		FETCH NEXT FROM cur_reindex INTO @TableName
		END
	CLOSE cur_reindex
	DEALLOCATE cur_reindex
END
GO


CREATE PROCEDURE [dbo].[Nop_SalesBestSellersReport]
(
	@LastDays int = 360,
	@RecordsToReturn int = 10,
	@OrderBy int = 1
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @cmd varchar(500)
	
	CREATE TABLE #tmp (
		ID int not null identity,
		ProductVariantID int,
		SalesTotalCount int,
		SalesTotalAmount money)
	INSERT #tmp (
		ProductVariantID,
		SalesTotalCount,
		SalesTotalAmount)
	SELECT 
		s.ProductVariantId,
		s.SalesTotalCount, 
		s.SalesTotalAmount 
	FROM (SELECT opv.ProductVariantId, SUM(opv.Quantity) AS SalesTotalCount, SUM(opv.PriceExclTax) AS SalesTotalAmount
		  FROM [Nop_OrderProductVariant] opv
				INNER JOIN [Nop_Order] o on opv.OrderID = o.OrderID 
				WHERE o.CreatedOn >= dateadd(dy, -@LastDays, getdate())
				AND o.Deleted=0
		  GROUP BY opv.ProductVariantID 
		 ) s
		INNER JOIN [Nop_ProductVariant] pv with (nolock) on s.ProductVariantID = pv.ProductVariantID
		INNER JOIN [Nop_Product] p with (nolock) on pv.ProductID = p.ProductID
	WHERE p.Deleted = 0 
		AND p.Published = 1  
		AND pv.Published = 1 
		AND pv.Deleted = 0
	ORDER BY case @OrderBy when 1 then s.SalesTotalCount when 2 then s.SalesTotalAmount else s.SalesTotalCount end desc

	SET @cmd = 'SELECT TOP ' + convert(varchar(10), @RecordsToReturn ) + ' * FROM #tmp Order By ID'

	EXEC (@cmd)

	DROP TABLE #tmp
END
GO


CREATE PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
(
	@ProductID			int,
	@ShowHidden			bit,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductID int NOT NULL,
		ProductsPurchased int NOT NULL,
	)

	INSERT INTO #PageIndex (ProductID, ProductsPurchased)
	SELECT p.ProductID, SUM(opv.Quantity) as ProductsPurchased
	FROM    
		dbo.Nop_OrderProductVariant opv WITH (NOLOCK)
		INNER JOIN dbo.Nop_ProductVariant pv ON pv.ProductVariantId = opv.ProductVariantId
		INNER JOIN dbo.Nop_Product p ON p.ProductId = pv.ProductId
	WHERE
		opv.OrderID IN 
		(
			/* This inner query should retrieve all orders that have contained the productID */
			SELECT 
				DISTINCT OrderID
			FROM 
				dbo.Nop_OrderProductVariant opv2 WITH (NOLOCK)
				INNER JOIN dbo.Nop_ProductVariant pv2 ON pv2.ProductVariantId = opv2.ProductVariantId
				INNER JOIN dbo.Nop_Product p2 ON p2.ProductId = pv2.ProductId			
			WHERE 
				p2.ProductID = @ProductID
		)
		AND 
			(
				p.ProductId != @ProductID
			)
		AND 
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Deleted = 0
			)
		AND 
			(
				@ShowHidden = 1
				OR
				GETUTCDATE() BETWEEN ISNULL(pv.AvailableStartDateTime, '1/1/1900') AND ISNULL(pv.AvailableEndDateTime, '1/1/2999')
			)
	GROUP BY
		p.ProductId
	ORDER BY 
		ProductsPurchased desc


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex

END
GO


CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.GiftCardType,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.CallForPrice,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@RelatedToProductID	int = 0,
	@Keywords			nvarchar(MAX),
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	DECLARE @SearchKeywords bit
	SET @SearchKeywords = 1
	IF (@Keywords IS NULL OR @Keywords = N'')
		SET @SearchKeywords = 0

	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
	DECLARE @SpecAttributesCount int	
	SELECT @SpecAttributesCount = COUNT(1) FROM #FilteredSpecs

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #DisplayOrderTmp ([ProductID])
	SELECT p.ProductID
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_RelatedProduct rp with (NOLOCK) ON p.ProductID=rp.ProductID2
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
		   (
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				@RelatedToProductID IS NULL OR @RelatedToProductID=0
				OR rp.ProductID1=@RelatedToProductID
			)
		AND	(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Deleted = 0
			)
		AND (
				@PriceMin IS NULL OR @PriceMin=0
				OR pv.Price > @PriceMin	
			)
		AND (
				@PriceMax IS NULL OR @PriceMax=2147483644 -- max value
				OR pv.Price < @PriceMax
			)
		AND	(
				@SearchKeywords = 0 or 
				(
					-- search standard content
					patindex(@Keywords, p.name) > 0
					or patindex(@Keywords, pv.name) > 0
					or patindex(@Keywords, pv.sku) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pv.Description) > 0)					
					-- search language content
					or patindex(@Keywords, pl.name) > 0
					or patindex(@Keywords, pvl.name) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pvl.Description) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		AND
			(
				--filter by specs
				@SpecAttributesCount = 0
				OR
				(
					NOT EXISTS(
						SELECT 1 
						FROM #FilteredSpecs [fs]
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @RelatedToProductID IS NOT NULL AND @RelatedToProductID > 0
		THEN rp.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOn END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p with (NOLOCK) on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO


CREATE PROCEDURE [dbo].[Nop_SpecificationAttributeOptionFilter_LoadByFilter]
(
	@CategoryID int,
	@LanguageID int
)
AS
BEGIN
	SELECT 
		sa.SpecificationAttributeId,
		dbo.NOP_getnotnullnotempty(sal.Name,sa.Name) as [SpecificationAttributeName],
		sa.DisplayOrder,
		sao.SpecificationAttributeOptionId,
		dbo.NOP_getnotnullnotempty(saol.Name,sao.Name) as [SpecificationAttributeOptionName]
	FROM Nop_Product_SpecificationAttribute_Mapping psam with (NOLOCK)
		INNER JOIN Nop_SpecificationAttributeOption sao with (NOLOCK) ON
			sao.SpecificationAttributeOptionID = psam.SpecificationAttributeOptionID
		INNER JOIN Nop_SpecificationAttribute sa with (NOLOCK) ON
			sa.SpecificationAttributeID = sao.SpecificationAttributeID	
		INNER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON 
			pcm.ProductID = psam.ProductID	
		INNER JOIN Nop_Product p ON 
			psam.ProductID = p.ProductID
		LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON 
			p.ProductID = pv.ProductID
		LEFT OUTER JOIN [Nop_SpecificationAttributeLocalized] sal with (NOLOCK) ON 
			sa.SpecificationAttributeID = sal.SpecificationAttributeID AND sal.LanguageID = @LanguageID	
		LEFT OUTER JOIN [Nop_SpecificationAttributeOptionLocalized] saol with (NOLOCK) ON 
			sao.SpecificationAttributeOptionID = saol.SpecificationAttributeOptionID AND saol.LanguageID = @LanguageID	
	WHERE 
			p.Published = 1
		AND 
			pv.Published = 1
		AND 
			p.Deleted=0
		AND
			pcm.CategoryID = @CategoryID
		AND
			psam.AllowFiltering = 1
		AND
			getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999')
	GROUP BY
		sa.SpecificationAttributeID, 
		dbo.NOP_getnotnullnotempty(sal.Name,sa.Name),
		sa.DisplayOrder,
		sao.SpecificationAttributeOptionID,
		dbo.NOP_getnotnullnotempty(saol.Name,sao.Name)
	ORDER BY sa.DisplayOrder, [SpecificationAttributeName], [SpecificationAttributeOptionName]
END
GO


CREATE PROCEDURE [dbo].[Nop_CustomerBestReport]
(
	@StartTime				datetime = NULL,
	@EndTime				datetime = NULL,
	@OrderStatusID			int,
	@PaymentStatusID		int,
	@ShippingStatusID		int,
	@OrderBy				int = 1 
)
AS
BEGIN

	SELECT TOP 20 c.CustomerId, SUM(o.OrderTotal) AS OrderTotal, COUNT(o.OrderID) AS OrderCount
	FROM [Nop_Customer] c
	INNER JOIN [Nop_Order] o
	ON c.CustomerID = o.CustomerID
	WHERE
		c.Deleted = 0 AND
		o.Deleted = 0 AND
		(@StartTime is NULL or @StartTime <= o.CreatedOn) AND
		(@EndTime is NULL or @EndTime >= o.CreatedOn) AND 
		(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) AND
		(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) AND
		(@ShippingStatusID IS NULL OR @ShippingStatusID = 0 OR o.ShippingStatusID = @ShippingStatusID) --AND
	GROUP BY c.CustomerID
	ORDER BY case @OrderBy when 1 then SUM(o.OrderTotal) when 2 then COUNT(o.OrderID) else SUM(o.OrderTotal) end desc

END
GO


CREATE PROCEDURE [dbo].[Nop_LanguagePackImport]
(
	@LanguageID int,
	@XmlPackage xml
)
AS
BEGIN
	IF EXISTS(SELECT * FROM [dbo].[Nop_Language] WHERE LanguageID = @LanguageID)
	BEGIN
		CREATE TABLE #LocaleStringResourceTmp
			(
				[LanguageID] [int] NOT NULL,
				[ResourceName] [nvarchar](200) NOT NULL,
				[ResourceValue] [nvarchar](max) NOT NULL
			)



		INSERT INTO #LocaleStringResourceTmp (LanguageID, ResourceName, ResourceValue)
		SELECT	@LanguageID, nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageID, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageID, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM Nop_LocaleStringResource WHERE LanguageID=@LanguageID AND ResourceName=@ResourceName))
			BEGIN
				UPDATE [Nop_LocaleStringResource]
				SET [ResourceValue]=@ResourceValue
				WHERE LanguageID=@LanguageID AND ResourceName=@ResourceName
			END
			ELSE 
			BEGIN
				INSERT INTO [Nop_LocaleStringResource]
				(
					[LanguageID],
					[ResourceName],
					[ResourceValue]
				)
				VALUES
				(
					@LanguageID,
					@ResourceName,
					@ResourceValue
				)
			END
			
			
			FETCH NEXT FROM cur_localeresource INTO @LanguageID, @ResourceName, @ResourceValue
			END
		CLOSE cur_localeresource
		DEALLOCATE cur_localeresource

		DROP TABLE #LocaleStringResourceTmp

		CREATE TABLE #MessageTemplateTmp
			(
				[LanguageID] [int] NOT NULL,
				[Name] [nvarchar](200) NOT NULL,
				[Subject] [nvarchar](200) NOT NULL,
				[Body] [nvarchar](max) NOT NULL
			)



		INSERT INTO #MessageTemplateTmp (LanguageID, [Name], [Subject], [Body])
		SELECT	@LanguageID, nref.value('@Name', 'nvarchar(200)'), nref.value('Subject[1]', 'nvarchar(200)'), nref.value('Body[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/MessageTemplate') AS R(nref)

		DECLARE @Name nvarchar(200)
		DECLARE @Subject nvarchar(200)
		DECLARE @Body nvarchar(MAX)
		DECLARE cur_messagetemplate CURSOR FOR
		SELECT LanguageID, [Name], [Subject], [Body]
		FROM #MessageTemplateTmp
		OPEN cur_messagetemplate
		FETCH NEXT FROM cur_messagetemplate INTO @LanguageID, @Name, @Subject, @Body
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @MessageTemplateID int
			IF (EXISTS (SELECT 1 FROM Nop_MessageTemplate WHERE [Name]=@Name))
			BEGIN
				SET @MessageTemplateID = (SELECT MessageTemplateID FROM Nop_MessageTemplate WHERE [Name]=@Name);
			END
			ELSE 
			BEGIN
				INSERT INTO Nop_MessageTemplate ([Name]) VALUES (@Name);
				SET @MessageTemplateID = SCOPE_IDENTITY()
			END

			IF (EXISTS (SELECT 1 FROM Nop_MessageTemplateLocalized WHERE MessageTemplateID=@MessageTemplateID AND LanguageID=@LanguageID))
			BEGIN
				UPDATE [Nop_MessageTemplateLocalized]
				SET 
					[Subject]=@Subject,
					[Body] = @Body
				WHERE 
					MessageTemplateID=@MessageTemplateID AND LanguageID=@LanguageID
			END
			ELSE 
			BEGIN
				INSERT INTO [Nop_MessageTemplateLocalized]
				(
					[MessageTemplateID],
					[LanguageID],
					[Subject],
					[Body]
				)
				VALUES
				(
					@MessageTemplateID,
					@LanguageID,
					@Subject,
					@Body
				)
			END
			
			
			FETCH NEXT FROM cur_messagetemplate INTO @LanguageID, @Name, @Subject, @Body
			END
		CLOSE cur_messagetemplate
		DEALLOCATE cur_messagetemplate

		DROP TABLE #MessageTemplateTmp
	END
END
GO


CREATE PROCEDURE [dbo].[Nop_LanguagePackExport]
(
	@LanguageID int,
	@XmlPackage xml output
)
AS
BEGIN
	SET NOCOUNT ON
	SET @XmlPackage = 
	(
		SELECT l.Name as '@Name',
		(
			SELECT 
				lsr.ResourceName AS '@Name', 
				lsr.ResourceValue AS 'Value' 
			FROM 
				Nop_LocaleStringResource lsr 
			WHERE 
				lsr.LanguageID = l.LanguageID 
			ORDER BY 
				lsr.ResourceName
			FOR 
				XML PATH('LocaleResource'), TYPE
		),
		(
			SELECT
				mt.Name AS '@Name',
				mtl.Subject AS 'Subject', 
				mtl.Body AS 'Body'
			FROM 
				Nop_MessageTemplateLocalized mtl
			INNER JOIN
				Nop_MessageTemplate mt
			ON
				mt.MessageTemplateID = mtl.MessageTemplateID
			WHERE 
				mtl.LanguageID = l.LanguageID 
			FOR 
				XML PATH('MessageTemplate'), TYPE
		)
		FROM 
			Nop_Language l
		WHERE
			LanguageID = @LanguageID
		FOR 
			XML PATH('Language')
	)
END
GO


CREATE PROCEDURE [dbo].[Nop_OrderProductVariantReport]
(
	@StartTime datetime = NULL,
	@EndTime datetime = NULL,
	@OrderStatusID int,
	@PaymentStatusID int,
	@BillingCountryID int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT opv.ProductVariantId, isnull(sum(opv.PriceExclTax), 0) as PriceExclTax, isnull(sum(opv.Quantity), 0) as Quantity
	FROM Nop_OrderProductVariant opv 
	INNER JOIN [Nop_Order] o ON o.OrderId = opv.OrderID
	WHERE
		(@StartTime is NULL or @StartTime <= o.CreatedOn) and
		(@EndTime is NULL or @EndTime >= o.CreatedOn) and 
		(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) and
		(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) and
		(@BillingCountryID IS NULL or @BillingCountryID=0 or o.BillingCountryID = @BillingCountryID) and
		(o.Deleted=0)
	GROUP BY opv.ProductVariantId
	ORDER BY PriceExclTax desc
END
GO


CREATE PROCEDURE [dbo].[Nop_CustomerReportByAttributeKey]
(
	@CustomerAttributeKey nvarchar(100)
)
AS
BEGIN

	SELECT dbo.[NOP_getcustomerattributevalue] (c.CustomerId, @CustomerAttributeKey) as AttributeKey, 
		   Count(c.CustomerId) as CustomerCount
FROM [Nop_Customer] c
WHERE
	c.Deleted = 0
GROUP BY dbo.[NOP_getcustomerattributevalue] (c.CustomerId, @CustomerAttributeKey)
ORDER BY CustomerCount desc

END
GO
