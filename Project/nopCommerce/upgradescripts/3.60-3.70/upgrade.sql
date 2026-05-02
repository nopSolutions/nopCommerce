--upgrade scripts from nopCommerce 3.60 to 3.70

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Products.MinimumQuantityNotification">
    <Value>This product has a minimum quantity of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart">
    <Value>Display tax/shipping info (shopping cart)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart.Hint">
    <Value>Check to display tax and shipping info on the shopping cart page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage">
    <Value>Default language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage.Hint">
    <Value>This property allows a store owner to specify a default language for a store. If not specified, then the default language display order will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor.Hint">
    <Value>Search by a specific vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OverriddenGiftCardAmount">
    <Value>Overridden gift card amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OverriddenGiftCardAmount.Hint">
    <Value>Enter gift card amount that can be used after purchase. If not specified, then product price will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthMinimumAge">
    <Value>Customer minimum age</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthMinimumAge.Hint">
    <Value>Enter minimum allowed age. Leave empty if customers of all ages are allowed.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.DateOfBirth.MinimumAge">
    <Value>You have to be {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation">
    <Value>Hide delivery information</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation.Hint">
    <Value>Check to hide delivery information as description of returned shipping methods.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.StartDateShouldBeFuture">
    <Value>Rental start date should be the future date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Picture.Hint">
    <Value>The vendor picture.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.VendorThumbPictureSize">
    <Value>Vendor thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.VendorThumbPictureSize.Hint">
    <Value>The default size (pixels) for vendor thumbnail images.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Vendor.ImageAlternateTextFormat">
    <Value>Picture for vendor {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Vendor.ImageLinkTitleFormat">
    <Value>Show products of vendor {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.GroupedProduct">
    <Value>Grouped (product with variants)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.SimpleProduct">
    <Value>Simple</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.AssociatedProducts">
    <Value>Associated products (variants)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToApplyForVendorAccount">
    <Value>Allow customers to apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToApplyForVendorAccount.Hint">
    <Value>Check to allow customers users to fill a form to become a new vendor. Then a store owner will have to manually approve it.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount">
    <Value>Apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email.Hint">
    <Value>Enter your email address</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email.Required">
    <Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name.Hint">
    <Value>Enter vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name.Required">
    <Value>Vendor name is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Submitted">
    <Value>Your request has been submitted successfully. We''ll contact you soon.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Vendors.Apply">
    <Value>Apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Button">
    <Value>Submit</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.AlreadyApplied">
    <Value>You already applied for a vendor account. Please register as a new customer in order to apply for one more vendor account.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsAccumulatedForAllStores">
    <Value>Points accumulated for all stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsAccumulatedForAllStores.Hint">
    <Value>Check to accumulate all reward points in one balance for all stores so they can be used in any store. Otherwise, each store has its own rewards points and they can only be used in that store. WARNING: not recommended to change in production environment with several stores already created.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore.Hint">
    <Value>Choose a store. It''s useful only when you have "Points accumulated for all stores" setting disabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.AllowViewUnpublishedProductPage">
    <Value>Allow viewing of unpublished product details page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.AllowViewUnpublishedProductPage.Hint">
    <Value>Check to allow viewing of unpublished product details page. This way SEO won''t be affected by search crawlers when a product is temporary unpublished. Please note that a store owner always has access to unpublished products.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsedAnymore">
    <Value>Sorry, you''ve used this discount already</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsedWithGiftCards">
    <Value>Sorry, this discount cannot be used with gift cards in the cart</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.Expired">
    <Value>Sorry, this offer is expired</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.NotStartedYet">
    <Value>Sorry, this offer is not started yet</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HadSpentAmount.NotEnough">
    <Value>Sorry, this offer requires more money spent (previously placed orders)</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.AddToCart.Error">
    <Value>Some product(s) from wishlist could not be moved to the cart for some reasons.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Acl">
    <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl">
    <Value>Subject to ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl.Hint">
    <Value>Determines whether the topic is subject to ACL (access control list).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AclCustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the topic will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions">
    <Value>Back in stock subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.CreatedOn">
    <Value>Subscribed on</Value>
  </LocaleResource>
  <LocaleResource Name="Search.Button">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint">
    <Value>Enable to combine (bundle) multiple CSS files into a single file. Do not enable if you''re running nopCommerce in IIS virtual directory. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint">
    <Value>Enable to combine (bundle) multiple JavaScript files into a single file. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition">
    <Value>Condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.ViewLink">
    <Value>View/Edit condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Description">
    <Value>Conditional attributes appear if a previous attribute is selected, such as having an option for personalizing clothing with a name and only providing the text input box if the "Personalize" radio button is checked</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition">
    <Value>Enable condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition.Hint">
    <Value>Check to specify a condition (depending on other attribute) when this attribute should be enabled (visible).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes">
    <Value>Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes.Hint">
    <Value>Choose an attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.EnableDynamicPriceUpdate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.EnableDynamicPriceUpdate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.MinOrderSubtotalAmountIncludingTax">
    <Value>Calculate ''Min order sub-total amount'' including tax</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.MinOrderSubtotalAmountIncludingTax.Hint">
    <Value>Check to calculate ''Min order sub-total amount'' value including tax; otherwise excluding tax.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNew">
    <Value>Mark as new</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNew.Hint">
    <Value>Check to mark this product as New</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNewStartDateTimeUtc">
    <Value>Mark as new. Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNewStartDateTimeUtc.Hint">
    <Value>Set Product as New from Date in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNewEndDateTimeUtc">
    <Value>Mark as new. End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNewEndDateTimeUtc.Hint">
    <Value>Set Product as New to Date in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RecentlyAddedProductsEnabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RecentlyAddedProductsEnabled.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RecentlyAddedProductsNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RecentlyAddedProductsNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NewProductsEnabled">
    <Value>''New products'' page enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NewProductsEnabled.Hint">
    <Value>Check to enable the ''New products'' page in your store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NewProductsNumber">
    <Value>Number of products on ''New products'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NewProductsNumber.Hint">
    <Value>The number of products to display when ''New products'' page is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.RecentlyAddedProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.NewProducts">
    <Value>New Products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Newsletter">
    <Value>Newsletter</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Newsletter.Hint">
    <Value>Check to subscribe to newsletter.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingLastName">
    <Value>Billing last name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingLastName.Hint">
    <Value>Filter by customer billing last name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.CustomerEmail">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.CustomerEmail.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingEmail">
    <Value>Billing email address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingEmail.Hint">
    <Value>Filter by customer billing email address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToSubCategories">
    <Value>Apply to subcategories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToSubCategories.Hint">
    <Value>Check to apply discount to subcategories of the selected parent. But please note that it can affect performance if you have a lot of nested subcategories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyVendorPage">
    <Value>Show on apply for vendor account page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyVendorPage.Hint">
    <Value>Check to show CAPTCHA on apply for vendor account page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons">
    <Value>Return request reasons</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Added">
	<Value>The new return request reason has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.AddNew">
	<Value>Add new return request reason</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.BackToList">
	<Value>back to return request reason list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted">
	<Value>The return request reason has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.DisplayOrder.Hint">
	<Value>The return request reason display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.EditDetails">
	<Value>Edit return request reason details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Hint">
    <Value>List of reasons a customer will be able to choose when submitting a return request.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Hint">
	<Value>The return request reason name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated">
	<Value>The return request reason has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions">
    <Value>Return request actions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Added">
	<Value>The new return request action has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.AddNew">
	<Value>Add new return request action</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.BackToList">
	<Value>back to return request action list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted">
	<Value>The return request action has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.DisplayOrder.Hint">
	<Value>The return request action display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.EditDetails">
	<Value>Edit return request action details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Hint">
    <Value>List of actions a customer will be able to choose when submitting a return request.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Name.Hint">
	<Value>The return request action name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestActions.Updated">
	<Value>The return request action has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.AddNewRecord">
	<Value>Add new record</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreClosedAllowForAdmins">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreClosedAllowForAdmins.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Store.Hint">
    <Value>Filter report by orders placed in a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedProducts">
    <Value>Discontinued message for unpublished products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedProducts.Hint">
    <Value>Check to display "a product has been discontinued" message when viewing details pages of unpublished products.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Discontinued">
    <Value>Sorry - this product is no longer available</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.RemoveDownload">
    <Value>Remove</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.DownloadUploadedFile">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.ReasonForReturn.Required">
    <Value>Reason for return is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.RequestedAction.Required">
    <Value>Requested action is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes">
    <Value>Vendor notes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.AddButton">
    <Value>Add vendor note</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.AddTitle">
    <Value>Add vendor note</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.Fields.CreatedOn">
    <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.Fields.Note">
    <Value>Note</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.Fields.Note.Hint">
    <Value>Enter this vendor note message.</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Message.ReturnedForOrder">
    <Value>Returned back for order #{0}</Value>
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



IF NOT EXISTS (
    SELECT 1
    FROM [MessageTemplate]
    WHERE [Name] = N'OrderRefunded.CustomerNotification')
BEGIN
  INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
  VALUES (N'OrderRefunded.CustomerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% refunded', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href="%Store.URL%">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>', 0, 0, 0, 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoshoppingcart')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoshoppingcart', N'false', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='DefaultLanguageId')
BEGIN
	ALTER TABLE [Store]
	ADD [DefaultLanguageId] int NULL
END
GO

UPDATE [Store]
SET [DefaultLanguageId] = 0
WHERE [DefaultLanguageId] IS NULL
GO

ALTER TABLE [Store] ALTER COLUMN [DefaultLanguageId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='OverriddenGiftCardAmount')
BEGIN
	ALTER TABLE [Product]
	ADD [OverriddenGiftCardAmount] decimal NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.dateofbirthminimumage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.dateofbirthminimumage', N'', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='PictureId')
BEGIN
	ALTER TABLE [Vendor]
	ADD [PictureId] int NULL
END
GO

UPDATE [Vendor]
SET [PictureId] = 0
WHERE [PictureId] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [PictureId] int NOT NULL
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.vendorthumbpicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.vendorthumbpicturesize', N'450', 0)
END
GO

--rename some product templates
UPDATE [ProductTemplate]
SET [Name] = 'Grouped product (with variants)'
WHERE [ViewPath] = N'ProductTemplate.Grouped'
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowcustomerstoapplyforvendoraccount')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.allowcustomerstoapplyforvendoraccount', N'false', 0)
END
GO

--new topic
IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Topic]
  WHERE [SystemName] = N'ApplyVendor')
