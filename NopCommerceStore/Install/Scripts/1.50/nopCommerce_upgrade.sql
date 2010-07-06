--upgrade scripts from nopCommerce 1.40 to nopCommerce 1.50




--new locale resources
declare @resources xml
set @resources='
<Language LanguageID="7">
 <LocaleResource Name="Account.PasswordIsRequired">
    <Value>Password is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductCost">
    <Value>Product cost:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductCost.Tooltip">
    <Value>The product cost is the cost of all the different components which make up the product. This may either be the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductCost.RequiredErrorMessage">
    <Value>Product cost is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductCost.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ProductCost">
    <Value>Product cost:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ProductCost.Tooltip">
    <Value>The product cost is the cost of all the different components which make up the product. This may either be the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ProductCost.RequiredErrorMessage">
    <Value>Product cost is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ProductCost.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageTemplateDetails.BCCEmailAddresses">
    <Value>BCC:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageTemplateDetails.BCCEmailAddresses.Tooltip">
    <Value>The blind carbon copy (BCC) recipients for this e-mail message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RegistrationMethod">
    <Value>Registration method:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RegistrationMethod.Other.Tooltip">
    <Value>Determines customer registration method. Standard - mode where vistors can register and no approval is required. Email Validation - mode where user must respond to validation email that is sent to them before they are activated. Admin Approval - mode where vistors can register but admin approval is required. Disabled - mode where registration is disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AdminApprovalRequired">
    <Value>Your account will be activated after approving by administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundButton.Text">
    <Value>Refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundButton.Tooltip">
    <Value>Refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundOfflineButton.Text">
    <Value>Refund (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundOfflineButton.Tooltip">
    <Value>Refund (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VoidButton.Text">
    <Value>Void</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VoidButton.Tooltip">
    <Value>Void</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VoidOfflineButton.Text">
    <Value>Void (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VoidOfflineButton.Tooltip">
    <Value>Void (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.Discount">
    <Value>Discount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.Total">
    <Value>Total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethods.IsActive">
    <Value>Is active</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ShippingRateComputationMethodInfo.Active">
    <Value>Active:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodInfo.Active.Tooltip">
    <Value>Determines whether this method is active and can be selected by customers during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryAdd.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryAdd.Discounts">
    <Value>Discounts applied to the category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerAdd.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.CustomerInfo">
    <Value>Customer Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.Roles">
    <Value>Customer Roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.CategoryMappings">
    <Value>Category Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.ManufacturerMappings">
    <Value>Manufacturer Mappings</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ProductInfo.StockQuantity.RangeErrorMessage">
    <Value>The value must be from -999999 to 999999</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ProductVariantInfo.StockQuantity.RangeErrorMessage">
    <Value>The value must be from -999999 to 999999</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ProductInfo.AllowOutOfStockOrders">
    <Value>Allow out of stock orders:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.AllowOutOfStockOrders.Tooltip">
    <Value>A value indicating whether to allow orders when out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.AllowOutOfStockOrders">
    <Value>Allow out of stock orders:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.AllowOutOfStockOrders.Tooltip">
    <Value>A value indicating whether to allow orders when out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.OutOfStock">
    <Value>Out of stock</Value>
  </LocaleResource>
  <LocaleResource Name="Search.SearchTermMinimumLengthIsNCharacters">
    <Value>Search term minimum length is {0} characters</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.SearchTermMinimumLengthIsNCharacters">
    <Value>Search term minimum length is {0} characters</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.Measures.Title">
    <Value>Measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.Measures.TitleDescription">
    <Value>Measures.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.Measures.Description">
    <Value>View all the measures for your store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Title">
    <Value>Manage measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.AddDimension.Text">
    <Value>Add dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.AddDimension.Tooltip">
    <Value>Add dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.AddWeight.Text">
    <Value>Add weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.AddWeight.Tooltip">
    <Value>Add weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.Title">
    <Value>Dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.Name.Edit">
    <Value>Edit dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.Ratio">
    <Value>Ratio</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.PrimaryDimension">
    <Value>Primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Dimensions.PrimaryDimension.Tooltip">
    <Value>Set as primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.Title">
    <Value>Weights</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.Name.Edit">
    <Value>Edit weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.Ratio">
    <Value>Ratio</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.PrimaryWeight">
    <Value>Primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.Weights.PrimaryWeight.Tooltip">
    <Value>Set as primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.MeasuresTitle">
    <Value>Measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.MeasuresDescription">
    <Value>Manage Measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionAdd.Title">
    <Value>Add a new dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionAdd.BackToMeasures">
    <Value>back to measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionAdd.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionAdd.SaveButton.Tooltip">
    <Value>Save dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.Title">
    <Value>Edit dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.BackToMeasures">
    <Value>back to measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.SaveButton.Tooltip">
    <Value>Save dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureDimensionDetails.DeleteButton.Tooltip">
    <Value>Delete dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightAdd.Title">
    <Value>Add a new weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightAdd.BackToMeasures">
    <Value>back to measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightAdd.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightAdd.SaveButton.Tooltip">
    <Value>Save weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.Title">
    <Value>Edit weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.BackToMeasures">
    <Value>back to measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.SaveButton.Tooltip">
    <Value>Save weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.MeasureWeightDetails.DeleteButton.Tooltip">
    <Value>Delete weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Name">
    <Value>Name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Name.Tooltip">
    <Value>The name of the dimension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Name.ErrorMessage">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.SystemKeyword">
    <Value>System keyword:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.SystemKeyword.Tooltip">
    <Value>A system keyword for this dimension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Ratio">
    <Value>Ratio to primary dimension:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Ratio.Tooltip">
    <Value>The ratio against the primary dimension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Ratio.RequiredErrorMessage">
    <Value>Ratio is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Ratio.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.DisplayOrder">
    <Value>Display order:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.DisplayOrder.Tooltip">
    <Value>The display order of the dimension. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.DisplayOrder.RequiredErrorMessage">
    <Value>Display order is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.DisplayOrder.RangeErrorMessage">
    <Value>The value must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Name">
    <Value>Name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Name.Tooltip">
    <Value>The name of the weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Name.ErrorMessage">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.SystemKeyword">
    <Value>System keyword:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.SystemKeyword.Tooltip">
    <Value>A system keyword for this weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Ratio">
    <Value>Ratio to primary weight:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Ratio.Tooltip">
    <Value>The ratio against the primary weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Ratio.RequiredErrorMessage">
    <Value>Ratio is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Ratio.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.DisplayOrder">
    <Value>Display order:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.DisplayOrder.Tooltip">
    <Value>The display order of the weight. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.DisplayOrder.RequiredErrorMessage">
    <Value>Display order is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.DisplayOrder.RangeErrorMessage">
    <Value>The value must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DownloadExpirationDays">
    <Value>Number of days:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DownloadExpirationDays.Tooltip">
    <Value>The number of days during customers keeps access to the file (e.g. 14). Leave this field blank to allow continuous downloads.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DownloadExpirationDays">
    <Value>Number of days:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DownloadExpirationDays.Tooltip">
    <Value>The number of days during customers keeps access to the file (e.g. 14). Leave this field blank to allow continuous downloads.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ID">
    <Value>ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ID.Tooltip">
    <Value>The product variant identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RestrictedProductVariants">
    <Value>Restricted product variants:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RestrictedProductVariants.Tooltip">
    <Value>The comma-separated list of product variant identifiers (e.g. 77, 123, 156).</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Dibs.FlexWin.Message">
    <Value>You will be redirected to DIBS site.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Svea.HostedPayment.Message">
    <Value>You will be redirected to Svea site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DownloadActivationType">
    <Value>Download activation type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DownloadActivationType.Tooltip">
    <Value>A value indicating when download links will be enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DownloadActivationType">
    <Value>Download activation type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DownloadActivationType.Tooltip">
    <Value>A value indicating when download links will be enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.DeactivateDownload">
    <Value>Deactivate</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.ActivateDownload">
    <Value>Activate</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.DiscountInfo.DiscountLimitation">
    <Value>Discount limitation:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.DiscountLimitation.Tooltip">
    <Value>Choose the limitation of discount.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ACLTitle">
    <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ACLDescription">
    <Value>Manage access control list</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.ACL.Title">
    <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.SaveButton.Tooltip">
    <Value>Save ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.ACLEnabled">
    <Value>Access control list enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.ACLEnabled.Tooltip">
    <Value>Check to enabled access control list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.Grid.CustomerAction">
    <Value>Customer action</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.ACL.Title">
    <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.ACL.TitleDescription">
    <Value>Manage access control list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.NoActionDefined">
    <Value>No customer actions defined.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ACL.NoRolesDefined">
    <Value>No customer roles defined.</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.Weight">
    <Value>Weight</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.UnnamedProductVariant">
    <Value>Unnamed product variant</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.BtnPDFExport.Text">
    <Value>Download catalog as PDF</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.BtnPDFExport.Tooltip">
    <Value>Print selected products to a PDF file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.Favicon">
    <Value>Favicon:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.Favicon.Tooltip">
    <Value>Allows to change your store "favourite icon" file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.TrackingNumber">
    <Value>Tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.TrackingNumber.Tooltip">
    <Value>Set a tracking number of the current order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SetTrackingNumberButton.Text">
    <Value>Set tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Order.TrackingNumber">
    <Value>Tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethods.BtnSave.Text">
	<Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethods.BtnSave.Tooltip">
	<Value>Save changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodsFilterControl.Grid.CountryName">
	<Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodsFilterControl.Title">
	<Value>Country restrictions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodsFilterControl.Info">
	<Value>Please mark the checkbox(es) for the country or countries in which you want the shipping method(s) not available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodsFilterControl.NoCountryDefined">
	<Value>No country defined.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodsFilterControl.NoShippingMethodDefined">
	<Value>No shipping method defined.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.BtnChangePassword.Text">
    <Value>Change Password</Value>
   </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.IsRecurring">
    <Value>Recurring product:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.IsRecurring.Tooltip">
    <Value>Check if this product is a recurring product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CycleLength">
    <Value>Cycle length:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CycleLength.Tooltip">
    <Value>Enter cycle length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CycleLength.RequiredErrorMessage">
    <Value>Cycle length is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CycleLength.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CyclePeriod">
    <Value>Cycle period:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CyclePeriod.Tooltip">
    <Value>Select cycle period.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TotalCycles">
    <Value>Total cycles:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TotalCycles.Tooltip">
    <Value>Enter total cycles.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TotalCycles.RequiredErrorMessage">
    <Value>Total cycles is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TotalCycles.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.IsRecurring">
    <Value>Recurring product:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.IsRecurring.Tooltip">
    <Value>Check if this product is a recurring product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CycleLength">
    <Value>Cycle length:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CycleLength.Tooltip">
    <Value>Enter cycle length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CycleLength.RequiredErrorMessage">
    <Value>Cycle length is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CycleLength.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CyclePeriod">
    <Value>Cycle period:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CyclePeriod.Tooltip">
    <Value>Select cycle period.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TotalCycles">
    <Value>Total cycles:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TotalCycles.Tooltip">
    <Value>Enter total cycles.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TotalCycles.RequiredErrorMessage">
    <Value>Total cycles is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TotalCycles.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShowCalendar">
    <Value>Click to show calendar</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Subject">
    <Value>Subject</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Subject.Tooltip">
    <Value>Message subject.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Subject.Required">
    <Value>Subject is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Body">
    <Value>Body</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Body.Tooltip">
    <Value>Message body.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.Body.Required">
    <Value>Body is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendEmail.SendButton">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerDetails.CustomerSendEmail">
    <Value>Send Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethods.BtnSave.Text">
	<Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethods.BtnSave.Tooltip">
	<Value>Save changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodsFilterControl.Grid.CountryName">
	<Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodsFilterControl.Title">
	<Value>Country restrictions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodsFilterControl.Info">
	<Value>Please mark the checkbox(es) for the country or countries in which you want the payment method(s) not available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodsFilterControl.NoCountryDefined">
	<Value>No country defined.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodsFilterControl.NoPaymentMethodDefined">
	<Value>No payment method defined.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.RecurringPaymentsTitle">
    <Value>Recurring Payments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.RecurringPaymentsDescription">
    <Value>Manage Recurring Payments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.RecurringPayments.Title">
    <Value>Recurring Payments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.RecurringPayments.TitleDescription">
    <Value>Recurring Payments.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.RecurringPayments.Description">
    <Value>Mange recurring payments for your store.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPayments.Title">
    <Value>Manage Recurring Products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.CustomerColumn">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.CustomerColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.CycleInfoColumn">
    <Value>Cycle info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.IsActiveColumn">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.StartDateColumn">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.NextPaymentColumn">
    <Value>Next payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.TotalCyclesColumn">
    <Value>Total cycles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.EditColumn">
    <Value>Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.EditColumn.Tooltip">
    <Value>Edit details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.NoRecurringPaymentsFound">
    <Value>No recurring payments found</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPaymentDetails.Title">
    <Value>Recurring payment details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.BackToRecurringPayments">
    <Value>back to recurring payments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.SaveButton">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.SaveButton.Tooltip">
    <Value>Save recurring payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.DeleteButton">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.DeleteButton.Tooltip">
    <Value>Delete recurring payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.RecurringPaymentInfo">
    <Value>Recurring payment info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.InitialOrder">
    <Value>Initial order:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.InitialOrder.Tooltip">
    <Value>Initial order of this recurring payment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.InitialOrder.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.Customer">
    <Value>Customer:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.Customer.Tooltip">
    <Value>Customer of this recurring payment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.Customer.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CycleLength">
    <Value>Cycle length:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CycleLength.Tooltip">
    <Value>Enter cycle length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CycleLength.RequiredErrorMessage">
    <Value>Cycle length is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CycleLength.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CyclePeriod">
    <Value>Cycle period:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CyclePeriod.Tooltip">
    <Value>Select cycle period.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.TotalCycles">
    <Value>Total cycles:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.TotalCycles.Tooltip">
    <Value>Enter total cycles.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.TotalCycles.RequiredErrorMessage">
    <Value>Total cycles is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.TotalCycles.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.StartDate">
    <Value>Start date:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.StartDate.Tooltip">
    <Value>Start date of this recurring payment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.IsActive">
    <Value>Is active:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.IsActive.Tooltip">
    <Value>Determines whether this recurring payment is active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History">
    <Value>History</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.NextPaymentDateIs">
    <Value>Next payment date is {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.ProcessNextPaymentButton.Text">
    <Value>Process next payment (create an order)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.OrderColumn">
    <Value>Created order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.OrderColumn.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.OrderStatusColumn">
    <Value>Order Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.PaymentStatusColumn">
    <Value>Payment Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.ShippingStatusColumn">
    <Value>Shipping Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.History.CreatedOnColumn">
    <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerDetails.CustomerSendPrivateMessage">
    <Value>Send Private Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.Subject">
    <Value>Subject</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.Subject.Tooltip">
    <Value>Message subject.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.Subject.Required">
    <Value>Subject is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.Body">
    <Value>Body</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.Body.Tooltip">
    <Value>Message body.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.SendButton">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerSendPrivateMessage.NotAllowed">
    <Value>Private messages are not allowed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber">
    <Value>Number of ''Recently viewed products'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.Tooltip">
    <Value>The number of ''Recently viewed products'' to display when ''Recently viewed products'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.RequiredErrorMessage">
    <Value>Enter a number of ''Recently viewed products'' to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber">
    <Value>Number of ''Recently added products'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.Tooltip">
    <Value>The number of ''Recently added products'' to display when ''Recently added products'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.RequiredErrorMessage">
    <Value>Enter a number of ''Recently added products'' to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber">
    <Value>Number of best sellers on home page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.Tooltip">
    <Value>The number of best sellers on home page to display when ''Show best sellers on home page'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.RequiredErrorMessage">
    <Value>Enter a number of best sellers on home page to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource> 
  <LocaleResource Name="NewsLetterSubscriptionActivation.Info">
    <Value>Subscription management page</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionActivation.ResultActivated">
    <Value>Your subscription has been activated.</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionActivation.ResultDectivated">
    <Value>Your subscription has been deactivated.</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.Info">
    <Value>Subscribe to newsletters</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.EmailInput">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.BtnSubscribe.Text">
    <Value>Submit</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.SubscriptionCreated">
    <Value>Thank you for signing up! A verification email has been sent. We appreciate your interest.</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.SubscriptionDeactivated">
    <Value>A verification email has been sent. Thank you!</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.Subscribe">
    <Value>Subscribe</Value>
  </LocaleResource>
  <LocaleResource Name="NewsLetterSubscriptionBox.Unsubscribe">
    <Value>Unsubscribe</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CannotMixStandardAndAutoshipProducts">
    <Value>Your cart has standard and auto-ship (recurring) items. Only one product type is allowed per order.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ConflictingShipmentSchedules">
    <Value>Your cart has auto-ship (recurring) items with conflicting shipment schedules. Only one auto-ship schedule is allowed per order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToReviewProduct">
    <Value>Allow anonymous users to write product reviews:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToReviewProduct.Tooltip">
    <Value>Check to allow anonymous users to write product reviews.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToSetProductRatings">
    <Value>Allow anonymous users to set product ratings:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToSetProductRatings.Tooltip">
    <Value>Check to allow anonymous users to set product ratings.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ProductReviewsMustBeApproved">
    <Value>Product reviews must be approved:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ProductReviewsMustBeApproved.Tooltip">
    <Value>Check if product reviews must be approved by administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.Information">
    <Value>Shopping Cart</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.CheckoutButton">
    <Value>Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.OrderSubtotal">
    <Value>Subtotal: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.OrderSubtotal.CalculatedDuringCheckout">
    <Value>Calculated during checkout</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.OneItemText">
    <Value>There is {0} in your cart.</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.SeveralItemsText">
    <Value>There are {0} in your cart.</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.OneItem">
    <Value>1 item</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.SeveralItems">
    <Value>{0} items</Value>
  </LocaleResource>
  <LocaleResource Name="MiniShoppingCartBox.NoItems">
    <Value>You have no items in your shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.Info">
    <Value>Topic Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaKeywords">
    <Value>Meta keywords:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaKeywords.ToolTip">
    <Value>Meta keywords to be added to topic page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaDescription">
    <Value>Meta description:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaDescription.ToolTip">
    <Value>Meta description to be added to topic page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaTitle">
    <Value>Meta title:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicLocalizedDetails.MetaTitle.ToolTip">
    <Value>Override the page title. The default is the name of the topic.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments">
    <Value>Recurring Payments</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.StartDateColumn">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.CycleInfoColumn">
    <Value>Cycle info</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.NextPaymentColumn">
    <Value>Next payment</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.TotalCyclesColumn">
    <Value>Total cycles</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.InitialOrderColumn">
    <Value>Initial order</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.CancelColumn">
    <Value>Cancel</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.Cancel">
    <Value>Cancel</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.InitialOrder.View">
    <Value>View order (ID - {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CbHasUserAgreement.Info">
    <Value>Has user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CbHasUserAgreement.Tooltip">
    <Value>Check if the product has an user agreement.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TxtUserAgreementText.Info">
    <Value>User agreement text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TxtUserAgreementText.Tooltip">
    <Value>The text of the user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CbHasUserAgreement.Info">
    <Value>Has user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CbHasUserAgreement.Tooltip">
    <Value>Check if the product has an user agreement.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TxtUserAgreementText.Info">
    <Value>User agreement text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TxtUserAgreementText.Tooltip">
    <Value>The text of the user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="UserAgreementControl.CbIsAgree.Text">
    <Value>I accept</Value>
  </LocaleResource>
  <LocaleResource Name="UserAgreementControl.BtnContinue.Text">
    <Value>Continue</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.BtnDuplicate.Text">
    <Value>Copy</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.BtnDuplicate.Tooltip">
    <Value>Create a full copy of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CancelPaymentButton.Text">
    <Value>Cancel payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageDetails.BtnXmlExport.Text">
    <Value>Export To XML</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageDetails.BtnXmlExport.Tooltip">
    <Value>Exports all string resources to a XML file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityLogHomeTitle">
    <Value>Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityLogHomeDescription">
    <Value>Activity Log Home</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityTypesTitle">
    <Value>Activity Types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityTypesDescription">
    <Value>Manage Activity Types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityLogTitle">
    <Value>Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ActivityLogDescription">
    <Value>View Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityLogHome">
    <Value>Activity Log Home</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.intro">
    <Value>Use the links on this page to manage activity types and view customer activity log.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityTypes.TitleDescription">
    <Value>Manage Customer Activity Types.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityTypes.Title">
    <Value>Manage Activity Types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityTypes.Description">
    <Value>Use this page to view list of customer activity types and turn on/off some of the activities.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityLog.TitleDescription">
    <Value>View Customer Activity Log.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityLog.Title">
    <Value>View Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogHome.ActivityLog.Description">
    <Value>Use this page to view customer activity logs.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogType.Title">
    <Value>Activity Types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogType.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogType.SaveButton.Tooltip">
    <Value>Save changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogType.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLogType.Enabled">
    <Value>Is Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.Title">
    <Value>View Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.SearchButton.Tooltip">
    <Value>Search for activity log based on the criteria below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CreatedOnFrom">
    <Value>Created from:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CreatedOnFrom.Tooltip">
    <Value>The creation from date for the search in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CreatedOnTo">
    <Value>Created to:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CreatedOnTo.Tooltip">
    <Value>The creation to date for the search in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CustomerEmail">
    <Value>Customer Email:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CustomerEmail.Tooltip">
    <Value>A customer Email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CustomerName">
    <Value>Username:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CustomerName.Tooltip">
    <Value>A customer username (if usernames are enabled).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.ActivityLogType">
    <Value>Activity Log Type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.ActivityLogType.Tooltip">
    <Value>Select an activity log type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.AllActivityLogType">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.ActivityLogTypeColumn">
    <Value>Activity Log Type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.MessageColumn">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CreateOnColumn">
    <Value>Created On</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreatePosts">
    <Value>Allow guests to create posts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreatePosts.Tooltip">
    <Value>Set if you want to allow guests to create posts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreateTopics">
    <Value>Allow guests to create topics</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreateTopics.Tooltip">
    <Value>Set if you want to allow guests to create topics.</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.QuotePost">
    <Value>Quote</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.CustomerIP">
    <Value>Customer IP address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.CustomerIP.Tooltip">
    <Value>Customer IP address from where order has been placed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.BtnBanByCustomerIP.Text">
    <Value>Ban IP Address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.BtnBanByCustomerIP.Tooltip">
    <Value>Add this IP address to blacklist</Value>
  </LocaleResource>
  <LocaleResource Name="Order.BtnReOrder.Text">
    <Value>Re-order</Value>
  </LocaleResource>
  <LocaleResource Name="Order.BtnReOrder.Tooltip">
    <Value>Place items in your shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ReOrderAllowed">
    <Value>Is re-order allowed:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ReOrderAllowed.Tooltip">
    <Value>Check if you want to allow customers to make re-orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Customer.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DisplayStockAvailability">
    <Value>Display Stock Availability:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DisplayStockAvailability.Tooltip">
    <Value>Check to display stock availability. When enabled, customers will see stock availability.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayStockAvailability">
    <Value>Display Stock Availability:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayStockAvailability.Tooltip">
    <Value>Check to display stock availability. When enabled, customers will see stock availability.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Availability">
    <Value>Availability: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Products.InStock">
    <Value>In stock</Value>
  </LocaleResource>
  <LocaleResource Name="Products.OutOfStock">
    <Value>Out of stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.BtnPrintPdfPackagingSlips.Text">
    <Value>Print Packaging Slips</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.BtnPrintPdfPackagingSlips.Tooltip">
    <Value>Print packaging slip for selected orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.BtnPrintPdfPackagingSlip.Text">
    <Value>Print Packaging Slip</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.BtnPrintPdfPackagingSlip.Tooltip">
    <Value>Print packaging slip for this order.</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Order">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.QTY">
    <Value>Quantity</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.ProductName">
    <Value>Product Name</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.AddToCart">
    <Value>Add to cart</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.AddToCartButton">
    <Value>Add to cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory">
    <Value>Usage History</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.CustomerColumn">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.CustomerColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.OrderColumn">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.OrderColumn.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.UsedOnColumn">
    <Value>Used</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.DeleteColumn">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.UsageHistory.DeleteButton.Tooltip">
    <Value>Click to delete this entry</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageInfo.ResourcesImported">
    <Value>All resources have been successfully imported.</Value>
  </LocaleResource>
  <LocaleResource Name="Search.SearchStoreTooltip">
    <Value>Search store</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.SearchForumsTooltip">
    <Value>Search forums</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.Inbox.MarkAsUnread">
    <Value>Mark selected as unread</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.News.Title">
    <Value>Manage News</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.News.NewsTitle">
    <Value>Title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistInfo.FormatLocalization.Tooltip">
    <Value>Select the required format localization.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAdd.Discounts">
    <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Print">
    <Value>Print</Value>
  </LocaleResource>
  <LocaleResource Name="Order.GetPDFInvoice">
    <Value>PDF Invoice</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ProductInfo.IsGiftCard">
    <Value>Is Gift Card:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.IsGiftCard.Tooltip">
    <Value>Check if it is a gift card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.IsGiftCard">
    <Value>Is Gift Card:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.IsGiftCard.Tooltip">
    <Value>Check if it is a gift card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Title">
    <Value>SMS Alerts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Enabled">
    <Value>Is Enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Enabled.Tooltip">
    <Value>Check if you want to receive SMS alerts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.PhoneNumber">
    <Value>Phone number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.PhoneNumber.Tooltip">
    <Value>Enter your mobile phone number here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.APIID">
    <Value>Clickatell API ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.APIID.Tooltip">
    <Value>Enter the clickatell API ID string here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Username">
    <Value>Clickatell username:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Username.Tooltip">
    <Value>Your Clickatell account username.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Password">
    <Value>Clickatell password:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Password.Tooltip">
    <Value>Your Clickatell account password.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.CustomerColumn">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.DeleteColumn">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.DeleteButton.Tooltip">
    <Value>Delete activity log entry</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.ClearButton.Text">
    <Value>Clear All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ActivityLog.ClearButton.Tooltip">
    <Value>Clears activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.PurchasedGiftCardsTitle">
    <Value>Purchased Gift Cards</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.PurchasedGiftCardsDescription">
    <Value>View and manage the gict cards that customers have purchased</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.PurchasedGiftCards.Title">
    <Value>Purchased Gift Cards</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.PurchasedGiftCards.TitleDescription">
    <Value>Manage Purchased Gift Cards.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.PurchasedGiftCards.Description">
    <Value>View and manage the gict cards that customers have purchased.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Title">
    <Value>Purchased Gift Cards</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.SearchButton.Tooltip">
    <Value>Search for gift cards based on the criteria below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.PurchasedFrom">
    <Value>Purchased from:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.PurchasedFrom.Tooltip">
    <Value>The purchased from date for the search in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.PurchasedTo">
    <Value>Purchased to:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.PurchasedTo.Tooltip">
    <Value>The purchased to date for the search in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.OrderStatus">
    <Value>Order status:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.OrderStatus.Tooltip">
    <Value>Search by a specific order status e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Activated">
    <Value>Activated:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Activated.Tooltip">
    <Value>Search by a specific gift card status e.g. Activated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Activated.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Activated.Activated">
    <Value>Activated</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Activated.Deactivated">
    <Value>Deactivated</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.GiftCardCouponCode">
    <Value>Gift card coupon code:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.GiftCardCouponCode.Tooltip">
    <Value>A gift card coupon code. Leave empty to load all records.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.CustomerColumn">
    <Value>Purchasing Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.OrderStatusColumn">
    <Value>Order Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.InitialValueColumn">
    <Value>Initial Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.RemainingAmountColumn">
    <Value>Remaining Amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.CouponCodeColumn">
    <Value>Coupon Code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.IsGiftCardActivatedColumn">
    <Value>Activated</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.PurchasedOnColumn">
    <Value>Purchased On</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.EditColumn">
    <Value>Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PurchasedGiftCards.EditColumn.Tooltip">
    <Value>Edit gift card details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardDetails.Title">
    <Value>Edit gift card details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardDetails.BackToGiftCards">
    <Value>back to gift card list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardDetails.SaveButton">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardDetails.SaveButton.Tooltip">
    <Value>Save gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.GiftCardInfo">
    <Value>Gift Card Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Order">
    <Value>Order:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Order.Tooltip">
    <Value>The order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Order.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Customer">
    <Value>Purchasing customer:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Customer.Tooltip">
    <Value>The purchasing customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Customer.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.InitialValue">
    <Value>Initial value:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.InitialValue.Tooltip">
    <Value>The initial value of this gift card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.InitialValue.RequiredErrorMessage">
    <Value>Initial value is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.InitialValue.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RemainingAmount">
    <Value>Remaining amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RemainingAmount.Tooltip">
    <Value>The remaining amount of this gift card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.IsGiftCardActivated">
    <Value>Is gift card activated:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.IsGiftCardActivated.Tooltip">
    <Value>Determines whether gift this card is activated and can be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.CouponCode">
    <Value>Coupon code:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.CouponCode.Tooltip">
    <Value>The gift card coupon code (used during checkout).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.CouponCode.ReGenerateNewButton">
    <Value>Regenerate code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RecipientName">
    <Value>Recipient''s Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RecipientName.Tooltip">
    <Value>Enter recipient''s name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RecipientName.ErrorMessage">
    <Value>Recipient''s name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RecipientEmail">
    <Value>Recipient''s Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.RecipientEmail.Tooltip">
    <Value>Enter recipient''s email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.SenderName">
    <Value>Sender''s Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.SenderName.Tooltip">
    <Value>Enter sender''s name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.SenderName.ErrorMessage">
    <Value>Sender''s name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.SenderEmail">
    <Value>Sender''s Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.SenderEmail.Tooltip">
    <Value>Enter sender''s email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Message">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.Message.Tooltip">
    <Value>Enter message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.IsSenderNotified">
    <Value>Is sender notified</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.IsSenderNotified.Tooltip">
    <Value>Is sender notified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.NotifySenderButton">
    <Value>Notify sender</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.PurchasedOn">
    <Value>Purchased on:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.PurchasedOn.Tooltip">
    <Value>The date/time the gift card was purchased.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory">
    <Value>Usage History</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.UsedValueColumn">
    <Value>Used amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.ByCustomerColumn">
    <Value>By customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.ByCustomerColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.OrderColumn">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.OrderColumn.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.UsageHistory.RecordDateColumn">
    <Value>Record date</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode">
    <Value>Discount Code</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Tooltip">
    <Value>Enter your coupon here</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ApplyDiscountCouponCodeButton">
    <Value>Apply coupon</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCards">
    <Value>Gift Cards</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCards.Tooltip">
    <Value>Enter gift card code</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ApplyGiftCardCouponCodeButton">
    <Value>Add gift card</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.GiftCardInfo">
    <Value>Gift Card ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.GiftCardInfo.Remaining">
    <Value>{0} remaining</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCards.WrongGiftCard">
    <Value>The coupon code you entered couldn''t be applied to your order</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.WrongDiscount">
    <Value>The coupon code you entered couldn''t be applied to your order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.Title">
    <Value>Maintenance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.BackupButton.Text">
    <Value>Backup Database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.BackupButton.Tooltip">
    <Value>Backup Database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.FileNameColumn">
    <Value>File Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.FileSizeColumn">
    <Value>Size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.RestoreColumn">
    <Value>Restore</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.RestoreButton.Text">
    <Value>Restore</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.RestoreButton.Tooltip">
    <Value>Restore Database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteColumn">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteButton.Tooltip">
    <Value>Delete backup file</Value>
  </LocaleResource>
  <LocaleResource Name="Products.GiftCard.RecipientName">
    <Value>Recipient''s Name:</Value>
  </LocaleResource>
  <LocaleResource Name="Products.GiftCard.RecipientEmail">
    <Value>Recipient''s Email:</Value>
  </LocaleResource>
  <LocaleResource Name="Products.GiftCard.SenderName">
    <Value>Your Name:</Value>
  </LocaleResource>
  <LocaleResource Name="Products.GiftCard.SenderEmail">
    <Value>Your Email:</Value>
  </LocaleResource>
  <LocaleResource Name="Products.GiftCard.Message">
    <Value>Message:</Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.For">
    <Value>For: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From">
    <Value>From: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.RecipientNameError">
    <Value>Enter valid recipient name</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.RecipientEmailError">
    <Value>Enter valid recipient email</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.SenderNameError">
    <Value>Enter valid sender name</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.SenderEmailError">
    <Value>Enter valid sender email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.MaintenanceTitle">
    <Value>Maintenance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.MaintenanceDescription">
    <Value>Maintenance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Maintenance.TitleDescription">
    <Value>Maintenance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Maintenance.Title">
    <Value>Maintenance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Maintenance.Description">
    <Value>Allow users to backup and restore database.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Discount">
    <Value>Discount</Value>
  </LocaleResource>
  <LocaleResource Name="Order.GiftCardInfo">
    <Value>Gift card ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.Discount">
    <Value>Discount:</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.GiftCardInfo">
    <Value>Gift card ({0}):</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.GiftCardInfo">
    <Value>Gift card ({0}):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.GiftCardInfo">
    <Value>Gift card ({0}):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.GiftCardInfo.Tooltip">
    <Value>Applied gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.DownloadColumn">
    <Value>Downloable product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.Download">
    <Value>Download file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.License">
    <Value>License file (optional):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.License.Download">
    <Value>Download license file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.License.Remove">
    <Value>Remove license file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.License.UploadButton">
    <Value>Upload license file</Value>
  </LocaleResource>
  <LocaleResource Name="Order.DownloadLicense">
    <Value>Download license</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.CheckoutOnePage">
    <Value>Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.General.UseOnePageCheckout">
    <Value>Use one page checkout:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.General.UseOnePageCheckout.Tooltip">
    <Value>One page checkout is a single web page your customers can use to buy a products/service from you.</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Categories">
    <Value>Categories</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Manufacturers">
    <Value>Manufacturers</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Products">
    <Value>Products</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Topics">
    <Value>Topics</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.OtherPages">
    <Value>Other</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.Title">
    <Value>Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.ShippingAddress.Title">
    <Value>Shipping Address</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.BillingAddress.Title">
    <Value>Billing Address</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.ShippingMethods.Title">
    <Value>Shipping Method</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.PaymentMethods.Title">
    <Value>Payment Method</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.PaymentInfo.Title">
    <Value>Payment Info</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.Confirm.Title">
    <Value>Order Confirmation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.Usernames">
    <Value>''Usernames'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CompareProducts">
    <Value>''Compare Products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.WishList">
    <Value>''Wishlist'' Enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.EmailAFriend">
    <Value>''Email a friend'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProducts">
    <Value>''Recently viewed products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProducts">
    <Value>''Recently added products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Amazon.SimplePay.Message">
    <Value>You will be redirected to Amazon site.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.PayPoint.HostedPayment.Message">
    <Value>You will be redirected to PayPoint site.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Manufacturer.ImageLinkTitleFormat">
    <Value>Show products manufactured by {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Manufacturer.ImageAlternateTextFormat">
    <Value>Picture for manufacturer {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManageInventoryMethod.DontManage">
    <Value>Don''t track inventory for this product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManageInventoryMethod.Manage">
    <Value>Track inventory for this product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManageInventoryMethod.ManageByAttributes">
    <Value>Track inventory for this variant by product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.Title">
    <Value>Attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.SelectCombination">
    <Value>Select new combination and enter details below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.StockQuantity">
    <Value>Stock quantity:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.StockQuantity.Tooltip">
    <Value>The current stock quantity of this combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.StockQuantity.RequiredErrorMessage">
    <Value>Enter stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.StockQuantity.RangeErrorMessage">
    <Value>The value must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.AllowOutOfStockOrders">
    <Value>Allow out of stock orders:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.AllowOutOfStockOrders.Tooltip">
    <Value>A value indicating whether to allow orders when out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.AddNewButton.Text">
    <Value>Add combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity.RequiredErrorMessage">
    <Value>Enter stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity.RangeErrorMessage">
    <Value>The value must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.AllowOutOfStockOrders">
    <Value>Allow out of stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.Update">
    <Value>Update</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.Delete">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Manufactures">
    <Value>Manufactures</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.USAePay.EPaymentForm.Message">
    <Value>You will be redirected to USA ePay site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Measures.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviders.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Campaigns.ExportEmailsButton.Text">
    <Value>Export emails to CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Campaigns.ExportEmailsButton.Tooltip">
    <Value>Export emails subscribed to newsletters to a comma-separated value (CSV) file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LogDetails.ReferrerURL">
    <Value>Referrer:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LogDetails.ReferrerURL.Tooltip">
    <Value>The referrer URL.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Boards.ActiveDiscussions">
    <Value>Active discussions</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.ActiveDiscussions.ViewAll">
    <Value>View all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.ShippingAddress.Google">
    <Value>View address on Google Maps</Value>
  </LocaleResource>
  <LocaleResource Name="Order.RecurringPayments.CyclesRemainingColumn">
    <Value>Cycles remaining</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.CyclesRemainingColumn">
    <Value>Cycles remaining</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CyclesRemaining">
    <Value>Cycles remaining:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CyclesRemaining.Tooltip">
    <Value>Number of cycles remaining.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.RecurringPaymentType">
    <Value>Payment type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.RecurringPaymentType.Tooltip">
    <Value>Indicates the payment type (Manual or Automatic).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AccessDenied.Title">
    <Value>Access denied.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AccessDenied.Description">
    <Value>You do not have permission to perform the selected operation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.BtnBackupPictures.Text">
    <Value>Backup Pictures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.BtnBackupPictures.Tooltip">
    <Value>Backup pictures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.AttributeCombinations">
    <Value>Attribute Combinations</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Moneris.HostedPayment.Message">
    <Value>You will be redirected to Moneris site.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Beanstream.HostedPayment.Message">
    <Value>You will be redirected to Beanstream site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.Username.ErrorMessage">
    <Value>Username is required</Value>
  </LocaleResource>
  <LocaleResource Name="Customer.PasswordIsRequired">
    <Value>Password is required</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Notes">
    <Value>Note(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Order.OrderNotes.CreatedOn">
    <Value>Created On</Value>
  </LocaleResource>
  <LocaleResource Name="Order.OrderNotes.Note">
    <Value>Note</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.OrderNotes.New.DisplayToCustomer.Text">
    <Value>Display To customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.OrderNotes.DisplayToCustomerColumn">
    <Value>Display To customer</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.Company">
    <Value>Company: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddToShoppingCart">
    <Value>Added a product (''{0}'') to shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.RemoveFromShoppingCart">
    <Value>Removed a product (''{0}'') from shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCategory">
    <Value>Added a new category (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCategory">
    <Value>Edited a category (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCategory">
    <Value>Deleted a category (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewManufacturer">
    <Value>Added a new manufacturer (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditManufacturer">
    <Value>Edited a manufacturer (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteManufacturer">
    <Value>Deleted a manufacturer (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewProduct">
    <Value>Added a new product (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditProduct">
    <Value>Edited a product (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteProduct">
    <Value>Deleted a product (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewProductVariant">
    <Value>Added a new product variant (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditProductVariant">
    <Value>Edited a product variant (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteProductVariant">
    <Value>Deleted a product variant (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewProductAttribute">
    <Value>Added a new product attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditProductAttribute">
    <Value>Edited a product attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteProductAttribute">
    <Value>Deleted a product attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewSpecAttribute">
    <Value>Added a new specification attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditSpecAttribute">
    <Value>Edited a specification attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteSpecAttribute">
    <Value>Deleted a specification attribute (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditGiftCard">
    <Value>Edited a gift card (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCustomer">
    <Value>Added a new customer (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCustomer">
    <Value>Edited a customer (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCustomer">
    <Value>Deleted a customer (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCustomerRole">
    <Value>Added a new customer role (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCustomerRole">
    <Value>Edited a customer role (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCustomerRole">
    <Value>Deleted a customer role (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewDiscount">
    <Value>Added a new discount (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditDiscount">
    <Value>Edited a discount (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteDiscount">
    <Value>Deleted a discount (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditGlobalSettings">
    <Value>Edited global settings</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditPaymentMethod">
    <Value>Edited a payment method (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditTaxSettings">
    <Value>Edited tax settings</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditTaxProvider">
    <Value>Edited a tax provider (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditShippingSettings">
    <Value>Edited shipping settings</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditShippingProvider">
    <Value>Edit a shipping rate computation method (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewSetting">
    <Value>Added a new setting (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditSetting">
    <Value>Edited a setting (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteSetting">
    <Value>Deleted a setting (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.CreateBackup">
    <Value>Created a backup</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.RestoreBackup">
    <Value>Restored a backup</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PlaceOrder">
    <Value>Placed an order (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.WriteProductReview">
    <Value>Wrote a product review (Product ID = ''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.ChronoPay.HostedPayment.Message">
    <Value>You will be redirected to ChronoPay site.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.Assist.HostedPayment.Message">
    <Value>You will be redirected to Assist site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviderInfo.InfoPanel">
    <Value>System info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviderInfo.ConfigurePanel">
    <Value>Configuration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodDetails.InfoPanel">
    <Value>System info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodDetails.ConfigurePanel">
    <Value>Configuration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodDetails.InfoPanel">
    <Value>System info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodDetails.ConfigurePanel">
    <Value>Configuration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Security.AllowedIPList">
    <Value>Admin area allowed IP:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Security.AllowedIPList.Tooltip">
    <Value>IP addresses allowed to access the Back End. Leave this field empty, if you do not want to restict access to the Back End. Use comma to separate them (e.g. 127.0.0.10,232.18.204.16)</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.CyberSource.HostedPayment.Message">
    <Value>You will be redirected to CyberSource site.</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.ShippingAddress.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.BillingAddress.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.ShippingMethods.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.PaymentMethods.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.PaymentInfo.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="CheckoutOnePage.Confirm.Modify">
    <Value>Modify</Value>
  </LocaleResource>
  <LocaleResource Name="Manufacturer.ViewAll">
    <Value>View All</Value>
  </LocaleResource>
  <LocaleResource Name="iDealPaymentModule.Message">
    <Value>You will be redirected to iDeal site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.ManufacturerList">
    <Value>Manufacturer List</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.RelatedProducts">
    <Value>Related Products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.Pictures">
    <Value>Pictures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.ProductSpecification">
    <Value>Product Specifications</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.ProductSpecification">
    <Value>Product Specifications</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RelatedProducts.AvailableAfterSaving">
    <Value>You need to save the product before you can add related products for this product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductPictures.AvailableAfterSaving">
    <Value>You need to save the product before you can upload pictures for this product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductSpecifications.AvailableAfterSaving">
    <Value>You need to save the product before you can add product specification attributes for this product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAdd.TierPrices">
    <Value>Tier Prices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAdd.Attributes">
    <Value>Product Variant Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantTierPrices.AvailableAfterSaving">
    <Value>You need to save the product variant before you can enable tier pricing for this product variant page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.AvailableAfterSaving">
    <Value>You need to save the product variant before you can add product attributes for this product variant page.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts">
    <Value>My Downloadable Products</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.NoProducts">
    <Value>You have not purchased any downloadable products yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.DownloadLicense">
    <Value>Download license</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.ProductsGrid.Order">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.ProductsGrid.Date">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.ProductsGrid.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DownloadableProducts.ProductsGrid.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowMiniShoppingCart">
    <Value>Show mini-shopping cart:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowMiniShoppingCart.Tooltip">
    <Value>Check to enabled this option.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.Title">
    <Value>Live Chat</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.Enabled">
    <Value>Enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.Enabled.ToolTip">
    <Value>Check if you want to use LiveChat system.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.SiteID">
    <Value>Site ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.SiteID.Tooltip">
    <Value>Enter your Site ID here</Value>
  </LocaleResource>
  <LocaleResource Name="Content.LiveChat">
    <Value>Live Chat</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SearchTermStat.ClearLabel.Text">
    <Value>[Clear]</Value>
  </LocaleResource>
  <LocaleResource Name="HomePage.FeaturedProducts">
    <Value>Featured Products</Value>
  </LocaleResource>
  <LocaleResource Name="Download.UserAgreement">
    <Value>User Agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.BtnCode">
    <Value>Button code(max 2000):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.BtnCode.Tooltip">
    <Value>Enter your button code here</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.MonCode">
    <Value>Monitoring code(max 2000):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.LiveChat.MonCode.Tooltip">
    <Value>Enter your monitoring code here</Value>
  </LocaleResource>
  <LocaleResource Name="SermepaPaymentModule.Message">
    <Value>You will be redirected to Sermepa site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="SagePayPaymentModule.Message">
    <Value>You will be redirected to SagePay site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="QuickPayPaymentModule.Message">
    <Value>You will be redirected to QuickPay site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.BackupCreated">
    <Value>Database backup was successfully created.</Value>
  </LocaleResource>
</Language>
'
 		
CREATE TABLE #LocaleStringResourceTmp
	(
		[LanguageID] [int] NOT NULL,
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)


INSERT INTO #LocaleStringResourceTmp (LanguageID, ResourceName, ResourceValue)
SELECT	@resources.value('(/Language/@LanguageID)[1]', 'int'), nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

DECLARE @LanguageID int
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
GO



--replaced @@identity with SCOPE_IDENTITY()
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShoppingCartItemInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShoppingCartItemInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShoppingCartItemInsert]
(
	@ShoppingCartItemID int = NULL output,
	@ShoppingCartTypeID int,
	@CustomerSessionGUID uniqueidentifier,
	@ProductVariantID int,
	@AttributesXML XML,
	@Quantity int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_ShoppingCartItem]
	(
		ShoppingCartTypeID,
		CustomerSessionGUID,
		ProductVariantID,
		AttributesXML,
		Quantity,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@ShoppingCartTypeID,
		@CustomerSessionGUID,
		@ProductVariantID,
		@AttributesXML,
		@Quantity,
		@CreatedOn,
		@UpdatedOn
	)

	set @ShoppingCartItemID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
(
	@OrderProductVariantID int = NULL output,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderProductVariant]
	(
		OrderID,
		ProductVariantID,
		UnitPriceInclTax,
		UnitPriceExclTax,
		PriceInclTax,
		PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency,
		AttributeDescription,
		Quantity,
		DiscountAmountInclTax,
		DiscountAmountExclTax,
		DownloadCount
	)
	VALUES
	(
		@OrderID,
		@ProductVariantID,
		@UnitPriceInclTax,
		@UnitPriceExclTax,
		@PriceInclTax,
		@PriceExclTax,
		@UnitPriceInclTaxInCustomerCurrency,
		@UnitPriceExclTaxInCustomerCurrency,
		@PriceInclTaxInCustomerCurrency,
		@PriceExclTaxInCustomerCurrency,
		@AttributeDescription,
		@Quantity,
		@DiscountAmountInclTax,
		@DiscountAmountExclTax,
		@DownloadCount
	)

	set @OrderProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariant_ProductAttribute_MappingInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariant_ProductAttribute_MappingInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariant_ProductAttribute_MappingInsert]
(
	@ProductVariantAttributeID int = NULL output,
	@ProductVariantID int,
	@ProductAttributeID int,
	@TextPrompt nvarchar(200),
	@IsRequired bit,
	@AttributeControlTypeID int,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductVariant_ProductAttribute_Mapping]
	(
		ProductVariantID,
		ProductAttributeID,
		TextPrompt,
		IsRequired,
		AttributeControlTypeID,
		DisplayOrder
	)
	VALUES
	(
		@ProductVariantID,
		@ProductAttributeID,
		@TextPrompt,
		@IsRequired,
		@AttributeControlTypeID,
		@DisplayOrder
	)

	set @ProductVariantAttributeID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		HasSampleDownload,
		SampleDownloadID,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@HasSampleDownload,
		@SampleDownloadID,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductInsert]
(
	@ProductID int = NULL output,
	@Name nvarchar(400),
	@ShortDescription ntext,
	@FullDescription ntext,
	@AdminComment ntext,
	@ProductTypeID int,
	@TemplateID int,
	@ShowOnHomePage bit,
	@MetaKeywords nvarchar(400),
	@MetaDescription nvarchar(4000),
	@MetaTitle nvarchar(400),
	@SEName nvarchar(100),
	@AllowCustomerReviews bit,
	@AllowCustomerRatings bit,
	@RatingSum int,
	@TotalRatingVotes int,
	@Published bit,
	@Deleted bit,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Product]
	(
		[Name],
		ShortDescription,
		FullDescription,
		AdminComment,
		ProductTypeID,
		TemplateID,
		ShowOnHomePage,
		MetaKeywords,
		MetaDescription,
		MetaTitle,
		SEName,
		AllowCustomerReviews,
		AllowCustomerRatings,
		RatingSum,
		TotalRatingVotes,
		Published,
		Deleted,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@Name,
		@ShortDescription,
		@FullDescription,
		@AdminComment,
		@ProductTypeID,
		@TemplateID,
		@ShowOnHomePage,
		@MetaKeywords,
		@MetaDescription,
		@MetaTitle,
		@SEName,
		@AllowCustomerReviews,
		@AllowCustomerRatings,
		@RatingSum,
		@TotalRatingVotes,
		@Published,
		@Deleted,
		@CreatedOn,
		@UpdatedOn
	)

	set @ProductID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DownloadInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DownloadInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DownloadInsert]
(
	@DownloadID int = NULL output,
	@UseDownloadURL bit,
	@DownloadURL nvarchar(400),
	@DownloadBinary image,	
	@ContentType nvarchar(20),
	@Extension nvarchar(20),
	@IsNew	bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Download]
	(
		UseDownloadURL,
		DownloadURL,
		DownloadBinary,
		ContentType,
		Extension,
		IsNew
	)
	VALUES
	(
		@UseDownloadURL,
		@DownloadURL,
		@DownloadBinary,
		@ContentType,
		@Extension,
		@IsNew
	)

	set @DownloadID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		PurchaseOrderNumber,
		PaymentStatusID,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerInsert]
(
	@CustomerId int = NULL output,
	@CustomerGUID uniqueidentifier,
	@Email nvarchar(255),
	@PasswordHash nvarchar(255),
	@SaltKey nvarchar(255),
	@AffiliateID int,
	@BillingAddressID int,
	@ShippingAddressID int,
	@LastPaymentMethodID int,
	@LastAppliedCouponCode nvarchar(100),
	@LanguageID int,
	@CurrencyID int,
	@TaxDisplayTypeID int,
	@IsTaxExempt bit,
	@IsAdmin bit,
	@IsGuest bit,
	@IsForumModerator bit,
	@TotalForumPosts int,
	@Signature nvarchar(300),
	@AdminComment nvarchar(4000),
	@Active bit,
	@Deleted bit,
	@RegistrationDate datetime,
	@TimeZoneID nvarchar(200),
	@Username nvarchar(100),
	@AvatarID int
)
AS
BEGIN
	INSERT
	INTO [Nop_Customer]
	(
		CustomerGUID,
		Email,
		PasswordHash,
		SaltKey,
		AffiliateID,
		BillingAddressID,
		ShippingAddressID,
		LastPaymentMethodID,
		LastAppliedCouponCode,
		LanguageID,
		CurrencyID,
		TaxDisplayTypeID,
		IsTaxExempt,
		IsAdmin,
		IsGuest,
		IsForumModerator,
		TotalForumPosts,
		[Signature],
		AdminComment,
		Active,
		Deleted,
		RegistrationDate,
		TimeZoneID,
		Username,
		AvatarID
	)
	VALUES
	(
		@CustomerGUID,
		@Email,
		@PasswordHash,
		@SaltKey,
		@AffiliateID,
		@BillingAddressID,
		@ShippingAddressID,
		@LastPaymentMethodID,
		@LastAppliedCouponCode,
		@LanguageID,
		@CurrencyID,
		@TaxDisplayTypeID,
		@IsTaxExempt,
		@IsAdmin,
		@IsGuest,
		@IsForumModerator,
		@TotalForumPosts,
		@Signature,
		@AdminComment,
		@Active,
		@Deleted,
		@RegistrationDate,
		@TimeZoneID,
		@Username,
		@AvatarID
	)

	set @CustomerId=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PrivateMessageInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PrivateMessageInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_PrivateMessageInsert]
(
	@PrivateMessageID int = NULL output,
	@FromUserID int,
	@ToUserID int,
	@Subject nvarchar(450),
	@Text nvarchar(max),
	@IsRead bit,
	@IsDeletedByAuthor bit,
	@IsDeletedByRecipient bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_PrivateMessage]
	(
		[FromUserID],
		[ToUserID],
		[Subject],
		[Text],
		[IsRead],
		[IsDeletedByAuthor],
		[IsDeletedByRecipient],
		[CreatedOn]
	)
	VALUES
	(
		@FromUserID,
		@ToUserID,
		@Subject,
		@Text,
		@IsRead,
		@IsDeletedByAuthor,
		@IsDeletedByRecipient,
		@CreatedOn
	)

	set @PrivateMessageID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductPictureInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductPictureInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductPictureInsert]
(
	@ProductPictureID int = NULL output,
	@ProductID int,	
	@PictureID int,	
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductPicture]
	(
		ProductID,
		PictureID,
		DisplayOrder
	)
	VALUES
	(
		@ProductID,
		@PictureID,
		@DisplayOrder
	)

	set @ProductPictureID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SearchLogInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SearchLogInsert]
GO
CREATE PROCEDURE [dbo].[Nop_SearchLogInsert]
(
	@SearchLogID int = NULL output,
	@SearchTerm nvarchar(100),
	@CustomerID int,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_SearchLog]
	(
		SearchTerm,
		CustomerID,
		CreatedOn
	)
	VALUES
	(
		@SearchTerm,
		@CustomerID,
		@CreatedOn
	)

	set @SearchLogID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerRoleInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerRoleInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerRoleInsert]
(
	@CustomerRoleID int = NULL output,
	@Name nvarchar(255),
	@FreeShipping bit,
	@TaxExempt bit,
	@Active bit,
	@Deleted bit
)
AS
BEGIN
	INSERT
	INTO [Nop_CustomerRole]
	(
		[Name],
		FreeShipping,
		TaxExempt,
		Active,
		Deleted	
	)
	VALUES
	(
		@Name,
		@FreeShipping,
		@TaxExempt,
		@Active,
		@Deleted
	)

	set @CustomerRoleID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LogInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LogInsert]
GO
CREATE PROCEDURE [dbo].[Nop_LogInsert]
(
	@LogID int = NULL output,
	@LogTypeID int,
	@Severity int,
	@Message nvarchar(1000),
	@Exception nvarchar(4000),
	@IPAddress nvarchar(100),
	@CustomerID int,
	@PageURL nvarchar(100),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Log]
	(
		LogTypeID,
		Severity,
		[Message],
		Exception,
		IPAddress,
		CustomerID,
		PageURL,
		CreatedOn
	)
	VALUES
	(
		@LogTypeID,
		@Severity,
		@Message,
		@Exception,
		@IPAddress,
		@CustomerID,
		@PageURL,
		@CreatedOn
	)

	set @LogID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CountryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CountryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CountryInsert]
(
	@CountryId int = NULL output,
	@Name nvarchar(100),
	@AllowsRegistration bit, 
	@AllowsBilling bit, 
	@AllowsShipping bit, 
	@TwoLetterISOCode nvarchar(2),
	@ThreeLetterISOCode nvarchar(3),
	@NumericISOCode int,
	@Published bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_Country]
	(
		[Name],
		[AllowsRegistration],
		[AllowsBilling],
		[AllowsShipping],
		[TwoLetterISOCode],
		[ThreeLetterISOCode],
		[NumericISOCode],
		[Published],
		[DisplayOrder]
	)
	VALUES
	(
		@Name,
		@AllowsRegistration,
		@AllowsBilling,
		@AllowsShipping,
		@TwoLetterISOCode,
		@ThreeLetterISOCode,
		@NumericISOCode,
		@Published,
		@DisplayOrder
	)

	set @CountryId=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_StateProvinceInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_StateProvinceInsert]
GO
CREATE PROCEDURE [dbo].[Nop_StateProvinceInsert]
(
	@StateProvinceID int = NULL output,
	@CountryID int,
	@Name nvarchar(100),
	@Abbreviation nvarchar (10),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_StateProvince]
	(
		CountryID,
		[Name],
		Abbreviation,
		[DisplayOrder]
	)
	VALUES
	(
		@CountryID,
		@Name,
		@Abbreviation,
		@DisplayOrder
	)

	set @StateProvinceID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TaxRateInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TaxRateInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TaxRateInsert]
(
	@TaxRateID int = NULL output,
	@TaxCategoryID int,
	@CountryID int,
	@StateProvinceID int,
	@Zip nvarchar(50),
	@Percentage decimal(18,4)
)
AS
BEGIN
	INSERT
	INTO [Nop_TaxRate]
	(
		[TaxCategoryID],
		[CountryID],
		[StateProvinceID],
		[Zip],
		[Percentage]
	)
	VALUES
	(
		@TaxCategoryID,
		@CountryID,
		@StateProvinceID,
		@Zip,
		@Percentage
	)

	set @TaxRateID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_WarehouseInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_WarehouseInsert]
GO
CREATE PROCEDURE [dbo].[Nop_WarehouseInsert]
(
	@WarehouseID int = NULL output,
	@Name nvarchar(255),
	@PhoneNumber nvarchar(50),
	@Email nvarchar(255),
	@FaxNumber nvarchar(50),
	@Address1 nvarchar(100),
	@Address2 nvarchar(100),
	@City nvarchar(100),
	@StateProvince nvarchar(100),
	@ZipPostalCode nvarchar(10),
	@CountryID int,
	@Deleted bit,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Warehouse]
	(
		[Name],
		PhoneNumber,
		Email,
		FaxNumber,
		Address1,
		Address2,
		City,
		StateProvince,
		ZipPostalCode,
		CountryID,
		Deleted,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@Name,
		@PhoneNumber,
		@Email,
		@FaxNumber,
		@Address1,
		@Address2,
		@City,
		@StateProvince,
		@ZipPostalCode,
		@CountryID,
		@Deleted,
		@CreatedOn,
		@UpdatedOn
	)

	set @WarehouseID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingByWeightAndCountryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingByWeightAndCountryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingByWeightAndCountryInsert]
(
	@ShippingByWeightAndCountryID int = NULL output,
	@ShippingMethodID int,
	@CountryID int,
	@From decimal(18, 2),
	@To decimal(18, 2),
	@UsePercentage bit,
	@ShippingChargePercentage decimal(18, 2),
	@ShippingChargeAmount decimal(18, 2)
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingByWeightAndCountry]
	(
		ShippingMethodID,
		CountryID,
		[From],
		[To],
		UsePercentage,
		ShippingChargePercentage,
		ShippingChargeAmount
	)
	VALUES
	(
		@ShippingMethodID,
		@CountryID,
		@From,
		@To,
		@UsePercentage,
		@ShippingChargePercentage,
		@ShippingChargeAmount
	)

	set @ShippingByWeightAndCountryID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_AffiliateInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_AffiliateInsert]
GO
CREATE PROCEDURE [dbo].[Nop_AffiliateInsert]
(
	@AffiliateID int = NULL output,
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@MiddleName nvarchar(100),
	@PhoneNumber nvarchar(50),
	@Email nvarchar(255),
	@FaxNumber nvarchar(50),
	@Company nvarchar(100),
	@Address1 nvarchar(100),
	@Address2 nvarchar(100),
	@City nvarchar(100),
	@StateProvince nvarchar(100),
	@ZipPostalCode nvarchar(10),
	@CountryID int,
	@Deleted bit,
	@Active bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Affiliate]
	(
		FirstName,
		LastName,
		MiddleName,
		PhoneNumber,
		Email,
		FaxNumber,
		Company,
		Address1,
		Address2,
		City,
		StateProvince,
		ZipPostalCode,
		CountryID,
		Deleted,
		Active
	)
	VALUES
	(
		@FirstName,
		@LastName,
		@MiddleName,
		@PhoneNumber,
		@Email,
		@FaxNumber,
		@Company,
		@Address1,
		@Address2,
		@City,
		@StateProvince,
		@ZipPostalCode,
		@CountryID,
		@Deleted,
		@Active
	)

	set @AffiliateID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TaxCategoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TaxCategoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TaxCategoryInsert]
(
	@TaxCategoryID int = NULL output,
	@Name nvarchar(100),
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_TaxCategory]
	(
		[Name],
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @TaxCategoryID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductAttributeInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductAttributeInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductAttributeInsert]
(
	@ProductAttributeID int = NULL output,
	@Name nvarchar(100),
	@Description nvarchar(4000)
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductAttribute]
	(
		[Name],
		[Description]
	)
	VALUES
	(
		@Name,
		@Description
	)

	set @ProductAttributeID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TaxProviderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TaxProviderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TaxProviderInsert]
(
	@TaxProviderID int = NULL output,
	@Name nvarchar(100),
	@Description nvarchar(4000),
	@ConfigureTemplatePath nvarchar(500),
	@ClassName nvarchar(500),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_TaxProvider]
	(
		[Name],
		[Description],
		ConfigureTemplatePath,
		ClassName,
		DisplayOrder
	)
	VALUES
	(
		@Name,
		@Description,
		@ConfigureTemplatePath,
		@ClassName,
		@DisplayOrder
	)

	set @TaxProviderID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_QueuedEmailInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_QueuedEmailInsert]
GO
CREATE PROCEDURE [dbo].[Nop_QueuedEmailInsert]
(
	@QueuedEmailID int = NULL output,
	@Priority int,
	@From nvarchar(500),
	@FromName nvarchar(500),
	@To nvarchar(500),
	@ToName nvarchar(500),
	@Cc nvarchar(500),
	@Bcc nvarchar(500),
	@Subject nvarchar(500),
	@Body nvarchar(MAX),
	@CreatedOn datetime,
	@SendTries int,
	@SentOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_QueuedEmail]
	(
		[Priority],
		[From],
		[FromName],
		[To],
		[ToName],
		[Cc],
		[Bcc],
		[Subject],
		[Body],
		[CreatedOn],
		[SendTries],
		[SentOn]
	)
	VALUES
	(
		@Priority,
		@From,
		@FromName,
		@To,
		@ToName,
		@Cc,
		@Bcc,
		@Subject,
		@Body,
		@CreatedOn,
		@SendTries,
		@SentOn
	)

	set @QueuedEmailID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethodInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingMethodInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingMethodInsert]
(
	@ShippingMethodID int = NULL output,
	@Name nvarchar(100),
	@Description nvarchar(2000),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingMethod]
	(
		[Name],
		[Description],
		DisplayOrder
	)
	VALUES
	(
		@Name,
		@Description,
		@DisplayOrder
	)

	set @ShippingMethodID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingRateComputationMethodInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingRateComputationMethodInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingRateComputationMethodInsert]
(
	@ShippingRateComputationMethodID int = NULL output,
	@Name nvarchar(100),
	@Description nvarchar(4000),
	@ConfigureTemplatePath nvarchar(500),
	@ClassName nvarchar(500),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingRateComputationMethod]
	(
		[Name],
		[Description],
		ConfigureTemplatePath,
		ClassName,
		DisplayOrder
	)
	VALUES
	(
		@Name,
		@Description,
		@ConfigureTemplatePath,
		@ClassName,
		@DisplayOrder
	)

	set @ShippingRateComputationMethodID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TierPriceInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TierPriceInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TierPriceInsert]
(
	@TierPriceID int = NULL output,
	@ProductVariantID int,
	@Quantity int,
	@Price money
)
AS
BEGIN
	INSERT
	INTO [Nop_TierPrice]
	(
		[ProductVariantID],
		[Quantity],
		[Price]
	)
	VALUES
	(
		@ProductVariantID,
		@Quantity,
		@Price
	)

	set @TierPriceID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeValueInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeValueInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeValueInsert]
(
	@ProductVariantAttributeValueID int = NULL output,
	@ProductVariantAttributeID int,
	@Name nvarchar (100),
	@PriceAdjustment money,
	@WeightAdjustment decimal(18, 4),
	@IsPreSelected bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductVariantAttributeValue]
	(
		[ProductVariantAttributeID],
		[Name],
		[PriceAdjustment],
		[WeightAdjustment],
		[IsPreSelected],
		[DisplayOrder]
	)
	VALUES
	(
		@ProductVariantAttributeID,
		@Name,
		@PriceAdjustment,
		@WeightAdjustment,
		@IsPreSelected,
		@DisplayOrder
	)

	set @ProductVariantAttributeValueID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CreditCardTypeInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CreditCardTypeInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CreditCardTypeInsert]
(
	@CreditCardTypeID int = NULL output,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@DisplayOrder int,
	@Deleted bit
)
AS
BEGIN
	INSERT
	INTO [Nop_CreditCardType]
	(
		[Name],
		SystemKeyword,
		DisplayOrder,
		Deleted
	)
	VALUES
	(
		@Name,
		@SystemKeyword,
		@DisplayOrder,
		@Deleted
	)

	set @CreditCardTypeID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingByTotalInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingByTotalInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingByTotalInsert]
(
	@ShippingByTotalID int = NULL output,
	@ShippingMethodID int,
	@From decimal(18, 2),
	@To decimal(18, 2),
	@UsePercentage bit,
	@ShippingChargePercentage decimal(18, 2),
	@ShippingChargeAmount decimal(18, 2)
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingByTotal]
	(
		ShippingMethodID,
		[From],
		[To],
		UsePercentage,
		ShippingChargePercentage,
		ShippingChargeAmount
	)
	VALUES
	(
		@ShippingMethodID,
		@From,
		@To,
		@UsePercentage,
		@ShippingChargePercentage,
		@ShippingChargeAmount
	)

	set @ShippingByTotalID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingByWeightInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingByWeightInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingByWeightInsert]
(
	@ShippingByWeightID int = NULL output,
	@ShippingMethodID int,
	@From decimal(18, 2),
	@To decimal(18, 2),
	@UsePercentage bit,
	@ShippingChargePercentage decimal(18, 2),
	@ShippingChargeAmount decimal(18, 2)
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingByWeight]
	(
		ShippingMethodID,
		[From],
		[To],
		UsePercentage,
		ShippingChargePercentage,
		ShippingChargeAmount
	)
	VALUES
	(
		@ShippingMethodID,
		@From,
		@To,
		@UsePercentage,
		@ShippingChargePercentage,
		@ShippingChargeAmount
	)

	set @ShippingByWeightID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SettingInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SettingInsert]
GO
CREATE PROCEDURE [dbo].[Nop_SettingInsert]
(
	@SettingId int = NULL output,
	@Name nvarchar(200),
	@Value nvarchar(2000),	
	@Description ntext
)
AS
BEGIN
	INSERT
	INTO [Nop_Setting]
	(
			[Name],
			[Value],	
			[Description]
	)
	VALUES
	(
			@Name,
			@Value,	
			@Description
	)

	set @SettingId=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductTemplateInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductTemplateInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductTemplateInsert]
(
	@ProductTemplateID int = NULL output,
	@Name nvarchar(100),
	@TemplatePath nvarchar(200),
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductTemplate]
	(
		[Name],
		TemplatePath,
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@TemplatePath,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @ProductTemplateID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CategoryTemplateInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CategoryTemplateInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CategoryTemplateInsert]
(
	@CategoryTemplateID int = NULL output,
	@Name nvarchar(100),
	@TemplatePath nvarchar(200),
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_CategoryTemplate]
	(
		[Name],
		TemplatePath,
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@TemplatePath,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @CategoryTemplateID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ManufacturerTemplateInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ManufacturerTemplateInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ManufacturerTemplateInsert]
(
	@ManufacturerTemplateID int = NULL output,
	@Name nvarchar(100),
	@TemplatePath nvarchar(200),
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_ManufacturerTemplate]
	(
		[Name],
		TemplatePath,
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@TemplatePath,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @ManufacturerTemplateID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TopicInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TopicInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TopicInsert]
(
	@TopicID int = NULL output,
	@Name nvarchar(200)
)
AS
BEGIN
	INSERT
	INTO [Nop_Topic]
	(
		[Name]
	)
	VALUES
	(
		@Name
	)

	set @TopicID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CampaignInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CampaignInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CampaignInsert]
(
	@CampaignID int = NULL output,
	@Name nvarchar(200),
	@Subject nvarchar(200),
	@Body nvarchar(MAX),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Campaign]
	(
		[Name],
		[Subject],
		Body,
		CreatedOn
	)
	VALUES
	(
		@Name,
		@Subject,
		@Body,
		@CreatedOn
	)

	set @CampaignID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderNoteInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderNoteInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderNoteInsert]
(
	@OrderNoteID int = NULL output,
	@OrderID int,
	@Note nvarchar(4000),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderNote]
	(
		OrderID,
		Note,
		CreatedOn
	)
	VALUES
	(
		@OrderID,
		@Note,
		@CreatedOn
	)

	set @OrderNoteID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MessageTemplateLocalizedInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MessageTemplateLocalizedInsert]
GO
CREATE PROCEDURE [dbo].[Nop_MessageTemplateLocalizedInsert]
(
	@MessageTemplateLocalizedID int = NULL output,
	@MessageTemplateID int,
	@LanguageID int,
	@Subject nvarchar(200),
	@Body nvarchar(MAX)
)
AS
BEGIN
	INSERT
	INTO [Nop_MessageTemplateLocalized]
	(
		MessageTemplateID,
		LanguageID,
		[Subject],
		Body
	)
	VALUES
	(
		@MessageTemplateID,
		@LanguageID,
		@Subject,
		@Body
	)

	set @MessageTemplateLocalizedID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PictureInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PictureInsert]
GO
CREATE PROCEDURE [dbo].[Nop_PictureInsert]
(
	@PictureID int = NULL output,
	@PictureBinary image,	
	@Extension nvarchar(20),
	@IsNew	bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Picture]
	(
		PictureBinary,
		Extension,
		IsNew
	)
	VALUES
	(
		@PictureBinary,
		@Extension,
		@IsNew
	)

	set @PictureID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CategoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CategoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CategoryInsert]
(
	@CategoryID int = NULL output,
	@Name nvarchar(400),
	@Description ntext,
	@TemplateID int,
	@MetaKeywords nvarchar(400),
	@MetaDescription nvarchar(4000),
	@MetaTitle nvarchar(400),
	@SEName nvarchar(100),
	@ParentCategoryID int,	
	@PictureID int,
	@PageSize int,
	@PriceRanges nvarchar(400),
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Category]
	(
		[Name],
		[Description],
		TemplateID,
		MetaKeywords,
		MetaDescription,
		MetaTitle,
		SEName,
		ParentCategoryID,
		PictureID,
		PageSize,
		PriceRanges,
		Published,
		Deleted,
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@Description,
		@TemplateID,
		@MetaKeywords,
		@MetaDescription,
		@MetaTitle,
		@SEName,
		@ParentCategoryID,
		@PictureID,
		@PageSize,
		@PriceRanges,
		@Published,
		@Deleted,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @CategoryID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_GroupInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_GroupInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_GroupInsert]
(
	@ForumGroupID int = NULL output,
	@Name nvarchar(200),
	@Description nvarchar(MAX),
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_Group]
	(
		[Name],
		[Description],
		DisplayOrder,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@Name,
		@Description,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @ForumGroupID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ManufacturerInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ManufacturerInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ManufacturerInsert]
(
	@ManufacturerID int = NULL output,
	@Name nvarchar(400),
	@Description ntext,
	@TemplateID int,
	@MetaKeywords nvarchar(400),
	@MetaDescription nvarchar(4000),
	@MetaTitle nvarchar(400),
	@SEName nvarchar(100),
	@PictureID int,
	@PageSize int,
	@PriceRanges nvarchar(400),
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Manufacturer]
	(
		[Name],
		[Description],
		TemplateID,
		MetaKeywords,
		MetaDescription,
		MetaTitle,
		SEName,
		PictureID,
		PageSize,
		PriceRanges,
		Published,
		Deleted,
		DisplayOrder,
		CreatedOn,
		UpdatedOn	
	)
	VALUES
	(
		@Name,
		@Description,
		@TemplateID,
		@MetaKeywords,
		@MetaDescription,
		@MetaTitle,
		@SEName,
		@PictureID,
		@PageSize,
		@PriceRanges,
		@Published,
		@Deleted,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @ManufacturerID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_SubscriptionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_SubscriptionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_SubscriptionInsert]
(
	@SubscriptionID int = NULL output,
	@SubscriptionGUID uniqueidentifier,
	@UserID int,
	@ForumID int,
	@TopicID int,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_Subscription]
	(
		[SubscriptionGUID],
		[UserID],
		[ForumID],
		[TopicID],
		[CreatedOn]
	)
	VALUES
	(
		@SubscriptionGUID,
		@UserID,
		@ForumID,
		@TopicID,
		@CreatedOn
	)

	set @SubscriptionID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CurrencyInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CurrencyInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CurrencyInsert]
(
	@CurrencyID int = NULL output,
	@Name nvarchar(50),
	@CurrencyCode nvarchar(5),
	@Rate decimal (18, 4),
	@DisplayLocale nvarchar(50),
	@CustomFormatting nvarchar(50),
	@Published bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Currency]
	(
		[Name],
		CurrencyCode,
		Rate,
		DisplayLocale,
		CustomFormatting,
		Published,
		DisplayOrder,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@Name,
		@CurrencyCode,
		@Rate,
		@DisplayLocale,
		@CustomFormatting,
		@Published,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @CurrencyID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TopicLocalizedInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TopicLocalizedInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TopicLocalizedInsert]
(
	@TopicLocalizedID int = NULL output,
	@TopicID int,
	@LanguageID int,
	@Title nvarchar(200),
	@Body nvarchar(MAX),
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_TopicLocalized]
	(
		TopicID,
		LanguageID,
		[Title],
		Body,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@TopicID,
		@LanguageID,
		@Title,
		@Body,
		@CreatedOn,
		@UpdatedOn
	)

	set @TopicLocalizedID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LanguageInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LanguageInsert]
GO
CREATE PROCEDURE [dbo].[Nop_LanguageInsert]
(
	@LanguageId int = NULL output,
	@Name nvarchar(100),
	@LanguageCulture nvarchar(20),
	@Published bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_Language]
	(
		[Name],
		[LanguageCulture],
		[Published],
		[DisplayOrder]
	)
	VALUES
	(
		@Name,
		@LanguageCulture,
		@Published,
		@DisplayOrder
	)

	set @LanguageId=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_ForumInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_ForumInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_ForumInsert]
(
	@ForumID int = NULL output,	
	@ForumGroupID int,
	@Name nvarchar(200),
	@Description nvarchar(MAX),
	@NumTopics int,
	@NumPosts int,
	@LastTopicID int,
	@LastPostID int,
	@LastPostUserID int,
	@LastPostTime datetime,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_Forum]
	(
		[ForumGroupID],
		[Name],
		[Description],
		[NumTopics],
		[NumPosts],
		[LastTopicID],
		[LastPostID],
		[LastPostUserID],
		[LastPostTime],
		[DisplayOrder],
		[CreatedOn],
		[UpdatedOn]
	)
	VALUES
	(
		@ForumGroupID,
		@Name,
		@Description,
		@NumTopics,
		@NumPosts,
		@LastTopicID,
		@LastPostID,
		@LastPostUserID,
		@LastPostTime,
		@DisplayOrder,
		@CreatedOn,
		@UpdatedOn
	)

	set @ForumID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethodInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethodInsert]
GO
CREATE PROCEDURE [dbo].[Nop_PaymentMethodInsert]
(
	@PaymentMethodID int = NULL output,
	@Name nvarchar(100),
	@VisibleName nvarchar(100),
	@Description nvarchar(4000),
	@ConfigureTemplatePath nvarchar(500),
	@UserTemplatePath nvarchar(500),
	@ClassName nvarchar(500),
	@SystemKeyword nvarchar(500),
	@IsActive bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_PaymentMethod]
	(
		[Name],
		VisibleName,
		[Description],
		ConfigureTemplatePath,
		UserTemplatePath,
		ClassName,
		SystemKeyword,
		IsActive,
		DisplayOrder
	)
	VALUES
	(
		@Name,
		@VisibleName,
		@Description,
		@ConfigureTemplatePath,
		@UserTemplatePath,
		@ClassName,
		@SystemKeyword,
		@IsActive,
		@DisplayOrder
	)

	set @PaymentMethodID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostInsert]
GO
CREATE PROCEDURE [dbo].[Nop_BlogPostInsert]
(
	@BlogPostID int = NULL output,
	@LanguageID int,
	@BlogPostTitle nvarchar(200),
	@BlogPostBody nvarchar(MAX),
	@BlogPostAllowComments bit,
	@CreatedByID int,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_BlogPost]
	(
		LanguageID,
		BlogPostTitle,
		BlogPostBody,
		BlogPostAllowComments,
		CreatedByID,
		CreatedOn
	)
	VALUES
	(
		@LanguageID,
		@BlogPostTitle,
		@BlogPostBody,
		@BlogPostAllowComments,
		@CreatedByID,
		@CreatedOn
	)

	set @BlogPostID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsInsert]
GO
CREATE PROCEDURE [dbo].[Nop_NewsInsert]
(
	@NewsID int = NULL output,
	@LanguageID	int,
	@Title nvarchar(1000),
	@Short nvarchar(4000),
	@Full nvarchar (max),
	@Published bit,
	@AllowComments bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_News]
	(
		LanguageID,
		Title,
		Short,
		[Full],
		Published,
		AllowComments,
		CreatedOn
	)
	VALUES
	(
		@LanguageID,
		@Title,
		@Short,
		@Full,
		@Published,
		@AllowComments,
		@CreatedOn
	)

	set @NewsID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PollInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PollInsert]
GO
CREATE PROCEDURE [dbo].[Nop_PollInsert]
(
	@PollID int = NULL output,
	@LanguageID int,
	@Name nvarchar(400),
	@SystemKeyword nvarchar(400),
	@Published bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_Poll]
	(
		LanguageID,
		[Name],
		SystemKeyword,
		Published,
		DisplayOrder		
	)
	VALUES
	(
		@LanguageID,
		@Name,
		@SystemKeyword,
		@Published,
		@DisplayOrder
	)

	set @PollID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceInsert]
