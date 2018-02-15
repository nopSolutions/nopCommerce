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
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
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
    <Value>Shopping cart and wishlist</Value>
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

IF EXISTS (SELECT 1 FROM sys.objects WHERE NAME = 'VendorAttributeValue_VendorAttribute' AND PARENT_OBJECT_ID = object_id('VendorAttributeValue') AND objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.VendorAttributeValue DROP CONSTRAINT [VendorAttributeValue_VendorAttribute]
END
GO

ALTER TABLE [dbo].[VendorAttributeValue] WITH CHECK 
	ADD CONSTRAINT [VendorAttributeValue_VendorAttribute] FOREIGN KEY([VendorAttributeId]) REFERENCES [dbo].[VendorAttribute] ([Id]) ON DELETE CASCADE
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