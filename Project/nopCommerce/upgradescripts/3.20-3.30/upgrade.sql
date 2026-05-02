--upgrade scripts from nopCommerce 3.20 to 3.30

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Catalog.Products.List.ImportFromExcelTip">
    <Value>Imported products are distinguished by SKU. If the SKU already exists, then its corresponding product will be updated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1">
    <Value>Invoice footer text (left column)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1.Hint">
    <Value>Enter the text that will appear at the bottom of generated invoices (left column).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2">
    <Value>Invoice footer text (right column)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2.Hint">
    <Value>Enter the text that will appear at the bottom of generated invoices (right column).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.DeleteAll">
    <Value>Delete all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.DeletedAll">
    <Value>All queued emails have been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores.Hint">
	<Value>Determines whether the attribute is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores.Hint">
	<Value>Select stores for which the attribute will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SeName">
	<Value>Search engine friendly page name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SeName.Hint">
	<Value>Set a search engine friendly page name e.g. ''some-topic-name'' to make your page URL ''http://www.yourStore.com/some-topic-name''. Leave empty to generate it automatically based on the title of the topic.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.OrderID.Hint">
	<Value>The order associated to this shipment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowAddingOnlyExistingAttributeCombinations">
	<Value>Allow only existing attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowAddingOnlyExistingAttributeCombinations.Hint">
	<Value>Check to allow adding to the cart/wishlist only attribute combinations that exist and have stock greater than zero. In this case you have to create all existing product attribute combinations that you have in stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.LimitedToStores.Hint">
	<Value>Determines whether the country is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores.Hint">
	<Value>Select stores for which the country will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage">
	<Value>Automatically detect language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage.Hint">
	<Value>Check to automatically detect language based on a customer browser settings.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct">
	<Value>Purchased with product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Hint">
	<Value>A customer is added to this customer role once a specified product is purchased (paid). Please note that in case of refund or order cancellation you have to manually remove a customer from this role.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Choose">
	<Value>Choose product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Remove">
	<Value>Remove</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered">
	<Value>You cannot specify "Purchased with product" value for "Registered" customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Common.OK">
	<Value>OK</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.OK">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Cancel">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Description2">
	<Value>Cookies help us deliver our services. By using our services, you agree to our use of cookies.</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.LearnMore">
	<Value>Learn more</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.ServerVariables">
	<Value>Server variables</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.ServerVariables.Hint">
	<Value>A list of server variables</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.IsPrimaryWeight">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.IsPrimaryDimension">
	<Value>Is primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.AddOrderNoteDisplayToCustomer">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.AddOrderNoteDisplayToCustomer.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.AddOrderNoteMessage">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.AddOrderNoteMessage.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.DisplayToCustomer.Hint">
	<Value>A value indicating whether to display this order note to a customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Note.Hint">
	<Value>Enter this order note message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Download">
	<Value>Attached file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Download.Hint">
	<Value>Upload a file attached to this order note.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Download.Link">
	<Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Fields.Download.Link.No">
	<Value>No file attached</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Notes.Download">
	<Value>Download attached file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules">
	<Value>Validation rules</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.ViewLink">
	<Value>View/Edit rules</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MinLength">
	<Value>Minimum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MinLength.Hint">
	<Value>Specify minimum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MaxLength">
	<Value>Maximum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MaxLength.Hint">
	<Value>Specify maximum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TextboxMinimumLength">
	<Value>{0} : minimum length is {1} chars</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TextboxMaximumLength">
	<Value>{0} : maximum length is {1} chars</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.MinLength">
	<Value>Minimum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.MinLength.Hint">
	<Value>Specify minimum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.MaxLength">
	<Value>Maximum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.MaxLength.Hint">
	<Value>Specify maximum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Failed">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Processing">
	<Value>Processing dropped files...</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Delete">
	<Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Retry">
	<Value>Retry</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Upload">
	<Value>Upload a file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileAllowedExtensions">
	<Value>Allowed file extensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileAllowedExtensions.Hint">
	<Value>Specify a comma-separated list of allowed file extensions. Leave empty to allow any file extension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileMaximumSize">
	<Value>Maximum file size (KB)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileMaximumSize.Hint">
	<Value>Specify maximum file size in kilobytes. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileAllowedExtensions">
	<Value>Allowed file extensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileAllowedExtensions.Hint">
	<Value>Specify a comma-separated list of allowed file extensions. Leave empty to allow any file extension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileMaximumSize">
	<Value>Maximum file size (KB)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileMaximumSize.Hint">
	<Value>Specify maximum file size in kilobytes. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep">
	<Value>Disable "Billing address" step</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep.Hint">
	<Value>Check to disable "Billing address" step during checkout. Billing address will be pre-filled and saved using the default registration data (this option cannot be used with guest checkout enabled). Also ensure that appropriate address fields that cannot be pre-filled are not required (or disabled).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PassDimensions">
	<Value>Pass dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PassDimensions.Hint">
	<Value>Check if you want to pass package dimensions when requesting rates.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackingType">
	<Value>Packing type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackingType.Hint">
	<Value>Choose preferred packing type.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions">
	<Value>Pack by dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage">
	<Value>Pack by one item per package</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume">
	<Value>Pack by volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackingPackageVolume">
	<Value>Package volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint">
	<Value>Enter your package volume.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Tracing">
	<Value>Tracing</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Tracing.Hint">
	<Value>Check if you want to record plugin tracing in System Log. Warning: The entire request and response XML will be logged (including AccessKey/UserName,Password). Do not leave this enabled in a production environment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Description">
	<Value>To find text or a specific setting (by name), you can apply a filter via the funnel icon in the "Value" or "Setting name" column headers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Description">
	<Value>To find text or a specific resource (by name), you can apply a filter via the funnel icon in the "Value" or "Resource name" column headers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes">
	<Value>Custom customer attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.AddNew">
	<Value>Add a new customer attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.BackToList">
	<Value>back to customer attribute list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Description">
	<Value>If the default form fields are not enough for your needs, then you can manage additional customer attributes below.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.EditAttributeDetails">
	<Value>Edit customer attribute details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Info">
	<Value>Attribute info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Added">
	<Value>The new attribute has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Deleted">
	<Value>The attribute has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Updated">
	<Value>The attribute has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.Name.Hint">
	<Value>The name of the customer attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.IsRequired">
	<Value>Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.IsRequired.Hint">
	<Value>When an attribute is required, the customer must choose an appropriate attribute value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.AttributeControlType">
	<Value>Control type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.AttributeControlType.Hint">
	<Value>Choose how to display your attribute values.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Fields.DisplayOrder.Hint">
	<Value>The customer attribute display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values">
	<Value>Attribute values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.AddNew">
	<Value>Add a new customer value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.EditValueDetails">
	<Value>Edit customer value details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.SaveBeforeEdit">
	<Value>You need to save the customer attribute before you can add values for this customer attribute page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.Name.Hint">
	<Value>The name of the customer value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected">
	<Value>Pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected.Hint">
	<Value>Determines whether this attribute value is pre selected for the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder.Hint">
	<Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ResponsiveDesignSupported">
	<Value>Responsive design supported</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ResponsiveDesignSupported.Hint">
	<Value>Check to enable responsive design. Also note that your graphical theme should also support it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.NoShipments">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.NoShipmentsSelected">
	<Value>No shipments selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliverySelected">
	<Value>Set as delivered (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipSelected">
	<Value>Set as shipped (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.ForceTaxExclusionFromOrderSubtotal">
	<Value>Force tax exclusion from order subtotal</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.ForceTaxExclusionFromOrderSubtotal.Hint">
	<Value>Check to always exclude tax from order subtotal (no matter of selected tax dispay type). This setting effects only pages where order totals are displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter">
	<Value>Display tax/shipping info (footer)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter.Hint">
	<Value>Check to display tax and shipping info in the footer. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductDetailsPage">
	<Value>Display tax/shipping info (product details page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductDetailsPage.Hint">
	<Value>Check to display tax and shipping info on product details pages. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductBoxes">
	<Value>Display tax/shipping info (product boxes)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductBoxes.Hint">
	<Value>Check to display tax and shipping info in product boxes (catalog pages). This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Footer.TaxShipping.InclTax">
	<Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Footer.TaxShipping.ExclTax">
	<Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.TaxShipping.InclTax">
	<Value><![CDATA[excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.TaxShipping.ExclTax">
	<Value><![CDATA[excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Description2">
	<Value>Also note that some attribute control types that support custom user input (e.g. file upload, textboxes, date picker) are useless with attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Info">
    <Value>Vendor Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AllowCustomersToSelectPageSize">
    <Value>Allow customers to select page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AllowCustomersToSelectPageSize.Hint">
    <Value>Whether customers are allowed to select the page size from a predefined list of options.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaDescription">
    <Value>Meta description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaDescription.Hint">
    <Value>Meta description to be added to vendor page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaKeywords">
    <Value>Meta keywords</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaKeywords.Hint">
    <Value>Meta keywords to be added to vendor page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaTitle">
    <Value>Meta title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.MetaTitle.Hint">
    <Value>Override the page title. The default is the name of the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSize">
    <Value>Page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSize.Hint">
    <Value>Set the page size for products in this vendor e.g. ''4'' products per page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions">
    <Value>Page Size options (comma separated)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions.Hint">
    <Value>Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.SeName">
    <Value>Search engine friendly page name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-vendor'' to make your page URL ''http://www.yourStore.com/the-best-vendor''. Leave empty to generate it automatically based on the name of the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.DisplayOrder.Hint">
    <Value>Set the vendor''s display order. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToSelectPageSize">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToSelectPageSize.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.PageSize">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.PageSize.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.PageSizeOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.PageSizeOptions.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.NivoSlider.Picture5">
    <Value>Picture 5</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreAcl">
    <Value>Ignore ACL rules (sitewide)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreAcl.Hint">
    <Value>Check to ignore ACL rules configured for entities (sitewide). Recommended to enable this setting if you don''t use it. It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations">
    <Value>Ignore "limit per store" rules (sitewide)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations.Hint">
    <Value>Check to ignore "limit per store" rules configured for entities (sitewide). Recommended to enable this setting if you have only one store or don''t use it. It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations">
    <Value>Performance. You use only one store. Recommended to ignore store limitations (catalog settings)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl">
    <Value>Performance. Recommended to ignore ACL rules if you don''t use them (catalog settings)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Hosts.Hint">
    <Value>The comma separated list of possible HTTP_HOST values (for example, "yourstore.com,www.yourstore.com"). This property is required only when you run a multi-store solution to determine the current store.</Value>
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

--'Clear log' schedule task (disabled by default)
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ScheduleTask]
		WHERE [Type] = N'Nop.Services.Logging.ClearLogTask, Nop.Services')
BEGIN
	INSERT [dbo].[ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError])
	VALUES (N'Clear log', 3600, N'Nop.Services.Logging.ClearLogTask, Nop.Services', 0, 0)
END
GO

--delete checkout attributes. now they store specific
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] = N'Customer' and [Key] = N'CheckoutAttributes'
GO
--Store mapping for checkout attributes
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [CheckoutAttribute]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [CheckoutAttribute] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO

--topic SEO names
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_topic_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[temp_topic_generate_sename]
GO
CREATE PROCEDURE [dbo].[temp_topic_generate_sename]
(
	@entity_id int,
    @topic_system_name nvarchar(1000),
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @sql nvarchar(4000)
	
	--if system name is empty, we exit
	IF (@topic_system_name is null or @topic_system_name = N'')
		RETURN
    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@topic_system_name)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@topic_system_name, @p, 1)
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
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename)
					BEGIN
						SELECT @sename_is_already_reserved = 1
					END'
		EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
		
		IF (@sename_is_already_reserved > 0)
		BEGIN
			--add some digit to the end in this case
			SET @new_sename = @new_sename + '-1'
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



--update [sename] column for topics
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Topic]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Topic'
		
		DECLARE @topic_system_name nvarchar(1000)
		SET @topic_system_name = null -- clear cache (variable scope)
		SELECT @topic_system_name = [SystemName] FROM [Topic] WHERE [Id] = @sename_existing_entity_id
		
		--main sename
		EXEC	[dbo].[temp_topic_generate_sename]
				@entity_id = @sename_existing_entity_id,
				@topic_system_name = @topic_system_name,
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
		WHERE object_id = OBJECT_ID(N'[temp_topic_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_topic_generate_sename]
GO

--New column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='AllowAddingOnlyExistingAttributeCombinations')
BEGIN
	ALTER TABLE [Product]
	ADD [AllowAddingOnlyExistingAttributeCombinations] bit NULL
