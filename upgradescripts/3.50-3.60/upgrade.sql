--upgrade scripts from nopCommerce 3.50 to 3.60

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations">
    <Value>Notify customer about shipping from multiple locations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations.Hint">
    <Value>Check if you want customers to be notified when shipping from multiple locations.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.ShippingMethod.ShippingFromMultipleLocations">
    <Value>Please note that your order will be shipped from multiple locations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Details">
    <Value>Edit page</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PricesConsiderPromotions">
    <Value>Prices consider promotions</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PricesConsiderPromotions.Hint">
    <Value>Check if you want prices to be calculated with promotions (tier prices, discounts, special prices, tax, etc). But please note that it can significantly reduce time required to generate the feed file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices.Hint">
    <Value>Check if it''s telecommunications, broadcasting and electronic services. It''s used for tax calculation in Europe Union.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToContactVendors">
    <Value>Allow customers to contact vendors</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToContactVendors.Hint">
    <Value>Check to allow customers to contact vendors.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.ContactVendor">
    <Value>Contact Vendor - {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor">
    <Value>Contact vendor</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Button">
    <Value>Submit</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email">
    <Value>Your email</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email.Hint">
    <Value>Enter your email address</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email.Required">
    <Value>Enter email</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.EmailSubject">
    <Value>{0}. Contact us</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry">
    <Value>Enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry.Hint">
    <Value>Enter your enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry.Required">
    <Value>Enter enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName">
    <Value>Your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName.Hint">
    <Value>Enter your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName.Required">
    <Value>Enter your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.YourEnquiryHasBeenSent">
    <Value>Your enquiry has been successfully sent to the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic">
    <Value>Topic templates</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.Name.Required">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.ViewPath">
    <Value>View path</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.ViewPath.Required">
    <Value>View path is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.TopicTemplate">
    <Value>Topic template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.TopicTemplate.Hint">
    <Value>Choose a topic template. This template defines how this topic will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.IsRequired.Hint">
	<Value>When an attribute is required, the customer must choose an appropriate attribute value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test">
	<Value>Test template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.BackToTemplate">
	<Value>back to template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Send">
	<Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.SendTo">
	<Value>Send email to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.SendTo.Hint">
	<Value>Send test email to ensure that everything is properly configured.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Success">
	<Value>Email has been successfully queued.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens">
	<Value>Tokens</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens.Description">
	<Value>Please enter tokens you want to be replaced below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens.Hint">
	<Value>Enter tokens.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.TestDetails">
	<Value>Send test email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions">
    <Value>Search page. Page size options (comma separated)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.All">
    <Value>Export to Excel (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.All">
    <Value>Export to XML (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.All">
    <Value>Print packaging slips (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.CustomGoods">
    <Value>Custom goods (no identifier exists)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.TopCategoryMenuSubcategoryLevelsToDisplay">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.TopCategoryMenuSubcategoryLevelsToDisplay.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.Priority.Range">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.QueuedEmailPriority.Low">
    <Value>Low</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.QueuedEmailPriority.High">
    <Value>High</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountAmount">
    <Value>Maximum discount amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountAmount.Hint">
    <Value>Maximum allowed discount amount. Leave empty to allow any discount amount. If you''re using "Assigned to products" discount type, then it''s applied to each product separately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultImageQuality">
    <Value>Default image quality (0 - 100)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultImageQuality.Hint">
    <Value>The image quality to be used for uploaded images. Once changed you have to manually delete already generated thumbs.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.PdfSettings">
    <Value>PDF settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea">
    <Value>Enable XSRF protection for admin area</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea.Hint">
    <Value>Check to enable XSRF protection for admin area.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Profit">
    <Value>Profit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Profit.Hint">
    <Value>Profit of this order.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.IncludingTax">
    <Value>Include tax</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.IncludingTax.Hint">
    <Value>Check to include tax when generating tracking code for {ECOMMERCE} part.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfoWeight">
    <Value>Pass shipping info (weight)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfoWeight.Hint">
    <Value>Check if you want to include shipping information (weight) in generated XML file.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfo.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfoDimensions">
    <Value>Pass shipping info (dimensions)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfoDimensions.Hint">
    <Value>Check if you want to include shipping information (dimensions) in generated XML file.</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Button">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Options.Subscribe">
    <Value>Subscribe</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Options.Unsubscribe">
    <Value>Unsubscribe</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.UnsubscribeEmailSent">
    <Value>A verification email has been sent. Thank you!</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterBlockAllowToUnsubscribe">
    <Value>Newsletter box. Allow to unsubscribe</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterBlockAllowToUnsubscribe.Hint">
    <Value>Check if you want to allow customers to display "unsubscribe" option in the newsletter block.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.General">
    <Value>General</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Performance">
    <Value>Performance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviews">
    <Value>Product reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Search">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CompareProducts">
    <Value>Compare products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Sharing">
    <Value>Sharing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore">
    <Value>Enable XSRF protection for public store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore.Hint">
    <Value>Check to enable XSRF protection for public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchTermMinimumLength">
    <Value>Search term minimum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchTermMinimumLength.Hint">
    <Value>Specify minimum length of search term.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ResponsiveDesignSupported">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ResponsiveDesignSupported.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountCouponCode">
    <Value>Coupon code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountCouponCode.Hint">
    <Value>Search by discount coupon code.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountType">
    <Value>Discount type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountType.Hint">
    <Value>Search by discount type.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PaymentError">
    <Value>Payment error: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled">
    <Value>Enable honeypot</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled.Hint">
    <Value>Check to enable honeypot technique for registration page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.FriendlyUrlName">
    <Value>Friendly URL name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.FriendlyUrlName.Hint">
    <Value>A friendly name for generated affiliate URL (by default affiliate ID is used). It''s more friendly for marketing purposes. Leave empty to use affiliate identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Affiliate.Remove">
    <Value>Remove affiliate</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchFriendlyUrlName">
    <Value>Friendly URL name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchFriendlyUrlName.Hint">
    <Value>Search by a friendly URL name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchFirstName">
    <Value>First name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchFirstName.Hint">
    <Value>Search by a first name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchLastName">
    <Value>Last name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.SearchLastName.Hint">
    <Value>Search by a last name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.LoadOnlyWithOrders">
    <Value>Load only with orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.LoadOnlyWithOrders.Hint">
    <Value>Check to load affiliates only with orders placed (by affiliated customers).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.OrdersCreatedFromUtc">
    <Value>Orders start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.OrdersCreatedFromUtc.Hint">
    <Value>The start date for the order search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.OrdersCreatedToUtc">
    <Value>Orders end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.List.OrdersCreatedToUtc.Hint">
    <Value>The end date for the order search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.OrderStatus.Hint">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.PaymentStatus.Hint">
    <Value>Search by a specific payment status e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.ShippingStatus.Hint">
    <Value>Search by a specific shipping status e.g. Not yet shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1">
    <Value>Include in footer (column 1)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1.Hint">
    <Value>Check to include this topic in the footer (column 1). Ensure that your theme supports it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2">
    <Value>Include in footer (column 2)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2.Hint">
    <Value>Check to include this topic in the footer (column 2). Ensure that your theme supports it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3">
    <Value>Include in footer (column 3)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3.Hint">
    <Value>Check to include this topic in the footer (column 3). Ensure that your theme supports it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed">
    <Value>Accessible when store closed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed.Hint">
    <Value>Check to allow customer to view this topic details page when the store is closed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderNotes">
    <Value>Order notes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderNotes.Hint">
    <Value>Search in order notes. Leave empty to load all orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendWelcomeMessage">
    <Value>Send welcome message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendWelcomeMessage.Success">
    <Value>Welcome email has been successfully sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordMinLength">
    <Value>Password minimum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordMinLength.Hint">
    <Value>Specify password minimum length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute">
    <Value>Alt</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute.Hint">
    <Value>Override "alt" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute">
    <Value>Title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute.Hint">
    <Value>Override "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.UsedByProducts">
    <Value>Used by products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.UsedByProducts.Hint">
    <Value>Here you can see a list of products which use this attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.UsedByProducts.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.UsedByProducts.Published">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.Info">
    <Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues">
    <Value>Predefined values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.AddNew">
    <Value>Add a new value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.EditValueDetails">
    <Value>Edit value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Cost">
    <Value>Cost</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Cost.Hint">
    <Value>The attribute value cost is the cost of all the different components which make up this value. This may be either the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.DisplayOrder.Hint">
    <Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.IsPreSelected">
    <Value>Is pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre selected for the customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name.Hint">
    <Value>The attribute value name e.g. ''Blue'' for Color attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustment">
    <Value>Price adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustment.Hint">
    <Value>The price adjustment applied when choosing this attribute value e.g. ''10'' to add 10 dollars.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.WeightAdjustment">
    <Value>Weight adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.WeightAdjustment.Hint">
    <Value>The weight adjustment applied when choosing this attribute value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Hint">
    <Value>Predefined (default) values are helpful for a store owner when creating new products. Then when you add the attribute to a product, you don''t have to create the values again.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.SaveBeforeEdit">
    <Value>You need to save the product attribute before you can add values for this page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.ViewLink">
    <Value>View/Edit values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.TotalValues">
    <Value>Total: </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts.NoRecords">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToProducts">
    <Value>Applied to products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToProducts.SaveBeforeEdit">
    <Value>You need to save the discount before you can add products for this discount page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToProducts.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToProducts.AddNew">
    <Value>Add a new product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories.NoRecords">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToCategories">
    <Value>Applied to categories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToCategories.SaveBeforeEdit">
    <Value>You need to save the discount before you can add categories for this discount page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToCategories.Category">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToCategories.AddNew">
    <Value>Add a new category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.SearchActive">
    <Value>Active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.Hint">
    <Value>Search by a specific status e.g. Active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.ActiveOnly">
    <Value>Active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.NotActiveOnly">
    <Value>Not active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.List.Activated.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.List.Activated.ActivatedOnly">
    <Value>Activated</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.List.Activated.DeactivatedOnly">
    <Value>Deactivated</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountName">
    <Value>Discount name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchDiscountName.Hint">
    <Value>Search by discount name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.Quantity.Hint">
    <Value>Entered quantity.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.History.OrderTotal">
    <Value>Order total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.History.Order">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ReSendActivationMessage">
    <Value>Re-send activation message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ReSendActivationMessage.Success">
    <Value>Activation email has been successfully sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.TimesUsed">
    <Value>Times used</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm">
    <Value>Contact us page. Use system email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm.Hint">
    <Value>Check to use your system email as "From" field when sending emails from contact us page. Otherwise, customer email will be used (please note that some email services do not allow it).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.ActiveDiscussionsPageSize">
    <Value>Active discussions page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.ActiveDiscussionsPageSize.Hint">
    <Value>Set the page size for active discussions page e.g. ''10'' results per page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRecoveryLinkDaysValid">
    <Value>Password recovery link. Days valid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRecoveryLinkDaysValid.Hint">
    <Value>Enter number of days for password recovery link. Set to 0 if it doesn''t expire..</Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.LinkExpired">
    <Value>Your password recovery link is expired</Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.WrongToken">
    <Value>Wrong password recovery token</Value>
  </LocaleResource>
  <LocaleResource Name="Messages.Order.Product(s).License">
    <Value>Download license</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Image">
    <Value>Image</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.General">
    <Value>General</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CheckUsernameAvailability.EnterUsername">
    <Value>Please enter username</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Compare.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.AddToCompareList">
    <Value>Add to compare list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.SystemKeyword.Hint">
    <Value>The system keyword for this poll. For example, you can enter ''LeftColumnPoll'' in order to display it in the left column.</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Email.Placeholder">
    <Value>Enter your email here...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.BodyOverview">
    <Value>Body overview</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.BodyOverview.Hint">
    <Value>Brief overview of blog post. If specified, then it will be used instead of full body on the main blog page. HTML is supported.</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.MoreInfo">
    <Value>details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoice.All">
    <Value>Print PDF invoices (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthRequired">
    <Value>''Date of Birth'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthRequired.Hint">
    <Value>Check if ''Date of Birth'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.DateOfBirth.Required">
    <Value>Check if ''Date of Birth'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.DateOfBirth.Required">
    <Value>Date of birth is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm">
    <Value>Contact us page. ''Subject'' field</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm.Hint">
    <Value>Check to allow a customer to type a subject on the contact us page.</Value>
  </LocaleResource>
  <LocaleResource Name="ContactUs.Subject">
    <Value>Subject</Value>
  </LocaleResource>
  <LocaleResource Name="ContactUs.Subject.Hint">
    <Value>Enter subject</Value>
  </LocaleResource>
  <LocaleResource Name="ContactUs.Subject.Required">
    <Value>Please enter subject</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Subject">
    <Value>Subject</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Subject.Hint">
    <Value>Enter subject</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Subject.Required">
    <Value>Please enter subject</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisablePdfInvoicesForPendingOrders">
    <Value>Disable PDF invoices for pending orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisablePdfInvoicesForPendingOrders.Hint">
    <Value>If checked, customers won''t be allowed to print PDF invoices for pending orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductAttributes.PriceAdjustment">
    <Value>{0} [{1}]</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CheckoutAttributes.PriceAdjustment">
    <Value>{0} [{1}]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.PaymentMethod">
    <Value>Payment method</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.PaymentMethod.Hint">
    <Value>Search by a specific payment method.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.MagnificPopup.Loading">
    <Value>Loading...</Value>
  </LocaleResource>
  <LocaleResource Name="Media.MagnificPopup.Close">
    <Value>Close (Esc)</Value>
  </LocaleResource>
  <LocaleResource Name="Media.MagnificPopup.Previous">
    <Value>Previous (Left arrow key)</Value>
  </LocaleResource>
  <LocaleResource Name="Media.MagnificPopup.Next">
    <Value>Next (Right arrow key)</Value>
  </LocaleResource>
  <LocaleResource Name="Media.MagnificPopup.Counter">
    <Value>%curr% of %total%</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Discounts">
    <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Discounts.NoDiscounts">
    <Value>No discounts available. Create at least one discount before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Discounts.DiscountType.AssignedToManufacturers">
    <Value>Assigned to manufacturers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToManufacturers">
    <Value>Applied to manufacturers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToManufacturers.SaveBeforeEdit">
    <Value>You need to save the discount before you can add manufacturers for this discount page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToManufacturers.Manufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.AppliedToManufacturers.AddNew">
    <Value>Add a new manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.CustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.CustomerRoles.Hint">
    <Value>Search by a specific customer role.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests.Action">
    <Value>Return Action:</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests.Date">
    <Value>Date Requested:</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests.Reason">
    <Value>Return Reason:</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Options">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="News.ViewAll">
    <Value>View News Archive</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.View.Message">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.GiftCardInfo">
    <Value>Gift Card</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.GiftCardInfo.Code">
    <Value>({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Search.SearchKeyword">
    <Value>Search keyword:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceEnabled">
    <Value>PAngV (base price) enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceEnabled.Hint">
    <Value>Check to display baseprice of a product. This is required according to the German law (PAngV). If you sell 500ml of beer for 1,50 euro, then you have to display baseprice: 3.00 euro per 1L.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceAmount">
    <Value>Amount in product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceAmount.Hint">
    <Value>Enter an amount in product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceUnit">
    <Value>Unit of product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceUnit.Hint">
    <Value>Enter a unit of product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceBaseAmount">
    <Value>Reference amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceBaseAmount.Hint">
    <Value>Enter a reference amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceBaseUnit">
    <Value>Reference unit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BasepriceBaseUnit.Hint">
    <Value>Enter a reference unit.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.BasePrice">
    <Value>equates to {0} per {1} {2}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Home">
    <Value>Home</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Description">
    <Value>View the sitemap for this website below, with links to each of the pages and brief descriptions of what to find in each section</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Options.Send">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Configuration > Catalog settings > Perfomance > Ignore "limit per store" rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Configuration > Catalog settings > Perfomance > Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.DisplayOrder.Hint">
    <Value>The topic display order. 1 represents the first item in the list. It''s used with properties such as "Include in top menu" or "Include in footer".</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingReturns">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="PrivacyNotice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ConditionsOfUse">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="AboutUs">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="DownloadableProducts.ReachedMaximumNumber">
    <Value>You have reached maximum number of downloads {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingCountry.Hint">
    <Value>Filter by order billing country.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToCompareList">
    <Value>The product has been added to your product comparison</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToCompareList.Link">
    <Value><![CDATA[The product has been added to your <a href="{0}">product comparison</a>]]></Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageID int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [ID]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageId],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageID,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource


	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.notifycustomeraboutshippingfrommultiplelocations')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.notifycustomeraboutshippingfrommultiplelocations', N'false', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.pricesconsiderpromotions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'frooglesettings.pricesconsiderpromotions', N'false', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowcustomerstocontactvendors')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.allowcustomerstocontactvendors', N'true', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'externalauthenticationsettings.requireemailvalidation')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'externalauthenticationsettings.requireemailvalidation', N'false', 0)
END
GO




--Topic templates
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TopicTemplate]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[TopicTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ViewPath] [nvarchar](400) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[TopicTemplate]
		WHERE [Name] = N'Default template')
BEGIN
	INSERT [dbo].[TopicTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Default template', N'TopicDetails', 1)
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='TopicTemplateId')
BEGIN
	ALTER TABLE [Topic]
	ADD [TopicTemplateId] int NULL
END
GO

UPDATE [Topic]
SET [TopicTemplateId] = 1
WHERE [TopicTemplateId] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [TopicTemplateId] int NOT NULL
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.expirationnumberofdays')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'frooglesettings.expirationnumberofdays', N'28', 0)
END
GO


--shipping by weight plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [StoreId] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''WarehouseId'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [WarehouseId] int NULL

		exec(''UPDATE [ShippingByWeight] SET [WarehouseId] = 0'')
		
		EXEC (''ALTER TABLE [ShippingByWeight] ALTER COLUMN [WarehouseId] int NOT NULL'')
	END')
END
GO


--froogle plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[GoogleProduct]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [StoreId] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[GoogleProduct]'') and NAME=''CustomGoods'')
	BEGIN
		ALTER TABLE [GoogleProduct]
		ADD [CustomGoods] bit NULL

		exec(''UPDATE [GoogleProduct] SET [CustomGoods] = 0'')
		
		EXEC (''ALTER TABLE [GoogleProduct] ALTER COLUMN [CustomGoods] bit NOT NULL'')
	END')
END
GO

--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.topcategorymenusubcategorylevelstodisplay'
GO


--queued email priority
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='Priority')
BEGIN
	EXEC sp_rename 'QueuedEmail.Priority', 'PriorityId', 'COLUMN';
	
	EXEC ('UPDATE [QueuedEmail] SET [PriorityId] = 0 WHERE [PriorityId] <> 5')
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='MaximumDiscountAmount')
BEGIN
	ALTER TABLE [Discount]
	ADD [MaximumDiscountAmount] decimal(18,4) NULL
