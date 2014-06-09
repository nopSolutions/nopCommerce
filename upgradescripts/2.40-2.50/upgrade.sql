--upgrade scripts from nopCommerce 2.40 to nopCommerce 2.50

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByDimensions">
    <Value>Pack by dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByOneItemPerPackage">
    <Value>Pack by one item per package</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByVolume">
    <Value>Pack by volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType">
    <Value>Packing type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType.Hint">
    <Value>Choose preferred packing type.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume">
    <Value>Package volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume.Hint">
    <Value>Enter your package volume.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.ApproveSelected">
    <Value>Approve selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.DisapproveSelected">
    <Value>Disapprove selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.All">
    <Value>Export to XML (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.Selected">
    <Value>Export to XML (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.All">
    <Value>Export to Excel (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.Selected">
    <Value>Export to Excel (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExcelFile">
    <Value>Excel file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile">
    <Value>Xml file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note1">
    <Value>NOTE: It can take up to several minutes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note2">
    <Value>NOTE: DO NOT click twice.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.CsvFile">
    <Value>CSV file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.LoseUnsavedChanges">
    <Value>You are going to lose any unsaved changes. Are you sure?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.Match">
    <Value>Specified store URL matches this store URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.NoMatch">
    <Value>Specified store URL ({0}) doesn''t match this store URL ({1})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Set">
    <Value>Primary exchange rate currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Rate1">
    <Value>Primary exchange rate currency. The rate should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.NotSet">
    <Value>Primary exchange rate currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.Set">
    <Value>Primary store currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.NotSet">
    <Value>Primary store currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Set">
    <Value>Default weight is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Ratio1">
    <Value>Default weight. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.NotSet">
    <Value>Default weight is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Set">
    <Value>Default dimension is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Ratio1">
    <Value>Default dimension. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.NotSet">
    <Value>Default dimension is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Shipping.OnlyOneOffline">
    <Value>Only one offline shipping rate computation method is recommended to use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.OK">
    <Value>Payment methods are OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.NoActive">
    <Value>You don''t have active payment methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeleteExchange">
    <Value>The primary exchange rate currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeletePrimary">
    <Value>The primary store currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.CantDeletePrimary">
    <Value>The primary weight can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.CantDeletePrimary">
    <Value>The primary dimension can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.States.CantDeleteWithAddresses">
    <Value>The state can''t be deleted. It has associated addresses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description">
    <Value>Manual plugin installation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step1">
    <Value>Upload the plugin to the /plugins folder in your nopCommerce directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step2">
    <Value>Restart your application (or click ''Reload list of plugins'' button).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step3">
    <Value>Scroll down through the list of plugins to find the newly installed plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step4">
    <Value>Click on the ''Install'' link to install the plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step5">
    <Value>Note: If you''re running nopCommerce in medium trust, then it''s recommended to clear your \Plugins\bin\ directory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing">
    <Value>Editing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing.Hint">
    <Value>This grid allows the bulk editing of the ''Friendly name'' and ''Display order'' fields. To enter edit mode just click a cell.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.Installation">
    <Value>Installation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage">
    <Value>Show on login page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage.Hint">
    <Value>Check to show CAPTCHA on login page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage">
    <Value>Show on ''email wishlist to a friend'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage.Hint">
    <Value>Check to show CAPTCHA on ''email wishlist to a friend'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage">
    <Value>Show on ''email product to a friend'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage.Hint">
    <Value>Check to show CAPTCHA on ''email product to a friend'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.Price">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.NameAsc">
    <Value>Name: A to Z</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.NameDesc">
    <Value>Name: Z to A</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.PriceAsc">
    <Value>Price: Low to High</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.PriceDesc">
    <Value>Price: High to Low</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct">
    <Value>Display wishlist after adding product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct.Hint">
    <Value>If checked, a customer will be taken to the Wishlist page immediately after adding a product to their wishlist. If unchecked, a customer will stay on the same page that they are adding the product to the wishlist from.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToTheWishlist">
    <Value>The product has been added to the wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers">
    <Value>Display shipment events</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers.Hint">
    <Value>Check if you want your customers to see shipment events on their shipment details pages (if supported by your shipping rate computation method).</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents">
    <Value>Shipment status events</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Event">
    <Value>Event</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Location">
    <Value>Location</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Date">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Departed">
    <Value>Departed</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.ExportScanned">
    <Value>Export scanned</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.OriginScanned">
    <Value>Origin scanned</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Arrived">
    <Value>Arrived</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.NotDelivered">
    <Value>Not delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Booked">
    <Value>Booked</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Delivered">
    <Value>Delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew">
    <Value>Add product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Note1">
    <Value>Click on interested product variant</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Note2">
    <Value>Do not to forget to update order totals after adding this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.UnitPriceInclTax">
    <Value>Price (incl tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.UnitPriceInclTax.Hint">
    <Value>Enter product price (incl tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.UnitPriceExclTax">
    <Value>Price (excl tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.UnitPriceExclTax.Hint">
    <Value>Enter product price (excl tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Quantity">
    <Value>Quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Quantity.Hint">
    <Value>Enter quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.SubTotalInclTax">
    <Value>Total (incl tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.SubTotalInclTax.Hint">
    <Value>Enter total (incl tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.SubTotalExclTax">
    <Value>Total (excl tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.SubTotalExclTax.Hint">
    <Value>Enter total (excl tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.ReturnRequests">
    <Value>Return request(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.GiftCards">
    <Value>Gift cards(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold">
    <Value>Products never purchased</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.RunReport">
    <Value>Run report</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Report.Summary">
    <Value>Summary</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Report.Tax">
    <Value>Tax</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Report.Total">
    <Value>Total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Report.Profit">
    <Value>Profit</Value>
  </LocaleResource>
  <LocaleResource Name="Messages.Order.Products(s).Download">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Messages.Order.Product(s).Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Search.IncludeSubCategories">
    <Value>Automatically search sub categories</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.JavaScript">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.JavaScript.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.TrackingScript">
    <Value>Tracking code with {ECOMMERCE} line</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.TrackingScript.Hint">
    <Value>Paste the tracking code generated by Google Analytics here. {GOOGLEID} and {ECOMMERCE} will be dynamically replaced.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.EcommerceScript">
    <Value>Tracking code for {ECOMMERCE} part, with {DETAILS} line</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.EcommerceScript.Hint">
    <Value>Paste the tracking code generated by Google analytics here. {ORDERID}, {SITE}, {TOTAL}, {TAX}, {SHIP}, {CITY}, {STATEPROVINCE}, {COUNTRY}, {DETAILS} will be dynamically replaced.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.EcommerceDetailScript">
    <Value>Tracking code for {DETAILS} part</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.EcommerceDetailScript.Hint">
    <Value>Paste the tracking code generated by Google analytics here. {ORDERID}, {PRODUCTSKU}, {PRODUCTNAME}, {CATEGORYNAME}, {UNITPRICE}, {QUANTITY} will be dynamically replaced.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.UploadFile">
    <Value>Please upload a file</Value>
  </LocaleResource>
  <LocaleResource Name="Polls.SelectAnswer">
    <Value>Please select an answer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductsFromSubcategories">
    <Value>Include products from subcategories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductsFromSubcategories.Hint">
    <Value>Check if you want a category details page to include products from subcategories.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskEnabled">
    <Value>Automatically generate a file</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskEnabled.Hint">
    <Value>Check if you want a file to be automatically generated.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.GenerateStaticFileEachMinutes">
    <Value>A task period (minutes)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.GenerateStaticFileEachMinutes.Hint">
    <Value>Specify a task period in minutes (generation of a new Froogle file).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskRestart">
    <Value>If a task settings (''Automatically generate a file'') have been changed, please restart the application</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.StaticFilePath">
    <Value>Generated file path (static)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.StaticFilePath.Hint">
    <Value>A file path of the generated Froogle file. It''s static for your store and can be shared with the Froogle service.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.ProductVariants.Hint">
    <Value>The comma-separated list of product variant identifiers (e.g. 77, 123, 156). You can find a product variant ID on its details page. You can also specify the comma-separated list of product variant identifiers with quantities ({Product variant ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product variant identifiers with quantity range ({Product variant ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants.Hint">
    <Value>The comma-separated list of product variant identifiers (e.g. 77, 123, 156). You can find a product variant ID on its details page. You can also specify the comma-separated list of product variant identifiers with quantities ({Product variant ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product variant identifiers with quantity range ({Product variant ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.AttributeControlType.FileUpload">
    <Value>File upload</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.MaximumUploadedFileSize">
    <Value>Maximum file size is {0} KB</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Title1">
    <Value>Add a new product to order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Title2">
    <Value>Add product ''{0}'' to order #{1}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.BackToList">
    <Value>back to product list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.BackToOrder">
    <Value>back to order details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchCompany">
    <Value>Company</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchCompany.Hint">
    <Value>Search by company.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchPhone">
    <Value>Phone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchPhone.Hint">
    <Value>Search by a phone number.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchZipCode">
    <Value>Zip code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchZipCode.Hint">
    <Value>Search by zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.LiveRates">
    <Value>Live currency rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.DropoffType">
    <Value>Dropoff Type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.DropoffType.Hint">
    <Value>Choose preferred dropoff type.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.DropoffType.BusinessServiceCenter">
    <Value>Business service center</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.DropoffType.DropBox">
    <Value>Drop box</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RegularPickup">
    <Value>Regular pickup</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RequestCourier">
    <Value>Request courier</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.DropoffType.Station">
    <Value>Station</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Avatar.MaximumUploadedFileSize">
    <Value>Maximum avatar size is {0} bytes</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.ShippingOptionCouldNotBeLoaded">
    <Value>Shipping options could not be loaded</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments">
    <Value>Shipments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ID">
    <Value>Shipment #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TrackingNumber">
    <Value>Tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate">
    <Value>Date shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate">
    <Value>Date delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate.NotYet">
    <Value>Not yet</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ViewDetails">
    <Value>View shipment details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.BackToOrder">
    <Value>back to order details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TrackingNumber.Hint">
    <Value>Set a tracking number of the current shipment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TrackingNumber.Button">
    <Value>Set tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate.Hint">
    <Value>The date this shipment was shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate.Hint">
    <Value>The date this shipment was delivered.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate.Button">
    <Value>Set as delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products">
    <Value>Products shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ProductName">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.QtyOrdered">
    <Value>Qty ordered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.QtyShipped">
    <Value>Qty shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.QtyToShip">
    <Value>Qty to ship</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AddNew">
    <Value>Add shipment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AddNew.Title">
    <Value>Add a new shipment to order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Added">
    <Value>The new shipment has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.NoProductsSelected">
    <Value>No products selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Deleted">
    <Value>The shipment has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Shipping.ShippingStatus.PartiallyShipped">
    <Value>Partially shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippedDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippedDate.Button">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippedDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippedDate.NotYet">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.DeliveryDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.DeliveryDate.Button">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.DeliveryDate.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.DeliveryDate.NotYet">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.TrackingNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.TrackingNumber.Button">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.TrackingNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShippedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.NotYetShipped">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.DeliveredOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.NotYetDelivered">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments">
    <Value>Shipments</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ID">
    <Value>Shipment #</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.TrackingNumber">
    <Value>Tracking number</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ShippedDate">
    <Value>Date shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.DeliveryDate">
    <Value>Date delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.DeliveryDate.NotYet">
    <Value>Not yet</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ViewDetails">
    <Value>View details</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.ShipmentDetails">
    <Value>Shipment details</Value>
  </LocaleResource>
  <LocaleResource Name="Order.TrackingNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Information">
    <Value>Shipment #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Order#">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ShippingMethod">
    <Value>Shipping Method</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ShippingAddress">
    <Value>Shipping Address</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Phone">
    <Value>Phone</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Fax">
    <Value>Fax</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Product(s)">
    <Value>Shipped product(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Product(s).SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Product(s).Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.Product(s).Quantity">
    <Value>Qty shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PrintPackagingSlip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip">
    <Value>Print packaging slip</Value>
  </LocaleResource>
  <LocaleResource Name="PDFPackagingSlip.Shipment">
    <Value>Shipment #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="PDFPackagingSlip.Order">
    <Value>Order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableForPreOrder">
    <Value>Available for pre-order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableForPreOrder.Hint">
    <Value>Check if this item is available for Pre-Order. It also displays "Pre-order" button instead of "Add to cart".</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.PreOrder">
    <Value>Pre-order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod">
    <Value>Manage inventory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.StockQuantity">
    <Value>Stock qty</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.MinOrderPlacementInterval">
    <Value>Please wait several seconds before placing a new order (already placed another order several seconds ago).</Value>
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
				[LanguageID],
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

--Customer currency rate issue fix
ALTER TABLE [dbo].[Order] ALTER COLUMN [CurrencyRate] decimal(18, 8) NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytierpriceswithdiscounts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.displaytierpriceswithdiscounts', N'true')
END
GO
--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fedexsettings.packingpackagevolume')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'fedexsettings.packingpackagevolume', N'5184')
END
GO


--Update stored procedure according to the sort options
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryId			int = 0,
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM Product p with (NOLOCK) 
	LEFT OUTER JOIN Product_Category_Mapping pcm with (NOLOCK) ON p.Id=pcm.ProductId
	LEFT OUTER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.Id=pmm.ProductId
	LEFT OUTER JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON p.Id=pptm.Product_Id
	LEFT OUTER JOIN ProductVariant pv with (NOLOCK) ON p.Id = pv.ProductId
	--searching of the localized values
	--comment the line below if you don't use it. It'll improve the performance
	LEFT OUTER JOIN LocalizedProperty lp with (NOLOCK) ON p.Id = lp.EntityId AND lp.LanguageId = @LanguageId AND lp.LocaleKeyGroup = N'Product'
	WHERE 
		(
		   (
				@CategoryId IS NULL OR @CategoryId=0
				OR (pcm.CategoryId=@CategoryId AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerId IS NULL OR @ManufacturerId=0
				OR (pmm.ManufacturerId=@ManufacturerId AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagId IS NULL OR @ProductTagId=0
				OR pptm.ProductTag_Id=@ProductTagId
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
				--min price
				(@PriceMin IS NULL OR @PriceMin=0)
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice >= @PriceMin)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price >= @PriceMin)
				)
			)
		AND (
				--max price
				(@PriceMax IS NULL OR @PriceMax=2147483644) -- max value
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice <= @PriceMax)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price <= @PriceMax)
				)
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
					--searching of the localized values
					--comment the lines below if you don't use it. It'll improve the performance
					or (lp.LocaleKey = N'Name' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'ShortDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'FullDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTimeUtc, '1/1/1900') and isnull(pv.AvailableEndDateTimeUtc, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionId NOT IN (
							SELECT psam.SpecificationAttributeOptionId
							FROM dbo.Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
							)
						)
					
				)
			)
		)
	ORDER BY 
		--category position
		CASE WHEN @OrderBy = 0 AND @CategoryId IS NOT NULL AND @CategoryId > 0
		THEN pcm.DisplayOrder END ASC,
		--manufacturer position
		CASE WHEN @OrderBy = 0 AND @ManufacturerId IS NOT NULL AND @ManufacturerId > 0
		THEN pmm.DisplayOrder END ASC,
		--sort by name (there's no any position if category or manufactur is not specified)
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		--Name: A to Z
		CASE WHEN @OrderBy = 5
		THEN p.[Name] END ASC, --THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END ASC,
		--Name: Z to A
		CASE WHEN @OrderBy = 6
		THEN p.[Name] END DESC, --THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END DESC,
		--Price: Low to High
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		--Price: High to Low
		CASE WHEN @OrderBy = 11
		THEN pv.Price END DESC,
		--Created on
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOnUtc END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return products (returned properties should be synchronized with 'Product' entity)
	SELECT  
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		IndexId
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.displaywishlistafteraddingproduct')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.displaywishlistafteraddingproduct', N'true')
END
GO

--more SQL indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_Deleted_and_Published' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_Deleted_and_Published] ON [dbo].[Product] 
	(
		[Published] ASC,
		[Deleted] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_Published' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_Published] ON [dbo].[Product] 
	(
		[Published] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_ShowOnHomepage' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [dbo].[Product] 
	(
		[ShowOnHomepage] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariant_ProductId_2' and object_id=object_id(N'[dbo].[ProductVariant]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId_2] ON [dbo].[ProductVariant] 
	(
		[ProductId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PCM_Product_and_Category' and object_id=object_id(N'[dbo].[Product_Category_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PCM_Product_and_Category] ON [dbo].[Product_Category_Mapping] 
	(
		[CategoryId] ASC,
		[ProductId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PMM_Product_and_Manufacturer' and object_id=object_id(N'[dbo].[Product_Manufacturer_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PMM_Product_and_Manufacturer] ON [dbo].[Product_Manufacturer_Mapping] 
	(
		[ManufacturerId] ASC,
		[ProductId] ASC
	)
END
GO



--New fast [ProductLoadAllPaged] stored procedure
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryId			int = 0,
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		SET @Keywords = isnull(@Keywords, '')
		SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'
		
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.sku) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
			
		IF @SearchDescriptions = 1 SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.ShortDescription) > 0
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.FullDescription) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.Description) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''ShortDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''FullDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
		
		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(MAX)', @Keywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	
	IF @CategoryId > 0
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
	
	IF @ShowHidden = 0
	OR @PriceMin > 0
	OR @PriceMax > 0
	OR @OrderBy = 10 /* Price: Low to High */
	OR @OrderBy = 11 /* Price: High to Low */
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ProductVariant pv with (NOLOCK)
			ON p.Id = pv.ProductId'
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
	IF @CategoryId > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId = ' + CAST(@CategoryId AS nvarchar(max))
		
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
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
		AND pv.Published = 1
		AND pv.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM dbo.Product_SpecificationAttribute_Mapping psam
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
		SET @sql_orderby = ' pv.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' pv.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryId > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
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
	
	PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredSpecs

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
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

--new shipping setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displaytrackingurltocustomers')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.displaytrackingurltocustomers', N'false')
END
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displayshipmenteventstocustomers')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.displayshipmenteventstocustomers', N'false')
END
GO

--'New order note' message template
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[MessageTemplate]
		WHERE [Name] = N'Customer.NewOrderNote')
BEGIN
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId])
	VALUES (N'Customer.NewOrderNote', null, N'%Store.Name%. New order note has been added', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />New order note has been added to your account:<br />"%Order.NewNoteText%".<br /><a target="_blank" href="%Order.OrderURLForCustomer%">%Order.OrderURLForCustomer%</a></p>', 1, 0)
END
GO


--New [ProductLoadAllPaged] stored procedure (allow searching in several categories)
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(300) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		SET @Keywords = isnull(@Keywords, '')
		SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'
		
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.sku) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
			
		IF @SearchDescriptions = 1 SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.ShortDescription) > 0
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.FullDescription) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.Description) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''ShortDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''FullDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
		
		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(MAX)', @Keywords

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
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	
	IF @ShowHidden = 0
	OR @PriceMin > 0
	OR @PriceMax > 0
	OR @OrderBy = 10 /* Price: Low to High */
	OR @OrderBy = 11 /* Price: High to Low */
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ProductVariant pv with (NOLOCK)
			ON p.Id = pv.ProductId'
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
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
		AND pv.Published = 1
		AND pv.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM dbo.Product_SpecificationAttribute_Mapping psam
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
		SET @sql_orderby = ' pv.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' pv.[Price] DESC'
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
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

--new Google Analytics setting
DELETE FROM [Setting]
WHERE [Name] = N'googleanalyticssettings.javascript'
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'googleanalyticssettings.trackingscript')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'googleanalyticssettings.trackingscript', N'<!-- Google code for Analytics tracking -->
<script type=""text/javascript"">
var _gaq = _gaq || [];
_gaq.push([''_setAccount'', ''{GOOGLEID}'']);
_gaq.push([''_trackPageview'']);
{ECOMMERCE}
(function() {
    var ga = document.createElement(''script''); ga.type = ''text/javascript''; ga.async = true;
    ga.src = (''https:'' == document.location.protocol ? ''https://ssl'' : ''http://www'') + ''.google-analytics.com/ga.js'';
    var s = document.getElementsByTagName(''script'')[0]; s.parentNode.insertBefore(ga, s);
})();
</script>')
END
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'googleanalyticssettings.ecommercescript')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'googleanalyticssettings.ecommercescript', N'_gaq.push([''_addTrans'', ''{ORDERID}'', ''{SITE}'', ''{TOTAL}'', ''{TAX}'', ''{SHIP}'', ''{CITY}'', ''{STATEPROVINCE}'', ''{COUNTRY}'']);
{DETAILS} 
_gaq.push([''_trackTrans'']); ')
END
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'googleanalyticssettings.ecommercedetailscript')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'googleanalyticssettings.ecommercedetailscript', N'_gaq.push([''_addItem'', ''{ORDERID}'', ''{PRODUCTSKU}'', ''{PRODUCTNAME}'', ''{CATEGORYNAME}'', ''{UNITPRICE}'', ''{QUANTITY}'' ]); ')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showproductsfromsubcategories')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.showproductsfromsubcategories', N'false')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.staticfilename')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'frooglesettings.staticfilename', N'froogle_' + CAST(CAST(RAND() * 1000000000 AS INT) AS NVARCHAR) + N'.xml')
END
GO
--'Froogle static file generation' schedule task (disabled by default)
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ScheduleTask]
		WHERE [Name] = N'Froogle static file generation')
BEGIN
	INSERT [dbo].[ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError])
	VALUES (N'Froogle static file generation', 3600, N'Nop.Plugin.Feed.Froogle.StaticFileGenerationTask, Nop.Plugin.Feed.Froogle', 0, 0)
END
GO


--more SQL indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PSAM_AllowFiltering' and object_id=object_id(N'[dbo].[Product_SpecificationAttribute_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PSAM_AllowFiltering] ON [dbo].[Product_SpecificationAttribute_Mapping] 
	(
		[AllowFiltering] ASC
	)
	INCLUDE ([ProductId],[SpecificationAttributeOptionId])
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PSAM_SpecificationAttributeOptionId_AllowFiltering' and object_id=object_id(N'[dbo].[Product_SpecificationAttribute_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PSAM_SpecificationAttributeOptionId_AllowFiltering] ON [dbo].[Product_SpecificationAttribute_Mapping] 
	(
		[SpecificationAttributeOptionId] ASC,
		[AllowFiltering] ASC
	)
	INCLUDE ([ProductId])
END
GO




--Add 'Guid' column to [Download] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Download]') and NAME='DownloadGuid')
BEGIN
	ALTER TABLE [dbo].[Download]
	ADD [DownloadGuid] uniqueidentifier NULL
END
GO

UPDATE [dbo].[Download]
SET [DownloadGuid] = NEWID()
WHERE [DownloadGuid] IS NULL
GO

ALTER TABLE [dbo].[Download] ALTER COLUMN [DownloadGuid] uniqueidentifier NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.fileuploadmaximumsizebytes')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.fileuploadmaximumsizebytes', N'204800')
END
GO

--delete old
DELETE FROM [Setting] 
WHERE [Name] = N'catalogsettings.ensurewehavefilterablespecattributes'
GO





--Update stored procedure according to the new search parameters (return filterable specs)
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(300) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(100) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
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
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		SET @Keywords = isnull(@Keywords, '')
		SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'
		
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.sku) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
			
		IF @SearchDescriptions = 1 SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.ShortDescription) > 0
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.FullDescription) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.Description) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''ShortDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''FullDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
		
		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(MAX)', @Keywords

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
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	
	IF @ShowHidden = 0
	OR @PriceMin > 0
	OR @PriceMax > 0
	OR @OrderBy = 10 /* Price: Low to High */
	OR @OrderBy = 11 /* Price: High to Low */
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ProductVariant pv with (NOLOCK)
			ON p.Id = pv.ProductId'
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
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
		AND pv.Published = 1
		AND pv.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM dbo.Product_SpecificationAttribute_Mapping psam
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
		SET @sql_orderby = ' pv.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' pv.[Price] DESC'
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
		FROM [Product_SpecificationAttribute_Mapping] [psam]
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(1000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO


--Add new columns to [ScheduleTask] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ScheduleTask]') and NAME='LastStartUtc')
BEGIN
	ALTER TABLE [dbo].[ScheduleTask]
	ADD [LastStartUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ScheduleTask]') and NAME='LastEndUtc')
