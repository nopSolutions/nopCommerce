--upgrade scripts from nopCommerce 4.00 to 4.10

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>  
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.CurrencyCode.Hint">
    <Value>The currency code. For a list of currency codes, go to: https://en.wikipedia.org/wiki/ISO_4217</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Customers.Customers.Fields.Avatar">
    <Value>Avatar</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages">
    <Value>Export/Import products. Allow download images</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages.Hint">
    <Value>Check if images can be downloaded from remote server when exporting products</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchKeywords">
    <Value>Search keywords</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchKeywords.Hint">
    <Value>Search topic(s) by specific keywords.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore.Hint">
    <Value>Check to display reviews written in the current store only (on a product details page and on the account product reviews page).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ViewMode.Grid">
    <Value>Grid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ViewMode.List">
    <Value>List</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DefaultViewMode">
    <Value>Default view mode</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DefaultViewMode.Hint">
    <Value>Choose the default view mode for catalog pages.</Value>
  </LocaleResource>     
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchEndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchEndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchStartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchStartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.AdminApproval">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.Disabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.EmailValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.Standard">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.AdminApproval">
    <Value>A customer should be approved by administrator</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.Disabled">
    <Value>Registration is disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.EmailValidation">
    <Value>Email validation is required after registration</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.Standard">
    <Value>Standard account creation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Sku.Reserved">
    <Value>The entered SKU is already reserved for the product ''{0}''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved">
    <Value>The entered SKU is already reserved for one of combinations of the product ''{0}''</Value>
  </LocaleResource>  
  <LocaleResource Name="Products.Availability.SelectRequiredAttributes">
    <Value>Please select required attribute(s)</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.VendorName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Product(s).VendorName">
    <Value>Vendor name</Value>
  </LocaleResource> 
  <LocaleResource Name="ShoppingCart.VendorName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage">
    <Value>Show vendor name on order details page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage.Hint">
    <Value>Check to show vendor name of product on the order details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Edit">
    <Value>Edit combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.Hint">
    <Value>Choose a picture associated to this attribute combination. This picture will replace the main product image when this product attribute combination is selected.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.NoPicture">
    <Value>No picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountUrl">
    <Value>URL with coupon code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountUrl.Hint">
    <Value>The sample link that includes a discount coupon code, so that customers do not have to input the coupon code at checkout. You can also use this query parameter with any other link to your store, for example link to certain product or category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NotifyCustomerAboutProductReviewReply">
    <Value>Notify customer about product review reply</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NotifyCustomerAboutProductReviewReply.Hint">
    <Value>Check to notify customer about product review reply.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.ProductReview.Reply.CustomerNotification">
    <Value><![CDATA[This message template is used to notify customers when a store owner (or vendor) replies to their product reviews. You can set up this option by ticking the checkbox <strong>Notify customer about product review reply</strong> in Configuration - Settings - Catalog settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPaid.AffiliateNotification">
	  <Value>This message template is used to notify an affiliate that the certain order was paid. The order gets the status Paid when the amount was charged.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPlaced.AffiliateNotification">
	  <Value>This message template is used to notify an affiliate that the certain order was placed.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Common.SaveChanges">
	  <Value>Save changes</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Common.CancelChanges">
	  <Value>Cancel changes</Value>
  </LocaleResource>      
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts">
    <Value>Used by products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Product">
    <Value>Product</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Published">
    <Value>Published</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SecureUrl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SecureUrl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Url.Hint">
    <Value>The URL of your store e.g. http://www.yourstore.com/ or https://www.yourstore.com/mystore/.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewsSortByCreatedDateAscending">
	  <Value>Sort by ascending</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewsSortByCreatedDateAscending.Hint">
	  <Value>Check if the product reviews should be sorted by creation date as ascending</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportSplitProductsFile">
    <Value>Export/Import products. Allow splitting file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportSplitProductsFile.Hint">
    <Value>Check if you want to import products from individual files of the optimal size, which were automatically created from the main file. This function will help you import a large amount of data with a smaller delay.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.CurrentCarts.CartsAndWishlists">
    <Value>Shopping carts and wishlists</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.CurrentCarts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrentWishlists">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ShoppingCartType.ShoppingCartType">
    <Value>Shopping cart type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.ShoppingCartType.Hint">
    <Value>Choose a shopping cart type.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Customers.Customers.CurrentShoppingCart">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Customers.Customers.ShoppingCartAndWishlist">
    <Value>Current shopping cart and wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.CurrentWishlist">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustmentUsePercentage">
    <Value>Price adjustment. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustmentUsePercentage.Hint">
    <Value>Determines whether to apply a percentage to the product. If not enabled, a fixed value is used.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustmentUsePercentage">
    <Value>Price adjustment. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustmentUsePercentage.Hint">
    <Value>Determines whether to apply a percentage to the product. If not enabled, a fixed value is used.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustment.Hint">
    <Value>The price adjustment applied when choosing this attribute value. For example ''10'' to add 10 dollars. Or 10% if ''Use percentage'' is ticked.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustment.Hint">
    <Value>The price adjustment applied when choosing this attribute value. For example ''10'' to add 10 dollars. Or 10% if ''Use percentage'' is ticked.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Vendors.VendorAttributes">
    <Value>Vendor attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Added">
    <Value>The new attribute has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.AddNew">
    <Value>Add new vendor attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.BackToList">
    <Value>back to vendor settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Deleted">
    <Value>The attribute has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Description">
    <Value>You can manage additional vendor attributes below.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.EditAttributeDetails">
    <Value>Edit vendor attribute details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.AttributeControlType">
    <Value>Control type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.AttributeControlType.Hint">
    <Value>Choose how to display your attribute values.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.DisplayOrder.Hint">
    <Value>The vendor attribute display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.IsRequired">
    <Value>Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.IsRequired.Hint">
    <Value>When an attribute is required, vendors must choose an appropriate attribute value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.Name.Hint">
    <Value>The name of the vendor attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Info">
    <Value>Attribute info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Updated">
    <Value>The attribute has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values">
    <Value>Attribute values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.AddNew">
    <Value>Add a new attribute value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.EditValueDetails">
    <Value>Edit attribute value details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder.Hint">
    <Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected">
    <Value>Pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.Name.Hint">
    <Value>The name of the vendor attribute value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorAttributes.Values.SaveBeforeEdit">
    <Value>You need to save the vendor attribute before you can add values for it.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewVendorAttribute">
    <Value>Added a new vendor attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewVendorAttributeValue">
    <Value>Added a new vendor attribute value (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteVendorAttribute">
    <Value>Deleted a vendor attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteVendorAttributeValue">
    <Value>Deleted a vendor attribute value (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditVendorAttribute">
    <Value>Edited a vendor attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditVendorAttributeValue">
    <Value>Edited a vendor attribute value (ID = {0})</Value>
  </LocaleResource> 
  <LocaleResource Name="Account.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Address.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Address.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Hint">
    <Value>Enter county.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled">
    <Value>''County'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled.Hint">
    <Value>Set if ''County'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired">
    <Value>''County'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired.Hint">
    <Value>Check if ''County'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled">
    <Value>''County'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled.Hint">
    <Value>Set if ''County'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired">
    <Value>''County'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired.Hint">
    <Value>Check if ''County'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Hint">
    <Value>The county.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Address.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County.Hint">
    <Value>Search by a specific county.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.CategoriesWithSameNameNotSupported">
    <Value>Categories with the same name are not supported in the same category level. Check your category list in "Catalog -> Categories" page</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem">
    <Value>Display "Apply for vendor account"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem.Hint">
    <Value>Check if "Apply for vendor account" menu item should be displayed in the footer. Vendor functionality should be also enabled in this case.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem">
    <Value>Display "Blog"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem.Hint">
    <Value>Check if "Blog" menu item should be displayed in the footer. Blog functionality should be also enabled in this case.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem">
    <Value>Display "Compare products list"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem.Hint">
    <Value>Check if "Compare products list" menu item should be displayed in the footer. Compare products functionality should be also enabled in this case.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem">
    <Value>Display "Contact us"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem.Hint">
    <Value>Check if "Contact us" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem">
    <Value>Display "Addresses"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem.Hint">
    <Value>Check if "Addresses" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem">
    <Value>Display "My account"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem.Hint">
    <Value>Check if "My account" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem">
    <Value>Display "Orders"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem.Hint">
    <Value>Check if "Orders" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem">
    <Value>Display "Forums"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem.Hint">
    <Value>Check if "Forums" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem">
    <Value>Display "New products"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem.Hint">
    <Value>Check if "New products" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem">
    <Value>Display "News"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem.Hint">
    <Value>Check if "News" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem">
    <Value>Display "Search"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem.Hint">
    <Value>Check if "Search" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem">
    <Value>Display "Recently viewed products"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem.Hint">
    <Value>Check if "Recently viewed products" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem">
    <Value>Display "Shopping cart"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem.Hint">
    <Value>Check if "Shopping cart" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem">
    <Value>Display "Sitemap"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem.Hint">
    <Value>Check if "Sitemap" menu item should be displayed in the footer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem">
    <Value>Display "Wishlist"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem.Hint">
    <Value>Check if "Wishlist" menu item should be displayed in the footer.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.FooterItems">
    <Value>Footer items</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Orders.List.BillingPhone">
    <Value>Billing phone number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.BillingPhone.Hint">
    <Value>Filter by customer billing phone number.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapPageSize">
    <Value>Sitemap page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapPageSize.Hint">
    <Value>A number of items displayed on one sitemap page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.GenerateSeveral">
    <Value>Generate several combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.GenerateSeveralTitle">
    <Value>Choose some attribute values to generate necessary combinations</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Generate">
    <Value>Generate</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.RequiredAttribute">
    <Value>* - required attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.SelectRequiredAttributes">
    <Value>There are required attributes: {0}</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.ActivatePointsImmediately">
    <Value>Activate points immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.ActivatePointsImmediately.Hint">
    <Value>Activate bonus points immediately after they are added.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.ActivationDelay">
    <Value>Reward points activation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.ActivationDelay.Hint">
    <Value>Specify how many days (hours) must elapse before earned points become active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Message.Hint">
    <Value>Enter message (comment).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Points.Hint">
    <Value>Enter points to add. Negative values are also supported (reduce points).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Store.Hint">
    <Value>Choose a store. It''s useful only when you have "Points accumulated for all stores" setting disabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages.Hint">
    <Value>By default not all site pages are SSL protected. Check to force SSL for the entire site. This setting is highly recommended when you have SSL enabled on your store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Date">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.CreatedDate">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Fields.Date">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Fields.CreatedDate">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Fields.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Message.Expired">
    <Value>Unused reward points from {0} have expired</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.RegistrationPointsValidity">
    <Value>Registration points validity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.RegistrationPointsValidity.Hint">
    <Value>Specify number of days when the points awarded for registration will be valid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.RegistrationPointsValidity.Postfix">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PurchasesPointsValidity">
    <Value>Purchases points validity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PurchasesPointsValidity.Hint">
    <Value>Specify number of days when the points awarded for purchases will be valid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PurchasesPointsValidity.Postfix">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.PointsValidity">
    <Value>Points validity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.PointsValidity.Hint">
    <Value>Specify number of days when the awarded points will be valid (only for positive amount of points).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.PointsValidity.Postfix">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinOrderTotalToAwardPoints">
    <Value>Minimum order total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinOrderTotalToAwardPoints.Hint">
    <Value>Specify the minimum order total (exclude shipping cost) to award points for purchases.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CheckoutDisabled">
    <Value>Checkout disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CheckoutDisabled.Hint">
    <Value>Check to disable the checkout process (a read-only mode where ordering is turned off temporarily).</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.Disabled">
    <Value>Sorry, checkout process is temporary disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.LimitedToStores">
    <Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.LimitedToStores.Hint">
    <Value>Option to limit this poll to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RemoveRequiredProducts">
    <Value>Remove required products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RemoveRequiredProducts.Hint">
    <Value>Remove required products from the cart if the main one is removed.</Value>
  </LocaleResource>  
  <LocaleResource Name="ShoppingCart.RequiredProductWarning">
    <Value>This product requires the following product is added to the cart in the quantity of {1}: {0}</Value>
  </LocaleResource>  
  <LocaleResource Name="ShoppingCart.RequiredProductUpdateWarning">
    <Value>This product is required in the quantity of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From.Hint">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To.Hint">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.WeightFrom">
    <Value>Order weight from</Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.WeightFrom.Hint">
    <Value>Order weight from.</Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.WeightTo">
    <Value>Order weight to</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.WeightTo.Hint">
    <Value>Order weight to.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom">
    <Value>Order subtotal from</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom.Hint">
    <Value>Order subtotal from.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo">
    <Value>Order subtotal to</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo.Hint">
    <Value>Order subtotal to.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.OrderSubtotalFrom">
    <Value>Order subtotal from</Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.OrderSubtotalFrom.Hint">
    <Value>Order subtotal from.</Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.OrderSubtotalTo">
    <Value>Order subtotal to</Value>
  </LocaleResource> 
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.OrderSubtotalTo.Hint">
    <Value>Order subtotal to.</Value>
  </LocaleResource>    
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.ShippingByWeight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fixed">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Rate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Store">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Store.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Warehouse">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Warehouse.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Country">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Country.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.StateProvince">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.StateProvince.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Zip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Zip.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod.Hint">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.DataHtml">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.AddRecord">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Formula">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Formula.Value">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.ShippingByWeight">
    <Value>By Weight/Total</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fixed">
    <Value>Fixed Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Rate">
    <Value>Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Store.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Country.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Zip">
    <Value>Zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.Zip.Hint">
    <Value>Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod">
    <Value>Shipping method</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod.Hint">
    <Value>Choose shipping method.</Value>
  </LocaleResource>  
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.From">
    <Value>Order weight from</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.From.Hint">
    <Value>Order weight from.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.To">
    <Value>Order weight to</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.To.Hint">
    <Value>Order weight to.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost">
    <Value>Additional fixed cost</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost.Hint">
    <Value>Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don''t want an additional fixed cost to be applied.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit">
    <Value>Lower weight limit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit.Hint">
    <Value>Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal">
    <Value>Charge percentage (of subtotal)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal.Hint">
    <Value>Charge percentage (of subtotal).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit">
    <Value>Rate per weight unit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit.Hint">
    <Value>Rate per weight unit.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated">
    <Value>Limit shipping methods to configured ones</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated.Hint">
    <Value>If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they''ll be able to choose any existing shipping options even they are not configured here (zero shipping fee in this case).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom">
    <Value>Order weight from</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom.Hint">
    <Value>Order weight from.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo">
    <Value>Order weight to</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo.Hint">
    <Value>Order weight to.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.DataHtml">
    <Value>Data</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.AddRecord">
    <Value>Add record</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Formula">
    <Value>Formula to calculate rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Formula.Value">
    <Value>[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.AddingZeroValueNotAllowed">
    <Value>Adding a new row with zero value isn''t allowed</Value>
  </LocaleResource>  
  <LocaleResource Name="Account.Fields.Username.NotValid">
    <Value>Username is not valid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationEnabled">
    <Value>Username validation is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationEnabled.Hint">
    <Value>Check to enable username validation (when registering or changing on the "My Account" page)</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationUseRegex">
    <Value>Use regex for username validation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationUseRegex.Hint">
    <Value>Check to use a regular expression for username validation (when registering or changing on the "My Account" page)</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationRule">
    <Value>Username validation rule</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UsernameValidationRule.Hint">
    <Value>Set the validation rule for username. You can specify a list of allowed characters or a regular expression. If you use a regular expression check the "Use regex for username validation" setting.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerSettings.RegexValidationRule.Error">
    <Value>The regular expression for username validation is incorrect</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeleteGiftCardUsageHistory">
    <Value>Delete gift card usage history after order cancellation</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeleteGiftCardUsageHistory.Hint">
    <Value>Check to delete gift card usage history after order cancellation</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod.MultipleWarehouse">
    <Value>(multi-warehouse)</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage.DefaultItemText">
    <Value>---</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountLimitation.Hint">
    <Value>Choose the limitation of discount. This parameter will not be taken into account for recurring products/orders.</Value>
  </LocaleResource> 
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version1">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version2">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Common.WrongCaptchaV2">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Common.WrongCaptcha">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Common.WrongCaptchaMessage">
    <Value>The reCAPTCHA response is invalid or malformed. Please try again.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.PaymentMethods">
    <Value>Payment methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.PaymentMethodsAndRestrictions">
    <Value>Payment methods and restrictions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Payment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MaximumRewardPointsToUsePerOrder">
    <Value>Maximum reward points to use per order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MaximumRewardPointsToUsePerOrder.Hint">
    <Value>Customers won''t be able to use more than X reward points per one order. Set to 0 if you do not want to use this setting.</Value>
  </LocaleResource>  
  <LocaleResource Name="Checkout.UseRewardPoints">
    <Value>Use my reward points, {0} reward points ({1}) available for this order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName">
    <Value>Export/Import related entities using name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName.Hint">
    <Value>Check if related entities should be exported/imported using name.</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Catalog.Products.Import.ManufacturersDontExist">
    <Value>Manufacturers with the following names and/or IDs don''t exist: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.CategoriesDontExist">
    <Value>Categories with the following names and/or IDs don''t exist: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.Note">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.ChooseZone">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.ChooseZone.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.TrackingScript.Hint">
    <Value>Paste the tracking code generated by Google Analytics here. {GOOGLEID} and {CUSTOMER_TRACKING} will be dynamically replaced.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr">
    <Value>GDPR settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.BlockTitle.Consents">
    <Value>Consents</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Added">
    <Value>The new consent has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.AddNew">
    <Value>Add consent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.BackToList">
    <Value>back to consent list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Deleted">
    <Value>The consent has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayDuringRegistration">
    <Value>Display during registration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayDuringRegistration.Hint">
    <Value>Check to display this consent on the registration page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayOnCustomerInfoPage">
    <Value>Display on ''customer info'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayOnCustomerInfoPage.Hint">
    <Value>Check to display this consent on the ''customer info'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.DisplayOrder.Hint">
    <Value>The consent display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Edit">
    <Value>Edit consent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.IsRequired">
    <Value>Is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.IsRequired.Hint">
    <Value>Check if this consent is required to be ticked.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Message">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Message.Hint">
    <Value>Enter message (question) displayed to customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Message.Required">
    <Value>Please provide a message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage">
    <Value>Required message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Hint">
    <Value>Enter message (error) displayed when this consent is not ticked by a customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required">
    <Value>Please provide a required message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.Consent.Updated">
    <Value>The GDPR consent has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.GdprEnabled">
    <Value>GDPR enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.GdprEnabled.Hint">
    <Value>Check to enable GDPR (General Data Protection Regulation).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogNewsletterConsent">
    <Value>Log "newsletter" consent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogNewsletterConsent.Hint">
    <Value>Check to log "newsletter" consent (if this feature is enabled in your store).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogPrivacyPolicyConsent">
    <Value>Log "accept privacy policy" consent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogPrivacyPolicyConsent.Hint">
    <Value>Check to log "accept privacy policy" consent (if this feature is enabled in your store).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Gdpr.GdprRequestType.ConsentAgree">
    <Value>Consent (agree)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Gdpr.GdprRequestType.ConsentDisagree">
    <Value>Consent (disagree)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Gdpr.GdprRequestType.ExportData">
    <Value>Export data</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Gdpr.GdprRequestType.DeleteCustomer">
    <Value>Delete customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog">
    <Value>GDPR requests (log)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.Fields.CreatedOn">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.Fields.CustomerInfo">
    <Value>Customer info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.Fields.RequestDetails">
    <Value>Request details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.Fields.RequestType">
    <Value>Request type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.List.SearchEmail">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.List.SearchEmail.Hint">
    <Value>Search by a specific email (exact match).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.List.SearchRequestType">
    <Value>Request type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.GdprLog.List.SearchRequestType.Hint">
    <Value>Search by request type.</Value>
  </LocaleResource>
  <LocaleResource Name="Gdpr.DeleteRequested">
    <Value>Requested to delete account</Value>
  </LocaleResource>
  <LocaleResource Name="Gdpr.DeleteRequested.Success">
    <Value>We''ll process your request as soon as possible</Value>
  </LocaleResource>
  <LocaleResource Name="Gdpr.Exported">
    <Value>Exported personal data</Value>
  </LocaleResource>
  <LocaleResource Name="Gdpr.Consent.Newsletter">
    <Value>Newsletter</Value>
  </LocaleResource>
  <LocaleResource Name="Gdpr.Consent.PrivacyPolicy">
    <Value>Privacy policy</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr">
    <Value>GDPR tools</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Delete">
    <Value>Right to be Forgotten</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Delete.Button">
    <Value>Delete account</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Delete.Hint">
    <Value>You can use the button below to remove your personal and other data from our store. Keep in mind that this process will delete your account, so you will no longer be able to access or use it anymore.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Export">
    <Value>Export personal Data</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Export.Button">
    <Value>Export</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Gdpr.Export.Hint">
    <Value>You can use the button below to download all the data we store and use for a better experience in our store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Gdpr">
    <Value>GDPR</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Gdpr.Delete">
    <Value>Permanent delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Gdpr.Export">
    <Value>Export data</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.AttributeControlType.ImageSquares">
    <Value>Image squares</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.DeleteConfirmation.Selected">
    <Value>Are you sure you want to delete selected items?</Value>
  </LocaleResource>
  <LocaleResource Name="Search.SearchBox.SearchPageLink">
    <Value>View all results...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowLinkToAllResultInSearchAutoComplete">
    <Value>Show a link to all search results in the autocomplete box</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowLinkToAllResultInSearchAutoComplete.Hint">
    <Value>Determines whether the link to all results should be displayed in the autocomplete search box. Displayed if the number of items found is greater than the displayed quantity in the autocomplete box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.ReIndexTables">
    <Value>Re-index database tables</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.ReIndexTables.Complete">
    <Value>Re-indexing of database tables complete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.ReIndexTables.Lable">
    <Value>Modifies existing tables by rebuilding the index. When you execute re-indexing in a table, only the statistics associated with the indexes are updated. Automatic or manual statistics created in the table (instead of an index) are not updated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.ReIndexTables.Progress">
    <Value>Re-indexing...</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.System.Maintenance.ReIndexTables.ReIndexNow">
    <Value>Re-index</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.PreOrderAvailability">
    <Value>pre-order availability</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability">
    <Value>Display the date for a pre-order availability</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability.Hint">
    <Value>Check to display the date for pre-order availability.</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AllowCustomersToCheckGiftCardBalance">
    <Value>Allow customers to check gift card balance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AllowCustomersToCheckGiftCardBalance.Hint">
    <Value>Check to allow customers to check gift card balance. If checked, then CAPTCHA setting must be enabled in the admin area. This feature is potentially not safe and CAPTCHA is needed to prevent and complicate bruteforce.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.CheckGiftCardBalance">
    <Value>Check gift card balance</Value>
  </LocaleResource>
  <LocaleResource Name="CheckGiftCardBalance">
    <Value>Check gift card balance</Value>
  </LocaleResource>
  <LocaleResource Name="CheckGiftCard.GiftCardCouponCode.Button">
    <Value>Check gift card</Value>
  </LocaleResource>
  <LocaleResource Name="CheckGiftCardBalance.GiftCardCouponCode.Invalid">
    <Value>Coupon code is not valid.</Value>
  </LocaleResource>
  <LocaleResource Name="CheckGiftCardBalance.GiftCardCouponCode.Empty">
    <Value>Coupon code is empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.Description">
    <Value>Database backup functionality works only when your nopCommerce application is deployed on the same server as the database. Otherwise you will have to take care of the backup yourself (contact your system administrator).</Value>
  </LocaleResource>
  <LocaleResource Name="Catalog.OrderBy.Label">
    <Value>Select product sort order</Value>
  </LocaleResource>
  <LocaleResource Name="Catalog.PageSize.Label">
    <Value>Select number of products per page</Value>
  </LocaleResource>
  <LocaleResource Name="Currency.Selector.Label">
    <Value>Currency selector</Value>
  </LocaleResource>
  <LocaleResource Name="Search.SearchBox.Text.Label">
    <Value>Search store</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Label">
    <Value>Enter discount coupon code</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCardCouponCode.Label">
    <Value>Enter gift card code</Value>
  </LocaleResource>	
  <LocaleResource Name="Admin.System.Warnings.PluginNotEnabled">
    <Value>You could uninstall and remove the plugin(s) which you don''t use, this might increase performance</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.System.Warnings.Errors">
    <Value>The store has some error(s) or warning(s). Please find more information on the Warnings page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType">
    <Value>Review types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Added">
    <Value>The new review type has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.AddNew">
    <Value>Add a new review type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.BackToList">
    <Value>back to catalog settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Deleted">
    <Value>The review type has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Description">
    <Value>You can configure a list of review types if you think that a basic review is not enough.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.EditDetails">
    <Value>Edit review type details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Description">
    <Value>Description</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Description.Hint">
    <Value>The description of the review type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Description.Required">
    <Value>Please provide a description.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.DisplayOrder.Hint">
    <Value>The review type display order. 1 represents the first item on the list.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.IsRequired">
    <Value>Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.IsRequired.Hint">
    <Value>When required, customers have to choose an appropriate rating value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Name.Hint">
    <Value>The name of the review type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.VisibleToAllCustomers">
    <Value>Visible to all customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Fields.VisibleToAllCustomers.Hint">
    <Value>Sets visibility of the review type for all customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Settings.ReviewType.Updated">
    <Value>The review type has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewReviewType">
    <Value>Added a new review type (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditReviewType">
    <Value>Edited a review type (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteReviewType">
   <Value>Deleted a review type (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.AdditionalProductReviews.Fields.Description">
    <Value>Description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.AdditionalProductReviews.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.AdditionalProductReviews.Fields.Rating">
    <Value>Rating</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.AdditionalProductReviews.Fields.VisibleToAllCustomers">
    <Value>Visible to all customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.BackToList">
    <Value>back to external authentication method list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersJoinDate">
    <Value>Show customers join date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersJoinDate.Hint">
    <Value>A value indicating whether to show customers join date.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Captcha.Instructions">
    <Value><![CDATA[<p>CAPTCHA is a program that can tell whether its user is a human or a computer. You''ve probably seen them — colorful images with distorted text at the bottom of Web registration forms. CAPTCHAs are used by many websites to prevent abuse from "bots" or automated programs usually written to generate spam. No computer programcan read distorted text as well as humans can, so bots cannot navigate sites protected by CAPTCHAs. nopCommerce uses <a href="http://www.google.com/recaptcha" target="_blank">reCAPTCHA</a>.</p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderCancelled.CustomerNotification">
	  <Value>This message template is used to notify a customer that the certain order was cancelled. The order can ba cancelled by a customer on the account page or by store owner in Customers - Customers in Orders tab or in Sales - Orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.RecurringPaymentCancelled.CustomerNotification">
	  <Value>This message template is used to notify a customer that the certain recurring payment is cancelled. Payment can be cancelled by a customer in the account page or by a store owner in Sales - Recurring payments in History tab by clicking "Cancel recurring payment" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.RecurringPaymentCancelled.StoreOwnerNotification">
	  <Value>This message template is used to notify a store owner that the certain recurring payment is cancelled. Payment can be cancelled by a customer in the account page or by a store owner in Sales - Recurring payments in History tab by clicking "Cancel recurring payment" button.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceName.Hint">
    <Value>Search for the name of a resource. These are system-internal names for a language entry. All resource names with ''admin.'' in their names, for example, are only displayed in the admin center, all others in the public area of the shop. Resource names with ''hint.'' in the name are helps like this.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled.Hint">
    <Value>Select to enable  the category path (breadcrumb). This is the bar at the top of the screen that indicates which categories and subcategories the product was viewed in on the product pages. Each sub-element of the bar is a separate hyperlink.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SslEnabled.Hint">
    <Value>Check if your store will be SSL secured. SSL (Secure Socket Layer) is the standard security technology for establishing an encrypted connection between a web server and the browser. This ensures that all data exchanged between web server and browser arrives unchanged.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.BulkEdit">
    <Value>Bulk edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports">
    <Value>Reports</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.PublishedOnly">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.UnpublishedOnly">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock">
  <Value>Low stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.LowStock.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.ByAmount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.ByQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.EndDate">
   <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.EndDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Fields.TotalAmount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Fields.TotalQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.OrderStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.OrderStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.PaymentStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.PaymentStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.RunReport">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.StartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.StartDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Store">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Store.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers">
    <Value>Bestsellers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.BillingCountry.Hint">
    <Value>Filter by order billing country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.ByAmount">
    <Value>Bestsellers by amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.ByQuantity">
    <Value>Bestsellers by quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Category">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Category.Hint">
    <Value>Search in a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Fields.TotalAmount">
    <Value>Total amount (excl tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity">
    <Value>Total quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Manufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Manufacturer.Hint">
    <Value>Search in a specific manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.OrderStatus">
    <Value>Order status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.OrderStatus.Hint">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.PaymentStatus">
    <Value>Payment status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.PaymentStatus.Hint">
    <Value>Search by a specific payment status e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.RunReport">
    <Value>Run report</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Store.Hint">
    <Value>Filter report by orders placed in a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Vendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Vendor.Hint">
    <Value>Search by a specific vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.EndDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.EndDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.RunReport">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchCategory">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchCategory.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchManufacturer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchManufacturer.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchVendor">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchVendor.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.StartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.StartDate.Hint">
   <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold">
    <Value>Products never purchased</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.RunReport">
    <Value>Run report</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchCategory">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchCategory.Hint">
    <Value>Load products only from a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchManufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchManufacturer.Hint">
    <Value>Load products only from a specific manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchStore.Hint">
    <Value>Load products only from a specific store (available in this store).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchVendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.SearchVendor.Hint">
    <Value>Load products only by a specific vendor (owned by this vendor).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.NeverSold.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.EndDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.EndDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.Fields.CountryName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.Fields.SumOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.Fields.TotalOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.OrderStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.OrderStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.PaymentStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.PaymentStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.RunReport">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.StartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Country.StartDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country">
    <Value>Country sales</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.Fields.CountryName">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.Fields.SumOrders">
    <Value>Order total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.Fields.TotalOrders">
    <Value>Number of orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.OrderStatus">
    <Value>Order status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.OrderStatus.Hint">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.PaymentStatus">
    <Value>Payment status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.PaymentStatus.Hint">
    <Value>Search by a specific payment status e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.RunReport">
    <Value>Run report</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Country.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByNumberOfOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByOrderTotal">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.EndDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.EndDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.Fields.Customer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.Fields.OrderCount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.Fields.OrderTotal">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.OrderStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.OrderStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.PaymentStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.PaymentStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.ShippingStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.ShippingStatus.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.StartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.StartDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Customers">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Period">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Period.14days">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Period.7days">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Period.month">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RegisteredCustomers.Fields.Period.year">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.RunReport">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Month">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Week">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Year">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers">
    <Value>Customer reports</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.BestByNumberOfOrders">
    <Value>Customers by number of orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.BestByOrderTotal">
    <Value>Customers by order total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.Fields.Customer">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.Fields.OrderCount">
    <Value>Number of orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.Fields.OrderTotal">
    <Value>Order total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.OrderStatus">
    <Value>Order status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.OrderStatus.Hint">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.PaymentStatus">
    <Value>Payment status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.PaymentStatus.Hint">
    <Value>Search by a specific payment status e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.ShippingStatus">
    <Value>Shipping status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.ShippingStatus.Hint">
    <Value>Search by a specific shipping status e.g. Not yet shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.BestBy.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers">
    <Value>Registered customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Customers">
    <Value>Count</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Period">
    <Value>Period</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Period.14days">
    <Value>In the last 14 days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Period.7days">
    <Value>In the last 7 days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Period.month">
    <Value>In the last month</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RegisteredCustomers.Fields.Period.year">
    <Value>In the last year</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.RunReport">
    <Value>Run report</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics">
    <Value>New customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics.Month">
    <Value>Month</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics.Week">
    <Value>Week</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics.Year">
    <Value>Year</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId">
    <Value>Blog post ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId.Hint">
    <Value>Search by blog post ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId">
    <Value>News item ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId.Hint">
    <Value>Search by news item ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog">
    <Value>Activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog">
    <Value>Activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType">
    <Value>Activity log type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType.Hint">
    <Value>The activity log type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogTypeColumn">
    <Value>Activity log type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderCancelled.CustomerNotification">
	  <Value>This message template is used to notify a customer that the certain order was cancelled. The order can be cancelled by a customer on the account page or by store owner in Customers - Customers in Orders tab or in Sales - Orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Captcha.Instructions">
    <Value><![CDATA[<p>CAPTCHA is a program that can tell whether its user is a human or a computer. You''ve probably seen them — colorful images with distorted text at the bottom of Web registration forms. CAPTCHAs are used by many websites to prevent abuse from "bots" or automated programs usually written to generate spam. No computer program can read distorted text as well as humans can, so bots cannot navigate sites protected by CAPTCHAs. nopCommerce uses <a href="http://www.google.com/recaptcha" target="_blank">reCAPTCHA</a>.</p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Login.NewCustomerText">
    <Value>By creating an account on our website, you will be able to shop faster, be up to date on an orders status, and keep track of the orders you have previously made.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNew.Hint">
    <Value>Check to mark the product as new. Use this option for promoting new products.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition.Hint">
    <Value>Check to specify a condition (depending on another attribute) when this attribute should be enabled (visible).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.EnableCondition.Hint">
    <Value>Check to specify a condition (depending on another attribute) when this attribute should be enabled (visible).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.IgnoreDiscounts.Warning">
    <Value>In order to use this functionality, you have to disable the following setting: Configuration > Catalog settings > Ignore discounts (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Warehouse.Hint">
    <Value>Choose the warehouse which will be used when calculating shipping rates. You can manage warehouses by selecting Configuration > Shipping > Warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate.EnterUtc">
    <Value>Date and time should be entered in Coordinated Universal Time (UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate.EnterUtc">
    <Value>Date and time should be entered in Coordinated Universal Time (UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement">
    <Value>For conditional expressions use the token %if (your conditions) ... endif%</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.IsCumulative.Hint">
    <Value>If checked, this discount can be used with other ones simultaneously. Please note that this feature works only for discounts with the same discount type. Right now, discounts with distinct types are already cumulative.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations.Notification">
    <Value>In order to use this functionality, you have to disable the following setting: Catalog settings > Ignore "limit per store" rules.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl.Notification">
    <Value>In order to use this functionality, you have to disable the following setting: Catalog settings > Ignore ACL rules.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
    <Value>Choose one or several customer roles i.e. administrators, vendors, guests, who will be able to see this product in catalog. If you don''t need this option just leave this field empty. In order to use this functionality, you have to disable the following setting: Configuration > Catalog settings > Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LimitedToStores.Hint">
    <Value>Option to limit this product to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty. In order to use this functionality, you have to disable the following setting: Configuration > Catalog settings > Ignore "limit per store" rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning">
    <Value>It seems that you use Redis server for caching, keep in mind that enabling this setting creates a lot of traffic between the Redis server and the application because of the large number of locales.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderStatus.Hint">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.PaymentStatus.Hint">
    <Value>Search by a specific payment status e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.ShippingStatus.Hint">
    <Value>Search by a specific shipping status e.g. Not yet shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManageInventoryMethod.Hint">
    <Value>Select inventory method. There are three methods: Don’t track inventory, Track inventory and Track inventory by attributes. You should use Track inventory by attributes when the product has different combinations of these attributes and then manage inventory for these combinations.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled.Hint">
    <Value>Select to enable the category path (breadcrumb). This is the bar at the top of the screen that indicates which categories and subcategories the product was viewed in on the product pages. Each sub-element of the bar is a separate hyperlink.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.OpeningHours.Hint">
    <Value>Specify opening hours of the pickup point (Monday - Friday: 09:00 - 19:00 for example).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.TaxDispaying">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.TaxDisplaying">
    <Value>Tax displaying</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.QuantityBelow.StoreOwnerNotification">
	  <Value><![CDATA[This message template is used to notify a store owner that the certain product is getting low stock. You can set up the minimum product quantity when creating or editing the product in Inventory section, <strong>Minimum stock qty field</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StoreIpAddresses.Hint">
    <Value>When enabled, IP addresses of customers will be stored. When disabled, it can improve performance. Furthermore, it''s prohibited to store IP addresses in some countries (private customer data).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StoreLastVisitedPage.Hint">
    <Value>When enabled, the last visited page will be stored. When disabled, it can improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.Instructions">
    <Value><![CDATA[<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href="https://developers.facebook.com/apps" target ="_blank" > Facebook for Developers</a> page and sign in. If you don''t already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App</b> button in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID</b>  button.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Select the option <b>Facebook Login</b> and press <b>Set up</b>.</li><li>Click on <b>Settings</b> on the left menu and in section <b>Client OAuth Settings</b> select the field <b>Valid OAuth Redirect URIs</b></li><li>Enter \"YourStoreUrl/signin-facebook\" in that field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>]]></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Common.DeleteConfirmation.Selected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.AdditionalConfirm">
    <Value><![CDATA[<p>WARNING. It is not recommended to do this on live sites because: </p><ol><li>Product prices, order totals, shipping rates, etc are not automatically converted to a new currency</li><li>Currency exhange rates are not automatically updated</li></ol>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.AdditionalConfirm">
    <Value><![CDATA[<p>WARNING. It is not recommended to do this on live sites because: </p><ol><li>Other rates have to be updated manually</li><li>You have to ensure that each product has a valid measure (dimension and weights) - they are not adjusted automatically</li></ol>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.BillingInfo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.ShippingInfo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.BillingShippingInfo">
    <Value>Billing &amp; shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersLocation">
    <Value>Show customer''s location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersLocation.Hint">
    <Value>A value indicating whether customer''s location is shown.</Value>
  </LocaleResource>  
  <LocaleResource Name="Account.UserAgreement">
    <Value>User agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Orders.ShoppingCartType.ShoppingCart">
    <Value>Shopping Cart</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Orders.ShoppingCartType.Wishlist">
    <Value>Wishlist</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.SEO.SeName.MaxLengthValidation">
    <Value>Max length of search name is {0} chars</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Note.Validation">
    <Value>Order note can not be empty.</Value>
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

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_GetLowStockProducts' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_GetLowStockProducts] ON [Product] (Deleted ASC, VendorId ASC, ProductTypeId ASC, ManageInventoryMethodId ASC, MinStockQuantity ASC, UseMultipleWarehouses ASC)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportallowdownloadimages')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.exportimportallowdownloadimages', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ActivityLog]') AND NAME = 'EntityId')
BEGIN
	ALTER TABLE [ActivityLog]
	ADD [EntityId] INT NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ActivityLog]') AND NAME = 'EntityName')
BEGIN
	ALTER TABLE [ActivityLog]
	ADD [EntityName] NVARCHAR(400) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.showvendoronorderdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.showvendoronorderdetailspage', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.preselectcountryifonlyone')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.preselectcountryifonlyone', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ProductAttributeCombination]') AND NAME = 'PictureId')
BEGIN
	ALTER TABLE [ProductAttributeCombination]
	ADD [PictureId] INT NULL
END
GO

UPDATE [ProductAttributeCombination]
SET [PictureId] = 0
WHERE [PictureId] IS NULL

ALTER TABLE [ProductAttributeCombination] ALTER COLUMN [PictureId] INT NOT NULL
GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'ProductReview.Reply.CustomerNotification')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'ProductReview.Reply.CustomerNotification', NULL, N'%Store.Name%. Product review reply.', N'<p>' + @NewLine + '<a href="%Store.URL%">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%,' + @NewLine + '<br />' + @NewLine + 'You received a reply from the store administration to your review for product "%ProductReview.ProductName%".' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
 END
 GO
 
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ProductReview]') AND NAME = 'CustomerNotifiedOfReply')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [CustomerNotifiedOfReply] BIT NULL
END
GO

UPDATE [ProductReview]
SET [CustomerNotifiedOfReply] = 0
WHERE [CustomerNotifiedOfReply] IS NULL

ALTER TABLE [ProductReview] ALTER COLUMN [CustomerNotifiedOfReply] BIT NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.notifycustomeraboutproductreviewreply')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.notifycustomeraboutproductreviewreply', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.uselinksinrequiredproductwarnings')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.uselinksinrequiredproductwarnings', N'true', 0)
END
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'OrderPlaced.AffiliateNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'OrderPlaced.AffiliateNotification', NULL, N'%Store.Name%. Order placed', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Customer.FullName% (%Customer.Email%) has just placed an order.' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order Number: %Order.OrderNumber%' + @NewLine + '<br />' + @NewLine + 'Date Ordered: %Order.CreatedOn%' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Order.Product(s)%' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
END
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'OrderPaid.AffiliateNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'OrderPaid.AffiliateNotification', NULL, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order #%Order.OrderNumber% has been just paid.' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order Number: %Order.OrderNumber%' + @NewLine + '<br />' + @NewLine + 'Date Ordered: %Order.CreatedOn%' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Order.Product(s)%' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.allownonasciicharactersinheaders')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'securitysettings.allownonasciicharactersinheaders', N'true', 0)
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Store]') AND NAME='SecureUrl')
BEGIN
	ALTER TABLE [Store] DROP COLUMN [SecureUrl]
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportsplitproductsfile')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.exportimportsplitproductsfile', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productreviewssortbycreateddateascending')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.productreviewssortbycreateddateascending', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportproductscountinonefile')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.exportimportproductscountinonefile', N'500', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductAttributeValue]') and NAME='PriceAdjustmentUsePercentage')
BEGIN
	ALTER TABLE [ProductAttributeValue]
	ADD [PriceAdjustmentUsePercentage] bit NULL
END
GO

UPDATE [ProductAttributeValue]
SET [PriceAdjustmentUsePercentage] = 0
WHERE [PriceAdjustmentUsePercentage] IS NULL
GO

ALTER TABLE [ProductAttributeValue] ALTER COLUMN [PriceAdjustmentUsePercentage] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[PredefinedProductAttributeValue]') and NAME='PriceAdjustmentUsePercentage')
BEGIN
	ALTER TABLE PredefinedProductAttributeValue
	ADD [PriceAdjustmentUsePercentage] bit NULL
END
GO

UPDATE [PredefinedProductAttributeValue]
SET [PriceAdjustmentUsePercentage] = 0
WHERE [PriceAdjustmentUsePercentage] IS NULL
GO

ALTER TABLE [PredefinedProductAttributeValue] ALTER COLUMN [PriceAdjustmentUsePercentage] bit NOT NULL
GO

--updated setting
UPDATE [Setting]
SET [Value] = N'true'
WHERE [Name] = N'commonsettings.usestoredprocedureforloadingcategories'
GO

--vendor attributes
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[VendorAttribute]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[VendorAttribute]
	(
		[Id] INT IDENTITY(1,1) NOT NULL,
		[Name] NVARCHAR(400) NOT NULL,
		[IsRequired] BIT NOT NULL,
		[AttributeControlTypeId] INT NOT NULL,
		[DisplayOrder] INT NOT NULL,

		PRIMARY KEY CLUSTERED ( [Id] ASC ) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[VendorAttributeValue]') and objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[VendorAttributeValue]
	(
		[Id] INT IDENTITY(1,1) NOT NULL,
		[VendorAttributeId] INT NOT NULL,
		[Name] NVARCHAR(400) NOT NULL,
		[IsPreSelected] BIT NOT NULL,
		[DisplayOrder] INT NOT NULL,

		PRIMARY KEY CLUSTERED ( [Id] ASC ) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE NAME = 'FK_VendorAttributeValue_VendorAttribute_VendorAttributeId' AND PARENT_OBJECT_ID = object_id('VendorAttributeValue') AND objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.VendorAttributeValue DROP CONSTRAINT [FK_VendorAttributeValue_VendorAttribute_VendorAttributeId]
END
GO

ALTER TABLE [dbo].[VendorAttributeValue] WITH CHECK 
	ADD CONSTRAINT [FK_VendorAttributeValue_VendorAttribute_VendorAttributeId] FOREIGN KEY([VendorAttributeId]) REFERENCES [dbo].[VendorAttribute] ([Id]) ON DELETE CASCADE
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewVendorAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewVendorAttribute', N'Add a new vendor attribute', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditVendorAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditVendorAttribute', N'Edit a vendor attribute', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteVendorAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteVendorAttribute', N'Delete a vendor attribute', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewVendorAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewVendorAttributeValue', N'Add a new vendor attribute value', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditVendorAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditVendorAttributeValue', N'Edit a vendor attribute value', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteVendorAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteVendorAttributeValue', N'Delete a vendor attribute value', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewReviewType')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewReviewType', N'Add a new review type', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteReviewType')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteReviewType', N'Delete a review type', N'true')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditReviewType')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditReviewType', N'Edit a review type', N'true')
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Address]') AND NAME = 'County')
BEGIN
	ALTER TABLE [Address]
	ADD [County] NVARCHAR (MAX) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayApplyVendorAccountFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayApplyVendorAccountFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayBlogFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayBlogFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayCompareProductsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayCompareProductsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayContactUsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayContactUsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayCustomerAddressesFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayCustomerAddressesFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayCustomerInfoFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayCustomerInfoFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayCustomerOrdersFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayCustomerOrdersFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayForumsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayForumsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayNewProductsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayNewProductsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayNewsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayNewsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.countyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.countyenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayProductSearchFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayProductSearchFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.countyrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.countyrequired', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayRecentlyViewedProductsFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayRecentlyViewedProductsFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countyenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayShoppingCartFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayShoppingCartFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countyrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countyrequired', N'false', 0)
END
GO

--new setting    
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplaySitemapFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplaySitemapFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.shipseparatelyoneitemeach')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.shipseparatelyoneitemeach', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultfooteritemsettings.DisplayWishlistFooterItem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultfooteritemsettings.DisplayWishlistFooterItem', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.sitemappagesize')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.sitemappagesize', N'200', 0)
END
GO

--rename setting
UPDATE [Setting] 
SET [Name] = N'adminareasettings.useisodateformatinjsonresult' 
WHERE [Name] = N'adminareasettings.useisodatetimeconverterinjson'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[RewardPointsHistory]') AND NAME = 'EndDateUtc')
BEGIN
	ALTER TABLE [RewardPointsHistory]
	ADD [EndDateUtc] DATETIME NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[RewardPointsHistory]') AND NAME = 'ValidPoints')
BEGIN
	ALTER TABLE [RewardPointsHistory]
	ADD [ValidPoints] INT NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'rewardpointssettings.registrationpointsvalidity')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'rewardpointssettings.registrationpointsvalidity', N'30', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.usernamevalidationenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.usernamevalidationenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'rewardpointssettings.purchasespointsvalidity')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'rewardpointssettings.purchasespointsvalidity', N'45', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.usernamevalidationuseregex')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.usernamevalidationuseregex', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'rewardpointssettings.minordertotaltoawardpoints')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'rewardpointssettings.minordertotaltoawardpoints', N'0', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.checkoutdisabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'ordersettings.checkoutdisabled', N'false', 0)
END
GO

--rename column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and NAME='From')
BEGIN
    EXEC sp_RENAME '[dbo].[ShippingByWeight].[From]', 'WeightFrom', 'COLUMN'
END
GO

--rename column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and NAME='To')
BEGIN
    EXEC sp_RENAME '[dbo].[ShippingByWeight].[To]', 'WeightTo', 'COLUMN'
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalFrom')
BEGIN
	ALTER TABLE [ShippingByWeight]
	ADD [OrderSubtotalFrom] DECIMAL NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalFrom')
BEGIN
	UPDATE [ShippingByWeight]
	SET [OrderSubtotalFrom] = 0
	WHERE [OrderSubtotalFrom] IS NULL
END
GO


IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalFrom')
BEGIN
	ALTER TABLE [ShippingByWeight] ALTER COLUMN [OrderSubtotalFrom] DECIMAL NOT NULL
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalTo')
BEGIN
	ALTER TABLE [ShippingByWeight]
	ADD [OrderSubtotalTo] DECIMAL NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalTo')
BEGIN
	UPDATE [ShippingByWeight]
	SET [OrderSubtotalTo] = 1000000
	WHERE [OrderSubtotalTo] IS NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeight]') AND NAME = 'OrderSubtotalTo')
BEGIN
	ALTER TABLE [ShippingByWeight] ALTER COLUMN [OrderSubtotalTo] DECIMAL NOT NULL
END
GO

--rename table
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
    EXEC sp_RENAME '[dbo].[ShippingByWeight]', 'ShippingByWeightByTotalRecord'
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Poll]') and NAME = 'LimitedToStores')
BEGIN
	ALTER TABLE [Poll]
	ADD [LimitedToStores] BIT NULL
END
GO

UPDATE [Poll]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Poll] ALTER COLUMN [LimitedToStores] BIT NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.removerequiredproducts')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.removerequiredproducts', N'false', 0)
END
GO