END
GO


--more indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder' and object_id=object_id(N'[Product_ProductAttribute_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder] ON [Product_ProductAttribute_Mapping] ([ProductId] ASC, [DisplayOrder] ASC)
END
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_ProductAttribute_Mapping_ProductId' and object_id=object_id(N'[Product_ProductAttribute_Mapping]'))
BEGIN
	DROP INDEX [IX_Product_ProductAttribute_Mapping_ProductId] ON [Product_ProductAttribute_Mapping]
END
GO


--more indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder' and object_id=object_id(N'[ProductAttributeValue]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder] ON [ProductAttributeValue] ([ProductAttributeMappingId] ASC, [DisplayOrder] ASC)
END
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductAttributeValue_ProductAttributeMappingId' and object_id=object_id(N'[ProductAttributeValue]'))
BEGIN
	DROP INDEX [IX_ProductAttributeValue_ProductAttributeMappingId] ON [ProductAttributeValue]
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.enablexsrfprotectionforadminarea')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'securitysettings.enablexsrfprotectionforadminarea', N'true', 0)
END
GO


--rename setting
UPDATE [Setting] 
SET [Name] = N'frooglesettings.passshippinginfoweight'
WHERE [Name] = N'frooglesettings.passshippinginfo'
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.passshippinginfodimensions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'frooglesettings.passshippinginfodimensions', N'false', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.newsletterblockallowtounsubscribe')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.newsletterblockallowtounsubscribe', N'false', 0)
END
GO