BEGIN
	INSERT [dbo].[Topic] ([SystemName], [TopicTemplateId], [IncludeInSitemap], [AccessibleWhenStoreClosed], [LimitedToStores], [IncludeInFooterColumn1], [IncludeInFooterColumn2], [IncludeInFooterColumn3], [IncludeInTopMenu], [IsPasswordProtected], [DisplayOrder] , [Title], [Body])
	VALUES (N'ApplyVendor', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, N'', N'<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>')
END
GO


--'New vendor account submitted' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'VendorAccountApply.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'VendorAccountApply.StoreOwnerNotification', null, N'%Store.Name%. New vendor account submitted.', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just submitted for a vendor account. Details are below:<br />Vendor name: %Vendor.Name%<br />Vendor email: %Vendor.Email%<br /><br />You can activate it in admin area.</p>', 1, 0, 0, 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='StoreId')
BEGIN
	ALTER TABLE [RewardPointsHistory]
	ADD [StoreId] int NULL
END
GO

--just use the first store
--we cannot find original store IDs of some orders
--furthermore, it won't work for points granted for registration (if enabled)
UPDATE [RewardPointsHistory]
SET [StoreId] = (SELECT TOP 1 [Id] from [Store])
WHERE [StoreId] IS NULL
GO


ALTER TABLE [RewardPointsHistory] ALTER COLUMN [StoreId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.pointsaccumulatedforallstores')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.pointsaccumulatedforallstores', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.allowviewunpublishedproductpage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.allowviewunpublishedproductpage', N'true', 0)
END
GO


--'Order refunded' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderRefunded.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'OrderRefunded.StoreOwnerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% refunded', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Order #%Order.OrderNumber% has been just refunded<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Date Ordered: %Order.CreatedOn%</p>', 0, 0, 0, 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [Topic]
	ADD [SubjectToAcl] bit NULL
END
GO

UPDATE [Topic]
SET [SubjectToAcl] = 0
WHERE [SubjectToAcl] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [SubjectToAcl] bit NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ScheduleTask]') and NAME='LeasedByMachineName')
BEGIN
	ALTER TABLE [ScheduleTask]
	ADD [LeasedByMachineName] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ScheduleTask]') and NAME='LeasedUntilUtc')