GO
CREATE PROCEDURE [dbo].[Nop_LocaleStringResourceInsert]
(
	@LocaleStringResourceID int = NULL output,
	@LanguageID int,
	@ResourceName nvarchar(200),
	@ResourceValue nvarchar(MAX)
)
AS
BEGIN
	INSERT
	INTO [Nop_LocaleStringResource]
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

	set @LocaleStringResourceID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PollAnswerInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PollAnswerInsert]
GO
CREATE PROCEDURE [dbo].[Nop_PollAnswerInsert]
(
	@PollAnswerID int = NULL output,
	@PollID int,
	@Name nvarchar(100),
	@Count int,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_PollAnswer]
	(
		PollID,
		[Name],
		[Count],
		DisplayOrder
	)
	VALUES
	(
		@PollID,
		@Name,
		@Count,
		@DisplayOrder
	)

	set @PollAnswerID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogCommentInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogCommentInsert]
GO
CREATE PROCEDURE [dbo].[Nop_BlogCommentInsert]
(
	@BlogCommentID int = NULL output,
	@BlogPostID int,
	@CustomerID int,
	@CommentText nvarchar(MAX),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_BlogComment]
	(
		BlogPostID,
		CustomerID,
		CommentText,
		CreatedOn
	)
	VALUES
	(
		@BlogPostID,
		@CustomerID,
		@CommentText,
		@CreatedOn
	)

	set @BlogCommentID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsCommentInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsCommentInsert]