--'Newsletter unsubscribe' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'NewsLetterSubscription.DeactivationMessage')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'NewsLetterSubscription.DeactivationMessage', null, N'%Store.Name%. Subscription deactivation message.', N'<p><a href="%NewsLetterSubscription.DeactivationUrl%">Click here to unsubscribe from our newsletter.</a></p><p>If you received this email by mistake, simply delete it.</p>', 1, 0, 0, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.enablexsrfprotectionforpublicstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'securitysettings.enablexsrfprotectionforpublicstore', N'true', 0)
END
GO

--Delete setting
DELETE FROM [Setting]
WHERE Name = N'storeinformationsettings.responsivedesignsupported'
GO


IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, LEN(@Keywords) - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, LEN(@Keywords)) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Name]) > 0 '


		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
	

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[ShortDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[ShortDescription]) > 0 '


			--product full description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[FullDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[FullDescription]) > 0 '



			--localized product short description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
				

			--localized product full description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

		--SKU
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[Sku], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[Sku]) > 0 '
		END

		IF @SearchProductTags = 1
		BEGIN
			--product tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pt.[Name], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pt.[Name]) > 0 '

			--localized product tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000)', @Keywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN (SELECT CategoryId FROM #FilteredCategoryIds)'
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by parent product identifer
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--show hidden and ACL
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 FROM #FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM Product_SpecificationAttribute_Mapping psam with (NOLOCK)
					WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
				)
			)'
	END
	
	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #KeywordProducts

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--prepare filterable specification attribute option identifier (if requested)
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
		SELECT DISTINCT [psam].SpecificationAttributeOptionId
		FROM [Product_SpecificationAttribute_Mapping] [psam] with (NOLOCK)
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.honeypotenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'securitysettings.honeypotenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.honeypotinputname')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'securitysettings.honeypotinputname', N'hpinput', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Affiliate]') and NAME='FriendlyUrlName')
BEGIN
	ALTER TABLE [Affiliate]
	ADD [FriendlyUrlName] nvarchar(MAX) NULL
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='IncludeInFooterColumn1')
BEGIN
	ALTER TABLE [Topic]
	ADD [IncludeInFooterColumn1] bit NULL