-- update the "ProductLoadAllPaged" stored procedure
ALTER PROCEDURE [ProductLoadAllPaged]
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
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
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
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
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

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[Sku] = @OriginalKeywords '
		END

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

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
	DECLARE @FilteredCustomerRoleIdsCount int	
	SET @FilteredCustomerRoleIdsCount = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)
	
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
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
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
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

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
		AND (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FilteredCustomerRoleIdsCount > 0
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
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
    --prepare filterable specification attribute option identifier (if requested)
    IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
        DECLARE @sql_filterableSpecs nvarchar(max)
        SET @sql_filterableSpecs = '
	        INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
	        SELECT DISTINCT [psam].SpecificationAttributeOptionId
	        FROM [Product_SpecificationAttribute_Mapping] [psam] WITH (NOLOCK)
	            WHERE [psam].[AllowFiltering] = 1
	            AND [psam].[ProductId] IN (' + @sql + ')'

        EXEC sp_executesql @sql_filterableSpecs

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',') 

    CREATE TABLE #FilteredSpecsWithAttributes
	(
        SpecificationAttributeId int not null,
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecsWithAttributes (SpecificationAttributeId, SpecificationAttributeOptionId)
	SELECT sao.SpecificationAttributeId, fs.SpecificationAttributeOptionId
    FROM #FilteredSpecs fs INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId
    ORDER BY sao.SpecificationAttributeId 

    DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecsWithAttributes)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
        DECLARE @SpecificationAttributeId int
        DECLARE @LastSpecificationAttributeId int
        SET @LastSpecificationAttributeId = 0
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT SpecificationAttributeId, SpecificationAttributeOptionId
		FROM #FilteredSpecsWithAttributes

		OPEN cur_SpecificationAttributeOption
        FOREACH:
            FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeId, @SpecificationAttributeOptionId
            IF (@LastSpecificationAttributeId <> 0 AND @SpecificationAttributeId <> @LastSpecificationAttributeId OR @@FETCH_STATUS <> 0) 
			    SET @sql = @sql + '
        AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId IN (SELECT SpecificationAttributeOptionId FROM #FilteredSpecsWithAttributes WHERE SpecificationAttributeId = ' + CAST(@LastSpecificationAttributeId AS nvarchar(max)) + '))'
            SET @LastSpecificationAttributeId = @SpecificationAttributeId
		IF @@FETCH_STATUS = 0 GOTO FOREACH
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
	
    SET @sql = '
    INSERT INTO #DisplayOrderTmp ([ProductId])' + @sql

	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
    DROP TABLE #FilteredSpecsWithAttributes
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

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended' and object_id=object_id(N'[dbo].[QueuedEmail]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON QueuedEmail ([SentOnUtc], [DontSendBeforeDateUtc]) INCLUDE ([SentTries])
END
GO

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_VisibleIndividually_Published_Deleted_Extended' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON Product ([VisibleIndividually],[Published],[Deleted]) INCLUDE ([Id],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc])
END
GO

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Category_Deleted_Extended' and object_id=object_id(N'[dbo].[Category]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Category_Deleted_Extended] ON Category ([Deleted]) INCLUDE ([Id],[Name],[SubjectToAcl],[LimitedToStores],[Published])
END
GO