GO
CREATE PROCEDURE [dbo].[Nop_NewsCommentInsert]
(
	@NewsCommentID int = NULL output,
	@NewsID int,
	@CustomerID int,
	@Title nvarchar(1000),
	@Comment nvarchar(max),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_NewsComment]
	(
		NewsID,
		CustomerID,
		Title,
		Comment,
		CreatedOn
	)
	VALUES
	(
		@NewsID,
		@CustomerID,
		@Title,
		@Comment,
		@CreatedOn
	)

	set @NewsCommentID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountInsert]
(
	@DiscountID int = NULL output,
	@DiscountTypeID int,
	@DiscountRequirementID int,
	@Name nvarchar(100),
	@UsePercentage bit, 
	@DiscountPercentage decimal (18, 4),
	@DiscountAmount decimal (18, 4),
	@StartDate datetime,
	@EndDate datetime,
	@RequiresCouponCode bit,
	@CouponCode nvarchar(100),
	@Deleted bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Discount]
	(
		DiscountTypeID,
		DiscountRequirementID,
		[Name],
		UsePercentage,
		DiscountPercentage,
		DiscountAmount,
		StartDate,
		EndDate,
		RequiresCouponCode,
		CouponCode,
		Deleted
	)
	VALUES
	(
		@DiscountTypeID,
		@DiscountRequirementID,
		@Name,
		@UsePercentage,
		@DiscountPercentage,
		@DiscountAmount,
		@StartDate,
		@EndDate,
		@RequiresCouponCode,
		@CouponCode,
		@Deleted
	)

	set @DiscountID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerAttributeInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerAttributeInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerAttributeInsert]
(
	@CustomerAttributeID int = NULL output,
	@CustomerID int,
	@Key nvarchar(100),
	@Value nvarchar(1000)
)
AS
BEGIN
	INSERT
	INTO [Nop_CustomerAttribute]
	(
		CustomerID,
		[Key],
		[Value]
	)
	VALUES
	(
		@CustomerID,
		@Key,
		@Value
	)

	set @CustomerAttributeID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_AddressInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_AddressInsert]
GO
CREATE PROCEDURE [dbo].[Nop_AddressInsert]
(
	@AddressId int = NULL output,
	@CustomerID int,
	@IsBillingAddress bit,
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@PhoneNumber nvarchar(50),
	@Email nvarchar(255),
	@FaxNumber nvarchar(50),
	@Company nvarchar(100),
	@Address1 nvarchar(100),
	@Address2 nvarchar(100),
	@City nvarchar(100),
	@StateProvinceID int,
	@ZipPostalCode nvarchar(10),
	@CountryID int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Address]
	(
		CustomerID,
		IsBillingAddress,
		FirstName,
		LastName,
		PhoneNumber,
		Email,
		FaxNumber,
		Company,
		Address1,
		Address2,
		City,
		StateProvinceID,
		ZipPostalCode,
		CountryID,
		CreatedOn,
		UpdatedOn
	)
	VALUES
	(
		@CustomerID,
		@IsBillingAddress,
		@FirstName,
		@LastName,
		@PhoneNumber,
		@Email,
		@FaxNumber,
		@Company,
		@Address1,
		@Address2,
		@City,
		@StateProvinceID,
		@ZipPostalCode,
		@CountryID,
		@CreatedOn,
		@UpdatedOn
	)

	set @AddressId=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Product_Manufacturer_MappingInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Product_Manufacturer_MappingInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Product_Manufacturer_MappingInsert]
(
	@ProductManufacturerID int = NULL output,
	@ProductID int,	
	@ManufacturerID int,
	@IsFeaturedProduct	bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_Product_Manufacturer_Mapping]
	(
		ProductID,
		ManufacturerID,
		IsFeaturedProduct,
		DisplayOrder
	)
	VALUES
	(
		@ProductID,
		@ManufacturerID,
		@IsFeaturedProduct,
		@DisplayOrder
	)

	set @ProductManufacturerID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Product_Category_MappingInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Product_Category_MappingInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Product_Category_MappingInsert]
(
	@ProductCategoryID int = NULL output,
	@ProductID int,	
	@CategoryID int,
	@IsFeaturedProduct	bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_Product_Category_Mapping]
	(
		ProductID,
		CategoryID,
		IsFeaturedProduct,
		DisplayOrder
	)
	VALUES
	(
		@ProductID,
		@CategoryID,
		@IsFeaturedProduct,
		@DisplayOrder
	)

	set @ProductCategoryID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RelatedProductInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RelatedProductInsert]
GO
CREATE PROCEDURE [dbo].[Nop_RelatedProductInsert]
(
	@RelatedProductID int = NULL output,
	@ProductID1 int,	
	@ProductID2 int,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_RelatedProduct]
	(
		ProductID1,
		ProductID2,
		DisplayOrder
	)
	VALUES
	(
		@ProductID1,
		@ProductID2,
		@DisplayOrder
	)

	set @RelatedProductID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductReviewInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductReviewInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductReviewInsert]
(
	@ProductReviewID int = NULL output,
	@ProductID int,
	@CustomerID int,
	@Title nvarchar(1000),
	@ReviewText nvarchar(max),
	@Rating int,
	@HelpfulYesTotal int,
	@HelpfulNoTotal int,
	@IsApproved bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductReview]
	(
		ProductID,
		CustomerID,
		Title,
		ReviewText,
		Rating,
		HelpfulYesTotal,
		HelpfulNoTotal,
		IsApproved,
		CreatedOn
	)
	VALUES
	(
		@ProductID,
		@CustomerID,
		@Title,
		@ReviewText,
		@Rating,
		@HelpfulYesTotal,
		@HelpfulNoTotal,
		@IsApproved,
		@CreatedOn
	)

	set @ProductReviewID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_TopicInsert]
(
	@TopicID int = NULL output,
	@ForumID int,
	@UserID int,
	@TopicTypeID int,
	@Subject nvarchar(450),
	@NumPosts int,
	@Views int,
	@LastPostID int,
	@LastPostUserID int,
	@LastPostTime datetime,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_Topic]
	(
		[ForumID],
		[UserID],
		[TopicTypeID],
		[Subject],
		[NumPosts],
		[Views],
		[LastPostID],
		[LastPostUserID],
		[LastPostTime],
		[CreatedOn],
		[UpdatedOn]
	)
	VALUES
	(
		@ForumID,
		@UserID,
		@TopicTypeID,
		@Subject,
		@NumPosts,
		@Views,
		@LastPostID,
		@LastPostUserID,
		@LastPostTime,
		@CreatedOn,
		@UpdatedOn
	)

	set @TopicID=SCOPE_IDENTITY()

	--update stats/info
	exec [dbo].[Nop_Forums_ForumUpdateCounts] @ForumID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostInsert]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_PostInsert]
(
	@PostID int = NULL output,
	@TopicID int,
	@UserID int,
	@Text nvarchar(max),
	@IPAddress nvarchar(100),
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Forums_Post]
	(
		[TopicID],
		[UserID],
		[Text],
		[IPAddress],
		[CreatedOn],
		[UpdatedOn]
	)
	VALUES
	(
		@TopicID,
		@UserID,
		@Text,
		@IPAddress,
		@CreatedOn,
		@UpdatedOn
	)

	set @PostID=SCOPE_IDENTITY()

	--update stats/info
	exec [dbo].[Nop_Forums_TopicUpdateCounts] @TopicID
	
	declare @ForumID int
	SELECT 
		@ForumID = ft.ForumID
	FROM
		[Nop_Forums_Topic] ft
	WHERE
		ft.TopicID = @TopicID 

	exec [dbo].[Nop_Forums_ForumUpdateCounts] @ForumID
	
	exec [dbo].[Nop_CustomerUpdateCounts] @UserID
END
GO










--added PaidDate column to Nop_Order table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='PaidDate')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD PaidDate DATETIME NULL

	exec('UPDATE [dbo].[Nop_Order] SET PaidDate=CreatedOn where PaymentStatusID=30')
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		PurchaseOrderNumber,
		PaymentStatusID,
		PaidDate,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@PaidDate,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderUpdate]
(
	@OrderID int,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_Order]
	SET
		OrderGUID=@OrderGUID,
		CustomerID=@CustomerID,
		CustomerLanguageID=@CustomerLanguageID,		
		CustomerTaxDisplayTypeID=@CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax=@OrderSubtotalInclTax,
		OrderSubtotalExclTax=@OrderSubtotalExclTax,		
		OrderShippingInclTax=@OrderShippingInclTax,
		OrderShippingExclTax=@OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax=@PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax=@PaymentMethodAdditionalFeeExclTax,
		OrderTax=@OrderTax,
		OrderTotal=@OrderTotal,
		OrderDiscount=@OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency=@OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency=@OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency=@OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency=@OrderShippingExclTaxInCustomerCurrency,	
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,	
		OrderTaxInCustomerCurrency=@OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency=@OrderTotalInCustomerCurrency,
		CustomerCurrencyCode=@CustomerCurrencyCode,
		OrderWeight=@OrderWeight,
		AffiliateID=@AffiliateID,
		OrderStatusID=@OrderStatusID,
		AllowStoringCreditCardNumber=@AllowStoringCreditCardNumber,
		CardType=@CardType,
		CardName=@CardName,
		CardNumber=@CardNumber,
		MaskedCreditCardNumber=@MaskedCreditCardNumber,
		CardCVV2=@CardCVV2,
		CardExpirationMonth=@CardExpirationMonth,
		CardExpirationYear=@CardExpirationYear,
		PaymentMethodID=@PaymentMethodID,
		PaymentMethodName=@PaymentMethodName,
		AuthorizationTransactionID=@AuthorizationTransactionID,
		AuthorizationTransactionCode=@AuthorizationTransactionCode,
		AuthorizationTransactionResult=@AuthorizationTransactionResult,
		CaptureTransactionID=@CaptureTransactionID,
		CaptureTransactionResult=@CaptureTransactionResult,
		PurchaseOrderNumber=@PurchaseOrderNumber,
		PaymentStatusID=@PaymentStatusID,
		PaidDate=@PaidDate,
		BillingFirstName=@BillingFirstName,
		BillingLastName=@BillingLastName,
		BillingPhoneNumber=@BillingPhoneNumber,
		BillingEmail=@BillingEmail,
		BillingFaxNumber=@BillingFaxNumber,
		BillingCompany=@BillingCompany,
		BillingAddress1=@BillingAddress1,
		BillingAddress2=@BillingAddress2,
		BillingCity=@BillingCity,
		BillingStateProvince=@BillingStateProvince,
		BillingStateProvinceID=@BillingStateProvinceID,
		BillingZipPostalCode=@BillingZipPostalCode,
		BillingCountry=@BillingCountry,
		BillingCountryID=@BillingCountryID,
		ShippingStatusID=@ShippingStatusID,
		ShippingFirstName=@ShippingFirstName,
		ShippingLastName=@ShippingLastName,
		ShippingPhoneNumber=@ShippingPhoneNumber,
		ShippingEmail=@ShippingEmail,
		ShippingFaxNumber=@ShippingFaxNumber,
		ShippingCompany=@ShippingCompany,
		ShippingAddress1=@ShippingAddress1,
		ShippingAddress2=@ShippingAddress2,
		ShippingCity=@ShippingCity,
		ShippingStateProvince=@ShippingStateProvince,
		ShippingStateProvinceID=@ShippingStateProvinceID,
		ShippingZipPostalCode=@ShippingZipPostalCode,
		ShippingCountry=@ShippingCountry,
		ShippingCountryID=@ShippingCountryID,
		ShippingMethod=@ShippingMethod,
		ShippingRateComputationMethodID=@ShippingRateComputationMethodID,
		ShippedDate=@ShippedDate,
		Deleted=@Deleted,
		CreatedOn=@CreatedOn
	WHERE
		OrderID = @OrderID
END
GO






--added ProductCost column to Nop_ProductVariant table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='ProductCost')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD ProductCost money NOT NULL CONSTRAINT [DF_Nop_ProductVariant_ProductCost] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		HasSampleDownload,
		SampleDownloadID,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@HasSampleDownload,
		@SampleDownloadID,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO



--added BCCEmailAddresses column to Nop_MessageTemplateLocalized table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_MessageTemplateLocalized]') and NAME='BCCEmailAddresses')
BEGIN
	ALTER TABLE [dbo].[Nop_MessageTemplateLocalized] 
	ADD BCCEmailAddresses nvarchar(200) NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_BCCEmailAddresses] DEFAULT ((''))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MessageTemplateLocalizedInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MessageTemplateLocalizedInsert]
GO
CREATE PROCEDURE [dbo].[Nop_MessageTemplateLocalizedInsert]
(
	@MessageTemplateLocalizedID int = NULL output,
	@MessageTemplateID int,
	@LanguageID int,
	@BCCEmailAddresses nvarchar(200),
	@Subject nvarchar(200),
	@Body nvarchar(MAX)
)
AS
BEGIN
	INSERT
	INTO [Nop_MessageTemplateLocalized]
	(
		MessageTemplateID,
		LanguageID,
		BCCEmailAddresses,
		[Subject],
		Body
	)
	VALUES
	(
		@MessageTemplateID,
		@LanguageID,
		@BCCEmailAddresses,
		@Subject,
		@Body
	)

	set @MessageTemplateLocalizedID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MessageTemplateLocalizedUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MessageTemplateLocalizedUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_MessageTemplateLocalizedUpdate]
(
	@MessageTemplateLocalizedID int,
	@MessageTemplateID int,
	@LanguageID int,
	@BCCEmailAddresses nvarchar(200),	
	@Subject nvarchar(200),
	@Body nvarchar(MAX)
)
AS
BEGIN

	UPDATE [Nop_MessageTemplateLocalized]
	SET
		MessageTemplateID=@MessageTemplateID,
		LanguageID=@LanguageID,
		BCCEmailAddresses=@BCCEmailAddresses,
		[Subject]=@Subject,
		Body=@Body
	WHERE
		MessageTemplateLocalizedID = @MessageTemplateLocalizedID

END
GO




-- Removed <br /> from OrderProductVariant.AttributeDescription
UPDATE Nop_OrderProductVariant
SET AttributeDescription = SUBSTRING(AttributeDescription, 7, LEN(AttributeDescription)-6)
WHERE CHARINDEX('<br />', AttributeDescription) = 1 AND LEN(AttributeDescription)>6
GO



-- Determines the maximum items in a shopping cart/wishlist
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.MaximumShoppingCartItems')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.MaximumShoppingCartItems', N'1000', N'Determines the maximum items in a shopping cart')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.MaximumWishlistItems')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.MaximumWishlistItems', N'1000', N'Determines the maximum items in a wishlist')
END
GO





--added OrderProductVariantGUID column to Nop_OrderProductVariant table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_OrderProductVariant]') and NAME='OrderProductVariantGUID')
BEGIN
	ALTER TABLE [dbo].[Nop_OrderProductVariant] 
	ADD OrderProductVariantGUID uniqueidentifier NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_OrderProductVariantGUID] DEFAULT ((NEWID()))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
(
	@OrderProductVariantID int = NULL output,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderProductVariant]
	(
		OrderProductVariantGUID,
		OrderID,
		ProductVariantID,
		UnitPriceInclTax,
		UnitPriceExclTax,
		PriceInclTax,
		PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency,
		AttributeDescription,
		Quantity,
		DiscountAmountInclTax,
		DiscountAmountExclTax,
		DownloadCount
	)
	VALUES
	(
		@OrderProductVariantGUID,
		@OrderID,
		@ProductVariantID,
		@UnitPriceInclTax,
		@UnitPriceExclTax,
		@PriceInclTax,
		@PriceExclTax,
		@UnitPriceInclTaxInCustomerCurrency,
		@UnitPriceExclTaxInCustomerCurrency,
		@PriceInclTaxInCustomerCurrency,
		@PriceExclTaxInCustomerCurrency,
		@AttributeDescription,
		@Quantity,
		@DiscountAmountInclTax,
		@DiscountAmountExclTax,
		@DownloadCount
	)

	set @OrderProductVariantID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
(
	@OrderProductVariantID int,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int
)
AS
BEGIN

	UPDATE [Nop_OrderProductVariant]
	SET		
		OrderProductVariantGUID=@OrderProductVariantGUID,
		OrderID=@OrderID,
		ProductVariantID=@ProductVariantID,
		UnitPriceInclTax=@UnitPriceInclTax,
		UnitPriceExclTax = @UnitPriceExclTax,
		PriceInclTax=@PriceInclTax,
		PriceExclTax=@PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency=@UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency=@UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency=@PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency=@PriceExclTaxInCustomerCurrency,
		AttributeDescription=@AttributeDescription,
		Quantity=@Quantity,
		DiscountAmountInclTax=@DiscountAmountInclTax,
		DiscountAmountExclTax=@DiscountAmountExclTax,
		DownloadCount=@DownloadCount
	WHERE
		OrderProductVariantID = @OrderProductVariantID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantLoadByGUID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantLoadByGUID]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantLoadByGUID]
(
	@OrderProductVariantGUID uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_OrderProductVariant]
	WHERE
		OrderProductVariantGUID = @OrderProductVariantGUID
END
GO




-- Registration methods
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.CustomerRegistrationType')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.CustomerRegistrationType', N'1', N'Determines the registration method')
END
GO



-- New discount type
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountType]
		WHERE [DiscountTypeID] = 10)
BEGIN
	INSERT [dbo].[Nop_DiscountType] (DiscountTypeID, [Name])
	VALUES (10, N'Assigned to shipping')
END
GO


-- New payment statuses
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentStatus]
		WHERE [PaymentStatusID] = 40)
BEGIN
	INSERT [dbo].[Nop_PaymentStatus] (PaymentStatusID, [Name])
	VALUES (40, N'Refunded')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentStatus]
		WHERE [PaymentStatusID] = 50)
BEGIN
	INSERT [dbo].[Nop_PaymentStatus] (PaymentStatusID, [Name])
	VALUES (50, N'Voided')
END
GO


--discount usage history
if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountUsageHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_DiscountUsageHistory](
	[DiscountUsageHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[DiscountID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_DiscountUsageHistory_PK] PRIMARY KEY CLUSTERED 
(
	[DiscountUsageHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_DiscountUsageHistory_Nop_Discount'
           AND parent_obj = Object_id('Nop_DiscountUsageHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_DiscountUsageHistory
DROP CONSTRAINT FK_Nop_DiscountUsageHistory_Nop_Discount
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_DiscountUsageHistory_Nop_Customer'
           AND parent_obj = Object_id('Nop_DiscountUsageHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_DiscountUsageHistory
DROP CONSTRAINT FK_Nop_DiscountUsageHistory_Nop_Customer
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_DiscountUsageHistory_Nop_Order'
           AND parent_obj = Object_id('Nop_DiscountUsageHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_DiscountUsageHistory
DROP CONSTRAINT FK_Nop_DiscountUsageHistory_Nop_Order
GO
ALTER TABLE [dbo].[Nop_DiscountUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountUsageHistory_Nop_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Nop_Order] ([OrderID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryDelete]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUsageHistoryDelete]
(
	@DiscountUsageHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_DiscountUsageHistory]
	WHERE
		DiscountUsageHistoryID = @DiscountUsageHistoryID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUsageHistoryInsert]
(
	@DiscountUsageHistoryID int = NULL output,
	@DiscountID int,
	@CustomerID int,
	@OrderID int,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_DiscountUsageHistory]
	(
		[DiscountID],
		[CustomerID],
		[OrderID],
		[CreatedOn]
	)
	VALUES
	(
		@DiscountID,
		@CustomerID,
		@OrderID,
		@CreatedOn
	)

	set @DiscountUsageHistoryID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadAll]
(
	@DiscountID int,
	@CustomerID int,
	@OrderID int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT * FROM [Nop_DiscountUsageHistory]
	WHERE DiscountUsageHistoryID IN 
		(
		SELECT DISTINCT duh.DiscountUsageHistoryID
		FROM [Nop_DiscountUsageHistory] duh WITH (NOLOCK)
		LEFT OUTER JOIN Nop_Discount d with (NOLOCK) ON duh.DiscountID=d.DiscountID
		LEFT OUTER JOIN Nop_Customer c with (NOLOCK) ON duh.CustomerID=c.CustomerID
		LEFT OUTER JOIN Nop_Order o with (NOLOCK) ON duh.OrderID=o.OrderID
		WHERE
				(
					d.Deleted=0 AND c.Deleted=0 AND o.Deleted=0 
				)
				AND
				(
					@DiscountID IS NULL OR @DiscountID=0
					OR (duh.DiscountID=@DiscountID)
				)
				AND
				(
					@CustomerID IS NULL OR @CustomerID=0
					OR (duh.CustomerID=@CustomerID)
				)
				AND
				(
					@OrderID IS NULL OR @OrderID=0
					OR (duh.OrderID=@OrderID)
				)
		)
	ORDER BY CreatedOn, DiscountUsageHistoryID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadByPrimaryKey]
(
	@DiscountUsageHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_DiscountUsageHistory]
	WHERE
		DiscountUsageHistoryID = @DiscountUsageHistoryID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUsageHistoryUpdate]
(
	@DiscountUsageHistoryID int,
	@DiscountID int,
	@CustomerID int,
	@OrderID int,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_DiscountUsageHistory]
	SET
		DiscountID=@DiscountID,
		CustomerID = @CustomerID,
		OrderID = @OrderID,
		CreatedOn = @CreatedOn
	WHERE
		DiscountUsageHistoryID=@DiscountUsageHistoryID
END
GO



--several active shipping rate computation methods
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ShippingRateComputationMethod]') and NAME='IsActive')
BEGIN
	ALTER TABLE [dbo].[Nop_ShippingRateComputationMethod] 
	ADD IsActive bit NOT NULL CONSTRAINT [DF_Nop_ShippingRateComputationMethod_IsActive] DEFAULT ((0))

	exec('UPDATE [dbo].[Nop_ShippingRateComputationMethod] SET IsActive=1 WHERE ShippingRateComputationMethodID=(SELECT s.[value] FROM [dbo].[Nop_Setting] s WHERE s.[Name] = N''Shipping.ShippingRateComputationMethod.ActiveID'')')

	exec('DELETE FROM [dbo].[Nop_Setting] WHERE [Name] = N''Shipping.ShippingRateComputationMethod.ActiveID''')
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingRateComputationMethodInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingRateComputationMethodInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingRateComputationMethodInsert]
(
	@ShippingRateComputationMethodID int = NULL output,
	@Name nvarchar(100),
	@Description nvarchar(4000),
	@ConfigureTemplatePath nvarchar(500),
	@ClassName nvarchar(500),
	@IsActive bit,
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_ShippingRateComputationMethod]
	(
		[Name],
		[Description],
		ConfigureTemplatePath,
		ClassName,
		IsActive,
		DisplayOrder
	)
	VALUES
	(
		@Name,
		@Description,
		@ConfigureTemplatePath,
		@ClassName,
		@IsActive,
		@DisplayOrder
	)

	set @ShippingRateComputationMethodID=SCOPE_IDENTITY()
END
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingRateComputationMethodUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingRateComputationMethodUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingRateComputationMethodUpdate]
(
	@ShippingRateComputationMethodID int,
	@Name nvarchar(100),
	@Description nvarchar(4000),
	@ConfigureTemplatePath nvarchar(500),
	@ClassName nvarchar(500),
	@IsActive bit,
	@DisplayOrder int
)
AS
BEGIN
	UPDATE [Nop_ShippingRateComputationMethod]
	SET
		[Name]=@Name,
		[Description]=@Description,
		ConfigureTemplatePath=@ConfigureTemplatePath,
		ClassName=@ClassName,
		IsActive=@IsActive,
		DisplayOrder=@DisplayOrder

	WHERE
		ShippingRateComputationMethodID = @ShippingRateComputationMethodID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingRateComputationMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingRateComputationMethodLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ShippingRateComputationMethodLoadAll]
