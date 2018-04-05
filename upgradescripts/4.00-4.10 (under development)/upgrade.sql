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
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountLimitation.Hint">
    <Value>Choose the limitation of discount. This parameter will not be taken into account for recurring products/orders.</Value>
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
    EXEC sp_RENAME '[dbo].[ShippingByWeight]', 'ShippingByWeightByTotal'
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