BEGIN
	ALTER TABLE [dbo].[ScheduleTask]
	ADD [LastEndUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ScheduleTask]') and NAME='LastSuccessUtc')
BEGIN
	ALTER TABLE [dbo].[ScheduleTask]
	ADD [LastSuccessUtc] datetime NULL
END
GO

--new 'FedEx' plugin setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fedexsettings.dropofftype')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'fedexsettings.dropofftype', N'0')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.returnvalidoptionsifthereareany')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.returnvalidoptionsifthereareany', N'true')
END
GO


--new shipment functionality
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shipment]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Shipment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] int NOT NULL,
	[TrackingNumber] [nvarchar](max) NULL,
	[ShippedDateUtc] [datetime] NOT NULL,
	[DeliveryDateUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) 
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Shipment_Order'
           AND parent_object_id = Object_id('Shipment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Shipment
DROP CONSTRAINT Shipment_Order
GO
ALTER TABLE [dbo].[Shipment]  WITH CHECK ADD  CONSTRAINT [Shipment_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
ON DELETE CASCADE
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shipment_OrderProductVariant]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Shipment_OrderProductVariant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentId] int NOT NULL,
	[OrderProductVariantId] int NOT NULL,
	[Quantity] int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ShipmentOrderProductVariant_Shipment'
           AND parent_object_id = Object_id('Shipment_OrderProductVariant')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Shipment_OrderProductVariant