--new setting   
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.usernamevalidationrule')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.usernamevalidationrule', N'', 0)
END
GO

--new setting   
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.deletegiftcardusagehistory')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.deletegiftcardusagehistory', N'False', 0)
END
GO

--update [sename] column for product tags
IF EXISTS (
        SELECT *
        FROM sysobjects
        WHERE id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO
CREATE PROCEDURE [temp_generate_sename]
(
    @table_name nvarchar(1000),
    @entity_id int,
    @language_id int = 0, --0 to process main sename column, --language id to process a localized value
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
    --get current name
    DECLARE @current_sename nvarchar(1000)
    DECLARE @sql nvarchar(4000)
    
    IF (@language_id = 0)
    BEGIN
        SET @sql = 'SELECT @current_sename = [Name] FROM [' + @table_name + '] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
        EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT        
    END
    ELSE
    BEGIN
        SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''Name'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
        EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT		
        
        --if not empty, se name is already specified by a store owner. if empty, we should use poduct name
        IF (@current_sename is null or @current_sename = N'')
        BEGIN
            SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''Name'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
            EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT        
        END
        
        --if localized product name is also empty, we exit
        IF (@current_sename is null or @current_sename = N'')
            RETURN
    END
    
    --generate se name    
    DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars nvarchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@current_sename)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
        DECLARE @c nvarchar(1)
        SET @c = substring(@current_sename, @p, 1)
        IF CHARINDEX(@c,@allowed_se_chars) > 0
        BEGIN
            SET @new_sename = @new_sename + @c
        END		
        SET @p = @p + 1
    END	
    --replace spaces with '-'
    SELECT @new_sename = REPLACE(@new_sename,' ','-');
    WHILE CHARINDEX('--',@new_sename) > 0
        SELECT @new_sename = REPLACE(@new_sename,'--','-');
    WHILE CHARINDEX('__',@new_sename) > 0
        SELECT @new_sename = REPLACE(@new_sename,'__','_');
    --ensure not empty
    IF (@new_sename is null or @new_sename = '')
        SELECT @new_sename = ISNULL(CAST(@entity_id AS nvarchar(max)), '0');
    --lowercase
    SELECT @new_sename = LOWER(@new_sename)
    --ensure this sename is not reserved
    WHILE (1=1)
    BEGIN
        DECLARE @sename_is_already_reserved bit
        SET @sename_is_already_reserved = 0
        SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename AND NOT ([EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0') + ' AND [EntityName] = ''' + @table_name + '''))
                    BEGIN
                        SELECT @sename_is_already_reserved = 1
                    END'
        EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
        
        IF (@sename_is_already_reserved > 0)
        BEGIN
            --add some digit to the end in this case
            SET @new_sename = @new_sename + '-2'
        END
        ELSE
        BEGIN
            BREAK
        END
    END
    
    --return
    SET @result = @new_sename
END
GO

BEGIN
    DECLARE @sename_existing_entity_id int
    DECLARE cur_sename_existing_entity CURSOR FOR
    SELECT [Id]
    FROM [ProductTag]
    OPEN cur_sename_existing_entity
    FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @sename nvarchar(1000)    
        SET @sename = null -- clear cache (variable scope)
        
        DECLARE @table_name nvarchar(1000)    
        SET @table_name = N'ProductTag'
        
        DECLARE @product_tag_system_name nvarchar(1000)
        SET @product_tag_system_name = null -- clear cache (variable scope)
        SELECT @product_tag_system_name = [Name] FROM [ProductTag] WHERE [Id] = @sename_existing_entity_id
        
        --main sename
        EXEC    [dbo].[temp_generate_sename]
				@table_name = @table_name,
                @entity_id = @sename_existing_entity_id,                
                @result = @sename OUTPUT
                
        IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
        BEGIN
            UPDATE [UrlRecord]
            SET [Slug] = @sename
            WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
        END
        ELSE
        BEGIN
            INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
            VALUES (@sename_existing_entity_id, @table_name, @sename, 1, 0)
        END        
		
		
		--localized values
        DECLARE @ExistingLanguageID int
        DECLARE cur_existinglanguage CURSOR FOR
        SELECT [ID]
        FROM [Language]
        OPEN cur_existinglanguage
        FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
        WHILE @@FETCH_STATUS = 0
        BEGIN    
            SET @sename = null -- clear cache (variable scope)
            
            EXEC    [dbo].[temp_generate_sename]
                    @table_name = @table_name,
                    @entity_id = @sename_existing_entity_id,
                    @language_id = @ExistingLanguageID,
                    @result = @sename OUTPUT
            IF (len(@sename) > 0)
            BEGIN
                
                DECLARE @sql nvarchar(4000)
                SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
                BEGIN
                    --update
                    UPDATE [UrlRecord]
                    SET [Slug] = @sename
                    WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
                END
                ELSE
                BEGIN
                    --insert
                    INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
                    VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, 1, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
                END
                '
                EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
                
            END
                    

            --fetch next language identifier
            FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
        END
        CLOSE cur_existinglanguage
        DEALLOCATE cur_existinglanguage


        --fetch next identifier
        FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
    END
    CLOSE cur_sename_existing_entity
    DEALLOCATE cur_sename_existing_entity
END
GO

--drop temporary procedures & functions
IF EXISTS (
        SELECT *
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'captchasettings.recaptchaversion'
GO

--new setting 
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.maximumrewardpointstouseperorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.maximumrewardpointstouseperorder', N'0', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportrelatedentitiesbyname')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.exportimportrelatedentitiesbyname', N'true', 0)
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'commonsettings.usestoredproceduresifsupported'
GO

--drop some indexes
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PMM_ProductId' and object_id=object_id(N'[Product_Manufacturer_Mapping]'))
BEGIN
	DROP INDEX [IX_PMM_ProductId] ON [Product_Manufacturer_Mapping]
END
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PCM_ProductId' and object_id=object_id(N'[Product_Category_Mapping]'))
BEGIN
	DROP INDEX [IX_PCM_ProductId] ON [Product_Category_Mapping]
END
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PSAM_ProductId' and object_id=object_id(N'[Product_SpecificationAttribute_Mapping]'))
BEGIN
	DROP INDEX [IX_PSAM_ProductId] ON [Product_SpecificationAttribute_Mapping]
END
GO

--update the FullText_IsSupported procedure
ALTER PROCEDURE [dbo].[FullText_IsSupported]
AS
BEGIN	
	EXEC('
	SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END as Value')
END
GO

--update setting
UPDATE [Setting]
SET [Value] = N'<!-- Global site tag (gtag.js) - Google Analytics -->
                <script async src=''https://www.googletagmanager.com/gtag/js?id={GOOGLEID}''></script>
                <script>
                  window.dataLayer = window.dataLayer || [];
                  function gtag(){dataLayer.push(arguments);}
                  gtag(''js'', new Date());

                  gtag(''config'', ''{GOOGLEID}'');
                  {CUSTOMER_TRACKING}
                </script>'
WHERE [Name] = N'googleanalyticssettings.trackingscript'
GO


--GDPR consent
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[GdprConsent]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[GdprConsent]
	(
		[Id] INT IDENTITY(1,1) NOT NULL,
		[Message] NVARCHAR(MAX) NOT NULL,
		[IsRequired] BIT NOT NULL,
		[RequiredMessage] NVARCHAR(MAX) NULL,
		[DisplayDuringRegistration] BIT NOT NULL,
		[DisplayOnCustomerInfoPage] BIT NOT NULL,
		[DisplayOrder] INT NOT NULL,

		PRIMARY KEY CLUSTERED ( [Id] ASC ) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

--GDPR log
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[GdprLog]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[GdprLog]
	(
		[Id] INT IDENTITY(1,1) NOT NULL,
		[CustomerId] INT NOT NULL,
		[ConsentId] INT NOT NULL,
		[CustomerInfo] NVARCHAR(MAX) NOT NULL,
		[RequestTypeId] INT NOT NULL,
		[RequestDetails] NVARCHAR(MAX) NOT NULL,
		[CreatedOnUtc] DATETIME NOT NULL,

		PRIMARY KEY CLUSTERED ( [Id] ASC ) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'gdprsettings.gdprenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'gdprsettings.gdprenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'gdprsettings.logprivacypolicyconsent')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'gdprsettings.logprivacypolicyconsent', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'gdprsettings.lognewsletterconsent')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'gdprsettings.lognewsletterconsent', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showlinktoallresultinsearchautocomplete')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.showlinktoallresultinsearchautocomplete', N'false', 0)
END
GO

--rename setting
UPDATE [Setting] SET [Name] = 'captchasettings.recaptchadefaultlanguage' WHERE [Name] = 'captchasettings.recaptchalanguage'

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchadefaultlanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'captchasettings.recaptchadefaultlanguage', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.countdisplayedyearsdatepicker')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.countdisplayedyearsdatepicker', N'1', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.automaticallychooselanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'captchasettings.automaticallychooselanguage', N'True', 0)
END
GO

 --new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PictureBinary]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN

EXEC('CREATE TABLE [dbo].[PictureBinary]
    (
		[Id] int IDENTITY(1,1) NOT NULL,
		[PictureId] int NOT NULL,
		[BinaryData] [varbinary](max) NULL,		
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)

   --copy existing data
	INSERT INTO [dbo].[PictureBinary](PictureId, BinaryData)
	SELECT [Id], [PictureBinary] FROM [dbo].[Picture]

	ALTER TABLE dbo.Picture DROP COLUMN [PictureBinary]
	ALTER INDEX ALL ON [Picture] REBUILD')	

END
GO

IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_PictureBinary_Picture_PictureId') AND parent_object_id = OBJECT_ID(N'PictureBinary'))
	ALTER TABLE [PictureBinary] DROP CONSTRAINT [FK_PictureBinary_Picture_PictureId]
GO

ALTER TABLE [dbo].[PictureBinary] WITH CHECK ADD CONSTRAINT [FK_PictureBinary_Picture_PictureId] FOREIGN KEY(PictureId)
REFERENCES [dbo].[Picture] ([Id])
ON DELETE CASCADE
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaydatepreorderavailability')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaydatepreorderavailability', N'False', 0)
END
GO
	
--new setting	
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.richeditorallowstyletag')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.richeditorallowstyletag', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.allowcustomerstocheckgiftcardbalance')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.allowcustomerstocheckgiftcardbalance', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.checkcopyrightremovalkey')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.checkcopyrightremovalkey', N'true', 0)
END
GO