END
GO

UPDATE [Topic]
SET [IncludeInFooterColumn1] = 0
WHERE [IncludeInFooterColumn1] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [IncludeInFooterColumn1] bit NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='IncludeInFooterColumn2')
BEGIN
	ALTER TABLE [Topic]
	ADD [IncludeInFooterColumn2] bit NULL
END
GO

UPDATE [Topic]
SET [IncludeInFooterColumn2] = 0
WHERE [IncludeInFooterColumn2] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [IncludeInFooterColumn2] bit NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='IncludeInFooterColumn3')
BEGIN
	ALTER TABLE [Topic]
	ADD [IncludeInFooterColumn3] bit NULL
END
GO

UPDATE [Topic]
SET [IncludeInFooterColumn3] = 0
WHERE [IncludeInFooterColumn3] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [IncludeInFooterColumn3] bit NOT NULL
GO

--rename setting
UPDATE [Setting]
SET [Name] = N'forumsettings.activediscussionspagesize'
WHERE [Name] =  N'forumsettings.activediscussionspagetopiccount'
GO

--delete setting
DELETE [Setting]
WHERE [Name] =  N'forumsettings.topicpostspagelinkdisplaycount'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='AccessibleWhenStoreClosed')
BEGIN
	ALTER TABLE [Topic]
	ADD [AccessibleWhenStoreClosed] bit NULL