DROP CONSTRAINT ShipmentOrderProductVariant_Shipment
GO
ALTER TABLE [dbo].[Shipment_OrderProductVariant]  WITH CHECK ADD  CONSTRAINT [ShipmentOrderProductVariant_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO

--new message template
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[MessageTemplate]
		WHERE [Name] = N'ShipmentSent.CustomerNotification')
BEGIN
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId])
	VALUES (N'ShipmentSent.CustomerNotification', null, N'Your order from %Store.Name% has been shipped.', N'<p><a href="%Store.URL%"> %Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%!, <br />Good news! You order has been shipped. <br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod% <br /> <br /> Shipped Products: <br /> <br /> %Shipment.Product(s)%</p>', 1, 0)
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[MessageTemplate]
		WHERE [Name] = N'ShipmentDelivered.CustomerNotification')
BEGIN
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId])
	VALUES (N'ShipmentDelivered.CustomerNotification', null, N'Your order from %Store.Name% has been delivered.', N'<p><a href="%Store.URL%"> %Store.Name%</a> <br /> <br /> Hello %Order.CustomerFullName%, <br /> Good news! You order has been delivered. <br /> Order Number: %Order.OrderNumber%<br /> Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br /> Date Ordered: %Order.CreatedOn%<br /> <br /> <br /> <br /> Billing Address<br /> %Order.BillingFirstName% %Order.BillingLastName%<br /> %Order.BillingAddress1%<br /> %Order.BillingCity% %Order.BillingZipPostalCode%<br /> %Order.BillingStateProvince% %Order.BillingCountry%<br /> <br /> <br /> <br /> Shipping Address<br /> %Order.ShippingFirstName% %Order.ShippingLastName%<br /> %Order.ShippingAddress1%<br /> %Order.ShippingCity% %Order.ShippingZipPostalCode%<br /> %Order.ShippingStateProvince% %Order.ShippingCountry%<br /> <br /> Shipping Method: %Order.ShippingMethod% <br /> <br /> Delivered Products: <br /> <br /> %Shipment.Product(s)%</p>', 1, 0)