--Review type
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[ReviewType]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[ReviewType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[DisplayOrder] [int] NOT NULL,	
	[VisibleToAllCustomers] [bit] NOT NULL,
	[IsRequired] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

--Product review and review type mapping
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[ProductReview_ReviewType_Mapping]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[ProductReview_ReviewType_Mapping](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ProductReviewID] [int] NOT NULL,
		[ReviewTypeID] [int] NOT NULL,
		[Rating] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY]	

	ALTER TABLE [dbo].[ProductReview_ReviewType_Mapping]  WITH CHECK ADD  CONSTRAINT [ProductReviewReviewTypeRel_ProductReview] FOREIGN KEY([ProductReviewID])
	REFERENCES [dbo].[ProductReview] ([Id])
	ON DELETE CASCADE	

	ALTER TABLE [dbo].[ProductReview_ReviewType_Mapping] CHECK CONSTRAINT [ProductReviewReviewTypeRel_ProductReview]	

	ALTER TABLE [dbo].[ProductReview_ReviewType_Mapping]  WITH CHECK ADD  CONSTRAINT [ProductReviewReviewTypeRel_ReviewType] FOREIGN KEY([ReviewTypeID])
	REFERENCES [dbo].[ReviewType] ([Id])
	ON DELETE CASCADE	

	ALTER TABLE [dbo].[ProductReview_ReviewType_Mapping] CHECK CONSTRAINT [ProductReviewReviewTypeRel_ReviewType]	
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.jquerymigratescriptloggingactive')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.jquerymigratescriptloggingactive', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.supportpreviousnopcommerceversions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.supportpreviousnopcommerceversions', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.useresponsecompression')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.useresponsecompression', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.staticfilescachecontrol')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.staticfilescachecontrol', N'public,max-age=604800', 0)
END
GO

--rename table
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingByWeightByTotal]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
    EXEC sp_RENAME '[dbo].[ShippingByWeightByTotal]', 'ShippingByWeightByTotalRecord'
END
GO