(	
	@ShowHidden bit = 0
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ShippingRateComputationMethod]
	WHERE @ShowHidden = 1 OR IsActive=1
	ORDER BY DisplayOrder
END
GO

-- Delete expired sessions task
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionDeleteExpired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerSessionDeleteExpired]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerSessionDeleteExpired]
(
	@OlderThan datetime
)
AS
BEGIN
	SET NOCOUNT ON
		
	DELETE FROM [Nop_CustomerSession]
	WHERE CustomerSessionGUID IN
	(
		SELECT cs.CustomerSessionGUID
		FROM [Nop_CustomerSession] cs
		WHERE 
			cs.CustomerSessionGUID NOT IN 
				(
					SELECT DISTINCT sci.CustomerSessionGUID FROM [Nop_ShoppingCartItem] sci
				)
			AND
			(
				cs.LastAccessed < @OlderThan
			)
	)
END
GO




--payJunction payment module
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.PayJunction.PayJunctionPaymentProcessor, Nop.Payment.PayJunction')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'PayJunction (QuickLink)', N'Credit Card', N'', N'Payment\PayJunction\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\PayJunction\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.PayJunction.PayJunctionPaymentProcessor, Nop.Payment.PayJunction', N'PAYJUNCTION_QUICKLINK', 0, 120)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PayJunction.UseSandbox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PayJunction.UseSandbox', N'false', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PayJunction.TransactionMode')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PayJunction.TransactionMode', N'Authorize', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PayJunction.pjlogon')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PayJunction.pjlogon', N'pj-ql-01', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PayJunction.pjpassword')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PayJunction.pjpassword', N'pj-ql-01p', N'')
END
GO



--Canada Post shipping module
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_ShippingRateComputationMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Shipping.Methods.CanadaPost.CanadaPostComputationMethod, Nop.Shipping.CanadaPost')
BEGIN
	INSERT [dbo].[Nop_ShippingRateComputationMethod] 
	([Name], [Description], [ConfigureTemplatePath], [ClassName], [DisplayOrder]) 
	VALUES (N'Canada Post', N'', N'Shipping\CanadaPostConfigure\ConfigureShipping.ascx', N'NopSolutions.NopCommerce.Shipping.Methods.CanadaPost.CanadaPostComputationMethod, Nop.Shipping.CanadaPost', 30)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ShippingRateComputationMethod.CanadaPost.CustomerID')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ShippingRateComputationMethod.CanadaPost.CustomerID', N'CPC_DEMO_XML', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ShippingRateComputationMethod.CanadaPost.URL')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ShippingRateComputationMethod.CanadaPost.URL', N'sellonline.canadapost.ca', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ShippingRateComputationMethod.CanadaPost.Port')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ShippingRateComputationMethod.CanadaPost.Port', N'30000', N'')
END
GO



--added AllowOutOfStockOrders column to Nop_ProductVariant table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='AllowOutOfStockOrders')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD AllowOutOfStockOrders bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_AllowOutOfStockOrders] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		HasSampleDownload,
		SampleDownloadID,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@HasSampleDownload,
		@SampleDownloadID,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO


-- Minimim search string length
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Search.ProductSearchTermMinimumLength')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Search.ProductSearchTermMinimumLength', N'3', N'')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Search.ForumSearchTermMinimumLength')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Search.ForumSearchTermMinimumLength', N'3', N'')
END
GO


--Delete expired shopping cart items
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShoppingCartItemDeleteExpired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShoppingCartItemDeleteExpired]
GO
CREATE PROCEDURE [dbo].[Nop_ShoppingCartItemDeleteExpired]
(
	@OlderThan datetime
)
AS
BEGIN
	SET NOCOUNT ON
		
	DELETE FROM [Nop_ShoppingCartItem]
	WHERE UpdatedOn < @OlderThan
END
GO


--measure rates
IF NOT EXISTS (
	SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_MeasureDimension]') and NAME='Ratio')
BEGIN
	ALTER TABLE [dbo].[Nop_MeasureDimension] 
	ADD Ratio decimal(18, 4) NOT NULL CONSTRAINT [DF_Nop_MeasureDimension_Ratio] DEFAULT ((1))

	exec ('UPDATE [Nop_Setting] SET [Value]=(select MeasureDimensionID from Nop_MeasureDimension where SystemKeyword=''inches'') WHERE [Name]=''Common.BaseDimensionIn''')

	exec ('UPDATE [Nop_MeasureDimension] SET Ratio=(select CONVERT(decimal(18,4), 1.00)) WHERE SystemKeyword=''inches''')
	exec ('UPDATE [Nop_MeasureDimension] SET Ratio=(select CONVERT(decimal(18,4), 0.0833)) WHERE SystemKeyword=''feets''')
	exec ('UPDATE [Nop_MeasureDimension] SET Ratio=(select CONVERT(decimal(18,4), 0.0254)) WHERE SystemKeyword=''meters''')
END
GO

IF NOT EXISTS (
	SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_MeasureWeight]') and NAME='Ratio')
BEGIN
	ALTER TABLE [dbo].[Nop_MeasureWeight] 
	ADD Ratio decimal(18, 4) NOT NULL CONSTRAINT [DF_Nop_MeasureWeight_Ratio] DEFAULT ((1))

	exec ('UPDATE [Nop_Setting] SET [Value]=(select MeasureWeightID from Nop_MeasureWeight where SystemKeyword=''lb'') WHERE [Name]=''Common.BaseWeightIn''')

	exec ('UPDATE [Nop_MeasureWeight] SET Ratio=(select CONVERT(decimal(18,4), 1.00)) WHERE SystemKeyword=''lb''')
	exec ('UPDATE [Nop_MeasureWeight] SET Ratio=(select CONVERT(decimal(18,4), 16.00)) WHERE SystemKeyword=''ounce''')
	exec ('UPDATE [Nop_MeasureWeight] SET Ratio=(select CONVERT(decimal(18,4), 0.4536)) WHERE SystemKeyword=''kg''')
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureDimensionDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureDimensionDelete]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureDimensionDelete]
(
	@MeasureDimensionID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_MeasureDimension]
	WHERE
		MeasureDimensionID = @MeasureDimensionID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureDimensionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureDimensionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureDimensionInsert]
(
	@MeasureDimensionID int = NULL output,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Ratio decimal(18, 4),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_MeasureDimension]
	(
		[Name],
		[SystemKeyword],
		[Ratio],
		[DisplayOrder]
	)
	VALUES
	(
		@Name,
		@SystemKeyword,
		@Ratio,
		@DisplayOrder
	)

	set @MeasureDimensionID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureDimensionUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureDimensionUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureDimensionUpdate]
(
	@MeasureDimensionID int,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Ratio decimal(18, 4),
	@DisplayOrder int
)
AS
BEGIN

	UPDATE [Nop_MeasureDimension]
	SET
		[Name]=@Name,
		[SystemKeyword]=@SystemKeyword,
		[Ratio]=@Ratio,
		[DisplayOrder]=@DisplayOrder
	WHERE
		MeasureDimensionID = @MeasureDimensionID

END
GO





IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureWeightDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureWeightDelete]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureWeightDelete]
(
	@MeasureWeightID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_MeasureWeight]
	WHERE
		MeasureWeightID = @MeasureWeightID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureWeightInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureWeightInsert]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureWeightInsert]
(
	@MeasureWeightID int = NULL output,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Ratio decimal(18, 4),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_MeasureWeight]
	(
		[Name],
		[SystemKeyword],
		[Ratio],
		[DisplayOrder]
	)
	VALUES
	(
		@Name,
		@SystemKeyword,
		@Ratio,
		@DisplayOrder
	)

	set @MeasureWeightID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_MeasureWeightUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_MeasureWeightUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_MeasureWeightUpdate]
(
	@MeasureWeightID int,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Ratio decimal(18, 4),
	@DisplayOrder int
)
AS
BEGIN

	UPDATE [Nop_MeasureWeight]
	SET
		[Name]=@Name,
		[SystemKeyword]=@SystemKeyword,
		[Ratio]=@Ratio,
		[DisplayOrder]=@DisplayOrder
	WHERE
		MeasureWeightID = @MeasureWeightID

END
GO



--fixed [Nop_SpecificationAttributeOptionFilter_LoadByFilter] stored procedure
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SpecificationAttributeOptionFilter_LoadByFilter]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SpecificationAttributeOptionFilter_LoadByFilter]
GO
CREATE PROCEDURE [dbo].[Nop_SpecificationAttributeOptionFilter_LoadByFilter]
(
	@CategoryID int
)
AS
BEGIN
	SELECT 
		sa.SpecificationAttributeID,
		sa.Name 'SpecificationAttributeName',
		sa.DisplayOrder,
		sao.SpecificationAttributeOptionID,
		sao.Name 'SpecificationAttributeOptionName'
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
		sa.Name,
		sa.DisplayOrder,
		sao.SpecificationAttributeOptionID,
		sao.Name
	ORDER BY sa.DisplayOrder, sa.Name, sao.Name
END
GO





--Download file name
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Download]') and NAME='Filename')
BEGIN
	ALTER TABLE [dbo].[Nop_Download] 
	ADD [Filename] nvarchar(100) NOT NULL CONSTRAINT [DF_Nop_Download_Filename] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DownloadInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DownloadInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DownloadInsert]
(
	@DownloadID int = NULL output,
	@UseDownloadURL bit,
	@DownloadURL nvarchar(400),
	@DownloadBinary image,
	@ContentType nvarchar(20),
	@Filename nvarchar(100),
	@Extension nvarchar(20),
	@IsNew	bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Download]
	(
		[UseDownloadURL],
		[DownloadURL],
		[DownloadBinary],
		[Filename],
		[ContentType],
		[Extension],
		[IsNew]
	)
	VALUES
	(
		@UseDownloadURL,
		@DownloadURL,
		@DownloadBinary,
		@Filename,
		@ContentType,
		@Extension,
		@IsNew
	)

	set @DownloadID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DownloadUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DownloadUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_DownloadUpdate]
(
	@DownloadID int,
	@UseDownloadURL bit,
	@DownloadURL nvarchar(400),
	@DownloadBinary image,
	@ContentType nvarchar(20),
	@Filename nvarchar(100),
	@Extension nvarchar(20),
	@IsNew	bit
)
AS
BEGIN

	UPDATE [Nop_Download]
	SET		
		[UseDownloadURL]=@UseDownloadURL,
		[DownloadURL]=@DownloadURL,
		[DownloadBinary]=@DownloadBinary,
		[ContentType] = @ContentType,
		[Filename]=@Filename,
		[Extension]=@Extension,
		[IsNew]=@IsNew
	WHERE
		DownloadID = @DownloadID

END
GO

--Downloadable products. Expiration days
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='DownloadExpirationDays')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD [DownloadExpirationDays] int NULL
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		HasSampleDownload,
		SampleDownloadID,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@HasSampleDownload,
		@SampleDownloadID,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO



-- Determines whether to validate users (downloadable products)
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Security.DownloadableProducts.ValidateUser')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Security.DownloadableProducts.ValidateUser', N'false', N'')
END
GO





--Load order by GUID
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderLoadByGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderLoadByGuid]
GO
CREATE PROCEDURE [dbo].[Nop_OrderLoadByGuid]
(
	@OrderGUID uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_Order]
	WHERE
		[OrderGUID] = @OrderGUID
END
GO

-- 2Checkout payment method. MD5 hashing
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.TwoCheckout.UseMD5Hashing')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.TwoCheckout.UseMD5Hashing', N'true', N'Determines whether to use MD5 hashing')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.TwoCheckout.SecretWord')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.TwoCheckout.SecretWord', N'', N'')
END
GO



--new discount requirements
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 10)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name]) VALUES (10, N'Had purchased all of these product variants')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 20)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name]) VALUES (20, N'Had purchased one of these product variants')
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantLoadAll]
(
	@OrderID int,
	@CustomerID int,
	@StartTime datetime = NULL,
	@EndTime datetime = NULL,
	@OrderStatusID int,
	@PaymentStatusID int,
	@ShippingStatusID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		opv.*
	FROM [Nop_OrderProductVariant] opv
	INNER JOIN [Nop_Order] o ON opv.OrderID=o.OrderID
	WHERE
		(@OrderID IS NULL OR @OrderID=0 or o.OrderID = @OrderID) and
		(@CustomerID IS NULL OR @CustomerID=0 or o.CustomerID = @CustomerID) and
		(@StartTime is NULL or DATEDIFF(day, @StartTime, o.CreatedOn) >= 0) and
		(@EndTime is NULL or DATEDIFF(day, @EndTime, o.CreatedOn) <= 0) and 
		(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) and
		(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) and
		(@ShippingStatusID IS NULL OR @ShippingStatusID = 0 OR o.ShippingStatusID = @ShippingStatusID) and
		(o.Deleted=0)		
	ORDER BY o.CreatedOn desc, [opv].OrderProductVariantID 
END
GO


if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountRestriction]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_DiscountRestriction](
	[ProductVariantID] [int] NOT NULL,
	[DiscountID] [int] NOT NULL,
 CONSTRAINT [Nop_DiscountRestriction_PK] PRIMARY KEY CLUSTERED 
(
	[ProductVariantID] ASC,
	[DiscountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_DiscountRestriction_Nop_ProductVariant'
           AND parent_obj = Object_id('Nop_DiscountRestriction')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_DiscountRestriction
DROP CONSTRAINT FK_Nop_DiscountRestriction_Nop_ProductVariant
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountRestriction_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_DiscountRestriction_Nop_Discount'
           AND parent_obj = Object_id('Nop_DiscountRestriction')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_DiscountRestriction
DROP CONSTRAINT FK_Nop_DiscountRestriction_Nop_Discount
GO
ALTER TABLE [dbo].[Nop_DiscountRestriction]  WITH CHECK ADD  CONSTRAINT [FK_Nop_DiscountRestriction_Nop_Discount] FOREIGN KEY([DiscountID])
REFERENCES [dbo].[Nop_Discount] ([DiscountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountRestrictionDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountRestrictionDelete]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountRestrictionDelete]
(
	@ProductVariantID int,
	@DiscountID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_DiscountRestriction]
	WHERE
		ProductVariantID = @ProductVariantID and DiscountID=@DiscountID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountRestrictionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountRestrictionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountRestrictionInsert]
(
	@ProductVariantID int,
	@DiscountID int
)
AS
BEGIN
	IF NOT EXISTS (SELECT (1) FROM [Nop_DiscountRestriction] WHERE ProductVariantID=@ProductVariantID and DiscountID=@DiscountID)
	INSERT
		INTO [Nop_DiscountRestriction]
		(
			ProductVariantID,
			DiscountID
		)
		VALUES
		(
			@ProductVariantID,
			@DiscountID
		)
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantRestrictedLoadDiscountID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantRestrictedLoadDiscountID]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantRestrictedLoadDiscountID]
(
	@DiscountID int
)
AS
BEGIN
	SELECT pv.*
	FROM
		[dbo].[Nop_ProductVariant] pv
		INNER JOIN [dbo].[Nop_DiscountRestriction] dr
		ON pv.ProductVariantID = dr.ProductVariantID
	WHERE
		dr.[DiscountID] = @DiscountID
END
GO

--DIBS FlexWin payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Dibs.FlexWinPaymentProcessor, Nop.Payment.Dibs')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'DIBS FlexWin', N'DIBS FlexWin', N'', N'Payment\Dibs\FlexWinConfig.ascx', N'~\Templates\Payment\Dibs\FlexWinPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Dibs.FlexWinPaymentProcessor, Nop.Payment.Dibs', N'DIBS.FLEXWIN', 0, 130)
END
GO


--Download Activation Type
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='DownloadActivationType')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD DownloadActivationType int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DownloadActivationType] DEFAULT ((1))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO



IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_OrderProductVariant]') and NAME='IsDownloadActivated')
BEGIN
	ALTER TABLE [dbo].[Nop_OrderProductVariant] 
	ADD IsDownloadActivated bit NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_IsDownloadActivated] DEFAULT ((0))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
(
	@OrderProductVariantID int = NULL output,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderProductVariant]
	(
		OrderProductVariantGUID,
		OrderID,
		ProductVariantID,
		UnitPriceInclTax,
		UnitPriceExclTax,
		PriceInclTax,
		PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency,
		AttributeDescription,
		Quantity,
		DiscountAmountInclTax,
		DiscountAmountExclTax,
		DownloadCount,
		IsDownloadActivated
	)
	VALUES
	(
		@OrderProductVariantGUID,
		@OrderID,
		@ProductVariantID,
		@UnitPriceInclTax,
		@UnitPriceExclTax,
		@PriceInclTax,
		@PriceExclTax,
		@UnitPriceInclTaxInCustomerCurrency,
		@UnitPriceExclTaxInCustomerCurrency,
		@PriceInclTaxInCustomerCurrency,
		@PriceExclTaxInCustomerCurrency,
		@AttributeDescription,
		@Quantity,
		@DiscountAmountInclTax,
		@DiscountAmountExclTax,
		@DownloadCount,
		@IsDownloadActivated
	)

	set @OrderProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
(
	@OrderProductVariantID int,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit
)
AS
BEGIN

	UPDATE [Nop_OrderProductVariant]
	SET		
		OrderProductVariantGUID=@OrderProductVariantGUID,
		OrderID=@OrderID,
		ProductVariantID=@ProductVariantID,
		UnitPriceInclTax=@UnitPriceInclTax,
		UnitPriceExclTax = @UnitPriceExclTax,
		PriceInclTax=@PriceInclTax,
		PriceExclTax=@PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency=@UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency=@UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency=@PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency=@PriceExclTaxInCustomerCurrency,
		AttributeDescription=@AttributeDescription,
		Quantity=@Quantity,
		DiscountAmountInclTax=@DiscountAmountInclTax,
		DiscountAmountExclTax=@DiscountAmountExclTax,
		DownloadCount=@DownloadCount,
		IsDownloadActivated=@IsDownloadActivated
	WHERE
		OrderProductVariantID = @OrderProductVariantID
END
GO

--Svea hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Svea.HostedPaymentProcessor, Nop.Payment.Svea')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Svea Hosted Payment', N'Svea Hosted Payment', N'', N'Payment\Svea\HostedPaymentConfig.ascx', N'~\Templates\Payment\Svea\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Svea.HostedPaymentProcessor, Nop.Payment.Svea', N'SVEA.HOSTEDPAYMENT', 0, 140)
END
GO





--Access control lists
if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_CustomerAction]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_CustomerAction](
	[CustomerActionID] [int] IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [SystemKeyword] nvarchar(100) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
 CONSTRAINT [Nop_CustomerAction_PK] PRIMARY KEY CLUSTERED 
(
	[CustomerActionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ACL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_ACL](
	[ACLID] [int] IDENTITY(1,1) NOT NULL,
    [CustomerActionID] [int] NOT NULL,
    [CustomerRoleID] [int] NOT NULL,
    [Allow] [bit] NOT NULL,
 CONSTRAINT [Nop_ACL_PK] PRIMARY KEY CLUSTERED 
(
	[AclID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
CONSTRAINT [IX_Nop_ACL_Unique] UNIQUE NONCLUSTERED 
(
	[CustomerActionID] ASC,
	[CustomerRoleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ACL_Nop_CustomerAction'
           AND parent_obj = Object_id('Nop_ACL')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ACL
DROP CONSTRAINT FK_Nop_ACL_Nop_CustomerAction
GO
ALTER TABLE [dbo].[Nop_ACL]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACL_Nop_CustomerAction] FOREIGN KEY([CustomerActionID])
REFERENCES [dbo].[Nop_CustomerAction] ([CustomerActionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ACL_Nop_CustomerRole'
           AND parent_obj = Object_id('Nop_ACL')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ACL
DROP CONSTRAINT FK_Nop_ACL_Nop_CustomerRole
GO
ALTER TABLE [dbo].[Nop_ACL]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACL_Nop_CustomerRole] FOREIGN KEY([CustomerRoleID])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO





IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionDelete]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionDelete]
(
	@CustomerActionID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_CustomerAction]
	WHERE
		[CustomerActionID] = @CustomerActionID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionInsert]
(
	@CustomerActionID int = NULL output,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Comment nvarchar(1000)
)
AS
BEGIN
	INSERT
	INTO [Nop_CustomerAction]
	(
		[Name],
		[SystemKeyword],
		[Comment]
	)
	VALUES
	(
		@Name,
		@SystemKeyword,
		@Comment
	)

	set @CustomerActionID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionLoadAll]
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_CustomerAction]
	ORDER BY [Name]
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionLoadByPrimaryKey]
(
	@CustomerActionID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_CustomerAction]
	WHERE
		[CustomerActionID] = @CustomerActionID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionUpdate]
(
	@CustomerActionID int,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Comment nvarchar(1000)
)
AS
BEGIN

	UPDATE [Nop_CustomerAction]
	SET
		[Name]=@Name,
		[SystemKeyword]=@SystemKeyword,
		[Comment]=@Comment
	WHERE
		[CustomerActionID] = @CustomerActionID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLDelete]
GO
CREATE PROCEDURE [dbo].[Nop_ACLDelete]
(
	@ACLID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_ACL]
	WHERE
		[ACLID] = @ACLID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ACLInsert]
(
	@ACLID int = NULL output,
	@CustomerActionID int,
	@CustomerRoleID int,
	@Allow bit
)
AS
BEGIN
	INSERT
	INTO [Nop_ACL]
	(
		[CustomerActionID],
		[CustomerRoleID],
		[Allow]
	)
	VALUES
	(
		@CustomerActionID,
		@CustomerRoleID,
		@Allow
	)

	set @ACLID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ACLLoadAll]
	@CustomerActionID int,
	@CustomerRoleID int,
	@Allow int = NULL	
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ACL]
	WHERE 
		(@CustomerActionID IS NULL or @CustomerActionID=0 or [CustomerActionID] = @CustomerActionID)
		AND
		(@CustomerRoleID IS NULL or @CustomerRoleID=0 or [CustomerRoleID] = @CustomerRoleID)
		AND
		(@Allow IS NULL OR [Allow]=@Allow)
	ORDER BY [ACLID] desc
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_ACLLoadByPrimaryKey]
(
	@ACLID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_ACL]
	WHERE
		ACLID = @ACLID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ACLUpdate]
(
	@ACLID int,
	@CustomerActionID int,
	@CustomerRoleID int,
	@Allow bit
)
AS
BEGIN

	UPDATE [Nop_ACL]
	SET
		[CustomerActionID]=@CustomerActionID,
		[CustomerRoleID]=@CustomerRoleID,
		[Allow]=@Allow
	WHERE
		[ACLID]=@ACLID

END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLIsAllowed]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ACLIsAllowed]
GO
CREATE PROCEDURE [dbo].[Nop_ACLIsAllowed]
(
	@CustomerID int,
	@ActionSystemKeyword nvarchar(100)
)
AS
BEGIN

	DECLARE @Result bit
	SET @Result=0
	
	IF EXISTS (SELECT 1 FROM [Nop_Customer_CustomerRole_Mapping] crm
				INNER JOIN [Nop_CustomerRole] cr ON crm.CustomerRoleID=cr.CustomerRoleID
				INNER JOIN [Nop_ACL] acl ON cr.CustomerRoleID=acl.CustomerRoleID
				INNER JOIN [Nop_CustomerAction] ca ON acl.CustomerActionID=ca.CustomerActionID
				WHERE 
					crm.CustomerID=@CustomerID 
					AND cr.Deleted=0 
					AND cr.Active=1
					AND acl.Allow=1
					AND ca.SystemKeyword=@ActionSystemKeyword 
				)	
		SET @Result=1
	ELSE
		SET @Result=0

	SELECT @Result as [result]

END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ACL.Enabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ACL.Enabled', N'false', N'Determines the ACL feature is enabled')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.ACLManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.ACLManager.CacheEnabled', N'true', N'')
END
GO





--discount limitations
if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountLimitation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_DiscountLimitation](
	[DiscountLimitationID] [int] NOT NULL,
	[Name] nvarchar(100) NOT NULL,
 CONSTRAINT [Nop_DiscountLimitation_PK] PRIMARY KEY CLUSTERED 
(
	[DiscountLimitationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 0)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name]) VALUES (0, N'Unlimited')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 10)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name]) VALUES (10, N'One Time Only')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 20)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name]) VALUES (20, N'One Time Per Customer')
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountLimitationLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountLimitationLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountLimitationLoadAll]
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_DiscountLimitation]
	ORDER BY DiscountLimitationID
END
GO



IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='DiscountLimitationID')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD DiscountLimitationID int NOT NULL CONSTRAINT [DF_Nop_Discount_DiscountLimitationID] DEFAULT ((0))
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountLimitation'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountLimitation
GO
ALTER TABLE [dbo].[Nop_Discount]  WITH CHECK ADD  CONSTRAINT [FK_Nop_Discount_Nop_DiscountLimitation] FOREIGN KEY([DiscountLimitationID])
REFERENCES [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountInsert]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountInsert]
(
	@DiscountID int = NULL output,
	@DiscountTypeID int,
	@DiscountRequirementID int,
	@DiscountLimitationID int,
	@Name nvarchar(100),
	@UsePercentage bit, 
	@DiscountPercentage decimal (18, 4),
	@DiscountAmount decimal (18, 4),
	@StartDate datetime,
	@EndDate datetime,
	@RequiresCouponCode bit,
	@CouponCode nvarchar(100),
	@Deleted bit
)
AS
BEGIN
	INSERT
	INTO [Nop_Discount]
	(
		[DiscountTypeID],
		[DiscountRequirementID],
		[DiscountLimitationID],
		[Name],
		[UsePercentage],
		[DiscountPercentage],
		[DiscountAmount],
		[StartDate],
		[EndDate],
		[RequiresCouponCode],
		[CouponCode],
		[Deleted]
	)
	VALUES
	(
		@DiscountTypeID,
		@DiscountRequirementID,
		@DiscountLimitationID,
		@Name,
		@UsePercentage,
		@DiscountPercentage,
		@DiscountAmount,
		@StartDate,
		@EndDate,
		@RequiresCouponCode,
		@CouponCode,
		@Deleted
	)

	set @DiscountID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_DiscountUpdate]
(
	@DiscountID int,
	@DiscountTypeID int,
	@DiscountRequirementID int,
	@DiscountLimitationID int,
	@Name nvarchar(100),
	@UsePercentage bit, 
	@DiscountPercentage decimal (18, 4),
	@DiscountAmount decimal (18, 4),
	@StartDate datetime,
	@EndDate datetime,
	@RequiresCouponCode bit,
	@CouponCode nvarchar(100),
	@Deleted bit
)
AS
BEGIN
	UPDATE [Nop_Discount]
	SET
		[DiscountTypeID]=@DiscountTypeID,
		[DiscountRequirementID]=@DiscountRequirementID,
		[DiscountLimitationID]=@DiscountLimitationID,
		[Name]=@Name,
		[UsePercentage]=@UsePercentage,
		[DiscountPercentage]=@DiscountPercentage,
		[DiscountAmount]=@DiscountAmount,
		[StartDate]=@StartDate,
		[EndDate]=@EndDate,
		[RequiresCouponCode]=@RequiresCouponCode,
		[CouponCode]=@CouponCode,
		[Deleted]=@Deleted
	WHERE
		[DiscountID] = @DiscountID
END
GO






-- fixed [Nop_OrderNoteLoadByOrderID] stored procedure
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderNoteLoadByOrderID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderNoteLoadByOrderID]
GO
CREATE PROCEDURE [dbo].[Nop_OrderNoteLoadByOrderID]
(
	@OrderID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_OrderNote]
	WHERE
		OrderID=@OrderID
	ORDER BY CreatedOn desc, OrderNoteID desc
END
GO



IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='TrackingNumber')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD TrackingNumber nvarchar(100) NOT NULL CONSTRAINT [DF_Nop_Order_TrackingNumber] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderUpdate]
(
	@OrderID int,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_Order]
	SET
		OrderGUID=@OrderGUID,
		CustomerID=@CustomerID,
		CustomerLanguageID=@CustomerLanguageID,		
		CustomerTaxDisplayTypeID=@CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax=@OrderSubtotalInclTax,
		OrderSubtotalExclTax=@OrderSubtotalExclTax,		
		OrderShippingInclTax=@OrderShippingInclTax,
		OrderShippingExclTax=@OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax=@PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax=@PaymentMethodAdditionalFeeExclTax,
		OrderTax=@OrderTax,
		OrderTotal=@OrderTotal,
		OrderDiscount=@OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency=@OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency=@OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency=@OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency=@OrderShippingExclTaxInCustomerCurrency,	
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,	
		OrderTaxInCustomerCurrency=@OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency=@OrderTotalInCustomerCurrency,
		CustomerCurrencyCode=@CustomerCurrencyCode,
		OrderWeight=@OrderWeight,
		AffiliateID=@AffiliateID,
		OrderStatusID=@OrderStatusID,
		AllowStoringCreditCardNumber=@AllowStoringCreditCardNumber,
		CardType=@CardType,
		CardName=@CardName,
		CardNumber=@CardNumber,
		MaskedCreditCardNumber=@MaskedCreditCardNumber,
		CardCVV2=@CardCVV2,
		CardExpirationMonth=@CardExpirationMonth,
		CardExpirationYear=@CardExpirationYear,
		PaymentMethodID=@PaymentMethodID,
		PaymentMethodName=@PaymentMethodName,
		AuthorizationTransactionID=@AuthorizationTransactionID,
		AuthorizationTransactionCode=@AuthorizationTransactionCode,
		AuthorizationTransactionResult=@AuthorizationTransactionResult,
		CaptureTransactionID=@CaptureTransactionID,
		CaptureTransactionResult=@CaptureTransactionResult,
		PurchaseOrderNumber=@PurchaseOrderNumber,
		PaymentStatusID=@PaymentStatusID,
		PaidDate=@PaidDate,
		BillingFirstName=@BillingFirstName,
		BillingLastName=@BillingLastName,
		BillingPhoneNumber=@BillingPhoneNumber,
		BillingEmail=@BillingEmail,
		BillingFaxNumber=@BillingFaxNumber,
		BillingCompany=@BillingCompany,
		BillingAddress1=@BillingAddress1,
		BillingAddress2=@BillingAddress2,
		BillingCity=@BillingCity,
		BillingStateProvince=@BillingStateProvince,
		BillingStateProvinceID=@BillingStateProvinceID,
		BillingZipPostalCode=@BillingZipPostalCode,
		BillingCountry=@BillingCountry,
		BillingCountryID=@BillingCountryID,
		ShippingStatusID=@ShippingStatusID,
		ShippingFirstName=@ShippingFirstName,
		ShippingLastName=@ShippingLastName,
		ShippingPhoneNumber=@ShippingPhoneNumber,
		ShippingEmail=@ShippingEmail,
		ShippingFaxNumber=@ShippingFaxNumber,
		ShippingCompany=@ShippingCompany,
		ShippingAddress1=@ShippingAddress1,
		ShippingAddress2=@ShippingAddress2,
		ShippingCity=@ShippingCity,
		ShippingStateProvince=@ShippingStateProvince,
		ShippingStateProvinceID=@ShippingStateProvinceID,
		ShippingZipPostalCode=@ShippingZipPostalCode,
		ShippingCountry=@ShippingCountry,
		ShippingCountryID=@ShippingCountryID,
		ShippingMethod=@ShippingMethod,
		ShippingRateComputationMethodID=@ShippingRateComputationMethodID,
		ShippedDate=@ShippedDate,
		TrackingNumber=@TrackingNumber,
		Deleted=@Deleted,
		CreatedOn=@CreatedOn
	WHERE
		OrderID = @OrderID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		PurchaseOrderNumber,
		PaymentStatusID,
		PaidDate,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		TrackingNumber,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@PaidDate,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@TrackingNumber,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO

-- [dbo].[Nop_ShippingMethod_RestrictedCountriesInsert] stored procedure

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethod_RestrictedCountriesInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesInsert]

GO

CREATE PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesInsert]
(
	@ShippingMethodID int,
	@CountryID int
)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM [Nop_ShippingMethod_RestrictedCountries] WHERE ShippingMethodID = @ShippingMethodID AND CountryID = @CountryID))
	BEGIN
		INSERT
		INTO [Nop_ShippingMethod_RestrictedCountries]
		(
			ShippingMethodID,
			CountryID
		)
		VALUES
		(
			@ShippingMethodID,
			@CountryID
		)
	END
END
GO

-- [Nop_ShippingMethod_RestrictedCountriesDelete] stored procedure

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethod_RestrictedCountriesDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesDelete]

GO

CREATE PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesDelete]
(
	@ShippingMethodID int,
	@CountryID int
)
AS
BEGIN
	DELETE FROM 
		[Nop_ShippingMethod_RestrictedCountries]
	WHERE
		ShippingMethodID = @ShippingMethodID AND
		CountryID = @CountryID
END
GO

-- [Nop_ShippingMethod_RestrictedCountriesContains] stored procedure

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethod_RestrictedCountriesContains]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesContains]

GO

CREATE PROCEDURE [dbo].[Nop_ShippingMethod_RestrictedCountriesContains]
(
	@ShippingMethodID int,
	@CountryID int,
	@Result bit output
)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM [Nop_ShippingMethod_RestrictedCountries] WHERE ShippingMethodID = @ShippingMethodID AND CountryID = @CountryID))
		SET @Result = 0
	ELSE
		SET @Result = 1
END

GO

-- [Nop_ShippingMethodLoadAll] stored procedure

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_ShippingMethodLoadAll]

GO

CREATE PROCEDURE [dbo].[Nop_ShippingMethodLoadAll]
(
	@FilterByCountryID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	IF(@FilterByCountryID IS NOT NULL AND @FilterByCountryID != 0)
		BEGIN
			SELECT  
				sm.*
		    FROM 
				[Nop_ShippingMethod] sm
		    WHERE 
                sm.ShippingMethodID NOT IN 
				(
				    SELECT 
						smc.ShippingMethodID
				    FROM 
						[Nop_ShippingMethod_RestrictedCountries] smc
				    WHERE 
						smc.CountryID = @FilterByCountryID AND 
						sm.ShippingMethodID = smc.ShippingMethodID
				)
		   ORDER BY 
				sm.DisplayOrder
		END
	ELSE
		BEGIN
			SELECT 
				*
			FROM 
				[Nop_ShippingMethod]
			ORDER BY
				DisplayOrder
		END
END

GO






--Recurring payments
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='IsRecurring')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD IsRecurring bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_IsRecurring] DEFAULT ((0))
END
GO
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='CycleLength')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD CycleLength int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CycleLength] DEFAULT ((1))
END
GO
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='CyclePeriod')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD CyclePeriod int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CyclePeriod] DEFAULT ((0))
END
GO
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='TotalCycles')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD TotalCycles int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_TotalCycles] DEFAULT ((1))
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
		IsRecurring,
		CycleLength,
		CyclePeriod,
		TotalCycles,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
		@IsRecurring,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		IsRecurring=@IsRecurring,
		CycleLength=@CycleLength,
		CyclePeriod=@CyclePeriod,
		TotalCycles=@TotalCycles,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO

--recurring payments
if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_RecurringPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentInsert]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentInsert]
(
	@RecurringPaymentID int = NULL output,
	@InitialOrderID int,
	@CycleLength int,
	@CyclePeriod int,
	@TotalCycles int,
	@StartDate datetime,
	@IsActive bit,
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_RecurringPayment]
	(
		[InitialOrderID],
		[CycleLength],
		[CyclePeriod],
		[TotalCycles],
		[StartDate],
		[IsActive],
		[Deleted],
		[CreatedOn]
	)
	VALUES
	(
		@InitialOrderID,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
		@StartDate,
		@IsActive,
		@Deleted,
		@CreatedOn
	)

	set @RecurringPaymentID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentUpdate]
(
	@RecurringPaymentID int,
	@InitialOrderID int,
	@CycleLength int,
	@CyclePeriod int,
	@TotalCycles int,
	@StartDate datetime,
	@IsActive bit,
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN

	UPDATE [Nop_RecurringPayment]
	SET
		[InitialOrderID]=@InitialOrderID,
		[CycleLength]=@CycleLength,
		[CyclePeriod]=@CyclePeriod,
		[TotalCycles]=@TotalCycles,
		[StartDate]=@StartDate,
		[IsActive]=@IsActive,
		[Deleted]=@Deleted,
		[CreatedOn]=@CreatedOn
	WHERE
		[RecurringPaymentID] = @RecurringPaymentID

END
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentByPrimaryKey]
(
	@RecurringPaymentID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_RecurringPayment]
	WHERE
		([RecurringPaymentID] = @RecurringPaymentID)
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentLoadAll]
(
	@ShowHidden		bit = 0,
	@CustomerID		int = NULL,
	@InitialOrderID	int = NULL,
	@InitialOrderStatusID int = NULL
)
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT * FROM [Nop_RecurringPayment]
	WHERE RecurringPaymentID IN 
		(
		SELECT DISTINCT rp.RecurringPaymentID
		FROM [Nop_RecurringPayment] rp WITH (NOLOCK)
		INNER JOIN Nop_Order o with (NOLOCK) ON rp.InitialOrderID=o.OrderID
		INNER JOIN Nop_Customer c with (NOLOCK) ON o.CustomerID=c.CustomerID
		WHERE
				(
					rp.Deleted=0 AND o.Deleted=0 AND c.Deleted=0
				)
				AND 
				(
					@ShowHidden = 1 OR rp.IsActive=1
				)
				AND
				(
					@CustomerID IS NULL OR @CustomerID=0
					OR (o.CustomerID=@CustomerID)
				)
				AND
				(
					@InitialOrderID IS NULL OR @InitialOrderID=0
					OR (rp.InitialOrderID=@InitialOrderID)
				)
				AND
				(
					@InitialOrderStatusID IS NULL OR @InitialOrderStatusID=0
					OR (o.OrderStatusID=@InitialOrderStatusID)
				)
		)
	ORDER BY StartDate, RecurringPaymentID
	
END
GO


--Recurring payments history
if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_RecurringPaymentHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_RecurringPaymentHistory](
	[RecurringPaymentHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[RecurringPaymentID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_RecurringPaymentHistory_PK] PRIMARY KEY CLUSTERED 
(
	[RecurringPaymentHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_RecurringPaymentHistory_Nop_RecurringPayment'
           AND parent_obj = Object_id('Nop_RecurringPaymentHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_RecurringPaymentHistory
DROP CONSTRAINT FK_Nop_RecurringPaymentHistory_Nop_RecurringPayment
GO
ALTER TABLE [dbo].[Nop_RecurringPaymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_RecurringPaymentHistory_Nop_RecurringPayment] FOREIGN KEY([RecurringPaymentID])
REFERENCES [dbo].[Nop_RecurringPayment] ([RecurringPaymentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryDelete]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentHistoryDelete]
(
	@RecurringPaymentHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_RecurringPaymentHistory]
	WHERE
		RecurringPaymentHistoryID = @RecurringPaymentHistoryID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentHistoryInsert]
(
	@RecurringPaymentHistoryID int = NULL output,
	@RecurringPaymentID int,
	@OrderID int,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_RecurringPaymentHistory]
	(
		[RecurringPaymentID],
		[OrderID],
		[CreatedOn]
	)
	VALUES
	(
		@RecurringPaymentID,
		@OrderID,
		@CreatedOn
	)

	set @RecurringPaymentHistoryID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentHistoryLoadByPrimaryKey]
(
	@RecurringPaymentHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_RecurringPaymentHistory]
	WHERE
		RecurringPaymentHistoryID = @RecurringPaymentHistoryID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentHistoryUpdate]
(
	@RecurringPaymentHistoryID int,
	@RecurringPaymentID int,
	@OrderID int,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_RecurringPaymentHistory]
	SET
		RecurringPaymentID=@RecurringPaymentID,
		OrderID = @OrderID,
		CreatedOn = @CreatedOn
	WHERE
		RecurringPaymentHistoryID=@RecurringPaymentHistoryID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_RecurringPaymentHistoryLoadAll]
(
	@RecurringPaymentID int = NULL,
	@OrderID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT * FROM [Nop_RecurringPaymentHistory]
	WHERE RecurringPaymentHistoryID IN 
		(
		SELECT DISTINCT rph.RecurringPaymentHistoryID
		FROM [Nop_RecurringPaymentHistory] rph WITH (NOLOCK)
		INNER JOIN Nop_RecurringPayment rp with (NOLOCK) ON rph.RecurringPaymentID=rp.RecurringPaymentID
		LEFT OUTER JOIN Nop_Order o with (NOLOCK) ON rph.OrderID=o.OrderID
		WHERE
				(
					rp.Deleted=0 AND o.Deleted=0 
				)
				AND
				(
					@RecurringPaymentID IS NULL OR @RecurringPaymentID=0
					OR (rph.RecurringPaymentID=@RecurringPaymentID)
				)
				AND
				(
					@OrderID IS NULL OR @OrderID=0
					OR (rph.OrderID=@OrderID)
				)
		)
	ORDER BY CreatedOn, RecurringPaymentHistoryID
END
GO

-- [Nop_PaymentMethod_RestrictedCountries] table
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethod_RestrictedCountries]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries](
		[PaymentMethodID] [int] NOT NULL,
		[CountryID] [int] NOT NULL,
	 CONSTRAINT [PK_Nop_PaymentMethod_RestrictedCountries] PRIMARY KEY CLUSTERED 
	(
		[PaymentMethodID] ASC,
		[CountryID] ASC
	)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

-- [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country] key
IF EXISTS (SELECT 1 FROM   sysobjects WHERE  name = 'FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country' AND parent_obj = OBJECT_ID('[Nop_PaymentMethod_RestrictedCountries]') AND OBJECTPROPERTY(id,N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] DROP CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country]
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Nop_Country] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_Country]
GO	

-- [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod] key
IF EXISTS (SELECT 1 FROM   sysobjects WHERE  name = 'FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod' AND parent_obj = OBJECT_ID('[Nop_PaymentMethod_RestrictedCountries]') AND OBJECTPROPERTY(id,N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] DROP CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod]
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries]  WITH CHECK ADD  CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod] FOREIGN KEY([PaymentMethodID])
REFERENCES [dbo].[Nop_PaymentMethod] ([PaymentMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_PaymentMethod_RestrictedCountries] CHECK CONSTRAINT [FK_Nop_PaymentMethod_RestrictedCountries_Nop_PaymentMethod]
GO

-- [Nop_PaymentMethod_RestrictedCountriesContains] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethod_RestrictedCountriesContains]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesInsert]
GO
CREATE PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesInsert]
(
	@PaymentMethodID int,
	@CountryID int
)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM [Nop_PaymentMethod_RestrictedCountries] WHERE PaymentMethodID = @PaymentMethodID AND CountryID = @CountryID))
	BEGIN
		INSERT
		INTO [Nop_PaymentMethod_RestrictedCountries]
		(
			PaymentMethodID,
			CountryID
		)
		VALUES
		(
			@PaymentMethodID,
			@CountryID
		)
	END
END
GO 


-- [Nop_PaymentMethod_RestrictedCountriesContains] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethod_RestrictedCountriesContains]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesContains]
GO
CREATE PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesContains]
(
	@PaymentMethodID int,
	@CountryID int,
	@Result bit output
)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM [Nop_PaymentMethod_RestrictedCountries]WHERE PaymentMethodID = @PaymentMethodID AND CountryID = @CountryID))
		SET @Result = 0
	ELSE
		SET @Result = 1
END
GO 


-- [Nop_PaymentMethod_RestrictedCountriesDelete] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethod_RestrictedCountriesDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesDelete]
GO
CREATE PROCEDURE [dbo].[Nop_PaymentMethod_RestrictedCountriesDelete]
(
	@PaymentMethodID int,
	@CountryID int
)
AS
BEGIN
	DELETE FROM 
		[Nop_PaymentMethod_RestrictedCountries]
	WHERE
		PaymentMethodID = @PaymentMethodID AND
		CountryID = @CountryID
END
GO 


-- [Nop_PaymentMethodLoadAll] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
(
	@ShowHidden bit = 0,
	@FilterByCountryID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	IF(@FilterByCountryID IS NOT NULL AND @FilterByCountryID != 0)
		BEGIN
			SELECT  
				pm.*
		    FROM 
				[Nop_PaymentMethod] pm
		    WHERE 
                pm.PaymentMethodID NOT IN 
				(
				    SELECT 
						pmc.PaymentMethodID
				    FROM 
						[Nop_PaymentMethod_RestrictedCountries] pmc
				    WHERE 
						pmc.CountryID = @FilterByCountryID AND 
						pm.PaymentMethodID = pmc.PaymentMethodID
				)
				AND
				(IsActive = 1 or @ShowHidden = 1)
		   ORDER BY 
				pm.DisplayOrder
		END
	ELSE
		BEGIN
			SELECT 
				*
			FROM 
				[Nop_PaymentMethod]
			WHERE 
				(IsActive = 1 or @ShowHidden = 1)
			ORDER BY 
				DisplayOrder
		END
END

GO 


-- [Nop_NewsLetterSubscription] table
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscription]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_NewsLetterSubscription](
	[NewsLetterSubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[NewsLetterSubscriptionGuid] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_NewsLetterSubscription] PRIMARY KEY CLUSTERED 
(
	[NewsLetterSubscriptionID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO


-- [Nop_NewsLetterSubscriptionInsert] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionInsert]
(
	@NewsLetterSubscriptionID int = NULL output,
	@NewsLetterSubscriptionGuid uniqueidentifier,
	@Email nvarchar(255),
	@Active bit,
	@CreatedOn datetime
)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM [Nop_NewsLetterSubscription] WHERE Email = @Email))
	BEGIN
		INSERT INTO 
			[Nop_NewsLetterSubscription]
			(
				NewsLetterSubscriptionGuid,
				Email,
				Active,
				CreatedOn
			)
		VALUES
			(
				@NewsLetterSubscriptionGuid,
				@Email,
				@Active,
				@CreatedOn
			)

		SET @NewsLetterSubscriptionID = SCOPE_IDENTITY()
	END
END
GO

-- [Nop_NewsLetterSubscriptionUpdate] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionUpdate]
(
	@NewsLetterSubscriptionID int,
	@NewsLetterSubscriptionGuid uniqueidentifier,
	@Email nvarchar(255),
	@Active bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE
		[Nop_NewsLetterSubscription]
	SET
		NewsLetterSubscriptionGuid = @NewsLetterSubscriptionGuid,
		Email = @Email,
		Active = @Active,
		CreatedOn = @CreatedOn
	WHERE
		NewsLetterSubscriptionID = @NewsLetterSubscriptionID
END
GO


-- [Nop_NewsLetterSubscriptionDelete] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionDelete]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionDelete]
(
	@NewsLetterSubscriptionID int
)
AS
BEGIN
	DELETE FROM
		[Nop_NewsLetterSubscription]
	WHERE
		NewsLetterSubscriptionID = @NewsLetterSubscriptionID
END
GO


-- [Nop_NewsLetterSubscriptionLoadAll] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
(
	@ShowHidden bit = 0
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
		* 
	FROM
		[Nop_NewsLetterSubscription]
	WHERE
		Active = 1 or @ShowHidden = 1
END
GO


-- [Nop_NewsLetterSubscriptionLoadByPrimaryKey] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByPrimaryKey]
(
	@NewsLetterSubscriptionID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
		* 
	FROM
		[Nop_NewsLetterSubscription]
	WHERE
		NewsLetterSubscriptionID = @NewsLetterSubscriptionID
END
GO


-- [Nop_NewsLetterSubscriptionLoadByGuid] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadByGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByGuid]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByGuid]
(
	@NewsLetterSubscriptionGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
		* 
	FROM
		[Nop_NewsLetterSubscription]
	WHERE
		NewsLetterSubscriptionGuid = @NewsLetterSubscriptionGuid
END
GO


-- [Nop_NewsLetterSubscriptionLoadByEmail] stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadByEmail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByEmail]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadByEmail]
(
	@Email nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
		* 
	FROM
		[Nop_NewsLetterSubscription]
	WHERE
		Email = @Email
END
GO

-- NewsLetterSubscription.ActivationMessage message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[Nop_MessageTemplate] WHERE [Name] = N'NewsLetterSubscription.ActivationMessage')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewsLetterSubscription.ActivationMessage')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = 'NewsLetterSubscription.ActivationMessage' 

	IF (@MessageTemplateID > 0)
	BEGIN
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'%Store.Name%. Subscription activation message.', N'<p><a href="%NewsLetterSubscription.ActivationUrl%">Click here to confirm your subscription to our list.</a></p><p>If you received this email by mistake, simply delete it.</p>')
	END
END
GO

-- NewsLetterSubscription.DeactivationMessage message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[Nop_MessageTemplate] WHERE [Name] = N'NewsLetterSubscription.DeactivationMessage')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewsLetterSubscription.DeactivationMessage')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = 'NewsLetterSubscription.DeactivationMessage' 

	IF (@MessageTemplateID > 0)
	BEGIN
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'%Store.Name%. Subscription deactivation message.', N'<p><a href="%NewsLetterSubscription.DeactivationUrl%">Click here to unsubscribe from news letters.</a></p><p>If you received this email by mistake, simply delete it.</p>')
	END
END
GO

-- [Nop_CustomerLoadAllForNewsLetters] import subscriptions from old DB
IF EXISTS (SELECT 1
    FROM [Nop_Customer] c 
    LEFT OUTER JOIN  Nop_CustomerAttribute ca
    ON c.CustomerID = ca.CustomerID
    WHERE c.deleted=0 AND c.active=1 AND ca.[key] = 'Newsletter')
BEGIN
INSERT INTO [Nop_NewsLetterSubscription]
(
 NewsLetterSubscriptionGuid,
 Email,
 Active,
 CreatedOn
 )
SELECT NEWID(), c.Email, 1, c.RegistrationDate
FROM [Nop_Customer] c 
LEFT OUTER JOIN Nop_CustomerAttribute ca ON c.CustomerID = ca.CustomerID
WHERE c.deleted=0 AND c.active=1 AND ca.[key] = 'Newsletter' AND ca.value = 'true'

DELETE FROM Nop_CustomerAttribute WHERE [key] = 'Newsletter'

END 
GO

-- [Nop_CustomerLoadAllForNewsLetters] stored procedure delete
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerLoadAllForNewsLetters]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerLoadAllForNewsLetters]
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Display.ShowBestsellersOnMainPageNumber')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Display.ShowBestsellersOnMainPageNumber', N'4', N'Number of best sellers on home page')
END
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Display.RecentlyViewedProductsNumber')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Display.RecentlyViewedProductsNumber', N'4', N'Number of Recently viewed products')
END
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Display.RecentlyAddedProductsNumber')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Display.RecentlyAddedProductsNumber', N'4', N'Number of Recently added products')
END
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Common.ProductReviewsMustBeApproved')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Common.ProductReviewsMustBeApproved', N'True', N'Product reviews must be approved by administrator')
END
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Common.AllowAnonymousUsersToReviewProduct')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Common.AllowAnonymousUsersToReviewProduct', N'False', N'Allow anonymous users write product reviews')
END
GO

IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Nop_Setting]
  WHERE [Name] = N'Common.AllowAnonymousUsersToSetProductRatings')
BEGIN
 INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
 VALUES (N'Common.AllowAnonymousUsersToSetProductRatings', N'False', N'Allow anonymous users to set product ratings')
END
GO
--recurring payments
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='SubscriptionTransactionID')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD SubscriptionTransactionID nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_Order_SubscriptionTransactionID] DEFAULT ((''))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		SubscriptionTransactionID,
		PurchaseOrderNumber,
		PaymentStatusID,
		PaidDate,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		TrackingNumber,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@SubscriptionTransactionID,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@PaidDate,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@TrackingNumber,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderUpdate]
(
	@OrderID int,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_Order]
	SET
		OrderGUID=@OrderGUID,
		CustomerID=@CustomerID,
		CustomerLanguageID=@CustomerLanguageID,		
		CustomerTaxDisplayTypeID=@CustomerTaxDisplayTypeID,
		OrderSubtotalInclTax=@OrderSubtotalInclTax,
		OrderSubtotalExclTax=@OrderSubtotalExclTax,		
		OrderShippingInclTax=@OrderShippingInclTax,
		OrderShippingExclTax=@OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax=@PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax=@PaymentMethodAdditionalFeeExclTax,
		OrderTax=@OrderTax,
		OrderTotal=@OrderTotal,
		OrderDiscount=@OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency=@OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency=@OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency=@OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency=@OrderShippingExclTaxInCustomerCurrency,	
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,	
		OrderTaxInCustomerCurrency=@OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency=@OrderTotalInCustomerCurrency,
		CustomerCurrencyCode=@CustomerCurrencyCode,
		OrderWeight=@OrderWeight,
		AffiliateID=@AffiliateID,
		OrderStatusID=@OrderStatusID,
		AllowStoringCreditCardNumber=@AllowStoringCreditCardNumber,
		CardType=@CardType,
		CardName=@CardName,
		CardNumber=@CardNumber,
		MaskedCreditCardNumber=@MaskedCreditCardNumber,
		CardCVV2=@CardCVV2,
		CardExpirationMonth=@CardExpirationMonth,
		CardExpirationYear=@CardExpirationYear,
		PaymentMethodID=@PaymentMethodID,
		PaymentMethodName=@PaymentMethodName,
		AuthorizationTransactionID=@AuthorizationTransactionID,
		AuthorizationTransactionCode=@AuthorizationTransactionCode,
		AuthorizationTransactionResult=@AuthorizationTransactionResult,
		CaptureTransactionID=@CaptureTransactionID,
		CaptureTransactionResult=@CaptureTransactionResult,
		SubscriptionTransactionID=@SubscriptionTransactionID,
		PurchaseOrderNumber=@PurchaseOrderNumber,
		PaymentStatusID=@PaymentStatusID,
		PaidDate=@PaidDate,
		BillingFirstName=@BillingFirstName,
		BillingLastName=@BillingLastName,
		BillingPhoneNumber=@BillingPhoneNumber,
		BillingEmail=@BillingEmail,
		BillingFaxNumber=@BillingFaxNumber,
		BillingCompany=@BillingCompany,
		BillingAddress1=@BillingAddress1,
		BillingAddress2=@BillingAddress2,
		BillingCity=@BillingCity,
		BillingStateProvince=@BillingStateProvince,
		BillingStateProvinceID=@BillingStateProvinceID,
		BillingZipPostalCode=@BillingZipPostalCode,
		BillingCountry=@BillingCountry,
		BillingCountryID=@BillingCountryID,
		ShippingStatusID=@ShippingStatusID,
		ShippingFirstName=@ShippingFirstName,
		ShippingLastName=@ShippingLastName,
		ShippingPhoneNumber=@ShippingPhoneNumber,
		ShippingEmail=@ShippingEmail,
		ShippingFaxNumber=@ShippingFaxNumber,
		ShippingCompany=@ShippingCompany,
		ShippingAddress1=@ShippingAddress1,
		ShippingAddress2=@ShippingAddress2,
		ShippingCity=@ShippingCity,
		ShippingStateProvince=@ShippingStateProvince,
		ShippingStateProvinceID=@ShippingStateProvinceID,
		ShippingZipPostalCode=@ShippingZipPostalCode,
		ShippingCountry=@ShippingCountry,
		ShippingCountryID=@ShippingCountryID,
		ShippingMethod=@ShippingMethod,
		ShippingRateComputationMethodID=@ShippingRateComputationMethodID,
		ShippedDate=@ShippedDate,
		TrackingNumber=@TrackingNumber,
		Deleted=@Deleted,
		CreatedOn=@CreatedOn
	WHERE
		OrderID = @OrderID
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_TopicLocalized]') and NAME='MetaTitle')
BEGIN
	ALTER TABLE [dbo].[Nop_TopicLocalized] 
	ADD MetaTitle nvarchar(400) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaTitle] DEFAULT ((''))
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_TopicLocalized]') and NAME='MetaKeywords')
BEGIN
	ALTER TABLE [dbo].[Nop_TopicLocalized] 
	ADD MetaKeywords nvarchar(400) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaKeywords] DEFAULT ((''))
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_TopicLocalized]') and NAME='MetaDescription')
BEGIN
	ALTER TABLE [dbo].[Nop_TopicLocalized] 
	ADD MetaDescription nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_TopicLocalized_MetaDescription] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TopicLocalizedUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TopicLocalizedUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_TopicLocalizedUpdate]
(
	@TopicLocalizedID int,
	@TopicID int,
	@LanguageID int,	
	@Title nvarchar(200),
	@Body nvarchar(MAX),
	@CreatedOn datetime,
	@UpdatedOn datetime,
	@MetaKeywords nvarchar(400),
	@MetaDescription nvarchar(4000),
	@MetaTitle nvarchar(400)
)
AS
BEGIN

	UPDATE [Nop_TopicLocalized]
	SET
		TopicID=@TopicID,
		LanguageID=@LanguageID,
		[Title]=@Title,
		Body=@Body,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn,
		MetaKeywords=@MetaKeywords,
		MetaDescription=@MetaDescription,
		MetaTitle=@MetaTitle 
	WHERE
		TopicLocalizedID = @TopicLocalizedID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TopicLocalizedInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TopicLocalizedInsert]
GO
CREATE PROCEDURE [dbo].[Nop_TopicLocalizedInsert]
(
	@TopicLocalizedID int = NULL output,
	@TopicID int,
	@LanguageID int,
	@Title nvarchar(200),
	@Body nvarchar(MAX),
	@CreatedOn datetime,
	@UpdatedOn datetime,
	@MetaKeywords nvarchar(400),
	@MetaDescription nvarchar(4000),
	@MetaTitle nvarchar(400)
)
AS
BEGIN
	INSERT
	INTO [Nop_TopicLocalized]
	(
		TopicID,
		LanguageID,
		[Title],
		Body,
		CreatedOn,
		UpdatedOn,
		MetaKeywords,
		MetaDescription,
		MetaTitle
	)
	VALUES
	(
		@TopicID,
		@LanguageID,
		@Title,
		@Body,
		@CreatedOn,
		@UpdatedOn,
		@MetaKeywords,
		@MetaDescription,
		@MetaTitle
	)

	set @TopicLocalizedID=SCOPE_IDENTITY()
END
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogType]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_ActivityLogType](
	[ActivityLogTypeID] [int] IDENTITY(1,1) NOT NULL,
	[SystemKeyword] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Enabled] [bit] NOT NULL,
CONSTRAINT [PK_Nop_ActivityLogType] PRIMARY KEY CLUSTERED 
(
	[ActivityLogTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLog]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_ActivityLog](
	[ActivityLogID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityLogTypeID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Comment] [nvarchar](4000) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ActivityLog] PRIMARY KEY CLUSTERED 
(
	[ActivityLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ActivityLog_Nop_ActivityLogType'
           AND parent_obj = Object_id('Nop_ActivityLog')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ActivityLog
DROP CONSTRAINT FK_Nop_ActivityLog_Nop_ActivityLogType
GO
ALTER TABLE [dbo].[Nop_ActivityLog] WITH CHECK ADD CONSTRAINT [FK_Nop_ActivityLog_Nop_ActivityLogType] FOREIGN KEY([ActivityLogTypeID])
REFERENCES [dbo].[Nop_ActivityLogType] ([ActivityLogTypeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ActivityLog] CHECK CONSTRAINT [FK_Nop_ActivityLog_Nop_ActivityLogType]
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ActivityLog_Nop_Customer'
           AND parent_obj = Object_id('Nop_ActivityLog')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ActivityLog
DROP CONSTRAINT FK_Nop_ActivityLog_Nop_Customer
GO
ALTER TABLE [dbo].[Nop_ActivityLog]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ActivityLog_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nop_ActivityLog] CHECK CONSTRAINT [FK_Nop_ActivityLog_Nop_Customer]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogTypeInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogTypeInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogTypeInsert]
(
	@ActivityLogTypeID int = NULL output,
	@SystemKeyword nvarchar(50),
    @Name nvarchar(100),
    @Enabled bit
)
AS
BEGIN
	INSERT
	INTO [Nop_ActivityLogType]
	(
		[SystemKeyword],
		[Name],
		[Enabled]
	)
	VALUES
	(
		@SystemKeyword,
		@Name,
		@Enabled
	)

	set @ActivityLogTypeID=SCOPE_IDENTITY()
END
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogTypeUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogTypeUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogTypeUpdate]
(
	@ActivityLogTypeID int,
	@SystemKeyword nvarchar(50),
    @Name nvarchar(100),
    @Enabled bit
)
AS
BEGIN
	UPDATE [Nop_ActivityLogType]
	SET
			[SystemKeyword] = @SystemKeyword,
			[Name] = @Name,
			[Enabled] = @Enabled
	WHERE
		ActivityLogTypeID = @ActivityLogTypeID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogTypeDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogTypeDelete]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogTypeDelete]
(
	@ActivityLogTypeID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_ActivityLogType]
	WHERE
		ActivityLogTypeID = @ActivityLogTypeID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogTypeLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogTypeLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogTypeLoadAll]
AS
BEGIN

	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ActivityLogType]
	ORDER BY [Name]
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogTypeLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogTypeLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogTypeLoadByPrimaryKey]
(
	@ActivityLogTypeID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ActivityLogType]
	WHERE
		ActivityLogTypeID = @ActivityLogTypeID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogInsert]
(
	@ActivityLogID int = NULL output,
	@ActivityLogTypeID int,
    @CustomerID int,
    @Comment nvarchar(4000),
    @CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_ActivityLog]
	(
		[ActivityLogTypeID],
		[CustomerID],
		[Comment],
		[CreatedOn]
	)
	VALUES
	(
		@ActivityLogTypeID,
		@CustomerID,
		@Comment,
		@CreatedOn
	)

	set @ActivityLogID=SCOPE_IDENTITY()
END
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogUpdate]
(
	@ActivityLogID int,
	@ActivityLogTypeID int,
    @CustomerID int,
    @Comment nvarchar(4000),
    @CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ActivityLog]
	SET
			[ActivityLogTypeID] = @ActivityLogTypeID,
			[CustomerID] = @CustomerID,
			[Comment] = @Comment,
			[CreatedOn] = @CreatedOn
	WHERE
		ActivityLogID = @ActivityLogID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogDelete]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogDelete]
(
	@ActivityLogID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_ActivityLog]
	WHERE
		ActivityLogID = @ActivityLogID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogLoadAll]
(
	@CreatedOnFrom datetime = NULL,
	@CreatedOnTo datetime = NULL,
	@Email nvarchar(200),
	@Username nvarchar(200),
	@ActivityLogTypeID int,
	@PageIndex int = 0, 
	@PageSize int = 2147483644,
	@TotalRecords int = null OUTPUT
)
AS
BEGIN
	SET @Email = isnull(@Email, '')
	SET @Email = '%' + rtrim(ltrim(@Email)) + '%'

	SET @Username = isnull(@Username, '')
	SET @Username = '%' + rtrim(ltrim(@Username)) + '%'


	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ActivityLogID int NOT NULL,
		CreatedOn datetime NOT NULL
	)

	INSERT INTO #PageIndex (ActivityLogID, CreatedOn)
	SELECT DISTINCT
		al.ActivityLogID,
		al.CreatedOn
	FROM [Nop_ActivityLog] al with (NOLOCK)
	INNER JOIN [Nop_Customer] c on c.CustomerID = al.CustomerID
	WHERE 
		(@CreatedOnFrom is NULL or DATEDIFF(day, @CreatedOnFrom, al.CreatedOn) >= 0) and
		(@CreatedOnTo is NULL or DATEDIFF(day, @CreatedOnTo, al.CreatedOn) <= 0) and 
		(patindex(@Email, isnull(c.Email, '')) > 0) and
		(patindex(@Username, isnull(c.Username, '')) > 0) and
		(c.IsGuest=0) and (c.deleted=0) and
		(@ActivityLogTypeID is null or (al.ActivityLogTypeID=@ActivityLogTypeID)) 
	ORDER BY al.CreatedOn DESC

	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT
		al.*
	FROM
		#PageIndex [pi]
		INNER JOIN [Nop_ActivityLog] al on al.ActivityLogID = [pi].ActivityLogID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex	
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogLoadByPrimaryKey]
(
	@ActivityLogID int
)
AS
BEGIN

	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ActivityLog]
	WHERE
		ActivityLogID = @ActivityLogID