END
GO

UPDATE [Topic]
SET [AccessibleWhenStoreClosed] = 0
WHERE [AccessibleWhenStoreClosed] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [AccessibleWhenStoreClosed] bit NOT NULL
GO


--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'adminareasettings.displayproductpictures'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Picture]') and NAME='TitleAttribute')
BEGIN
	ALTER TABLE [Picture]
	ADD [TitleAttribute] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Picture]') and NAME='AltAttribute')
BEGIN
	ALTER TABLE [Picture]
	ADD [AltAttribute] nvarchar(MAX) NULL
END
GO

--New table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PredefinedProductAttributeValue]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[PredefinedProductAttributeValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductAttributeId] [int] NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[PriceAdjustment] [decimal](18,4) NOT NULL,
	[WeightAdjustment] [decimal](18,4) NOT NULL,
	[Cost] [decimal](18,4) NOT NULL,
	[IsPreSelected] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PredefinedProductAttributeValue_ProductAttribute'
           AND parent_object_id = Object_id('PredefinedProductAttributeValue')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.PredefinedProductAttributeValue
DROP CONSTRAINT PredefinedProductAttributeValue_ProductAttribute
GO
ALTER TABLE [dbo].[PredefinedProductAttributeValue]  WITH CHECK ADD  CONSTRAINT [PredefinedProductAttributeValue_ProductAttribute] FOREIGN KEY([ProductAttributeId])
REFERENCES [dbo].[ProductAttribute] ([Id])
ON DELETE CASCADE
GO