END
GO

UPDATE [Product]
SET [AllowAddingOnlyExistingAttributeCombinations] = 0
WHERE [AllowAddingOnlyExistingAttributeCombinations] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [AllowAddingOnlyExistingAttributeCombinations] bit NOT NULL
GO

--Store mapping for countries
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Country]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Country]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Country]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Country] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'localizationsettings.automaticallydetectlanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'localizationsettings.automaticallydetectlanguage', N'false', 0)
END
GO


--New "customer role" column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CustomerRole]') and NAME='PurchasedWithProductId')
BEGIN
	ALTER TABLE [CustomerRole]
	ADD [PurchasedWithProductId] int NULL
END
GO

UPDATE [CustomerRole]
SET [PurchasedWithProductId] = 0
WHERE [PurchasedWithProductId] IS NULL
GO

ALTER TABLE [CustomerRole] ALTER COLUMN [PurchasedWithProductId] int NOT NULL
GO

--New column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[OrderNote]') and NAME='DownloadId')
BEGIN
	ALTER TABLE [OrderNote]
	ADD [DownloadId] int NULL
END
GO

UPDATE [OrderNote]
SET [DownloadId] = 0
WHERE [DownloadId] IS NULL
GO

ALTER TABLE [OrderNote] ALTER COLUMN [DownloadId] int NOT NULL
GO