END
GO

-- Add HasUserAgreement column to Nop_ProductVariant
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='HasUserAgreement')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD HasUserAgreement bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_HasUserAgreement] DEFAULT ((0))
END
GO

-- Add UserAgreementText column to Nop_ProductVariant
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='UserAgreementText')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD UserAgreementText nvarchar(MAX) NOT NULL CONSTRAINT [DF_Nop_ProductVariant_UserAgreementText] DEFAULT ((''))
END
GO

-- Create stored procedure Nop_ProductVariantInsert
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
		HasUserAgreement,
		UserAgreementText,
		IsRecurring,
		CycleLength,
		CyclePeriod,
		TotalCycles,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
		@HasUserAgreement,
		@UserAgreementText,
		@IsRecurring,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO

-- Create stored procedure Nop_ProductVariantUpdate
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		HasUserAgreement=@HasUserAgreement,
		UserAgreementText=@UserAgreementText,
		IsRecurring=@IsRecurring,
		CycleLength=@CycleLength,
		CyclePeriod=@CyclePeriod,
		TotalCycles=@TotalCycles,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO

-- Create stored procedure Nop_LocaleStringResourceLoadAllAsXML
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceLoadAllAsXML]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceLoadAllAsXML]
GO
CREATE PROCEDURE [dbo].[Nop_LocaleStringResourceLoadAllAsXML]
(
	@LanguageID int
)
AS
BEGIN
	SET NOCOUNT ON
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
	)
	FROM 
		Nop_Language l
	WHERE
		LanguageID = @LanguageID
	FOR 
		XML PATH('Language')
END
GO

-- Add CustomerIP column to Nop_Order table
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='CustomerIP')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD CustomerIP nvarchar(50) NOT NULL CONSTRAINT [DF_Nop_Order_CustomerIP] DEFAULT ((''))
END
GO

--Add Nop_OrderInsert stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@CustomerIP nvarchar(50),
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		CustomerIP,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		SubscriptionTransactionID,
		PurchaseOrderNumber,
		PaymentStatusID,
		PaidDate,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		TrackingNumber,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@CustomerIP,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@SubscriptionTransactionID,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@PaidDate,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@TrackingNumber,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO 

--Add Nop_OrderUpdate stored procedure
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderUpdate]
(
	@OrderID int,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@CustomerIP nvarchar(50),
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_Order]
	SET
		OrderGUID=@OrderGUID,
		CustomerID=@CustomerID,
		CustomerLanguageID=@CustomerLanguageID,		
		CustomerTaxDisplayTypeID=@CustomerTaxDisplayTypeID,
		CustomerIP=@CustomerIP,
		OrderSubtotalInclTax=@OrderSubtotalInclTax,
		OrderSubtotalExclTax=@OrderSubtotalExclTax,		
		OrderShippingInclTax=@OrderShippingInclTax,
		OrderShippingExclTax=@OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax=@PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax=@PaymentMethodAdditionalFeeExclTax,
		OrderTax=@OrderTax,
		OrderTotal=@OrderTotal,
		OrderDiscount=@OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency=@OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency=@OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency=@OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency=@OrderShippingExclTaxInCustomerCurrency,	
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,	
		OrderTaxInCustomerCurrency=@OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency=@OrderTotalInCustomerCurrency,
		CustomerCurrencyCode=@CustomerCurrencyCode,
		OrderWeight=@OrderWeight,
		AffiliateID=@AffiliateID,
		OrderStatusID=@OrderStatusID,
		AllowStoringCreditCardNumber=@AllowStoringCreditCardNumber,
		CardType=@CardType,
		CardName=@CardName,
		CardNumber=@CardNumber,
		MaskedCreditCardNumber=@MaskedCreditCardNumber,
		CardCVV2=@CardCVV2,
		CardExpirationMonth=@CardExpirationMonth,
		CardExpirationYear=@CardExpirationYear,
		PaymentMethodID=@PaymentMethodID,
		PaymentMethodName=@PaymentMethodName,
		AuthorizationTransactionID=@AuthorizationTransactionID,
		AuthorizationTransactionCode=@AuthorizationTransactionCode,
		AuthorizationTransactionResult=@AuthorizationTransactionResult,
		CaptureTransactionID=@CaptureTransactionID,
		CaptureTransactionResult=@CaptureTransactionResult,
		SubscriptionTransactionID=@SubscriptionTransactionID,
		PurchaseOrderNumber=@PurchaseOrderNumber,
		PaymentStatusID=@PaymentStatusID,
		PaidDate=@PaidDate,
		BillingFirstName=@BillingFirstName,
		BillingLastName=@BillingLastName,
		BillingPhoneNumber=@BillingPhoneNumber,
		BillingEmail=@BillingEmail,
		BillingFaxNumber=@BillingFaxNumber,
		BillingCompany=@BillingCompany,
		BillingAddress1=@BillingAddress1,
		BillingAddress2=@BillingAddress2,
		BillingCity=@BillingCity,
		BillingStateProvince=@BillingStateProvince,
		BillingStateProvinceID=@BillingStateProvinceID,
		BillingZipPostalCode=@BillingZipPostalCode,
		BillingCountry=@BillingCountry,
		BillingCountryID=@BillingCountryID,
		ShippingStatusID=@ShippingStatusID,
		ShippingFirstName=@ShippingFirstName,
		ShippingLastName=@ShippingLastName,
		ShippingPhoneNumber=@ShippingPhoneNumber,
		ShippingEmail=@ShippingEmail,
		ShippingFaxNumber=@ShippingFaxNumber,
		ShippingCompany=@ShippingCompany,
		ShippingAddress1=@ShippingAddress1,
		ShippingAddress2=@ShippingAddress2,
		ShippingCity=@ShippingCity,
		ShippingStateProvince=@ShippingStateProvince,
		ShippingStateProvinceID=@ShippingStateProvinceID,
		ShippingZipPostalCode=@ShippingZipPostalCode,
		ShippingCountry=@ShippingCountry,
		ShippingCountryID=@ShippingCountryID,
		ShippingMethod=@ShippingMethod,
		ShippingRateComputationMethodID=@ShippingRateComputationMethodID,
		ShippedDate=@ShippedDate,
		TrackingNumber=@TrackingNumber,
		Deleted=@Deleted,
		CreatedOn=@CreatedOn
	WHERE
		OrderID = @OrderID
END
GO 




--added Display stock availability column to Nop_ProductVariant table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='DisplayStockAvailability')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD DisplayStockAvailability bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DisplayStockAvailability] DEFAULT ((1))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
	@DisplayStockAvailability bit,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
		HasUserAgreement,
		UserAgreementText,
		IsRecurring,
		CycleLength,
		CyclePeriod,
		TotalCycles,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
		DisplayStockAvailability,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
		@HasUserAgreement,
		@UserAgreementText,
		@IsRecurring,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
		@DisplayStockAvailability,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@DisplayStockAvailability bit,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		HasUserAgreement=@HasUserAgreement,
		UserAgreementText=@UserAgreementText,
		IsRecurring=@IsRecurring,
		CycleLength=@CycleLength,
		CyclePeriod=@CyclePeriod,
		TotalCycles=@TotalCycles,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		DisplayStockAvailability=@DisplayStockAvailability,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO

-- Create stored procedure Nop_LocaleStringResourceLoadAllAsXML
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceLoadAllAsXML]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceLoadAllAsXML]
GO
CREATE PROCEDURE [dbo].[Nop_LocaleStringResourceLoadAllAsXML]
(
	@LanguageID int,
	@XmlPackage xml output
)
AS
BEGIN
	SET NOCOUNT ON
	SET @XmlPackage = (SELECT l.Name as '@Name',
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
	)
	FROM 
		Nop_Language l
	WHERE
		LanguageID = @LanguageID
	FOR 
		XML PATH('Language'))
END
GO

-- Create stored procedure Nop_LocaleStringResourceInsertAllFromXML
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceInsertAllFromXML]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceInsertAllFromXML]
GO
CREATE PROCEDURE [dbo].[Nop_LocaleStringResourceInsertAllFromXML]
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
	END
	
END
GO

--typo
UPDATE [Nop_MeasureDimension] 
SET 
	[Name]='feet',
	[SystemKeyword]='feet'
WHERE SystemKeyword='feets'



--Gift cards
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='IsGiftCard')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD IsGiftCard bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_IsGiftCard] DEFAULT ((0))
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
	@IsGiftCard bit,
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory bit,
    @StockQuantity int,
	@DisplayStockAvailability bit,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
		IsGiftCard,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
		HasUserAgreement,
		UserAgreementText,
		IsRecurring,
		CycleLength,
		CyclePeriod,
		TotalCycles,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
		DisplayStockAvailability,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
		@IsGiftCard,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
		@HasUserAgreement,
		@UserAgreementText,
		@IsRecurring,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
		@DisplayStockAvailability,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsGiftCard bit,
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory bit,
	@StockQuantity int,
	@DisplayStockAvailability bit,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,		
		IsGiftCard=@IsGiftCard,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		HasUserAgreement=@HasUserAgreement,
		UserAgreementText=@UserAgreementText,
		IsRecurring=@IsRecurring,
		CycleLength=@CycleLength,
		CyclePeriod=@CyclePeriod,
		TotalCycles=@TotalCycles,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		DisplayStockAvailability=@DisplayStockAvailability,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO

if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_GiftCard]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_GiftCard](
	[GiftCardID] [int] IDENTITY(1,1) NOT NULL,
	[PurchasedOrderProductVariantID] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[IsGiftCardActivated] bit NOT NULL,
	[GiftCardCouponCode] nvarchar(100) NOT NULL,
	[RecipientName] nvarchar(100) NOT NULL,
	[RecipientEmail] nvarchar(100) NOT NULL,
	[SenderName] nvarchar(100) NOT NULL,
	[SenderEmail] nvarchar(100) NOT NULL,
	[Message] nvarchar(4000) NOT NULL,
	[IsSenderNotified] bit NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_GiftCard_PK] PRIMARY KEY CLUSTERED 
(
	[GiftCardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_GiftCard_Nop_OrderProductVariant'
           AND parent_obj = Object_id('Nop_GiftCard')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_GiftCard
DROP CONSTRAINT FK_Nop_GiftCard_Nop_OrderProductVariant
GO
ALTER TABLE [dbo].[Nop_GiftCard]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCard_Nop_OrderProductVariant] FOREIGN KEY([PurchasedOrderProductVariantID])
REFERENCES [dbo].[Nop_OrderProductVariant] ([OrderProductVariantID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardDelete]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardDelete]
(
	@GiftCardID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_GiftCard]
	WHERE
		GiftCardID = @GiftCardID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardInsert]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardInsert]
(
	@GiftCardID int = NULL output,
	@PurchasedOrderProductVariantID int,
	@Amount money,
	@IsGiftCardActivated bit,
	@GiftCardCouponCode nvarchar(100),
	@RecipientName nvarchar(100),
	@RecipientEmail nvarchar(100),
	@SenderName nvarchar(100),
	@SenderEmail nvarchar(100),
	@Message nvarchar(4000),
	@IsSenderNotified bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_GiftCard]
	(
		[PurchasedOrderProductVariantID],
		[Amount],
		[IsGiftCardActivated],
		[GiftCardCouponCode],
		[RecipientName],
		[RecipientEmail],
		[SenderName],
		[SenderEmail],
		[Message],
		[IsSenderNotified],
		[CreatedOn]
	)
	VALUES
	(
		@PurchasedOrderProductVariantID,
		@Amount,
		@IsGiftCardActivated,
		@GiftCardCouponCode,
		@RecipientName,
		@RecipientEmail,
		@SenderName,
		@SenderEmail,
		@Message,
		@IsSenderNotified,
		@CreatedOn
	)

	set @GiftCardID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardLoadAll]
(
	@OrderID int,
	@CustomerID int,
	@StartTime datetime = NULL,
	@EndTime datetime = NULL,
	@OrderStatusID int,
	@PaymentStatusID int,
	@ShippingStatusID int,
	@IsGiftCardActivated bit = null, --0 not activated records, 1 activated records, null - load all records
	@GiftCardCouponCode nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_GiftCard]
	WHERE GiftCardID IN
	(
		SELECT DISTINCT gc.GiftCardID
		FROM [Nop_GiftCard] gc
		INNER JOIN [Nop_OrderProductVariant] opv ON gc.PurchasedOrderProductVariantID=opv.OrderProductVariantID
		INNER JOIN [Nop_Order] o ON opv.OrderID=o.OrderID
		WHERE
			(@OrderID IS NULL OR @OrderID=0 or o.OrderID = @OrderID) and
			(@CustomerID IS NULL OR @CustomerID=0 or o.CustomerID = @CustomerID) and
			(@StartTime is NULL or DATEDIFF(day, @StartTime, gc.CreatedOn) >= 0) and
			(@EndTime is NULL or DATEDIFF(day, @EndTime, gc.CreatedOn) <= 0) and 
			(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) and
			(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) and
			(@ShippingStatusID IS NULL OR @ShippingStatusID = 0 OR o.ShippingStatusID = @ShippingStatusID) and
			(@IsGiftCardActivated IS NULL OR gc.IsGiftCardActivated = @IsGiftCardActivated) and
			(@GiftCardCouponCode IS NULL OR @GiftCardCouponCode ='' OR gc.GiftCardCouponCode = @GiftCardCouponCode)		
	)
	ORDER BY CreatedOn desc, GiftCardID 
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardLoadByPrimaryKey]
(
	@GiftCardID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_GiftCard]
	WHERE
		GiftCardID = @GiftCardID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUpdate]
(
	@GiftCardID int,
	@PurchasedOrderProductVariantID int,
	@Amount money,
	@IsGiftCardActivated bit,
	@GiftCardCouponCode nvarchar(100),
	@RecipientName nvarchar(100),
	@RecipientEmail nvarchar(100),
	@SenderName nvarchar(100),
	@SenderEmail nvarchar(100),
	@Message nvarchar(4000),
	@IsSenderNotified bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_GiftCard]
	SET
		[PurchasedOrderProductVariantID] = @PurchasedOrderProductVariantID,
		[Amount] = @Amount,
		[IsGiftCardActivated] = @IsGiftCardActivated,
		[GiftCardCouponCode]= @GiftCardCouponCode,
		[RecipientName]=@RecipientName,
		[RecipientEmail]=@RecipientEmail,
		[SenderName]=@SenderName,
		[SenderEmail]=@SenderEmail,
		[Message]=@Message,
		[IsSenderNotified]=@IsSenderNotified,
		[CreatedOn] = @CreatedOn
	WHERE
		GiftCardID=@GiftCardID
END
GO

if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_GiftCardUsageHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_GiftCardUsageHistory](
	[GiftCardUsageHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[GiftCardID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[UsedValue] [money] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [Nop_GiftCardUsageHistory_PK] PRIMARY KEY CLUSTERED 
(
	[GiftCardUsageHistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_GiftCardUsageHistory_Nop_GiftCard'
           AND parent_obj = Object_id('Nop_GiftCardUsageHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_GiftCardUsageHistory
DROP CONSTRAINT FK_Nop_GiftCardUsageHistory_Nop_GiftCard
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_GiftCard] FOREIGN KEY([GiftCardID])
REFERENCES [dbo].[Nop_GiftCard] ([GiftCardID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_GiftCardUsageHistory_Nop_Customer'
           AND parent_obj = Object_id('Nop_GiftCardUsageHistory')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_GiftCardUsageHistory
DROP CONSTRAINT FK_Nop_GiftCardUsageHistory_Nop_Customer
GO
ALTER TABLE [dbo].[Nop_GiftCardUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_Nop_GiftCardUsageHistory_Nop_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Nop_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryDelete]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryDelete]
(
	@GiftCardUsageHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_GiftCardUsageHistory]
	WHERE
		GiftCardUsageHistoryID = @GiftCardUsageHistoryID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryInsert]
(
	@GiftCardUsageHistoryID int = NULL output,
	@GiftCardID int,
	@CustomerID int,
	@OrderID int,
	@UsedValue money,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_GiftCardUsageHistory]
	(
		[GiftCardID],
		[CustomerID],
		[OrderID],
		[UsedValue],
		[CreatedOn]
	)
	VALUES
	(
		@GiftCardID,
		@CustomerID,
		@OrderID,
		@UsedValue,
		@CreatedOn
	)

	set @GiftCardUsageHistoryID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadAll]
(
	@GiftCardID int,
	@CustomerID int,
	@OrderID int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT * FROM [Nop_GiftCardUsageHistory]
	WHERE GiftCardUsageHistoryID IN 
		(
		SELECT DISTINCT gcuh.GiftCardUsageHistoryID
		FROM [Nop_GiftCardUsageHistory] gcuh WITH (NOLOCK)
		LEFT OUTER JOIN Nop_GiftCard gc with (NOLOCK) ON gcuh.GiftCardID=gc.GiftCardID
		LEFT OUTER JOIN Nop_OrderProductVariant opv with (NOLOCK) ON gc.PurchasedOrderProductVariantID=opv.OrderProductVariantID
		LEFT OUTER JOIN Nop_Order o with (NOLOCK) ON gcuh.OrderID=o.OrderID
		WHERE
				(
					o.Deleted=0
				)
				AND
				(
					@GiftCardID IS NULL OR @GiftCardID=0
					OR (gcuh.GiftCardID=@GiftCardID)
				)
				AND
				(
					@CustomerID IS NULL OR @CustomerID=0
					OR (gcuh.CustomerID=@CustomerID)
				)
				AND
				(
					@OrderID IS NULL OR @OrderID=0
					OR (gcuh.OrderID=@OrderID)
				)
		)
	ORDER BY CreatedOn, GiftCardUsageHistoryID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadByPrimaryKey]
(
	@GiftCardUsageHistoryID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_GiftCardUsageHistory]
	WHERE
		GiftCardUsageHistoryID = @GiftCardUsageHistoryID
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryUpdate]
(
	@GiftCardUsageHistoryID int,
	@GiftCardID int,
	@CustomerID int,
	@OrderID int,
	@UsedValue money,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_GiftCardUsageHistory]
	SET
		[GiftCardID]=@GiftCardID,
		[CustomerID] = @CustomerID,
		[OrderID] = @OrderID,
		[UsedValue]= @UsedValue,
		[CreatedOn] = @CreatedOn
	WHERE
		GiftCardUsageHistoryID=@GiftCardUsageHistoryID
END
GO

-- drop Nop_LocaleStringResourceInsertAllFromXML and create Nop_LanguagePackImport
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceInsertAllFromXML]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceInsertAllFromXML]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LanguagePackImport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LanguagePackImport]
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

-- drop Nop_LocaleStringResourceLoadAllAsXML and create Nop_LanguagePackExport
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LocaleStringResourceLoadAllAsXML]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LocaleStringResourceLoadAllAsXML]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_LanguagePackExport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LanguagePackExport]
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


--gift cards
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Customer]') and NAME='GiftCardCouponCodes')
BEGIN
	ALTER TABLE [dbo].[Nop_Customer] 
	ADD GiftCardCouponCodes xml NOT NULL CONSTRAINT [DFNop_Customer_GiftCardCouponCodes] DEFAULT ((''))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerInsert]
(
	@CustomerId int = NULL output,
	@CustomerGUID uniqueidentifier,
	@Email nvarchar(255),
	@PasswordHash nvarchar(255),
	@SaltKey nvarchar(255),
	@AffiliateID int,
	@BillingAddressID int,
	@ShippingAddressID int,
	@LastPaymentMethodID int,
	@LastAppliedCouponCode nvarchar(100),
	@GiftCardCouponCodes xml,
	@LanguageID int,
	@CurrencyID int,
	@TaxDisplayTypeID int,
	@IsTaxExempt bit,
	@IsAdmin bit,
	@IsGuest bit,
	@IsForumModerator bit,
	@TotalForumPosts int,
	@Signature nvarchar(300),
	@AdminComment nvarchar(4000),
	@Active bit,
	@Deleted bit,
	@RegistrationDate datetime,
	@TimeZoneID nvarchar(200),
	@Username nvarchar(100),
	@AvatarID int
)
AS
BEGIN
	INSERT
	INTO [Nop_Customer]
	(
		CustomerGUID,
		Email,
		PasswordHash,
		SaltKey,
		AffiliateID,
		BillingAddressID,
		ShippingAddressID,
		LastPaymentMethodID,
		LastAppliedCouponCode,
		GiftCardCouponCodes,
		LanguageID,
		CurrencyID,
		TaxDisplayTypeID,
		IsTaxExempt,
		IsAdmin,
		IsGuest,
		IsForumModerator,
		TotalForumPosts,
		[Signature],
		AdminComment,
		Active,
		Deleted,
		RegistrationDate,
		TimeZoneID,
		Username,
		AvatarID
	)
	VALUES
	(
		@CustomerGUID,
		@Email,
		@PasswordHash,
		@SaltKey,
		@AffiliateID,
		@BillingAddressID,
		@ShippingAddressID,
		@LastPaymentMethodID,
		@LastAppliedCouponCode,
		@GiftCardCouponCodes,
		@LanguageID,
		@CurrencyID,
		@TaxDisplayTypeID,
		@IsTaxExempt,
		@IsAdmin,
		@IsGuest,
		@IsForumModerator,
		@TotalForumPosts,
		@Signature,
		@AdminComment,
		@Active,
		@Deleted,
		@RegistrationDate,
		@TimeZoneID,
		@Username,
		@AvatarID
	)

	set @CustomerId=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerUpdate]
(
	@CustomerId int,
	@CustomerGUID uniqueidentifier,
	@Email nvarchar(255),
	@PasswordHash nvarchar(255),
	@SaltKey nvarchar(255),
	@AffiliateID int,
	@BillingAddressID int,
	@ShippingAddressID int,
	@LastPaymentMethodID int,
	@LastAppliedCouponCode nvarchar(100),
	@GiftCardCouponCodes xml,
	@LanguageID int,
	@CurrencyID int,
	@TaxDisplayTypeID int,
	@IsTaxExempt bit,
	@IsAdmin bit,
	@IsGuest bit,
	@IsForumModerator bit,
	@TotalForumPosts int,
	@Signature nvarchar(300),
	@AdminComment nvarchar(4000),
	@Active bit,
	@Deleted bit,
	@RegistrationDate datetime,
	@TimeZoneID nvarchar(200),
	@Username nvarchar(100),
	@AvatarID int
)
AS
BEGIN

	UPDATE [Nop_Customer]
	SET
		CustomerGUID=@CustomerGUID,
		Email=@Email,
		PasswordHash=@PasswordHash,
		SaltKey=@SaltKey,
		AffiliateID=@AffiliateID,
		BillingAddressID=@BillingAddressID,
		ShippingAddressID=@ShippingAddressID,
		LastPaymentMethodID=@LastPaymentMethodID,
		LastAppliedCouponCode=@LastAppliedCouponCode,
		GiftCardCouponCodes=@GiftCardCouponCodes,
		LanguageID=@LanguageID,
		CurrencyID=@CurrencyID,
		TaxDisplayTypeID=@TaxDisplayTypeID,
		IsTaxExempt=@IsTaxExempt,
		IsAdmin=@IsAdmin,
		IsGuest=@IsGuest,
		IsForumModerator=@IsForumModerator,
		TotalForumPosts=@TotalForumPosts,
		[Signature]=@Signature,
		AdminComment=@AdminComment,
		Active=@Active,
		Deleted=@Deleted,
		RegistrationDate=@RegistrationDate,
		TimeZoneID=@TimeZoneID,
		Username=@Username,
		AvatarID=@AvatarID
	WHERE
		[CustomerId] = @CustomerId

END
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[Nop_MessageTemplate] WHERE [Name] = N'GiftCard.Notification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'GiftCard.Notification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = 'GiftCard.Notification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'%GiftCard.SenderName% has sent you a gift card for %Store.Name%', N'<p>You have received a gift card for %Store.Name%</p><p>Dear %GiftCard.RecipientName%, <br /><br />%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href="%Store.URL%"> %Store.Name%</a></p><p>You gift card code is %GiftCard.CouponCode%</p><p>%GiftCard.Message%</p>')
	END
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogClearAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogClearAll]
GO
CREATE PROCEDURE [dbo].[Nop_ActivityLogClearAll]
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_ActivityLog]
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SearchLogClear]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SearchLogClear]
GO
CREATE PROCEDURE [dbo].[Nop_SearchLogClear]
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_SearchLog]
END
GO




-- Pay In Store payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.PayInStore.PayInStorePaymentProcessor, Nop.Payment.PayInStore')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Pay In Store', N'Pay In Store', N'', N'Payment\PayInStore\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\PayInStore\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.PayInStore.PayInStorePaymentProcessor, Nop.Payment.PayInStore', N'PAYINSTORE', 0, 120)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PayInStore.Info')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PayInStore.Info', N'<p>Reserve items at your local store, and pay in store when you pick up your order.<br />  Our store location: USA, New York,...</p>  <p>P.S. You can edit this text from admin panel.</p>', N'')
END
GO

--order discounts
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderDiscountInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD OrderDiscountInCustomerCurrency money NOT NULL CONSTRAINT [DF_Nop_Order_OrderDiscountInCustomerCurrency] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderInsert]
(
	@OrderID int = NULL output,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@CustomerIP nvarchar(50),
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@OrderDiscountInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Order]
	(
		OrderGUID,
		CustomerID,
		CustomerLanguageID,
		CustomerTaxDisplayTypeID,
		CustomerIP,
		OrderSubtotalInclTax,
		OrderSubtotalExclTax,
		OrderShippingInclTax,
		OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax,
		OrderTax,
		OrderTotal,
		OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency,
		OrderDiscountInCustomerCurrency,
		CustomerCurrencyCode,
		OrderWeight,
		AffiliateID,
		OrderStatusID,
		AllowStoringCreditCardNumber,
		CardType,
		CardName,
		CardNumber,
		MaskedCreditCardNumber,
		CardCVV2,
		CardExpirationMonth,
		CardExpirationYear,
		PaymentMethodID,
		PaymentMethodName,
		AuthorizationTransactionID,
		AuthorizationTransactionCode,
		AuthorizationTransactionResult,
		CaptureTransactionID,
		CaptureTransactionResult,
		SubscriptionTransactionID,
		PurchaseOrderNumber,
		PaymentStatusID,
		PaidDate,
		BillingFirstName,
		BillingLastName,
		BillingPhoneNumber,
		BillingEmail,
		BillingFaxNumber,
		BillingCompany,
		BillingAddress1,
		BillingAddress2,
		BillingCity,
		BillingStateProvince,
		BillingStateProvinceID,
		BillingZipPostalCode,
		BillingCountry,
		BillingCountryID,
		ShippingStatusID,
		ShippingFirstName,
		ShippingLastName,
		ShippingPhoneNumber,
		ShippingEmail,
		ShippingFaxNumber,
		ShippingCompany,
		ShippingAddress1,
		ShippingAddress2,
		ShippingCity,
		ShippingStateProvince,
		ShippingZipPostalCode,
		ShippingStateProvinceID,
		ShippingCountry,
		ShippingCountryID,
		ShippingMethod,
		ShippingRateComputationMethodID,
		ShippedDate,
		TrackingNumber,
		Deleted,
		CreatedOn
	)
	VALUES
	(
		@OrderGUID,
		@CustomerID,
		@CustomerLanguageID,		
		@CustomerTaxDisplayTypeID,
		@CustomerIP,
		@OrderSubtotalInclTax,
		@OrderSubtotalExclTax,		
		@OrderShippingInclTax,
		@OrderShippingExclTax,
		@PaymentMethodAdditionalFeeInclTax,
		@PaymentMethodAdditionalFeeExclTax,
		@OrderTax,
		@OrderTotal,
		@OrderDiscount,		
		@OrderSubtotalInclTaxInCustomerCurrency,
		@OrderSubtotalExclTaxInCustomerCurrency,		
		@OrderShippingInclTaxInCustomerCurrency,
		@OrderShippingExclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
		@OrderTaxInCustomerCurrency,
		@OrderTotalInCustomerCurrency,
		@OrderDiscountInCustomerCurrency,
		@CustomerCurrencyCode,
		@OrderWeight,
		@AffiliateID,
		@OrderStatusID,
		@AllowStoringCreditCardNumber,
		@CardType,
		@CardName,
		@CardNumber,
		@MaskedCreditCardNumber,
		@CardCVV2,
		@CardExpirationMonth,
		@CardExpirationYear,
		@PaymentMethodID,
		@PaymentMethodName,
		@AuthorizationTransactionID,
		@AuthorizationTransactionCode,
		@AuthorizationTransactionResult,
		@CaptureTransactionID,
		@CaptureTransactionResult,
		@SubscriptionTransactionID,
		@PurchaseOrderNumber,
		@PaymentStatusID,
		@PaidDate,
		@BillingFirstName,
		@BillingLastName,
		@BillingPhoneNumber,
		@BillingEmail,
		@BillingFaxNumber,
		@BillingCompany,
		@BillingAddress1,
		@BillingAddress2,
		@BillingCity,
		@BillingStateProvince,
		@BillingStateProvinceID,
		@BillingZipPostalCode,
		@BillingCountry,
		@BillingCountryID,
		@ShippingStatusID,
		@ShippingFirstName,
		@ShippingLastName,
		@ShippingPhoneNumber,
		@ShippingEmail,
		@ShippingFaxNumber,
		@ShippingCompany,
		@ShippingAddress1,
		@ShippingAddress2,
		@ShippingCity,
		@ShippingStateProvince,
		@ShippingZipPostalCode,
		@ShippingStateProvinceID,
		@ShippingCountry,
		@ShippingCountryID,
		@ShippingMethod,
		@ShippingRateComputationMethodID,
		@ShippedDate,
		@TrackingNumber,
		@Deleted,
		@CreatedOn
	)

	set @OrderID=SCOPE_IDENTITY()
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderUpdate]
(
	@OrderID int,
	@OrderGUID uniqueidentifier,
	@CustomerID int,
	@CustomerLanguageID int,
	@CustomerTaxDisplayTypeID int,
	@CustomerIP nvarchar(50),
	@OrderSubtotalInclTax money,
	@OrderSubtotalExclTax money,
	@OrderShippingInclTax money,
	@OrderShippingExclTax money,
	@PaymentMethodAdditionalFeeInclTax money,
	@PaymentMethodAdditionalFeeExclTax money,
	@OrderTax money,
	@OrderTotal money,
	@OrderDiscount money,
	@OrderSubtotalInclTaxInCustomerCurrency money,
	@OrderSubtotalExclTaxInCustomerCurrency money,
	@OrderShippingInclTaxInCustomerCurrency money,
	@OrderShippingExclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency money,
	@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency money,
	@OrderTaxInCustomerCurrency money,
	@OrderTotalInCustomerCurrency money,
	@OrderDiscountInCustomerCurrency money,
	@CustomerCurrencyCode nvarchar(5),
	@OrderWeight float,
	@AffiliateID int,
	@OrderStatusID int,
	@AllowStoringCreditCardNumber bit,
	@CardType nvarchar(100),
	@CardName nvarchar(100),
	@CardNumber nvarchar(100),
	@MaskedCreditCardNumber nvarchar(100),
	@CardCVV2 nvarchar(100),
	@CardExpirationMonth nvarchar(100),
	@CardExpirationYear nvarchar(100),
	@PaymentMethodID int,
	@PaymentMethodName nvarchar(100),
	@AuthorizationTransactionID nvarchar(4000),
	@AuthorizationTransactionCode nvarchar(4000),
	@AuthorizationTransactionResult nvarchar(1000),
	@CaptureTransactionID nvarchar(4000),
	@CaptureTransactionResult nvarchar(1000),
	@SubscriptionTransactionID nvarchar(4000),
	@PurchaseOrderNumber nvarchar(100),
	@PaymentStatusID int,
	@PaidDate datetime,
	@BillingFirstName nvarchar(100),
	@BillingLastName nvarchar(100),
	@BillingPhoneNumber nvarchar(50),
	@BillingEmail nvarchar(255),
	@BillingFaxNumber nvarchar(50),
	@BillingCompany nvarchar(100),
	@BillingAddress1 nvarchar(100),
	@BillingAddress2 nvarchar(100),
	@BillingCity nvarchar(100),
	@BillingStateProvince nvarchar(100),
	@BillingStateProvinceID int,
	@BillingZipPostalCode nvarchar(10),
	@BillingCountry nvarchar(100),
	@BillingCountryID int,
	@ShippingStatusID int,
	@ShippingFirstName nvarchar(100),
	@ShippingLastName nvarchar(100),
	@ShippingPhoneNumber nvarchar(50),
	@ShippingEmail nvarchar(255),
	@ShippingFaxNumber nvarchar(50),
	@ShippingCompany nvarchar(100),
	@ShippingAddress1 nvarchar(100),
	@ShippingAddress2 nvarchar(100),
	@ShippingCity nvarchar(100),
	@ShippingStateProvince nvarchar(100),
	@ShippingStateProvinceID int,
	@ShippingZipPostalCode nvarchar(10),
	@ShippingCountry nvarchar(100),
	@ShippingCountryID int,
	@ShippingMethod nvarchar(100),
	@ShippingRateComputationMethodID int,
	@ShippedDate datetime,
	@TrackingNumber nvarchar(100),
	@Deleted bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_Order]
	SET
		OrderGUID=@OrderGUID,
		CustomerID=@CustomerID,
		CustomerLanguageID=@CustomerLanguageID,		
		CustomerTaxDisplayTypeID=@CustomerTaxDisplayTypeID,
		CustomerIP=@CustomerIP,
		OrderSubtotalInclTax=@OrderSubtotalInclTax,
		OrderSubtotalExclTax=@OrderSubtotalExclTax,		
		OrderShippingInclTax=@OrderShippingInclTax,
		OrderShippingExclTax=@OrderShippingExclTax,
		PaymentMethodAdditionalFeeInclTax=@PaymentMethodAdditionalFeeInclTax,
		PaymentMethodAdditionalFeeExclTax=@PaymentMethodAdditionalFeeExclTax,
		OrderTax=@OrderTax,
		OrderTotal=@OrderTotal,
		OrderDiscount=@OrderDiscount,
		OrderSubtotalInclTaxInCustomerCurrency=@OrderSubtotalInclTaxInCustomerCurrency,
		OrderSubtotalExclTaxInCustomerCurrency=@OrderSubtotalExclTaxInCustomerCurrency,
		OrderShippingInclTaxInCustomerCurrency=@OrderShippingInclTaxInCustomerCurrency,
		OrderShippingExclTaxInCustomerCurrency=@OrderShippingExclTaxInCustomerCurrency,	
		PaymentMethodAdditionalFeeInclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeInclTaxInCustomerCurrency,
		PaymentMethodAdditionalFeeExclTaxInCustomerCurrency=@PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,	
		OrderTaxInCustomerCurrency=@OrderTaxInCustomerCurrency,
		OrderTotalInCustomerCurrency=@OrderTotalInCustomerCurrency,
		OrderDiscountInCustomerCurrency=@OrderDiscountInCustomerCurrency,
		CustomerCurrencyCode=@CustomerCurrencyCode,
		OrderWeight=@OrderWeight,
		AffiliateID=@AffiliateID,
		OrderStatusID=@OrderStatusID,
		AllowStoringCreditCardNumber=@AllowStoringCreditCardNumber,
		CardType=@CardType,
		CardName=@CardName,
		CardNumber=@CardNumber,
		MaskedCreditCardNumber=@MaskedCreditCardNumber,
		CardCVV2=@CardCVV2,
		CardExpirationMonth=@CardExpirationMonth,
		CardExpirationYear=@CardExpirationYear,
		PaymentMethodID=@PaymentMethodID,
		PaymentMethodName=@PaymentMethodName,
		AuthorizationTransactionID=@AuthorizationTransactionID,
		AuthorizationTransactionCode=@AuthorizationTransactionCode,
		AuthorizationTransactionResult=@AuthorizationTransactionResult,
		CaptureTransactionID=@CaptureTransactionID,
		CaptureTransactionResult=@CaptureTransactionResult,
		SubscriptionTransactionID=@SubscriptionTransactionID,
		PurchaseOrderNumber=@PurchaseOrderNumber,
		PaymentStatusID=@PaymentStatusID,
		PaidDate=@PaidDate,
		BillingFirstName=@BillingFirstName,
		BillingLastName=@BillingLastName,
		BillingPhoneNumber=@BillingPhoneNumber,
		BillingEmail=@BillingEmail,
		BillingFaxNumber=@BillingFaxNumber,
		BillingCompany=@BillingCompany,
		BillingAddress1=@BillingAddress1,
		BillingAddress2=@BillingAddress2,
		BillingCity=@BillingCity,
		BillingStateProvince=@BillingStateProvince,
		BillingStateProvinceID=@BillingStateProvinceID,
		BillingZipPostalCode=@BillingZipPostalCode,
		BillingCountry=@BillingCountry,
		BillingCountryID=@BillingCountryID,
		ShippingStatusID=@ShippingStatusID,
		ShippingFirstName=@ShippingFirstName,
		ShippingLastName=@ShippingLastName,
		ShippingPhoneNumber=@ShippingPhoneNumber,
		ShippingEmail=@ShippingEmail,
		ShippingFaxNumber=@ShippingFaxNumber,
		ShippingCompany=@ShippingCompany,
		ShippingAddress1=@ShippingAddress1,
		ShippingAddress2=@ShippingAddress2,
		ShippingCity=@ShippingCity,
		ShippingStateProvince=@ShippingStateProvince,
		ShippingStateProvinceID=@ShippingStateProvinceID,
		ShippingZipPostalCode=@ShippingZipPostalCode,
		ShippingCountry=@ShippingCountry,
		ShippingCountryID=@ShippingCountryID,
		ShippingMethod=@ShippingMethod,
		ShippingRateComputationMethodID=@ShippingRateComputationMethodID,
		ShippedDate=@ShippedDate,
		TrackingNumber=@TrackingNumber,
		Deleted=@Deleted,
		CreatedOn=@CreatedOn
	WHERE
		OrderID = @OrderID
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_GiftCardUsageHistory]') and NAME='UsedValueInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_GiftCardUsageHistory] 
	ADD UsedValueInCustomerCurrency money NOT NULL CONSTRAINT [DF_Nop_GiftCardUsageHistory_UsedValueInCustomerCurrency] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryInsert]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryInsert]