--rename some properties in attributes (XML)
UPDATE [ShoppingCartItem] 
SET AttributesXml = REPLACE(AttributesXml, 'ProductVariantAttribute', 'ProductAttribute')
GO

UPDATE [OrderItem] 
SET AttributesXml = REPLACE(AttributesXml, 'ProductVariantAttribute', 'ProductAttribute')
GO

UPDATE [ProductAttributeCombination] 
SET AttributesXml = REPLACE(AttributesXml, 'ProductVariantAttribute', 'ProductAttribute')
GO


--more SQL indexes
ALTER TABLE [Customer] ALTER COLUMN [SystemName] nvarchar(400) NULL
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Customer_SystemName' and object_id=object_id(N'[Customer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Customer_SystemName] ON [Customer] ([SystemName] ASC)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.passwordrecoverylinkdaysvalid')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.passwordrecoverylinkdaysvalid', N'7', 0)
END
GO


DELETE FROM [Setting] WHERE [name] = N'mediasettings.productthumbperrowonproductdetailspage'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='BodyOverview')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [BodyOverview] nvarchar(MAX) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.dateofbirthrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.dateofbirthrequired', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.subjectfieldoncontactusform')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.subjectfieldoncontactusform', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'pdfsettings.disablepdfinvoicesforpendingorders')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'pdfsettings.disablepdfinvoicesforpendingorders', N'false', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Manufacturer]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [Manufacturer]
	ADD [HasDiscountsApplied] bit NULL
END
GO

UPDATE [Manufacturer]
SET [HasDiscountsApplied] = 0
WHERE [HasDiscountsApplied] IS NULL
GO

ALTER TABLE [Manufacturer] ALTER COLUMN [HasDiscountsApplied] bit NOT NULL
GO



--New table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Discount_AppliedToManufacturers]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Discount_AppliedToManufacturers](
	[Discount_Id] [int] NOT NULL,
	[Manufacturer_Id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Discount_Id] ASC,
	[Manufacturer_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Discount_AppliedToManufacturers_Source'
           AND parent_object_id = Object_id('Discount_AppliedToManufacturers')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Discount_AppliedToManufacturers
DROP CONSTRAINT Discount_AppliedToManufacturers_Source
GO
ALTER TABLE [dbo].[Discount_AppliedToManufacturers]  WITH CHECK ADD  CONSTRAINT [Discount_AppliedToManufacturers_Source] FOREIGN KEY([Discount_Id])
REFERENCES [dbo].[Discount] ([Id])
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Discount_AppliedToManufacturers_Target'
           AND parent_object_id = Object_id('Discount_AppliedToManufacturers')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Discount_AppliedToManufacturers
DROP CONSTRAINT Discount_AppliedToManufacturers_Target
GO
ALTER TABLE [dbo].[Discount_AppliedToManufacturers]  WITH CHECK ADD  CONSTRAINT [Discount_AppliedToManufacturers_Target] FOREIGN KEY([Manufacturer_Id])
REFERENCES [dbo].[Manufacturer] ([Id])
ON DELETE CASCADE
GO


--'Order refunded' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderRefunded.CustomerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'OrderRefunded.CustomerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% refunded', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href="%Store.URL%">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>', 0, 0, 0, 0)
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='BasepriceEnabled')
BEGIN
	ALTER TABLE [Product]
	ADD [BasepriceEnabled] bit NULL
END
GO

UPDATE [Product]
SET [BasepriceEnabled] = 0
WHERE [BasepriceEnabled] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [BasepriceEnabled] bit NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='BasepriceAmount')
BEGIN
	ALTER TABLE [Product]
	ADD [BasepriceAmount] decimal(18,4) NULL