END
GO
--delete old shipping message templates
DELETE FROM [MessageTemplate] 
WHERE [Name] = N'OrderShipped.CustomerNotification'
GO
DELETE FROM [MessageTemplate] 
WHERE [Name] = N'OrderDelivered.CustomerNotification'
GO

--create shipments for the previous orders
IF (EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='ShippedDateUtc')
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='DeliveryDateUtc')
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='TrackingNumber'))
BEGIN
	EXEC('
	DECLARE @OrderId int
	DECLARE cur_order CURSOR FOR
	SELECT [Id]
	FROM [Order]
	ORDER BY [Id]
	OPEN cur_order
	FETCH NEXT FROM cur_order INTO @OrderId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--shipping status
		DECLARE @ShippingStatusId int
		SET @ShippingStatusId = null -- clear cache (variable scope)
		SELECT @ShippingStatusId = [ShippingStatusId] FROM [Order] WHERE [Id]=@OrderId
		--is order already shipped or delivered?
		IF (@ShippingStatusId = 30 OR @ShippingStatusId = 40)
		BEGIN
			--select shippable order product variant identifiers
			CREATE TABLE #OrderedProductVariants 
			(
				[Id] int NOT NULL,
				[Quantity] int NOT NULL
			)
			INSERT INTO #OrderedProductVariants ([Id], [Quantity])
			SELECT opv.[Id], opv.[Quantity] FROM [Order] o
				JOIN [OrderProductVariant] opv ON o.[Id] = opv.[OrderId]
				JOIN [ProductVariant] pv ON opv.[ProductVariantId] = pv.[Id]
			WHERE o.[Id] = @OrderID AND pv.[IsShipEnabled] = 1

			
			DECLARE @HasShippableProducts bit
			SET @HasShippableProducts = null -- clear cache (variable scope)
			SELECT @HasShippableProducts = COUNT(1) FROM #OrderedProductVariants
			IF @HasShippableProducts = 1
			BEGIN
				--tracking number
				DECLARE @TrackingNumber nvarchar(MAX)
				SET @TrackingNumber = null -- clear cache (variable scope)
				SELECT @TrackingNumber = [TrackingNumber] FROM [Order] WHERE [Id]=@OrderId
				--shipped date
				DECLARE @ShippedDateUtc datetime
				SET @ShippedDateUtc = null -- clear cache (variable scope)
				SELECT @ShippedDateUtc = [ShippedDateUtc] FROM [Order] WHERE [Id]=@OrderId
				IF (@ShippedDateUtc is null)
				BEGIN
					SELECT @ShippedDateUtc = [CreatedOnUtc] FROM [Order] WHERE [Id]=@OrderId
				END
				--delivery date
				DECLARE @DeliveryDateUtc datetime
				SET @DeliveryDateUtc = null -- clear cache (variable scope)
				SELECT @DeliveryDateUtc = [DeliveryDateUtc] FROM [Order] WHERE [Id]=@OrderId

				--insert shipment
				DECLARE @ShipmentId int
				SET @ShipmentId = null -- clear cache (variable scope)
				INSERT INTO [Shipment] ([OrderId], [TrackingNumber], [ShippedDateUtc], [DeliveryDateUtc])
				VALUES (@OrderId, @TrackingNumber, @ShippedDateUtc, @DeliveryDateUtc)
				SET @ShipmentId = @@IDENTITY

				--now insert shipment order product variants
				INSERT INTO [Shipment_OrderProductVariant] ([ShipmentId], [OrderProductVariantId], [Quantity])
				SELECT @ShipmentId, [Id], [Quantity]
				FROM #OrderedProductVariants
			END

			DROP TABLE #OrderedProductVariants
		END
	
		--fetch next identifier
		FETCH NEXT FROM cur_order INTO @OrderId
	END
	CLOSE cur_order
	DEALLOCATE cur_order
	')
END
GO

--drop old column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='ShippedDateUtc')
BEGIN
	ALTER TABLE [dbo].[Order] DROP COLUMN [ShippedDateUtc]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='DeliveryDateUtc')
BEGIN
	ALTER TABLE [dbo].[Order] DROP COLUMN [DeliveryDateUtc]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Order]') and NAME='TrackingNumber')
BEGIN
	ALTER TABLE [dbo].[Order] DROP COLUMN [TrackingNumber]
END
GO


--"pre-order" support
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ProductVariant]') and NAME='AvailableForPreOrder')
BEGIN
	ALTER TABLE [dbo].[ProductVariant]
	ADD [AvailableForPreOrder] bit NULL
END
GO

UPDATE [dbo].[ProductVariant]
SET [AvailableForPreOrder] = 0
WHERE [AvailableForPreOrder] IS NULL
GO

ALTER TABLE [dbo].[ProductVariant] ALTER COLUMN [AvailableForPreOrder] bit NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.minimumorderplacementinterval')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'ordersettings.minimumorderplacementinterval', N'30')
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'commonsettings.enablehttpcompression'
GO