--New column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ValidationMinLength')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ValidationMinLength] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ValidationMaxLength')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ValidationMaxLength] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='ValidationMinLength')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [ValidationMinLength] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='ValidationMaxLength')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [ValidationMaxLength] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ValidationFileAllowedExtensions')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ValidationFileAllowedExtensions] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ValidationFileMaximumSize')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ValidationFileMaximumSize] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='ValidationFileAllowedExtensions')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [ValidationFileAllowedExtensions] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='ValidationFileMaximumSize')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [ValidationFileMaximumSize] int NULL
END
GO

DELETE FROM [Setting]
WHERE [name] = N'catalogsettings.fileuploadallowedextensions'
GO

DELETE FROM [Setting]
WHERE [name] = N'catalogsettings.fileuploadmaximumsizebytes'
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.disablebillingaddresscheckoutstep')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.disablebillingaddresscheckoutstep', N'false', 0)
END
GO

--new UPS settings
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'upssettings.passdimensions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'upssettings.passdimensions', N'true', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'upssettings.packingtype')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'upssettings.packingtype', N'0', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'upssettings.packingpackagevolume')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'upssettings.packingpackagevolume', N'5184', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'upssettings.tracing')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'upssettings.tracing', N'false', 0)
END
GO

--customer attributes
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerAttribute]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[CustomerAttribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(400) NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[AttributeControlTypeId] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerAttributeValue]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[CustomerAttributeValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerAttributeId] [int] NOT NULL,
	[Name] nvarchar(400) NOT NULL,
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
           WHERE  name = 'CustomerAttributeValue_CustomerAttribute'
           AND parent_object_id = Object_id('CustomerAttributeValue')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.CustomerAttributeValue