END
GO

UPDATE [Product]
SET [BasepriceAmount] = 0
WHERE [BasepriceAmount] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [BasepriceAmount] decimal(18,4) NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='BasepriceUnitId')
BEGIN
	ALTER TABLE [Product]
	ADD [BasepriceUnitId] int NULL
END
GO

UPDATE [Product]
SET [BasepriceUnitId] = 0
WHERE [BasepriceUnitId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [BasepriceUnitId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='BasepriceBaseAmount')
BEGIN
	ALTER TABLE [Product]
	ADD [BasepriceBaseAmount] decimal(18,4) NULL
END
GO

UPDATE [Product]
SET [BasepriceBaseAmount] = 0
WHERE [BasepriceBaseAmount] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [BasepriceBaseAmount] decimal(18,4) NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='BasepriceBaseUnitId')
BEGIN
	ALTER TABLE [Product]
	ADD [BasepriceBaseUnitId] int NULL
END
GO

UPDATE [Product]
SET [BasepriceBaseUnitId] = 0
WHERE [BasepriceBaseUnitId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [BasepriceBaseUnitId] int NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultcategorypagesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.defaultcategorypagesize', N'6', 0)
END
GO
--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultmanufacturerpagesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.defaultmanufacturerpagesize', N'6', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='DisplayOrder')
BEGIN
	ALTER TABLE [Topic]
	ADD [DisplayOrder] int NULL
END
GO

UPDATE [Topic]
SET [DisplayOrder] = 1
WHERE [DisplayOrder] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [DisplayOrder] int NOT NULL
GO


--update DefaultClean theme settings. You should remove this code if you're going to use the old theme
IF EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.defaultstoretheme' and [Value] = N'DefaultClean')
BEGIN

	UPDATE [Setting]
	SET [Value] = N'120'
	WHERE [Name] = 'mediasettings.avatarpicturesize'
	
	UPDATE [Setting]
	SET [Value] = N'415'
	WHERE [Name] = 'mediasettings.productthumbpicturesize'
	
	UPDATE [Setting]
	SET [Value] = N'550'
	WHERE [Name] = 'mediasettings.productdetailspicturesize'
	
	UPDATE [Setting]
	SET [Value] = N'100'
	WHERE [Name] = 'mediasettings.productthumbpicturesizeonproductdetailspage'
	
	UPDATE [Setting]
	SET [Value] = N'220'
	WHERE [Name] = 'mediasettings.associatedproductpicturesize'
	
	UPDATE [Setting]
	SET [Value] = N'450'
	WHERE [Name] = 'mediasettings.categorythumbpicturesize'
		
	UPDATE [Setting]
	SET [Value] = N'420'
	WHERE [Name] = 'mediasettings.manufacturerthumbpicturesize'
		
	UPDATE [Setting]
	SET [Value] = N'4'
	WHERE [Name] = 'catalogsettings.numberofbestsellersonhomepage'
		
	UPDATE [Setting]
	SET [Value] = N'true'
	WHERE [Name] = 'newssettings.shownewsonmainpage'
	
	UPDATE [Setting]
	SET [Value] = N'4'
	WHERE [Name] = 'shoppingcartsettings.crosssellsnumber'
	
	UPDATE [Setting]
	SET [Value] = N'6, 3, 9, 18'
	WHERE [Name] = 'catalogsettings.searchpagepagesizeoptions'
	
	UPDATE [Setting]
	SET [Value] = N'6, 3, 9'
	WHERE [Name] = 'catalogsettings.defaultcategorypagesizeoptions'
	
	UPDATE [Setting]
	SET [Value] = N'6, 3, 9'
	WHERE [Name] = 'catalogsettings.defaultmanufacturerpagesizeoptions'
	
	UPDATE [Setting]
	SET [Value] = N'6, 3, 9, 18'
	WHERE [Name] = 'catalogsettings.productsbytagpagesizeoptions'
	
	UPDATE [Setting]
	SET [Value] = N'6, 3, 9'
	WHERE [Name] = 'vendorsettings.defaultvendorpagesizeoptions'
	
	UPDATE [Setting]
	SET [Value] = N'3'
	WHERE [Name] = 'catalogsettings.recentlyviewedproductsnumber'
	
	UPDATE [Setting]
	SET [Value] = N'6'
	WHERE [Name] = 'catalogsettings.recentlyaddedproductsnumber'
	
	UPDATE [Topic]
	SET [DisplayOrder] = 5,
	[IncludeInFooterColumn1] = 1
	WHERE [SystemName] = 'ShippingInfo'
	
	UPDATE [Topic]
	SET [DisplayOrder] = 10,
	[IncludeInFooterColumn1] = 1
	WHERE [SystemName] = 'PrivacyInfo'
	
	UPDATE [Topic]
	SET [DisplayOrder] = 15,
	[IncludeInFooterColumn1] = 1
	WHERE [SystemName] = 'ConditionsOfUse'
	
	UPDATE [Topic]
	SET [DisplayOrder] = 20,
	[IncludeInFooterColumn1] = 1
	WHERE [SystemName] = 'AboutUs'
END
GO

--a stored procedure update
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductTagCountLoadAll]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductTagCountLoadAll]
GO
CREATE PROCEDURE [dbo].[ProductTagCountLoadAll]
(
	@StoreId int
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT pt.Id as [ProductTagId], COUNT(p.Id) as [ProductCount]
	FROM ProductTag pt with (NOLOCK)
	LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON pt.[Id] = pptm.[ProductTag_Id]
	LEFT JOIN Product p with (NOLOCK) ON pptm.[Product_Id] = p.[Id]
	WHERE
		p.[Deleted] = 0
		AND p.Published = 1
		AND (@StoreId = 0 or (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = 'Product' and [sm].StoreId=@StoreId
			)))
	GROUP BY pt.Id
	ORDER BY pt.Id