(
	@GiftCardUsageHistoryID int = NULL output,
	@GiftCardID int,
	@CustomerID int,
	@OrderID int,
	@UsedValue money,
	@UsedValueInCustomerCurrency money,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_GiftCardUsageHistory]
	(
		[GiftCardID],
		[CustomerID],
		[OrderID],
		[UsedValue],
		[UsedValueInCustomerCurrency],
		[CreatedOn]
	)
	VALUES
	(
		@GiftCardID,
		@CustomerID,
		@OrderID,
		@UsedValue,
		@UsedValueInCustomerCurrency,
		@CreatedOn
	)

	set @GiftCardUsageHistoryID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_GiftCardUsageHistoryUpdate]
(
	@GiftCardUsageHistoryID int,
	@GiftCardID int,
	@CustomerID int,
	@OrderID int,
	@UsedValue money,
	@UsedValueInCustomerCurrency money,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_GiftCardUsageHistory]
	SET
		[GiftCardID]=@GiftCardID,
		[CustomerID] = @CustomerID,
		[OrderID] = @OrderID,
		[UsedValue]= @UsedValue,
		[UsedValueInCustomerCurrency] = @UsedValueInCustomerCurrency,
		[CreatedOn] = @CreatedOn
	WHERE
		GiftCardUsageHistoryID=@GiftCardUsageHistoryID
END
GO



--licenses
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_OrderProductVariant]') and NAME='LicenseDownloadID')
BEGIN
	ALTER TABLE [dbo].[Nop_OrderProductVariant] 
	ADD LicenseDownloadID int NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_LicenseDownloadID] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
(
	@OrderProductVariantID int = NULL output,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit,
	@LicenseDownloadID int
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderProductVariant]
	(
		OrderProductVariantGUID,
		OrderID,
		ProductVariantID,
		UnitPriceInclTax,
		UnitPriceExclTax,
		PriceInclTax,
		PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency,
		AttributeDescription,
		Quantity,
		DiscountAmountInclTax,
		DiscountAmountExclTax,
		DownloadCount,
		IsDownloadActivated,
		LicenseDownloadID
	)
	VALUES
	(
		@OrderProductVariantGUID,
		@OrderID,
		@ProductVariantID,
		@UnitPriceInclTax,
		@UnitPriceExclTax,
		@PriceInclTax,
		@PriceExclTax,
		@UnitPriceInclTaxInCustomerCurrency,
		@UnitPriceExclTaxInCustomerCurrency,
		@PriceInclTaxInCustomerCurrency,
		@PriceExclTaxInCustomerCurrency,
		@AttributeDescription,
		@Quantity,
		@DiscountAmountInclTax,
		@DiscountAmountExclTax,
		@DownloadCount,
		@IsDownloadActivated,
		@LicenseDownloadID
	)

	set @OrderProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
(
	@OrderProductVariantID int,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit,
	@LicenseDownloadID int
)
AS
BEGIN

	UPDATE [Nop_OrderProductVariant]
	SET		
		OrderProductVariantGUID=@OrderProductVariantGUID,
		OrderID=@OrderID,
		ProductVariantID=@ProductVariantID,
		UnitPriceInclTax=@UnitPriceInclTax,
		UnitPriceExclTax = @UnitPriceExclTax,
		PriceInclTax=@PriceInclTax,
		PriceExclTax=@PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency=@UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency=@UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency=@PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency=@PriceExclTaxInCustomerCurrency,
		AttributeDescription=@AttributeDescription,
		Quantity=@Quantity,
		DiscountAmountInclTax=@DiscountAmountInclTax,
		DiscountAmountExclTax=@DiscountAmountExclTax,
		DownloadCount=@DownloadCount,
		IsDownloadActivated=@IsDownloadActivated,
		LicenseDownloadID=@LicenseDownloadID
	WHERE
		OrderProductVariantID = @OrderProductVariantID
END
GO

-- setting Sitemap.IncludeCategories
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Sitemap.IncludeCategories')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Sitemap.IncludeCategories', N'True', N'')
END
GO

-- setting Sitemap.IncludeManufacturers
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Sitemap.IncludeManufacturers')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Sitemap.IncludeManufacturers', N'True', N'')
END
GO

-- setting Sitemap.IncludeProducts
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Sitemap.IncludeProducts')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Sitemap.IncludeProducts', N'True', N'')
END
GO

-- setting Sitemap.IncludeTopics
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Sitemap.IncludeTopics')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Sitemap.IncludeTopics', N'True', N'')
END
GO

-- setting Sitemap.OtherPages
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Sitemap.OtherPages')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Sitemap.OtherPages', N'', N'')
END
GO


-- Amazon Simple Pay payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Amazon.SimplePayPaymentProcessor, Nop.Payment.Amazon')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Amazon Simple Pay', N'Amazon Simple Pay', N'', N'Payment\Amazon\SimplePayConfig.ascx', N'~\Templates\Payment\Amazon\SimplePayPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Amazon.SimplePayPaymentProcessor, Nop.Payment.Amazon', N'AMAZON.SIMPLEPAY', 0, 150)
END
GO

-- PayPoint Hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.PayPoint.HostedPaymentProcessor, Nop.Payment.PayPoint')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'PayPoint (hosted)', N'PayPoint', N'', N'Payment\PayPoint\HostedPaymentConfig.ascx', N'~\Templates\Payment\PayPoint\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.PayPoint.HostedPaymentProcessor, Nop.Payment.PayPoint', N'PAYPOINT.HOSTED', 0, 160)
END
GO

--Inventory management for product attributes
IF EXISTS(SELECT 1 from dbo.sysobjects 
			WHERE NAME='DF_Nop_ProductVariant_ManageInventory' 
			and parent_obj=object_id('[dbo].[Nop_ProductVariant]'))
	ALTER TABLE [dbo].[Nop_ProductVariant] DROP CONSTRAINT [DF_Nop_ProductVariant_ManageInventory]

	exec ('ALTER TABLE [dbo].[Nop_ProductVariant] ALTER COLUMN ManageInventory int NOT NULL')
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantInsert]
(
    @ProductVariantID int = NULL output,
    @ProductId int,
    @Name nvarchar(400),
    @SKU nvarchar (400),
    @Description nvarchar(4000),
    @AdminComment nvarchar(4000),
    @ManufacturerPartNumber nvarchar(100),
	@IsGiftCard bit,
    @IsDownload bit,
    @DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
	@AdditionalShippingCharge money,
    @IsTaxExempt bit,
    @TaxCategoryID int,
	@ManageInventory int,
    @StockQuantity int,
	@DisplayStockAvailability bit,
    @MinStockQuantity int,
    @LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
    @WarehouseId int,
    @DisableBuyButton int,
    @Price money,
    @OldPrice money,
	@ProductCost money,
    @Weight float,
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
	@CreatedOn datetime,
    @UpdatedOn datetime
)
AS
BEGIN
    INSERT
    INTO [Nop_ProductVariant]
    (
        ProductId,
        [Name],
        SKU,
        [Description],
        AdminComment,
        ManufacturerPartNumber,
		IsGiftCard,
        IsDownload,
        DownloadID,
		UnlimitedDownloads,
		MaxNumberOfDownloads,
		DownloadExpirationDays,
		DownloadActivationType,
		HasSampleDownload,
		SampleDownloadID,
		HasUserAgreement,
		UserAgreementText,
		IsRecurring,
		CycleLength,
		CyclePeriod,
		TotalCycles,
        IsShipEnabled,
        IsFreeShipping,
		AdditionalShippingCharge,
        IsTaxExempt,
        TaxCategoryID,
		ManageInventory,
		DisplayStockAvailability,
        StockQuantity,
        MinStockQuantity,
        LowStockActivityID,
		NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders,
		OrderMinimumQuantity,
		OrderMaximumQuantity,
        WarehouseId,
        DisableBuyButton,
        Price,
        OldPrice,
		ProductCost,
        Weight,
        [Length],
        Width,
        Height,
        PictureID,
		AvailableStartDateTime,
		AvailableEndDateTime,
        Published,
        Deleted,
        DisplayOrder,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @ProductId,
        @Name,
        @SKU,
        @Description,
        @AdminComment,
        @ManufacturerPartNumber,
		@IsGiftCard,
        @IsDownload,
        @DownloadID,
		@UnlimitedDownloads,
		@MaxNumberOfDownloads,
		@DownloadExpirationDays,
		@DownloadActivationType,
		@HasSampleDownload,
		@SampleDownloadID,
		@HasUserAgreement,
		@UserAgreementText,
		@IsRecurring,
		@CycleLength,
		@CyclePeriod,
		@TotalCycles,
        @IsShipEnabled,
        @IsFreeShipping,
		@AdditionalShippingCharge,
        @IsTaxExempt,
        @TaxCategoryID,
		@ManageInventory,
		@DisplayStockAvailability,
        @StockQuantity,
        @MinStockQuantity,
        @LowStockActivityID,
		@NotifyAdminForQuantityBelow,
		@AllowOutOfStockOrders,
		@OrderMinimumQuantity,
		@OrderMaximumQuantity,
        @WarehouseId,
        @DisableBuyButton,
        @Price,
        @OldPrice,
		@ProductCost,
        @Weight,
        @Length,
        @Width,
        @Height,
        @PictureID,
		@AvailableStartDateTime,
		@AvailableEndDateTime,
        @Published,
        @Deleted,
        @DisplayOrder,
        @CreatedOn,
        @UpdatedOn
    )

    set @ProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantUpdate]
(
	@ProductVariantID int,
	@ProductId int,
	@Name nvarchar(400),
	@SKU nvarchar (400),
	@Description nvarchar(4000),
	@AdminComment nvarchar(4000),
	@ManufacturerPartNumber nvarchar(100),
	@IsGiftCard bit,
	@IsDownload bit,
	@DownloadID int,
	@UnlimitedDownloads bit,
	@MaxNumberOfDownloads int,
	@DownloadExpirationDays int,
	@DownloadActivationType int,
	@HasSampleDownload bit,
	@SampleDownloadID int,
	@HasUserAgreement bit,
	@UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @CycleLength int,
    @CyclePeriod int,
    @TotalCycles int,
	@IsShipEnabled bit,
	@IsFreeShipping bit,
	@AdditionalShippingCharge money,
	@IsTaxExempt bit,
	@TaxCategoryID int,
	@ManageInventory int,
	@StockQuantity int,
	@DisplayStockAvailability bit,
	@MinStockQuantity int,
	@LowStockActivityID int,
	@NotifyAdminForQuantityBelow int,
	@AllowOutOfStockOrders bit,
	@OrderMinimumQuantity int,
	@OrderMaximumQuantity int,
	@WarehouseId int,
	@DisableBuyButton bit,
	@Price money,
	@OldPrice money,
	@ProductCost money,
	@Weight float,
	@Length decimal(18, 4),
	@Width decimal(18, 4),
	@Height decimal(18, 4),
	@PictureID int,
	@AvailableStartDateTime datetime,
	@AvailableEndDateTime datetime,
	@Published bit,
	@Deleted bit,
	@DisplayOrder int,
	@CreatedOn datetime,
	@UpdatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_ProductVariant]
	SET
		ProductId=@ProductId,
		[Name]=@Name,
		[SKU]=@SKU,
		[Description]=@Description,
		AdminComment=@AdminComment,
		ManufacturerPartNumber=@ManufacturerPartNumber,		
		IsGiftCard=@IsGiftCard,
		IsDownload=@IsDownload,
		DownloadID=@DownloadID,
		UnlimitedDownloads=@UnlimitedDownloads,
		MaxNumberOfDownloads=@MaxNumberOfDownloads,
		DownloadExpirationDays=@DownloadExpirationDays,
		DownloadActivationType=@DownloadActivationType,
		HasSampleDownload=@HasSampleDownload,
		SampleDownloadID=@SampleDownloadID,
		HasUserAgreement=@HasUserAgreement,
		UserAgreementText=@UserAgreementText,
		IsRecurring=@IsRecurring,
		CycleLength=@CycleLength,
		CyclePeriod=@CyclePeriod,
		TotalCycles=@TotalCycles,
		IsShipEnabled=@IsShipEnabled,
		IsFreeShipping=@IsFreeShipping,
		AdditionalShippingCharge=@AdditionalShippingCharge,
		IsTaxExempt=@IsTaxExempt,
		TaxCategoryID=@TaxCategoryID,
		ManageInventory=@ManageInventory,
		StockQuantity=@StockQuantity,
		DisplayStockAvailability=@DisplayStockAvailability,
		MinStockQuantity=@MinStockQuantity,
		LowStockActivityID=@LowStockActivityID,
		NotifyAdminForQuantityBelow=@NotifyAdminForQuantityBelow,
		AllowOutOfStockOrders=@AllowOutOfStockOrders,
		OrderMinimumQuantity=@OrderMinimumQuantity,
		OrderMaximumQuantity=@OrderMaximumQuantity,
		WarehouseId=@WarehouseId,
		DisableBuyButton=@DisableBuyButton,
		Price=@Price,
		OldPrice=@OldPrice,
		ProductCost=@ProductCost,
		Weight=@Weight,
		[Length]=@Length,
		Width=@Width,
		Height=@Height,
		PictureID=@PictureID,
		AvailableStartDateTime=@AvailableStartDateTime,
		AvailableEndDateTime=@AvailableEndDateTime,
		Published=@Published,
		Deleted=@Deleted,
		DisplayOrder=@DisplayOrder,
		CreatedOn=@CreatedOn,
		UpdatedOn=@UpdatedOn
	WHERE
		ProductVariantID = @ProductVariantID
END
GO

if not exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ProductVariantAttributeCombination]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Nop_ProductVariantAttributeCombination](
	[ProductVariantAttributeCombinationID] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantID] [int] NOT NULL,
	[AttributesXML] [xml] NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[AllowOutOfStockOrders] [bit] NOT NULL,
 CONSTRAINT [Nop_ProductVariantAttributeCombination_PK] PRIMARY KEY CLUSTERED 
(
	[ProductVariantAttributeCombinationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ProductVariantAttributeCombination_Nop_ProductVariant'
           AND parent_obj = Object_id('Nop_ProductVariantAttributeCombination')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ProductVariantAttributeCombination
DROP CONSTRAINT FK_Nop_ProductVariantAttributeCombination_Nop_ProductVariant
GO
ALTER TABLE [dbo].[Nop_ProductVariantAttributeCombination]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ProductVariantAttributeCombination_Nop_ProductVariant] FOREIGN KEY([ProductVariantID])
REFERENCES [dbo].[Nop_ProductVariant] ([ProductVariantID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeCombinationDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationDelete]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationDelete]
(
	@ProductVariantAttributeCombinationID int
)
AS
BEGIN
	SET NOCOUNT ON
	DELETE
	FROM [Nop_ProductVariantAttributeCombination]
	WHERE
		ProductVariantAttributeCombinationID = @ProductVariantAttributeCombinationID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeCombinationInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationInsert]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationInsert]
(
	@ProductVariantAttributeCombinationID int = NULL output,
	@ProductVariantID int,
	@AttributesXML xml,
	@StockQuantity int,
	@AllowOutOfStockOrders bit
)
AS
BEGIN
	INSERT
	INTO [Nop_ProductVariantAttributeCombination]
	(
		[ProductVariantID],
		[AttributesXML],
		[StockQuantity],
		[AllowOutOfStockOrders]
	)
	VALUES
	(	
		@ProductVariantID,
		@AttributesXML,
		@StockQuantity,
		@AllowOutOfStockOrders
	)

	set @ProductVariantAttributeCombinationID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeCombinationLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationLoadAll]	
	@ProductVariantID int
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_ProductVariantAttributeCombination]
	WHERE
		ProductVariantID = @ProductVariantID
	order by [ProductVariantAttributeCombinationID]
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeCombinationLoadByPrimaryKey]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationLoadByPrimaryKey]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationLoadByPrimaryKey]
(
	@ProductVariantAttributeCombinationID int
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_ProductVariantAttributeCombination]
	WHERE
		ProductVariantAttributeCombinationID = @ProductVariantAttributeCombinationID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantAttributeCombinationUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantAttributeCombinationUpdate]
(
	@ProductVariantAttributeCombinationID int,
	@ProductVariantID int,
	@AttributesXML xml,
	@StockQuantity int,
	@AllowOutOfStockOrders bit
)
AS
BEGIN

	UPDATE [Nop_ProductVariantAttributeCombination]
	SET
		[ProductVariantID] = @ProductVariantID,
		[AttributesXML] = @AttributesXML,
		[StockQuantity] = @StockQuantity,
		[AllowOutOfStockOrders] = @AllowOutOfStockOrders
	WHERE
		ProductVariantAttributeCombinationID = @ProductVariantAttributeCombinationID
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_OrderProductVariant]') and NAME='AttributesXML')
BEGIN
	ALTER TABLE [dbo].[Nop_OrderProductVariant] 
	ADD AttributesXML xml NOT NULL CONSTRAINT [DF_Nop_OrderProductVariant_AttributesXML] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantInsert]
(
	@OrderProductVariantID int = NULL output,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@AttributesXML xml,
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit,
	@LicenseDownloadID int
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderProductVariant]
	(
		OrderProductVariantGUID,
		OrderID,
		ProductVariantID,
		UnitPriceInclTax,
		UnitPriceExclTax,
		PriceInclTax,
		PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency,
		AttributeDescription,
		AttributesXML,
		Quantity,
		DiscountAmountInclTax,
		DiscountAmountExclTax,
		DownloadCount,
		IsDownloadActivated,
		LicenseDownloadID
	)
	VALUES
	(
		@OrderProductVariantGUID,
		@OrderID,
		@ProductVariantID,
		@UnitPriceInclTax,
		@UnitPriceExclTax,
		@PriceInclTax,
		@PriceExclTax,
		@UnitPriceInclTaxInCustomerCurrency,
		@UnitPriceExclTaxInCustomerCurrency,
		@PriceInclTaxInCustomerCurrency,
		@PriceExclTaxInCustomerCurrency,
		@AttributeDescription,
		@AttributesXML,
		@Quantity,
		@DiscountAmountInclTax,
		@DiscountAmountExclTax,
		@DownloadCount,
		@IsDownloadActivated,
		@LicenseDownloadID
	)

	set @OrderProductVariantID=SCOPE_IDENTITY()
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantUpdate]
(
	@OrderProductVariantID int,
	@OrderProductVariantGUID uniqueidentifier,
	@OrderID int,
	@ProductVariantID int,
	@UnitPriceInclTax money,
	@UnitPriceExclTax money,
	@PriceInclTax money,
	@PriceExclTax money,
	@UnitPriceInclTaxInCustomerCurrency money,
	@UnitPriceExclTaxInCustomerCurrency money,
	@PriceInclTaxInCustomerCurrency money,
	@PriceExclTaxInCustomerCurrency money,
	@AttributeDescription nvarchar(4000),
	@AttributesXML xml,
	@Quantity int,
	@DiscountAmountInclTax decimal (18, 4),
	@DiscountAmountExclTax decimal (18, 4),
	@DownloadCount int,
	@IsDownloadActivated bit,
	@LicenseDownloadID int
)
AS
BEGIN

	UPDATE [Nop_OrderProductVariant]
	SET		
		OrderProductVariantGUID=@OrderProductVariantGUID,
		OrderID=@OrderID,
		ProductVariantID=@ProductVariantID,
		UnitPriceInclTax=@UnitPriceInclTax,
		UnitPriceExclTax = @UnitPriceExclTax,
		PriceInclTax=@PriceInclTax,
		PriceExclTax=@PriceExclTax,
		UnitPriceInclTaxInCustomerCurrency=@UnitPriceInclTaxInCustomerCurrency,
		UnitPriceExclTaxInCustomerCurrency=@UnitPriceExclTaxInCustomerCurrency,
		PriceInclTaxInCustomerCurrency=@PriceInclTaxInCustomerCurrency,
		PriceExclTaxInCustomerCurrency=@PriceExclTaxInCustomerCurrency,
		AttributeDescription=@AttributeDescription,
		AttributesXML=@AttributesXML,
		Quantity=@Quantity,
		DiscountAmountInclTax=@DiscountAmountInclTax,
		DiscountAmountExclTax=@DiscountAmountExclTax,
		DownloadCount=@DownloadCount,
		IsDownloadActivated=@IsDownloadActivated,
		LicenseDownloadID=@LicenseDownloadID
	WHERE
		OrderProductVariantID = @OrderProductVariantID
END
GO


-- Single Product Variant template
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_ProductTemplate]
		WHERE [TemplatePath] = N'Templates\Products\OneVariant.ascx')
BEGIN
	INSERT [dbo].[Nop_ProductTemplate] ([Name], [TemplatePath], [DisplayOrder], [CreatedOn], [UpdatedOn]) 
	VALUES (N'Single Product Variant', N'Templates\Products\OneVariant.ascx', 10, N'04.02.2010 13:25:52', N'04.02.2010 13:25:52')
END
GO

-- USAePay ePayment Form payment methods
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.USAePay.EPaymentFormPaymentProcessor, Nop.Payment.USAePay')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'USA ePay (hosted)', N'USA ePay (hosted)', N'', N'Payment\USAePay\EPaymentFormConfig.ascx', N'~\Templates\Payment\USAePay\EPaymentFormPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.USAePay.EPaymentFormPaymentProcessor, Nop.Payment.USAePay', N'USAEPAY.EPAYMENTFORM', 0, 170)
END
GO


--referrer URL for log
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Log]') and NAME='ReferrerURL')
BEGIN
	ALTER TABLE [dbo].[Nop_Log] 
	ADD ReferrerURL nvarchar(100) NOT NULL CONSTRAINT [DF_Nop_Log_ReferrerURL] DEFAULT ((''))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LogInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LogInsert]
GO
CREATE PROCEDURE [dbo].[Nop_LogInsert]
(
	@LogID int = NULL output,
	@LogTypeID int,
	@Severity int,
	@Message nvarchar(1000),
	@Exception nvarchar(4000),
	@IPAddress nvarchar(100),
	@CustomerID int,
	@PageURL nvarchar(100),
	@ReferrerURL nvarchar(100),
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_Log]
	(
		LogTypeID,
		Severity,
		[Message],
		Exception,
		IPAddress,
		CustomerID,
		PageURL,
		ReferrerURL,
		CreatedOn
	)
	VALUES
	(
		@LogTypeID,
		@Severity,
		@Message,
		@Exception,
		@IPAddress,
		@CustomerID,
		@PageURL,
		@ReferrerURL,
		@CreatedOn
	)

	set @LogID=SCOPE_IDENTITY()
END
GO

--load product variant by SKU
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadBySKU]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadBySKU]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadBySKU]
(
	@SKU nvarchar (400)
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_ProductVariant] pv
	WHERE 
		pv.Deleted=0 AND 
		pv.SKU = @SKU
	order by DisplayOrder
END
GO

-- Shipping per kg or per range
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ShippingByWeight.CalculatePerWeightUnit')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ShippingByWeight.CalculatePerWeightUnit', N'true', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ShippingByWeightAndCountry.CalculatePerWeightUnit')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ShippingByWeightAndCountry.CalculatePerWeightUnit', N'true', N'')
END
GO

-- Beanstream Hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Beanstream.HostedPaymentProcessor, Nop.Payment.Beanstream')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Beanstream (hosted)', N'Beanstream', N'', N'Payment\Beanstream\HostedPaymentConfig.ascx', N'~\Templates\Payment\Beanstream\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Beanstream.HostedPaymentProcessor, Nop.Payment.Beanstream', N'BEANSTREAM.HOSTED', 0, 180)
END
GO


--added DisplayOrder column to Nop_CustomerAction table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_CustomerAction]') and NAME='DisplayOrder')
BEGIN
	ALTER TABLE [dbo].[Nop_CustomerAction] 
	ADD DisplayOrder int NOT NULL CONSTRAINT [DF_Nop_CustomerAction_DisplayOrder] DEFAULT ((1))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionInsert]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionInsert]
(
	@CustomerActionID int = NULL output,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Comment nvarchar(1000),
	@DisplayOrder int
)
AS
BEGIN
	INSERT
	INTO [Nop_CustomerAction]
	(
		[Name],
		[SystemKeyword],
		[Comment],
		[DisplayOrder]
	)
	VALUES
	(
		@Name,
		@SystemKeyword,
		@Comment,
		@DisplayOrder
	)

	set @CustomerActionID=SCOPE_IDENTITY()
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionUpdate]
(
	@CustomerActionID int,
	@Name nvarchar(100),
	@SystemKeyword nvarchar(100),
	@Comment nvarchar(1000),
	@DisplayOrder int
)
AS
BEGIN

	UPDATE [Nop_CustomerAction]
	SET
		[Name]=@Name,
		[SystemKeyword]=@SystemKeyword,
		[Comment]=@Comment,
		DisplayOrder=@DisplayOrder
	WHERE
		[CustomerActionID] = @CustomerActionID
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerActionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerActionLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerActionLoadAll]
AS
BEGIN
	SET NOCOUNT ON
	SELECT *
	FROM [Nop_CustomerAction]
	ORDER BY [DisplayOrder], [Name]
END
GO


--customer actions
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCatalog')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Catalog', N'ManageCatalog', N'',10)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageOrders')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Orders', N'ManageOrders', N'',30)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageGiftCards')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Gift Cards', N'ManageGiftCards', N'',40)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCustomers')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Customers', N'ManageCustomers', N'',50)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCustomerRoles')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Customer Roles', N'ManageCustomerRoles', N'',60)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageAffiliates')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Affiliates', N'ManageAffiliates', N'',70)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCampaigns')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Campaigns', N'ManageCampaigns', N'',80)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageDiscounts')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Discounts', N'ManageDiscounts', N'',90)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManagePriceLists')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Price Lists', N'ManagePriceLists', N'',100)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageProductFeeds')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Product Feeds', N'ManageProductFeeds', N'',110)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManagePolls')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Polls', N'ManagePolls', N'',140)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageNews')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage News', N'ManageNews', N'',130)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageBlog')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Blog', N'ManageBlog', N'',150)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageTopics')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Topics', N'ManageTopics', N'',160)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageForums')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Forums', N'ManageForums', N'',170)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageTemplates')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Templates', N'ManageTemplates', N'',180)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageLanguagesLocalization')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Languages/Localization', N'ManageLanguagesLocalization', N'',190)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageGlobalSettings')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Global Settings', N'ManageGlobalSettings', N'',200)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageBlacklist')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Blacklist', N'ManageBlacklist', N'',210)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManagePaymentSettings')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Payment Settings', N'ManagePaymentSettings', N'',220)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageTaxSettings')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Tax Settings', N'ManageTaxSettings', N'',230)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageShippingSettings')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Shipping Settings', N'ManageShippingSettings', N'',240)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCoutriesStates')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Coutries / States', N'ManageCoutriesStates', N'',250)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageCurrencies')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Currencies', N'ManageCurrencies', N'',260)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageWarehouses')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Warehouses', N'ManageWarehouses', N'',270)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageMeasures')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Measures', N'ManageMeasures', N'',280)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageActivityLog')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Activity Log', N'ManageActivityLog', N'',290)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageACL')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage ACL', N'ManageACL', N'',300)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageSystemLog')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage System Log', N'ManageSystemLog', N'',310)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageMessageQueue')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Message Queue', N'ManageMessageQueue', N'',320)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageMaintenance')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Maintenance', N'ManageMaintenance', N'',330)
END
GO

-- Media.Images.StoreInDB setting
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Media.Images.StoreInDB')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Media.Images.StoreInDB', N'true', N'A value indicating whether the pictures should be stored in database')
END
GO

-- Moneris Hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Moneris.HostedPaymentProcessor, Nop.Payment.Moneris')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Moneris (hosted)', N'Moneris', N'', N'Payment\Moneris\HostedPaymentConfig.ascx', N'~\Templates\Payment\Moneris\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Moneris.HostedPaymentProcessor, Nop.Payment.Moneris', N'MONERIS.HOSTED', 0, 190)
END
GO

--add DisplayToCustomer column to Nop_OrderNote table
IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_OrderNote]') and NAME='DisplayToCustomer')
BEGIN
	ALTER TABLE [dbo].[Nop_OrderNote] 
	ADD DisplayToCustomer bit NOT NULL CONSTRAINT [DF_Nop_OrderNote_DisplayToCustomer] DEFAULT ((0))
END
GO

--modify Nop_OrderNoteInsert SP
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderNoteInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_OrderNoteInsert]
GO
CREATE PROCEDURE [dbo].[Nop_OrderNoteInsert]
(
	@OrderNoteID int = NULL output,
	@OrderID int,
	@Note nvarchar(4000),
	@DisplayToCustomer bit,
	@CreatedOn datetime
)
AS
BEGIN
	INSERT
	INTO [Nop_OrderNote]
	(
		OrderID,
		Note,
		DisplayToCustomer,
		CreatedOn
	)
	VALUES
	(
		@OrderID,
		@Note,
		@DisplayToCustomer,
		@CreatedOn
	)

	set @OrderNoteID=SCOPE_IDENTITY()
END
GO