BEGIN
	ALTER TABLE [ScheduleTask]
	ADD [LeasedUntilUtc] datetime NULL
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ConditionAttributeXml')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ConditionAttributeXml] nvarchar(MAX) NULL
END
GO

--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.dynamicpriceupdateajax'
GO


--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.enabledynamicpriceupdate'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.ajaxprocessattributechange')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.ajaxprocessattributechange', N'true', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.minordersubtotalamountincludingtax')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.minordersubtotalamountincludingtax', N'false', 0)
END
GO





--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='MarkAsNew')
BEGIN
	ALTER TABLE [Product]
	ADD [MarkAsNew] bit NULL
END
GO

UPDATE [Product]
SET [MarkAsNew] = 0
WHERE [MarkAsNew] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [MarkAsNew] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='MarkAsNewStartDateTimeUtc')
BEGIN
	ALTER TABLE [Product]
	ADD [MarkAsNewStartDateTimeUtc] datetime NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='MarkAsNewEndDateTimeUtc')
BEGIN
	ALTER TABLE [Product]
	ADD [MarkAsNewEndDateTimeUtc] datetime NULL
END
GO


--a stored procedure update
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
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
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
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
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

--renamed settings
UPDATE [Setting]
SET [Name] = N'catalogsettings.newproductsnumber'
WHERE [Name] = N'catalogsettings.recentlyaddedproductsnumber'
GO