END
GO



IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[DeleteGuests]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [DeleteGuests]
GO
CREATE PROCEDURE [dbo].[DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT [Id] FROM [Customer] c with (NOLOCK)
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
	AND
	--shopping cart items
	((@OnlyWithoutShoppingCart=0) OR (NOT EXISTS(SELECT 1 FROM [ShoppingCartItem] sci with (NOLOCK) inner join [Customer] with (NOLOCK) on sci.[CustomerId]=c.[Id])))
	AND
	--guests only
	(EXISTS(SELECT 1 FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK) inner join [Customer] with (NOLOCK) on ccrm.[Customer_Id]=c.[Id] inner join [CustomerRole] cr with (NOLOCK) on cr.[Id]=ccrm.[CustomerRole_Id] WHERE cr.[SystemName] = N'Guests'))
	AND
	--no orders
	(NOT EXISTS(SELECT 1 FROM [Order] o with (NOLOCK) inner join [Customer] with (NOLOCK) on o.[CustomerId]=c.[Id]))
	AND
	--no blog comments
	(NOT EXISTS(SELECT 1 FROM [BlogComment] bc with (NOLOCK) inner join [Customer] with (NOLOCK) on bc.[CustomerId]=c.[Id]))
	AND
	--no news comments
	(NOT EXISTS(SELECT 1 FROM [NewsComment] nc  with (NOLOCK)inner join [Customer] with (NOLOCK) on nc.[CustomerId]=c.[Id]))
	AND
	--no product reviews
	(NOT EXISTS(SELECT 1 FROM [ProductReview] pr with (NOLOCK) inner join [Customer] with (NOLOCK) on pr.[CustomerId]=c.[Id]))
	AND
	--no product reviews helpfulness
	(NOT EXISTS(SELECT 1 FROM [ProductReviewHelpfulness] prh with (NOLOCK) inner join [Customer] with (NOLOCK) on prh.[CustomerId]=c.[Id]))
	AND
	--no poll voting
	(NOT EXISTS(SELECT 1 FROM [PollVotingRecord] pvr with (NOLOCK) inner join [Customer] with (NOLOCK) on pvr.[CustomerId]=c.[Id]))
	AND
	--no forum topics 
	(NOT EXISTS(SELECT 1 FROM [Forums_Topic] ft with (NOLOCK) inner join [Customer] with (NOLOCK) on ft.[CustomerId]=c.[Id]))
	AND
	--no forum posts 
	(NOT EXISTS(SELECT 1 FROM [Forums_Post] fp with (NOLOCK) inner join [Customer] with (NOLOCK) on fp.[CustomerId]=c.[Id]))
	AND
	--no system accounts
	(c.IsSystemAccount = 0)
	
	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerID] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityID] IN (SELECT [CustomerID] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO

--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.loadallsidecategorymenusubcategories'
GO