--modify Nop_OrderNoteUpdate SP
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderNoteUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_OrderNoteUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_OrderNoteUpdate]
(
	@OrderNoteID int,
	@OrderID int,
	@Note nvarchar(4000),
	@DisplayToCustomer bit,
	@CreatedOn datetime
)
AS
BEGIN
	UPDATE [Nop_OrderNote]
	SET
	OrderID=@OrderID,
	Note=@Note,
	DisplayToCustomer = @DisplayToCustomer,
	CreatedOn=@CreatedOn
	WHERE
		OrderNoteID = @OrderNoteID
END
GO


--modify Nop_OrderNoteLoadByOrderID SP
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderNoteLoadByOrderID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[Nop_OrderNoteLoadByOrderID]
GO
CREATE PROCEDURE [dbo].[Nop_OrderNoteLoadByOrderID]
(
	@OrderID int,
	@ShowHidden bit
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		*
	FROM [Nop_OrderNote]
	WHERE
		OrderID=@OrderID AND
		(@ShowHidden = 1 OR DisplayToCustomer = 1)
	ORDER BY CreatedOn desc, OrderNoteID desc
END
GO

-- SecurePay XML payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.SecurePay.XmlPaymentProcessor, Nop.Payment.SecurePay')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'SecurePay (Secure XML)', N'Credit Card', N'', N'Payment\SecurePay\XmlPaymentConfig.ascx', N'~\Templates\Payment\SecurePay\XmlPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.SecurePay.XmlPaymentProcessor, Nop.Payment.SecurePay', N'SECUREPAY.XML', 0, 200)
END
GO


--activity log types
IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddToShoppingCart'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddToShoppingCart', N'Add a product to shopping cart', 0)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'RemoveFromShoppingCart'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'RemoveFromShoppingCart', N'Remove a product from shopping cart', 0)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewCategory'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewCategory', N'Add a new category', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditCategory'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditCategory', N'Edit a category', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteCategory'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteCategory', N'Delete a category', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewManufacturer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewManufacturer', N'Add a new manufacturer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditManufacturer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditManufacturer', N'Edit a manufacturer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteManufacturer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteManufacturer', N'Delete a manufacturer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewProduct'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewProduct', N'Add a new product', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditProduct'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditProduct', N'Edit a product', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteProduct'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteProduct', N'Delete a product', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewProductVariant'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewProductVariant', N'Add a new product variant', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditProductVariant'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditProductVariant', N'Edit a product variant', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteProductVariant'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteProductVariant', N'Delete a product variant', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewProductAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewProductAttribute', N'Add a new product attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditProductAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditProductAttribute', N'Edit a product attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteProductAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteProductAttribute', N'Delete a product attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewSpecAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewSpecAttribute', N'Add a new specification attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditSpecAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditSpecAttribute', N'Edit a specification attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteSpecAttribute'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteSpecAttribute', N'Delete a specification attribute', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditGiftCard'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditGiftCard', N'Edit a gift card', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewCustomer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewCustomer', N'Add a new customer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditCustomer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditCustomer', N'Edit a customer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteCustomer'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteCustomer', N'Delete a customer', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewCustomerRole'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewCustomerRole', N'Add a new customer role', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditCustomerRole'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditCustomerRole', N'Edit a customer role', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteCustomerRole'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteCustomerRole', N'Delete a customer role', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewDiscount'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewDiscount', N'Add a new discount', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditDiscount'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditDiscount', N'Edit a discount', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteDiscount'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteDiscount', N'Delete a discount', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditGlobalSettings'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditGlobalSettings', N'Edit global settings', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditPaymentMethod'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditPaymentMethod', N'Edit a payment method', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditTaxSettings'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditTaxSettings', N'Edit tax settings', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditTaxProvider'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditTaxProvider', N'Edit a tax provider', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditShippingSettings'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditShippingSettings', N'Edit shipping settings', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditShippingProvider'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditShippingProvider', N'Edit a shipping rate computation method', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'AddNewSetting'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'AddNewSetting', N'Add a new setting', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditSetting'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditSetting', N'Edit a setting', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteSetting'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteSetting', N'Delete a setting', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'CreateBackup'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'CreateBackup', N'Create a backup', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'RestoreBackup'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'RestoreBackup', N'Restore a backup', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'PlaceOrder'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'PlaceOrder', N'Place an order', 1)
END
GO

IF (NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'WriteProductReview'))
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'WriteProductReview', N'Write a product review', 1)
END
GO

-- ChronoPay hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.ChronoPay.HostedPaymentProcessor, Nop.Payment.ChronoPay')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'ChronoPay (hosted)', N'ChronoPay', N'', N'Payment\ChronoPay\HostedPaymentConfig.ascx', N'~\Templates\Payment\ChronoPay\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.ChronoPay.HostedPaymentProcessor, Nop.Payment.ChronoPay', N'CHRONOPAY.HOSTED', 0, 210)
END
GO

-- Assist hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Assist.HostedPaymentProcessor, Nop.Payment.Assist')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Assist (hosted)', N'Assist', N'', N'Payment\Assist\HostedPaymentConfig.ascx', N'~\Templates\Payment\Assist\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Assist.HostedPaymentProcessor, Nop.Payment.Assist', N'ASSIST.HOSTED', 0, 220)
END
GO

-- Determines whether to load all products (in back end)
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Admin.LoadAllProducts')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Admin.LoadAllProducts', N'true', N'')
END
GO

-- Australia post shipping rate computation method

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_ShippingRateComputationMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Shipping.Methods.AustraliaPost.AustraliaPostComputationMethod, Nop.Shipping.AustraliaPost')
BEGIN
	INSERT [dbo].[Nop_ShippingRateComputationMethod] 
	([Name], [Description], [ConfigureTemplatePath], [ClassName], [DisplayOrder]) 
	VALUES (N'Australia Post', N'', N'Shipping\AustraliaPostConfigure\ConfigureShipping.ascx', N'NopSolutions.NopCommerce.Shipping.Methods.AustraliaPost.AustraliaPostComputationMethod, Nop.Shipping.AustraliaPost', 30)
END
GO

-- millimetres measure dimension
IF NOT EXISTS 
(
	SELECT 1 FROM [dbo].[Nop_MeasureDimension] WHERE [SystemKeyword] = N'millimetres'
)
BEGIN
	INSERT INTO [dbo].[Nop_MeasureDimension] ([Name], [SystemKeyword], [DisplayOrder], [Ratio]) VALUES(N'millimetre(s)', N'millimetres', 4, 25.4)
END
GO

-- grams measure weight
IF NOT EXISTS 
(
	SELECT 1 FROM [dbo].[Nop_MeasureWeight] WHERE [SystemKeyword] = N'grams'
)
BEGIN
	INSERT INTO [dbo].[Nop_MeasureWeight] ([Name], [SystemKeyword], [DisplayOrder], [Ratio]) VALUES(N'gram(s)', N'grams', 4, 453.59)
END
GO

-- CyberSource hosted payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.CyberSource.HostedPaymentProcessor, Nop.Payment.CyberSource')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'CyberSource (hosted)', N'CyberSource', N'', N'Payment\CyberSource\HostedPaymentConfig.ascx', N'~\Templates\Payment\CyberSource\HostedPayment.ascx', N'NopSolutions.NopCommerce.Payment.Methods.CyberSource.HostedPaymentProcessor, Nop.Payment.CyberSource', N'CYBERSOURCE.HOSTED', 0, 230)
END
GO

-- iDeal basic payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.iDeal.iDealBasicPaymentProcessor, Nop.Payment.iDeal')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'iDeal (Basic)', N'iDeal', N'', N'Payment\iDeal\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\iDeal\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.iDeal.iDealBasicPaymentProcessor, Nop.Payment.iDeal', N'iDeal.Basic', 0, 240)
END
GO









--new countries
ALTER TABLE [dbo].[Nop_StateProvince] ALTER COLUMN [Abbreviation] [nvarchar](30) NOT NULL
GO

IF NOT EXISTS (
SELECT 1 
FROM [dbo].[Nop_Setting]
WHERE [Name] = N'SystemUpgrade-2.0-NewCountries')
BEGIN
	DECLARE @CountryId int
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AFG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AFG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Afghanistan', 1, 1, 1, N'AF', N'AFG', 4, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ALB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ALB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Albania', 1, 1, 1, N'AL', N'ALB', 8, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DZA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DZA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Algeria', 1, 1, 1, N'DZ', N'DZA', 12, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ASM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ASM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'American Samoa', 1, 1, 1, N'AS', N'ASM', 16, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AND'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AND'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Andorra', 1, 1, 1, N'AD', N'AND', 20, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AGO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AGO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Angola', 1, 1, 1, N'AO', N'AGO', 24, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AIA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AIA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Anguilla', 1, 1, 1, N'AI', N'AIA', 660, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Antarctica', 1, 1, 1, N'AQ', N'ATA', 10, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Antigua and Barbuda', 1, 1, 1, N'AG', N'ATG', 28, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Argentina', 1, 1, 1, N'AR', N'ARG', 32, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Armenia', 1, 1, 1, N'AM', N'ARM', 51, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ABW'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ABW'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Aruba', 1, 1, 1, N'AW', N'ABW', 533, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AUS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AUS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Australia', 1, 1, 1, N'AU', N'AUS', 36, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AUT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AUT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Austria', 1, 1, 1, N'AT', N'AUT', 40, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AZE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='AZE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Azerbaijan', 1, 1, 1, N'AZ', N'AZE', 31, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BHS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BHS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bahamas', 1, 1, 1, N'BS', N'BHS', 44, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BHR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BHR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bahrain', 1, 1, 1, N'BH', N'BHR', 48, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BGD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BGD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bangladesh', 1, 1, 1, N'BD', N'BGD', 50, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Barbados', 1, 1, 1, N'BB', N'BRB', 52, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BLR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BLR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Belarus', 1, 1, 1, N'BY', N'BLR', 112, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BEL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BEL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Belgium', 1, 1, 1, N'BE', N'BEL', 56, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BLZ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BLZ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Belize', 1, 1, 1, N'BZ', N'BLZ', 84, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BEN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BEN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Benin', 1, 1, 1, N'BJ', N'BEN', 204, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BMU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BMU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bermuda', 1, 1, 1, N'BM', N'BMU', 60, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BTN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BTN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bhutan', 1, 1, 1, N'BT', N'BTN', 64, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BOL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BOL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bolivia', 1, 1, 1, N'BO', N'BOL', 68, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BIH'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BIH'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bosnia and Herzegowina', 1, 1, 1, N'BA', N'BIH', 70, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BWA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BWA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Botswana', 1, 1, 1, N'BW', N'BWA', 72, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BVT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BVT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bouvet Island', 1, 1, 1, N'BV', N'BVT', 74, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Brazil', 1, 1, 1, N'BR', N'BRA', 76, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IOT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IOT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'British Indian Ocean Territory', 1, 1, 1, N'IO', N'IOT', 86, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BRN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Brunei Darussalam', 1, 1, 1, N'BN', N'BRN', 96, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BGR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BGR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Bulgaria', 1, 1, 1, N'BG', N'BGR', 100, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BFA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BFA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Burkina Faso', 1, 1, 1, N'BF', N'BFA', 854, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BDI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='BDI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Burundi', 1, 1, 1, N'BI', N'BDI', 108, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KHM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KHM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cambodia', 1, 1, 1, N'KH', N'KHM', 116, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CMR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CMR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cameroon', 1, 1, 1, N'CM', N'CMR', 120, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CAN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CAN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Canada', 1, 1, 1, N'CA', N'CAN', 124, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CPV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CPV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cape Verde', 1, 1, 1, N'CV', N'CPV', 132, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CYM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CYM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cayman Islands', 1, 1, 1, N'KY', N'CYM', 136, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CAF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CAF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Central African Republic', 1, 1, 1, N'CF', N'CAF', 140, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TCD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TCD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Chad', 1, 1, 1, N'TD', N'TCD', 148, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Chile', 1, 1, 1, N'CL', N'CHL', 152, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'China', 1, 1, 1, N'CN', N'CHN', 156, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CXR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CXR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Christmas Island', 1, 1, 1, N'CX', N'CXR', 162, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CCK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CCK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cocos (Keeling) Islands', 1, 1, 1, N'CC', N'CCK', 166, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Colombia', 1, 1, 1, N'CO', N'COL', 170, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Comoros', 1, 1, 1, N'KM', N'COM', 174, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Congo', 1, 1, 1, N'CG', N'COG', 178, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='COK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cook Islands', 1, 1, 1, N'CK', N'COK', 184, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CRI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CRI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Costa Rica', 1, 1, 1, N'CR', N'CRI', 188, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CIV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CIV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cote D''Ivoire', 1, 1, 1, N'CI', N'CIV', 384, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HRV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HRV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Croatia', 1, 1, 1, N'HR', N'HRV', 191, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CUB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CUB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cuba', 1, 1, 1, N'CU', N'CUB', 192, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CYP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CYP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Cyprus', 1, 1, 1, N'CY', N'CYP', 196, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CZE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CZE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Czech Republic', 1, 1, 1, N'CZ', N'CZE', 203, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DNK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DNK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Denmark', 1, 1, 1, N'DK', N'DNK', 208, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DJI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DJI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Djibouti', 1, 1, 1, N'DJ', N'DJI', 262, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DMA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DMA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Dominica', 1, 1, 1, N'DM', N'DMA', 212, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DOM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DOM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Dominican Republic', 1, 1, 1, N'DO', N'DOM', 214, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ECU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ECU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Ecuador', 1, 1, 1, N'EC', N'ECU', 218, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='EGY'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='EGY'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Egypt', 1, 1, 1, N'EG', N'EGY', 818, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'El Salvador', 1, 1, 1, N'SV', N'SLV', 222, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GNQ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GNQ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Equatorial Guinea', 1, 1, 1, N'GQ', N'GNQ', 226, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ERI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ERI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Eritrea', 1, 1, 1, N'ER', N'ERI', 232, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='EST'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='EST'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Estonia', 1, 1, 1, N'EE', N'EST', 233, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ETH'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ETH'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Ethiopia', 1, 1, 1, N'ET', N'ETH', 231, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FLK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FLK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Falkland Islands (Malvinas)', 1, 1, 1, N'FK', N'FLK', 238, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FRO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FRO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Faroe Islands', 1, 1, 1, N'FO', N'FRO', 234, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FJI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FJI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Fiji', 1, 1, 1, N'FJ', N'FJI', 242, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FIN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FIN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Finland', 1, 1, 1, N'FI', N'FIN', 246, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FRA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FRA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'France', 1, 1, 1, N'FR', N'FRA', 250, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'French Guiana', 1, 1, 1, N'GF', N'GUF', 254, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PYF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PYF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'French Polynesia', 1, 1, 1, N'PF', N'PYF', 258, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ATF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'French Southern Territories', 1, 1, 1, N'TF', N'ATF', 260, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GAB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GAB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Gabon', 1, 1, 1, N'GA', N'GAB', 266, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GMB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GMB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Gambia', 1, 1, 1, N'GM', N'GMB', 270, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GEO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GEO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Georgia', 1, 1, 1, N'GE', N'GEO', 268, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DEU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='DEU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Germany', 1, 1, 1, N'DE', N'DEU', 276, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GHA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GHA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Ghana', 1, 1, 1, N'GH', N'GHA', 288, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GIB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GIB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Gibraltar', 1, 1, 1, N'GI', N'GIB', 292, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRC'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRC'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Greece', 1, 1, 1, N'GR', N'GRC', 300, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Greenland', 1, 1, 1, N'GL', N'GRL', 304, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GRD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Grenada', 1, 1, 1, N'GD', N'GRD', 308, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GLP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GLP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guadeloupe', 1, 1, 1, N'GP', N'GLP', 312, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guam', 1, 1, 1, N'GU', N'GUM', 316, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GTM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GTM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guatemala', 1, 1, 1, N'GT', N'GTM', 320, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GIN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GIN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guinea', 1, 1, 1, N'GN', N'GIN', 324, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GNB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GNB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guinea-bissau', 1, 1, 1, N'GW', N'GNB', 624, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUY'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GUY'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Guyana', 1, 1, 1, N'GY', N'GUY', 328, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HTI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HTI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Haiti', 1, 1, 1, N'HT', N'HTI', 332, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HMD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HMD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Heard and Mc Donald Islands', 1, 1, 1, N'HM', N'HMD', 334, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HND'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HND'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Honduras', 1, 1, 1, N'HN', N'HND', 340, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HKG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HKG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Hong Kong', 1, 1, 1, N'HK', N'HKG', 344, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HUN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='HUN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Hungary', 1, 1, 1, N'HU', N'HUN', 348, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ISL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ISL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Iceland', 1, 1, 1, N'IS', N'ISL', 352, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IND'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IND'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'India', 1, 1, 1, N'IN', N'IND', 356, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IDN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IDN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Indonesia', 1, 1, 1, N'ID', N'IDN', 360, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Iran (Islamic Republic of)', 1, 1, 1, N'IR', N'IRN', 364, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRQ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRQ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Iraq', 1, 1, 1, N'IQ', N'IRQ', 368, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='IRL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Ireland', 1, 1, 1, N'IE', N'IRL', 372, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ISR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ISR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Israel', 1, 1, 1, N'IL', N'ISR', 376, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ITA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ITA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Italy', 1, 1, 1, N'IT', N'ITA', 380, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JAM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JAM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Jamaica', 1, 1, 1, N'JM', N'JAM', 388, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JPN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JPN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Japan', 1, 1, 1, N'JP', N'JPN', 392, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JOR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='JOR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Jordan', 1, 1, 1, N'JO', N'JOR', 400, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KAZ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KAZ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Kazakhstan', 1, 1, 1, N'KZ', N'KAZ', 398, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KEN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KEN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Kenya', 1, 1, 1, N'KE', N'KEN', 404, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KIR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KIR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Kiribati', 1, 1, 1, N'KI', N'KIR', 296, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'North Korea', 1, 1, 1, N'KP', N'PRK', 408, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KOR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KOR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Korea', 1, 1, 1, N'KR', N'KOR', 410, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KWT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KWT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Kuwait', 1, 1, 1, N'KW', N'KWT', 414, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KGZ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KGZ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Kyrgyzstan', 1, 1, 1, N'KG', N'KGZ', 417, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LAO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LAO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Lao People''s Democratic Republic', 1, 1, 1, N'LA', N'LAO', 418, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LVA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LVA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Latvia', 1, 1, 1, N'LV', N'LVA', 428, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Lebanon', 1, 1, 1, N'LB', N'LBN', 422, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LSO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LSO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Lesotho', 1, 1, 1, N'LS', N'LSO', 426, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Liberia', 1, 1, 1, N'LR', N'LBR', 430, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBY'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LBY'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Libyan Arab Jamahiriya', 1, 1, 1, N'LY', N'LBY', 434, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LIE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LIE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Liechtenstein', 1, 1, 1, N'LI', N'LIE', 438, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LTU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LTU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Lithuania', 1, 1, 1, N'LT', N'LTU', 440, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LUX'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LUX'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Luxembourg', 1, 1, 1, N'LU', N'LUX', 442, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MAC'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MAC'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Macau', 1, 1, 1, N'MO', N'MAC', 446, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MKD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MKD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Macedonia', 1, 1, 1, N'MK', N'MKD', 807, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Madagascar', 1, 1, 1, N'MG', N'MDG', 450, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MWI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MWI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Malawi', 1, 1, 1, N'MW', N'MWI', 454, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MYS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MYS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Malaysia', 1, 1, 1, N'MY', N'MYS', 458, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Maldives', 1, 1, 1, N'MV', N'MDV', 462, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MLI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MLI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mali', 1, 1, 1, N'ML', N'MLI', 466, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MLT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MLT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Malta', 1, 1, 1, N'MT', N'MLT', 470, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MHL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MHL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Marshall Islands', 1, 1, 1, N'MH', N'MHL', 584, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MTQ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MTQ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Martinique', 1, 1, 1, N'MQ', N'MTQ', 474, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MRT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MRT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mauritania', 1, 1, 1, N'MR', N'MRT', 478, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MUS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MUS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mauritius', 1, 1, 1, N'MU', N'MUS', 480, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MYT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MYT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mayotte', 1, 1, 1, N'YT', N'MYT', 175, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MEX'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MEX'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mexico', 1, 1, 1, N'MX', N'MEX', 484, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FSM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='FSM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Micronesia', 1, 1, 1, N'FM', N'FSM', 583, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MDA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Moldova', 1, 1, 1, N'MD', N'MDA', 498, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MCO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MCO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Monaco', 1, 1, 1, N'MC', N'MCO', 492, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MNG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MNG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mongolia', 1, 1, 1, N'MN', N'MNG', 496, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MSR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MSR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Montserrat', 1, 1, 1, N'MS', N'MSR', 500, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MAR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MAR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Morocco', 1, 1, 1, N'MA', N'MAR', 504, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MOZ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MOZ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Mozambique', 1, 1, 1, N'MZ', N'MOZ', 508, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MMR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MMR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Myanmar', 1, 1, 1, N'MM', N'MMR', 104, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NAM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NAM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Namibia', 1, 1, 1, N'NA', N'NAM', 516, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NRU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NRU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Nauru', 1, 1, 1, N'NR', N'NRU', 520, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NPL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NPL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Nepal', 1, 1, 1, N'NP', N'NPL', 524, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NLD'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NLD'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Netherlands', 1, 1, 1, N'NL', N'NLD', 528, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ANT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ANT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Netherlands Antilles', 1, 1, 1, N'AN', N'ANT', 530, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NCL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NCL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'New Caledonia', 1, 1, 1, N'NC', N'NCL', 540, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NZL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NZL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'New Zealand', 1, 1, 1, N'NZ', N'NZL', 554, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NIC'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NIC'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Nicaragua', 1, 1, 1, N'NI', N'NIC', 558, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NER'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NER'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Niger', 1, 1, 1, N'NE', N'NER', 562, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NGA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NGA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Nigeria', 1, 1, 1, N'NG', N'NGA', 566, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NIU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NIU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Niue', 1, 1, 1, N'NU', N'NIU', 570, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NFK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NFK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Norfolk Island', 1, 1, 1, N'NF', N'NFK', 574, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MNP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='MNP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Northern Mariana Islands', 1, 1, 1, N'MP', N'MNP', 580, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NOR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='NOR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Norway', 1, 1, 1, N'NO', N'NOR', 578, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='OMN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='OMN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Oman', 1, 1, 1, N'OM', N'OMN', 512, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PAK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PAK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Pakistan', 1, 1, 1, N'PK', N'PAK', 586, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PLW'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PLW'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Palau', 1, 1, 1, N'PW', N'PLW', 585, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PAN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PAN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Panama', 1, 1, 1, N'PA', N'PAN', 591, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PNG'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PNG'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Papua New Guinea', 1, 1, 1, N'PG', N'PNG', 598, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRY'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRY'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Paraguay', 1, 1, 1, N'PY', N'PRY', 600, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PER'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PER'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Peru', 1, 1, 1, N'PE', N'PER', 604, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PHL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PHL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Philippines', 1, 1, 1, N'PH', N'PHL', 608, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PCN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PCN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Pitcairn', 1, 1, 1, N'PN', N'PCN', 612, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='POL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='POL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Poland', 1, 1, 1, N'PL', N'POL', 616, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Portugal', 1, 1, 1, N'PT', N'PRT', 620, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='PRI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Puerto Rico', 1, 1, 1, N'PR', N'PRI', 630, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='QAT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='QAT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Qatar', 1, 1, 1, N'QA', N'QAT', 634, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='REU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='REU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Reunion', 1, 1, 1, N'RE', N'REU', 638, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ROM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ROM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Romania', 1, 1, 1, N'RO', N'ROM', 642, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='RUS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='RUS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Russian Federation', 1, 1, 1, N'RU', N'RUS', 643, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='RWA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='RWA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Rwanda', 1, 1, 1, N'RW', N'RWA', 646, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KNA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='KNA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Saint Kitts and Nevis', 1, 1, 1, N'KN', N'KNA', 659, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LCA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LCA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Saint Lucia', 1, 1, 1, N'LC', N'LCA', 662, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VCT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VCT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Saint Vincent and the Grenadines', 1, 1, 1, N'VC', N'VCT', 670, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='WSM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='WSM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Samoa', 1, 1, 1, N'WS', N'WSM', 882, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SMR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SMR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'San Marino', 1, 1, 1, N'SM', N'SMR', 674, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='STP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='STP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Sao Tome and Principe', 1, 1, 1, N'ST', N'STP', 678, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SAU'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SAU'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Saudi Arabia', 1, 1, 1, N'SA', N'SAU', 682, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SEN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SEN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Senegal', 1, 1, 1, N'SN', N'SEN', 686, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SYC'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SYC'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Seychelles', 1, 1, 1, N'SC', N'SYC', 690, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Sierra Leone', 1, 1, 1, N'SL', N'SLE', 694, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SGP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SGP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Singapore', 1, 1, 1, N'SG', N'SGP', 702, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SVK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SVK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Slovakia (Slovak Republic)', 1, 1, 1, N'SK', N'SVK', 703, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SVN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SVN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Slovenia', 1, 1, 1, N'SI', N'SVN', 705, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SLB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Solomon Islands', 1, 1, 1, N'SB', N'SLB', 90, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SOM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SOM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Somalia', 1, 1, 1, N'SO', N'SOM', 706, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZAF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZAF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'South Africa', 1, 1, 1, N'ZA', N'ZAF', 710, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SGS'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SGS'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'South Georgia & South Sandwich Islands', 1, 1, 1, N'GS', N'SGS', 239, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ESP'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ESP'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Spain', 1, 1, 1, N'ES', N'ESP', 724, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LKA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='LKA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Sri Lanka', 1, 1, 1, N'LK', N'LKA', 144, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SHN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SHN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'St. Helena', 1, 1, 1, N'SH', N'SHN', 654, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SPM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SPM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'St. Pierre and Miquelon', 1, 1, 1, N'PM', N'SPM', 666, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SDN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SDN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Sudan', 1, 1, 1, N'SD', N'SDN', 736, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SUR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SUR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Suriname', 1, 1, 1, N'SR', N'SUR', 740, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SJM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SJM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Svalbard and Jan Mayen Islands', 1, 1, 1, N'SJ', N'SJM', 744, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SWZ'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SWZ'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Swaziland', 1, 1, 1, N'SZ', N'SWZ', 748, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SWE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SWE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Sweden', 1, 1, 1, N'SE', N'SWE', 752, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='CHE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Switzerland', 1, 1, 1, N'CH', N'CHE', 756, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SYR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='SYR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Syrian Arab Republic', 1, 1, 1, N'SY', N'SYR', 760, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TWN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TWN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Taiwan', 1, 1, 1, N'TW', N'TWN', 158, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TJK'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TJK'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tajikistan', 1, 1, 1, N'TJ', N'TJK', 762, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TZA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TZA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tanzania', 1, 1, 1, N'TZ', N'TZA', 834, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='THA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='THA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Thailand', 1, 1, 1, N'TH', N'THA', 764, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TGO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TGO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Togo', 1, 1, 1, N'TG', N'TGO', 768, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TKL'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TKL'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tokelau', 1, 1, 1, N'TK', N'TKL', 772, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TON'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TON'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tonga', 1, 1, 1, N'TO', N'TON', 776, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TTO'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TTO'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Trinidad and Tobago', 1, 1, 1, N'TT', N'TTO', 780, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tunisia', 1, 1, 1, N'TN', N'TUN', 788, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Turkey', 1, 1, 1, N'TR', N'TUR', 792, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TKM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TKM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Turkmenistan', 1, 1, 1, N'TM', N'TKM', 795, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TCA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TCA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Turks and Caicos Islands', 1, 1, 1, N'TC', N'TCA', 796, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUV'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='TUV'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Tuvalu', 1, 1, 1, N'TV', N'TUV', 798, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UGA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UGA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Uganda', 1, 1, 1, N'UG', N'UGA', 800, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UKR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UKR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Ukraine', 1, 1, 1, N'UA', N'UKR', 804, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ARE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'United Arab Emirates', 1, 1, 1, N'AE', N'ARE', 784, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GBR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='GBR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'United Kingdom', 1, 1, 1, N'GB', N'GBR', 826, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='USA'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='USA'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'United States', 1, 1, 1, N'US', N'USA', 840, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UMI'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UMI'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'United States Minor Outlying Islands', 1, 1, 1, N'UM', N'UMI', 581, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='URY'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='URY'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Uruguay', 1, 1, 1, N'UY', N'URY', 858, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UZB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='UZB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Uzbekistan', 1, 1, 1, N'UZ', N'UZB', 860, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VUT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VUT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Vanuatu', 1, 1, 1, N'VU', N'VUT', 548, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VAT'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VAT'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Vatican City State (Holy See)', 1, 1, 1, N'VA', N'VAT', 336, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VEN'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VEN'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Venezuela', 1, 1, 1, N'VE', N'VEN', 862, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VNM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VNM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Viet Nam', 1, 1, 1, N'VN', N'VNM', 704, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VGB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VGB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Virgin Islands (British)', 1, 1, 1, N'VG', N'VGB', 92, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VIR'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='VIR'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Virgin Islands (U.S.)', 1, 1, 1, N'VI', N'VIR', 850, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='WLF'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='WLF'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Wallis and Futuna Islands', 1, 1, 1, N'WF', N'WLF', 876, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ESH'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ESH'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Western Sahara', 1, 1, 1, N'EH', N'ESH', 732, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='YEM'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='YEM'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Yemen', 1, 1, 1, N'YE', N'YEM', 887, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZMB'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZMB'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Zambia', 1, 1, 1, N'ZM', N'ZMB', 894, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END
	IF (EXISTS(SELECT [Name] FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZWE'))
	BEGIN
		SELECT @CountryId = CountryID FROM [Nop_Country] WHERE [ThreeLetterISOCode]='ZWE'
	END
	ELSE
	BEGIN
		INSERT [dbo].[Nop_Country] ([Name], [AllowsRegistration], [AllowsBilling], [AllowsShipping], [TwoLetterISOCode], [ThreeLetterISOCode], [NumericISOCode], [Published], [DisplayOrder]) VALUES (N'Zimbabwe', 1, 1, 1, N'ZW', N'ZWE', 716, 1, 100)
		SET @CountryId = SCOPE_IDENTITY()
	END

	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SystemUpgrade-2.0-NewCountries', N'true', N'')
END
GO


--downloadable products
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantLoadAll]
(
	@OrderID int,
	@CustomerID int,
	@StartTime datetime = NULL,
	@EndTime datetime = NULL,
	@OrderStatusID int,
	@PaymentStatusID int,
	@ShippingStatusID int,
	@LoadDownloableProductsOnly bit = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	SELECT
		opv.*
	FROM [Nop_OrderProductVariant] opv
	INNER JOIN [Nop_Order] o ON opv.OrderID=o.OrderID
	INNER JOIN [Nop_ProductVariant] pv ON opv.ProductVariantID=pv.ProductVariantID
	WHERE
		(@OrderID IS NULL OR @OrderID=0 or o.OrderID = @OrderID) and
		(@CustomerID IS NULL OR @CustomerID=0 or o.CustomerID = @CustomerID) and
		(@StartTime is NULL or DATEDIFF(day, @StartTime, o.CreatedOn) >= 0) and
		(@EndTime is NULL or DATEDIFF(day, @EndTime, o.CreatedOn) <= 0) and 
		(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) and
		(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) and
		(@ShippingStatusID IS NULL OR @ShippingStatusID = 0 OR o.ShippingStatusID = @ShippingStatusID) and
		((@LoadDownloableProductsOnly IS NULL OR @LoadDownloableProductsOnly = 0) OR (pv.IsDownload=1)) and
		(o.Deleted=0)		
	ORDER BY o.CreatedOn desc, [opv].OrderProductVariantID 
END
GO


-- mini-shopping cart
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.ShowMiniShoppingCart')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.ShowMiniShoppingCart', N'true', N'')
END
GO

--stored procedure fixed
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TierPriceUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TierPriceUpdate]
GO
CREATE PROCEDURE [dbo].[Nop_TierPriceUpdate]
(
	@TierPriceID int,
	@ProductVariantID int,
	@Quantity int,
	@Price money
)
AS
BEGIN
	UPDATE [Nop_TierPrice]
	SET
		ProductVariantID=@ProductVariantID,
		[Quantity]=@Quantity,
		[Price]=@Price
	WHERE
		TierPriceID = @TierPriceID
END
GO

-- Sermepa payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.Sermepa.SermepaPaymentProcessor, Nop.Payment.Sermepa')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'Sermepa', N'Sermepa', N'', N'Payment\Sermepa\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\Sermepa\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.Sermepa.SermepaPaymentProcessor, Nop.Payment.Sermepa', N'SERMEPA', 0, 250)
END
GO

-- SagePay payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.SagePay.SagePayPaymentProcessor, Nop.Payment.SagePay')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'SagePay', N'SagePay', N'', N'Payment\SagePay\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\SagePay\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.SagePay.SagePayPaymentProcessor, Nop.Payment.SagePay', N'SAGEPAY', 0, 260)
END
GO

-- QuickPay payment method
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.QuickPay.QuickPayPaymentProcessor, Nop.Payment.QuickPay')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'QuickPay Dankort', N'QuickPay Dankort', N'', N'Payment\QuickPay\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\QuickPay\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.QuickPay.QuickPayPaymentProcessor, Nop.Payment.QuickPay', N'QUICKPAY', 0, 270)
END
GO

--update current version
UPDATE [dbo].[Nop_Setting] 
SET [Value]='1.50'
WHERE [Name]='Common.CurrentVersion'
GO