DROP CONSTRAINT CustomerAttributeValue_CustomerAttribute
GO
ALTER TABLE [dbo].[CustomerAttributeValue]  WITH CHECK ADD  CONSTRAINT [CustomerAttributeValue_CustomerAttribute] FOREIGN KEY([CustomerAttributeId])
REFERENCES [dbo].[CustomerAttribute] ([Id])
ON DELETE CASCADE
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.responsivedesignsupported')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'storeinformationsettings.responsivedesignsupported', N'true', 0)
END
GO

--remove some overridden settings that should not exist for stores
DELETE FROM [Setting]
WHERE [name] = N'ordersettings.returnrequestactions' AND [StoreId] > 0
GO

DELETE FROM [Setting]
WHERE [name] = N'ordersettings.returnrequestreasons' AND [StoreId] > 0
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'taxsettings.forcetaxexclusionfromordersubtotal')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'taxsettings.forcetaxexclusionfromordersubtotal', N'false', 0)
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfofooter')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfofooter', N'false', 0)
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoproductdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoproductdetailspage', N'false', 0)
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoproductboxes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoproductboxes', N'false', 0)
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.enabledynamicskumpngtinupdate')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.enabledynamicskumpngtinupdate', N'false', 0)
END
GO

--New columns for vendor
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='MetaKeywords')
BEGIN
	ALTER TABLE [Vendor]
	ADD [MetaKeywords] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='MetaDescription')