UPDATE [Setting]
SET [Name] = N'catalogsettings.newproductsenabled'
WHERE [Name] = N'catalogsettings.recentlyaddedproductsenabled'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='AppliedToSubCategories')
BEGIN
	ALTER TABLE [Discount]
	ADD [AppliedToSubCategories] bit NULL
END
GO

UPDATE [Discount]
SET [AppliedToSubCategories] = 0
WHERE [AppliedToSubCategories] IS NULL
GO

ALTER TABLE [Discount] ALTER COLUMN [AppliedToSubCategories] bit NOT NULL
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Category]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [Category] DROP COLUMN [HasDiscountsApplied]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Manufacturer]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [Manufacturer] DROP COLUMN [HasDiscountsApplied]
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.publishbackproductwhencancellingorders')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.publishbackproductwhencancellingorders', N'false', 0)
END
GO


--move return request actions to table (this way we can localize them)
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReturnRequestAction]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[ReturnRequestAction](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](400) NOT NULL,
		[DisplayOrder] [int] NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
	
	DECLARE @ReturnRequestActions nvarchar(1000)
	SELECT @ReturnRequestActions = [Value] FROM [Setting] WHERE [name] = N'ordersettings.returnrequestactions'
	SET @ReturnRequestActions = isnull(@ReturnRequestActions, '')
	INSERT INTO [ReturnRequestAction] ([Name], [DisplayOrder])
	SELECT [data], 1 FROM [nop_splitstring_to_table](@ReturnRequestActions, ',')

	DELETE FROM [Setting] WHERE [name] = N'ordersettings.returnrequestactions'
END
GO


--move return request reasons to table (this way we can localize them)
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReturnRequestReason]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[ReturnRequestReason](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](400) NOT NULL,
		[DisplayOrder] [int] NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
	
	DECLARE @ReturnRequestReasons nvarchar(1000)
	SELECT @ReturnRequestReasons = [Value] FROM [Setting] WHERE [name] = N'ordersettings.returnrequestreasons'
	SET @ReturnRequestReasons = isnull(@ReturnRequestReasons, '')
	INSERT INTO [ReturnRequestReason] ([Name], [DisplayOrder])
	SELECT [data], 1 FROM [nop_splitstring_to_table](@ReturnRequestReasons, ',')

	DELETE FROM [Setting] WHERE [name] = N'ordersettings.returnrequestreasons'
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.hidepoweredbynopCommerce')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'storeinformationsettings.hidepoweredbynopCommerce', N'false', 0)
END
GO

--update a state abbreviation
UPDATE [StateProvince]
SET [Abbreviation] = N'YT'
WHERE [Name] = N'Yukon Territory'
GO


--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'AccessClosedStore')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Public store. Access a closed store', N'AccessClosedStore', N'PublicStore')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admin role by default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO


--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'storeinformationsettings.storeclosedallowforadmins'
GO


UPDATE [ScheduleTask]
SET [Seconds] = 3600
WHERE [Type] = N'Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services'
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaydiscontinuedmessageforunpublishedproducts')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaydiscontinuedmessageforunpublishedproducts', N'true', 0)
END
GO




--new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VendorNote]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[VendorNote](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[VendorId] [int] NOT NULL,
		[Note] [nvarchar](MAX) NOT NULL,
		[CreatedOnUtc] [datetime] NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'VendorNote_Vendor'
           AND parent_object_id = Object_id('VendorNote')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.VendorNote
DROP CONSTRAINT VendorNote_Vendor
GO
ALTER TABLE [dbo].[VendorNote]  WITH CHECK ADD  CONSTRAINT [VendorNote_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendor] ([Id])
ON DELETE CASCADE
GO



--updated a stored procedure
ALTER PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
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
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
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
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
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
	
	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT [SpecificationAttributeOptionId]
		FROM [#FilteredSpecs]
		OPEN cur_SpecificationAttributeOption
		FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId = ' + CAST(@SpecificationAttributeOptionId AS nvarchar(max)) + ')'
			--fetch next identifier
			FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		END
		CLOSE cur_SpecificationAttributeOption
		DEALLOCATE cur_SpecificationAttributeOption
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'pdfsettings.fontfilename')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'pdfsettings.fontfilename', N'FreeSerif.ttf', 0)
END
GO