BEGIN
	ALTER TABLE [Vendor]
	ADD [MetaDescription] nvarchar(MAX) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='MetaTitle')
BEGIN
	ALTER TABLE [Vendor]
	ADD [MetaTitle] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='PageSize')
BEGIN
	ALTER TABLE [Vendor]
	ADD [PageSize] int NULL
END
GO

UPDATE [Vendor]
SET [PageSize] = 4
WHERE [PageSize] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [PageSize] int NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='AllowCustomersToSelectPageSize')
BEGIN
	ALTER TABLE [Vendor]
	ADD [AllowCustomersToSelectPageSize] bit NULL
END
GO

UPDATE [Vendor]
SET [AllowCustomersToSelectPageSize] = 1
WHERE [AllowCustomersToSelectPageSize] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [AllowCustomersToSelectPageSize] bit NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='PageSizeOptions')
BEGIN
	ALTER TABLE [Vendor]
	ADD [PageSizeOptions] nvarchar(200) NULL
END
GO

UPDATE [Vendor]
SET [PageSizeOptions] = N'8, 4, 12'
WHERE [PageSizeOptions] IS NULL
GO


IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='DisplayOrder')
BEGIN
	ALTER TABLE [Vendor]
	ADD [DisplayOrder] int NULL
END
GO

UPDATE [Vendor]
SET [DisplayOrder] = 1
WHERE [DisplayOrder] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [DisplayOrder] int NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.defaultvendorpagesizeoptions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.defaultvendorpagesizeoptions', N'8, 4, 12', 0)
END
GO

DELETE FROM [Setting]
WHERE [name] = N'vendorsettings.pagesize'
GO

DELETE FROM [Setting]
WHERE [name] = N'vendorsettings.allowcustomerstoselectpagesize'
GO

DELETE FROM [Setting]
WHERE [name] = N'vendorsettings.pagesizeoptions'
GO




--vendor SEO names
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_vendor_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[temp_vendor_generate_sename]
GO
CREATE PROCEDURE [dbo].[temp_vendor_generate_sename]
(
	@entity_id int,
    @vendor_name nvarchar(1000),
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @sql nvarchar(4000)
	
	--if name is empty, we exit
	IF (@vendor_name is null or @vendor_name = N'')
		RETURN
    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@vendor_name)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@vendor_name, @p, 1)
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
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename)
					BEGIN
						SELECT @sename_is_already_reserved = 1
					END'
		EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
		
		IF (@sename_is_already_reserved > 0)
		BEGIN
			--add some digit to the end in this case
			SET @new_sename = @new_sename + '-1'
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



--update [sename] column for vendors
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Vendor]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Vendor'
		
		DECLARE @vendor_name nvarchar(1000)
		SET @vendor_name = null -- clear cache (variable scope)
		SELECT @vendor_name = [Name] FROM [Vendor] WHERE [Id] = @sename_existing_entity_id
		
		--main sename
		EXEC	[dbo].[temp_vendor_generate_sename]
				@entity_id = @sename_existing_entity_id,
				@vendor_name = @vendor_name,
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
		WHERE object_id = OBJECT_ID(N'[temp_vendor_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_vendor_generate_sename]
GO

--new setings
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.ignoreacl')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.ignoreacl', N'false', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.ignorestorelimitations')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.ignorestorelimitations', N'false', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.usecuberootmethod')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.usecuberootmethod', N'true', 0)
END
GO