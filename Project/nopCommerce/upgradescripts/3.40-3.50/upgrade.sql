--upgrade scripts from nopCommerce 3.40 to 3.50

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.Store">
    <Value>Limited to store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.Store.Hint">
    <Value>Choose a store which subscribers will get this email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Plugins.Saved">
    <Value>The plugin has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.List.SearchName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.List.SearchName.Hint">
    <Value>A vendor name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Incomplete.View">
    <Value>view all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInTopMenu">
    <Value>Include in top menu</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInTopMenu.Hint">
    <Value>Check to include this topic in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots">
    <Value>Dots are not supported by tags.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ExportToCsv">
    <Value>Export states to CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ImportFromCsv">
    <Value>Import states from CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ImportSuccess">
    <Value>{0} states have been successfully imported</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.DisplayHowMuchWillBeEarned">
    <Value>Display how much will be earned</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.DisplayHowMuchWillBeEarned.Hint">
    <Value>Check to display how much point will be earned before checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.RewardPoints.WillEarn">
    <Value>You will earn</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.RewardPoints.WillEarn.Point">
    <Value>{0} points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MultipleThumbDirectories">
    <Value>Multiple thumb directories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MultipleThumbDirectories.Hint">
    <Value>Check to enable multiple thumb directories. It can be helpful if your hosting company has some limitations to the number of allowed files per directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CacheProductPrices.Hint">
    <Value>Check to cache product prices. It can significantly improve performance. But you not should enable it if you use some complex discounts, discount requirement rules, or coupon codes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountedQuantity">
    <Value>Maximum discounted quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountedQuantity.Hint">
    <Value>Maximum product quantity which could be discounted. For example, you can have two products (the same) in the cart but only one of them will be discounted. It can be used for scenarios like "buy 2 get 1 free". Leave empty if any quantity could be discounted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomValues">
    <Value>Custom values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomValues.Hint">
    <Value>Custom values from the payment method.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShipSeparately">
    <Value>Ship separately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShipSeparately.Hint">
    <Value>Check to mark a product as being able to be shipped by itself in a single box (separate shipment). This way shipping rates are calculated separately for this product regardless of what other products are also in the cart. Please note that if you have several quantities of this product in the cart, then all of them will be packed and shipped in a single box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ShipSeparately">
    <Value>this product should be shipped separately!</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ShipSeparately.Warning">
    <Value>Warning: </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountryRequired">
    <Value>''Country'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountryRequired.Hint">
    <Value>Check if ''Country'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StateProvinceRequired">
    <Value>''State/province'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StateProvinceRequired.Hint">
    <Value>Check if ''State/province'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Address.SelectState">
    <Value>Select state</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Country.Required">
    <Value>Country is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.StateProvince.Required">
    <Value>State / province is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.SelectState">
    <Value>Select state</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UseMultipleWarehouses">
    <Value>Use multiple warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UseMultipleWarehouses.Hint">
    <Value>Check if you want to support shipping and inventory management from multiple warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory">
    <Value>Warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Hint">
    <Value>Manage inventory per warehouse.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.Warehouse.NotDefined">
    <Value>No warehouses defined</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.WarehouseUsed">
    <Value>Use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.NotAvailabe">
    <Value>No warehouses available. You cannot ship this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.ChooseQty">
    <Value>{0} ({1} qty in stock, {2} qty reserved)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description1">
    <Value>"Stock quantity" is total quantity. It''s reduced when a shipment is shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description2">
    <Value>"Reserved qty" is product quantity that is ordered but not shipped yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description3">
    <Value>"Planned qty" is product quantity that is ordered and already added to a shipment but not shipped yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Warehouses.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Warehouses.Fields.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.ReservedQuantity">
    <Value>Reserved qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.PlannedQuantity">
    <Value>Planned qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds.AddNew">
    <Value>Add required product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products.AddNew">
    <Value>Add product</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products.AddNew">
    <Value>Add product</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.AttributeType">
    <Value>Attribute type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.AttributeType.Hint">
    <Value>Choose attribute type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.Value">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue.Hint">
    <Value>Custom value (text, hyperlink, etc).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.Option">
    <Value>Option</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.CustomText">
    <Value>Custom text</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.CustomHtmlText">
    <Value>Custom HTML text</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.Hyperlink">
    <Value>Hyperlink</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist">
    <Value>Display tax/shipping info (wishlist)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist.Hint">
    <Value>Check to display tax and shipping info on the wishlist page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage">
    <Value>Display tax/shipping info (order details page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage.Hint">
    <Value>Check to display tax and shipping info on the order details page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Order.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Order.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyName">
    <Value>Company name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyName.Hint">
    <Value>Enter your company name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyAddress">
    <Value>Company address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyAddress.Hint">
    <Value>Enter your company address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyPhoneNumber">
    <Value>Company phone number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyPhoneNumber.Hint">
    <Value>Enter your company phone number.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyVat">
    <Value>Company VAT</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyVat.Hint">
    <Value>Enter your company VAT (the European Union Value Added Tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Product.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByNumberOfOrders">
    <Value>Customers by number of orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByOrderTotal">
    <Value>Customers by order total</Value>
  </LocaleResource>
  <LocaleResource Name="Order.PaymentMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment">
    <Value>Payment</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment.Method">
    <Value>Payment Method</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment.Status">
    <Value>Payment Status</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShippingMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping">
    <Value>Shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping.Name">
    <Value>Shipping Method</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping.Status">
    <Value>Shipping Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee">
    <Value>"Pick Up in Store" fee</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee.Hint">
    <Value>Specify "Pick Up in Store" fee.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStoreAndFee">
    <Value>In-Store Pickup ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow">
    <Value>Notify admin for quantity below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value>When the current stock quantity falls below (reaches) this quantity, a store owner will receive a notification.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes">
	<Value>Custom address attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.AddNew">
	<Value>Add a new address attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.BackToList">
	<Value>back to address attribute list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Description">
	<Value>If the default form fields are not enough for your needs, then you can manage additional address attributes below.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.EditAttributeDetails">
	<Value>Edit address attribute details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Info">
	<Value>Attribute info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Added">
	<Value>The new attribute has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Deleted">
	<Value>The attribute has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Updated">
	<Value>The attribute has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.Name.Hint">
	<Value>The name of the address attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.IsRequired">
	<Value>Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.IsRequired.Hint">
	<Value>When an attribute is required, the address must choose an appropriate attribute value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.AttributeControlType">
	<Value>Control type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.AttributeControlType.Hint">
	<Value>Choose how to display your attribute values.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.DisplayOrder.Hint">
	<Value>The address attribute display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values">
	<Value>Attribute values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.AddNew">
	<Value>Add a new address value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.EditValueDetails">
	<Value>Edit address value details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.SaveBeforeEdit">
	<Value>You need to save the address attribute before you can add values for this address attribute page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.Name.Hint">
	<Value>The name of the address value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.IsPreSelected">
	<Value>Pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.IsPreSelected.Hint">
	<Value>Determines whether this attribute value is pre selected for the address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.DisplayOrder.Hint">
	<Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CashOnDelivery.ShippableProductRequired">
    <Value>Shippable product required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CashOnDelivery.ShippableProductRequired.Hint">
    <Value>An option indicating whether shippable products are required in order to display this payment method during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.ShippableProductRequired">
    <Value>Shippable product required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.ShippableProductRequired.Hint">
    <Value>An option indicating whether shippable products are required in order to display this payment method during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.ShippableProductRequired">
    <Value>Shippable product required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.ShippableProductRequired.Hint">
    <Value>An option indicating whether shippable products are required in order to display this payment method during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsRental">
    <Value>Is rental</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsRental.Hint">
    <Value>Check if this is a rental product (price is set for some period). Please note that inventory management is not fully supported for rental products yet. It''s recommended to set ''Manage inventory method'' to ''Don''t track inventory'' now.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RentalPriceLength">
    <Value>Rental period length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RentalPriceLength.Hint">
    <Value>Specify period length for rental product. Price is specified for this period.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RentalPricePeriod">
    <Value>Rental period</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RentalPricePeriod.Hint">
    <Value>Specify period for rental product. Price is specified for this period.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.RentalPricePeriod.Days">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.RentalPricePeriod.Weeks">
    <Value>Weeks</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.RentalPricePeriod.Months">
    <Value>Months</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.RentalPricePeriod.Years">
    <Value>Years</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.Rental.Days">
    <Value>{0} per {1} day(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.Rental.Weeks">
    <Value>{0} per {1} week(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.Rental.Months">
    <Value>{0} per {1} month(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.Rental.Years">
    <Value>{0} per {1} year(s)</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rent">
    <Value>Rent</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Price.RentalPrice">
    <Value>Rental price</Value>
  </LocaleResource>
  <LocaleResource Name="Products.RentalStartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Products.RentalEndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.EnterEndDate">
    <Value>Enter rental end date</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.EnterStartDate">
    <Value>Enter rental start date</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.StartDateLessEndDate">
    <Value>Rental start date should be less than end date</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.FormattedDate">
    <Value>Start date: {0}. End date: {1}.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Rental.FormattedDate">
    <Value>Start date: {0}. End date: {1}.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Login.Fields.Email.Required">
    <Value>Please enter your email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PurchaseOrderNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PurchaseOrderNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.PurchaseOrderNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.PurchaseOrderNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.ChooseQty">
    <Value>{0} ({1} qty in stock, {2} qty reserved, {3} qty planned)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.QuantityNotEnough">
    <Value>[Reserved - Planned] quantity value of some products are less than specified quantity to be shipped. Are you sure?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.LoadMode">
    <Value>Load mode</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.LoadMode.Hint">
    <Value>Search by a load mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.InstalledOnly">
    <Value>Installed</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.NotInstalledOnly">
    <Value>Not installed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Group">
    <Value>Group</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Group.Hint">
    <Value>Search by a group.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices">
    <Value>Telecommunications, broadcasting and electronic services</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices.Hint">
    <Value>Check if it''s telecommunications, broadcasting and electronic services. It''s used for tax caclulation in Europe Union.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.AddTitle">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Description2">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Attributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.GTIN">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.GTIN.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.OverriddenPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.OverriddenPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Sku">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Sku.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.GenerateAll">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.Attribute">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.AttributeControlType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.IsRequired">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.TextPrompt">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.DefaultValue">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.DefaultValue.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileAllowedExtensions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileAllowedExtensions.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileMaximumSize">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.FileMaximumSize.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MaxLength">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MaxLength.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MinLength">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.MinLength.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.ValidationRules.ViewLink">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.BackToProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.EditAttributeDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.EditValueDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.AssociatedProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.AssociatedProduct.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.AssociatedProduct.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.AttributeValueType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.AttributeValueType.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Cost">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Cost.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Picture">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Picture.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Picture.NoPicture">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Quantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Quantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.ViewLink">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.NoAttributesAvailable">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.SaveBeforeEdit">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes">
    <Value>Product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations">
    <Value>Attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations">
    <Value>Attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AddNew">
    <Value>Add combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AddTitle">
    <Value>Select new combination and enter details below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Description">
    <Value>Attribute combinations are useful when your ''Manage inventory method'' is set to ''Track inventory by product attributes''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Description2">
    <Value>Also note that some attribute control types that support custom user input (e.g. file upload, textboxes, date picker) are useless with attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders">
    <Value>Allow out of stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders.Hint">
    <Value>A value indicating whether to allow orders when out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.GTIN">
    <Value>GTIN</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.GTIN.Hint">
    <Value>Enter global trade item number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.ManufacturerPartNumber">
    <Value>Manufacturer part number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.ManufacturerPartNumber.Hint">
    <Value>The manufacturer''s part number for this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow">
    <Value>Notify admin for quantity below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value>When the current stock quantity falls below (reaches) this quantity, a store owner will receive a notification.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenPrice">
    <Value>Overridden price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenPrice.Hint">
    <Value>Override price for this attribute combination. This way a store owner can override the default product price when this attribute combination is added to the cart. For example, you can give a discount this way. Leave empty to ignore field. All other applied discounts will be ignored when this field is specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku">
    <Value>Sku</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Hint">
    <Value>Product stock keeping unit (SKU). Your internal unique identifier that can be used to track this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.StockQuantity.Hint">
    <Value>The current stock quantity of this combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.GenerateAll">
    <Value>Generate all possible combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Fields.Attribute">
    <Value>Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Fields.AttributeControlType">
    <Value>Control type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Fields.IsRequired">
    <Value>Is Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Fields.TextPrompt">
    <Value>Text prompt</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules">
    <Value>Validation rules</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue">
    <Value>Default value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue.Hint">
    <Value>Enter default value for attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions">
    <Value>Allowed file extensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions.Hint">
    <Value>Specify a comma-separated list of allowed file extensions. Leave empty to allow any file extension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize">
    <Value>Maximum file size (KB)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize.Hint">
    <Value>Specify maximum file size in kilobytes. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength">
    <Value>Maximum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength.Hint">
    <Value>Specify maximum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength">
    <Value>Minimum length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength.Hint">
    <Value>Specify minimum length. Leave empty to skip this validation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.ViewLink">
    <Value>View/Edit rules</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values">
    <Value>Values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.AddNew">
    <Value>Add a new value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.BackToProduct">
    <Value>back to product details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.EditAttributeDetails">
    <Value>Add/Edit values for [{0}] attribute. Product: {1}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.EditValueDetails">
    <Value>Edit value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct">
    <Value>Associated product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.AddNew">
    <Value>Associate a product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Hint">
    <Value>Associated product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AttributeValueType">
    <Value>Attribute value type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AttributeValueType.Hint">
    <Value>Choose your attribute value type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ColorSquaresRgb">
    <Value>RGB color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value>Choose color to be used with the color squares attribute control.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Cost">
    <Value>Cost</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Cost.Hint">
    <Value>The attribute value cost is the cost of all the different components which make up this value. This may be either the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.DisplayOrder.Hint">
    <Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.IsPreSelected">
    <Value>Is pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre selected for the customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name.Hint">
    <Value>The attribute value name e.g. ''Blue'' for Color attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.Hint">
    <Value>Choose a picture associated to this attribute value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.NoPicture">
    <Value>No picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustment">
    <Value>Price adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustment.Hint">
    <Value>The price adjustment applied when choosing this attribute value e.g. ''10'' to add 10 dollars.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity">
    <Value>Product quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1">
    <Value>Quantity should be greater than or equal to 1</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity.Hint">
    <Value>Specify quantity of the associated product which will be added. Minimum allowed value is 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.WeightAdjustment">
    <Value>Weight adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.WeightAdjustment.Hint">
    <Value>The weight adjustment applied when choosing this attribute value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.ViewLink">
    <Value>View/Edit values (Total: {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.NoAttributesAvailable">
    <Value>No product attributes available. Create at least one product attribute before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.SaveBeforeEdit">
    <Value>You need to save the product before you can add attributes for this page.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfo">
    <Value>Pass shipping info</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PassShippingInfo.Hint">
    <Value>Check if you want to include shipping information (weight) in generated XML file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload">
    <Value>Attached static file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload.Hint">
    <Value>Upload a static file you want to attach to each sent email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload.Exists">
    <Value>Has attached file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Local">
    <Value>Local plugins</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed">
    <Value>All plugins and themes directory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Category">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Category.Hint">
    <Value>Search by category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Name.Hint">
    <Value>Search by name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Price.Free">e
    <Value>Free</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Price.Hint">
    <Value>Search by price.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Price.Commercial">
    <Value>Commercial</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Version">
    <Value>Version</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Version.Hint">
    <Value>Search by version.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.SupportedVersions">
    <Value>Supported versions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SentOn.NotSent">
    <Value>Not sent yet</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotShipped">
    <Value>Load not shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotShipped.Hint">
    <Value>Load only not shipped shipments.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderPaidEmail">
    <Value>Attach PDF invoice ("order paid" email)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderPaidEmail.Hint">
    <Value>Check to attach PDF invoice to the "order paid" email sent to a customer.</Value>
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


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Campaign]') and NAME='StoreId')
BEGIN
	ALTER TABLE [Campaign]
	ADD [StoreId] int NULL
END
GO

     
UPDATE [Campaign]
SET [StoreId] = 0
WHERE [StoreId] IS NULL
GO

ALTER TABLE [Campaign] ALTER COLUMN [StoreId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='IncludeInTopMenu')
BEGIN
	ALTER TABLE [Topic]
	ADD [IncludeInTopMenu] bit NULL
END
GO

UPDATE [Topic]
SET [IncludeInTopMenu] = 0
WHERE [IncludeInTopMenu] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [IncludeInTopMenu] bit NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.ignorelogwordlist')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.ignorelogwordlist', N'', 0)
END
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.displayhowmuchwillbeearned')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.displayhowmuchwillbeearned', N'true', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Affiliate]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Affiliate]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='MaximumDiscountedQuantity')
BEGIN
	ALTER TABLE [Discount]
	ADD [MaximumDiscountedQuantity] int NULL
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='ShipSeparately')
BEGIN
	ALTER TABLE [Product]
	ADD [ShipSeparately] bit NULL
END
GO

UPDATE [Product]
SET [ShipSeparately] = 0
WHERE [ShipSeparately] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [ShipSeparately] bit NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countryrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countryrequired', N'false', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.stateprovincerequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.stateprovincerequired', N'false', 0)
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='UseMultipleWarehouses')
BEGIN
	ALTER TABLE [Product]
	ADD [UseMultipleWarehouses] bit NULL
END
GO

UPDATE [Product]
SET [UseMultipleWarehouses] = 0
WHERE [UseMultipleWarehouses] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [UseMultipleWarehouses] bit NOT NULL
GO


--new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductWarehouseInventory]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ProductWarehouseInventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[WarehouseId] [int] NOT NULL,
	[StockQuantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductWarehouseInventory_Product'
           AND parent_object_id = Object_id('ProductWarehouseInventory')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.ProductWarehouseInventory
DROP CONSTRAINT ProductWarehouseInventory_Product
GO
ALTER TABLE [dbo].[ProductWarehouseInventory]  WITH CHECK ADD  CONSTRAINT [ProductWarehouseInventory_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductWarehouseInventory_Warehouse'
           AND parent_object_id = Object_id('ProductWarehouseInventory')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.ProductWarehouseInventory
DROP CONSTRAINT ProductWarehouseInventory_Warehouse
GO
ALTER TABLE [dbo].[ProductWarehouseInventory]  WITH CHECK ADD  CONSTRAINT [ProductWarehouseInventory_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShipmentItem]') and NAME='WarehouseId')
BEGIN
	ALTER TABLE [ShipmentItem]
	ADD [WarehouseId] int NULL
END
GO

UPDATE [ShipmentItem]
SET [WarehouseId] = 0
WHERE [WarehouseId] IS NULL
GO

ALTER TABLE [ShipmentItem] ALTER COLUMN [WarehouseId] int NOT NULL
GO

UPDATE [ShipmentItem]
SET [WarehouseId] = COALESCE((SELECT p.[WarehouseId] FROM [Product] p
INNER JOIN [OrderItem] oi ON p.[Id] = oi.[ProductId]
WHERE [oi].[Id] = [ShipmentItem].[OrderItemId]), 0)
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
	@ParentGroupedProductId	int = 0,
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
	
	--filter by parent grouped product identifer
	IF @ParentGroupedProductId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.ParentGroupedProductId = ' + CAST(@ParentGroupedProductId AS nvarchar(max))
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
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
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
		
		--parent grouped product specified (sort associated products)
		IF @ParentGroupedProductId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' p.[DisplayOrder] ASC'
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


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Warehouse]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Warehouse]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductWarehouseInventory]') and NAME='ReservedQuantity')
BEGIN
	ALTER TABLE [ProductWarehouseInventory]
	ADD [ReservedQuantity] int NULL
END
GO

UPDATE [ProductWarehouseInventory]
SET [ReservedQuantity] = 0
WHERE [ReservedQuantity] IS NULL
GO

ALTER TABLE [ProductWarehouseInventory] ALTER COLUMN [ReservedQuantity] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.loadallsidecategorymenusubcategories')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.loadallsidecategorymenusubcategories', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_SpecificationAttribute_Mapping]') and NAME='AttributeTypeId')
BEGIN
	ALTER TABLE [Product_SpecificationAttribute_Mapping]
	ADD [AttributeTypeId] int NULL
END
GO

--"custom text" attribute type (if "CustomValue" column is specified)
UPDATE [Product_SpecificationAttribute_Mapping]
SET [AttributeTypeId] = 10
WHERE [AttributeTypeId] IS NULL AND LEN([CustomValue]) > 0
GO

UPDATE [Product_SpecificationAttribute_Mapping]
SET [AttributeTypeId] = 0
WHERE [AttributeTypeId] IS NULL
GO

ALTER TABLE [Product_SpecificationAttribute_Mapping] ALTER COLUMN [AttributeTypeId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfowishlist')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfowishlist', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoorderdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoorderdetailspage', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyName')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyName] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyAddress')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyAddress] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyPhoneNumber')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyPhoneNumber] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyVat')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyVat] nvarchar(1000) NULL
END
GO


--'Order paid' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderPaid.CustomerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'OrderPaid.CustomerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href="%Store.URL%">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>', 0, 0, 0)
END
GO

--delete a setting
DELETE FROM [Setting]
WHERE [name] = N'commonsettings.sitemapincludetopics'
GO


--'Order paid' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderPaid.VendorNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'OrderPaid.VendorNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Order #%Order.OrderNumber% has been just paid. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%<br /><br />%Order.Product(s)%</p>', 0, 0, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.richeditoradditionalsettings')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.richeditoradditionalsettings', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.pickupinstorefee')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.pickupinstorefee', N'0', 0)
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductVariantAttributeCombination]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='NotifyAdminForQuantityBelow')
	BEGIN
		EXEC ('ALTER TABLE [ProductVariantAttributeCombination] ADD [NotifyAdminForQuantityBelow] int NULL')

		EXEC ('UPDATE [ProductVariantAttributeCombination] SET [NotifyAdminForQuantityBelow] = 1 WHERE [NotifyAdminForQuantityBelow] IS NULL')
		
		EXEC ('ALTER TABLE [ProductVariantAttributeCombination] ALTER COLUMN [NotifyAdminForQuantityBelow] int NOT NULL')
	END
END
GO

--'Quantity below' message template for attribute combinations
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'QuantityBelow.AttributeCombination.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'QuantityBelow.AttributeCombination.StoreOwnerNotification', null, N'%Store.Name%. Quantity below notification. %Product.Name%', N'<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Product.Name% (ID: %Product.ID%) low quantity. <br />%AttributeCombination.Formatted%<br />Quantity: %AttributeCombination.StockQuantity%<br /></p>', 1, 0, 0)
END
GO



--address attributes
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[AddressAttribute]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[AddressAttribute](
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

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[AddressAttributeValue]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[AddressAttributeValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddressAttributeId] [int] NOT NULL,
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
           WHERE  name = 'AddressAttributeValue_AddressAttribute'
           AND parent_object_id = Object_id('AddressAttributeValue')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.AddressAttributeValue
DROP CONSTRAINT AddressAttributeValue_AddressAttribute
GO
ALTER TABLE [dbo].[AddressAttributeValue]  WITH CHECK ADD  CONSTRAINT [AddressAttributeValue_AddressAttribute] FOREIGN KEY([AddressAttributeId])
REFERENCES [dbo].[AddressAttribute] ([Id])
ON DELETE CASCADE
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Address]') and NAME='CustomAttributes')
BEGIN
	ALTER TABLE [Address]
	ADD [CustomAttributes] nvarchar(MAX) NULL
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='IsRental')
BEGIN
	ALTER TABLE [Product]
	ADD [IsRental] bit NULL
END
GO

UPDATE [Product]
SET [IsRental] = 0
WHERE [IsRental] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [IsRental] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='RentalPriceLength')
BEGIN
	ALTER TABLE [Product]
	ADD [RentalPriceLength] int NULL
END
GO

UPDATE [Product]
SET [RentalPriceLength] = 0
WHERE [RentalPriceLength] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [RentalPriceLength] int NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='RentalPricePeriodId')
BEGIN
	ALTER TABLE [Product]
	ADD [RentalPricePeriodId] bit NULL
END
GO

UPDATE [Product]
SET [RentalPricePeriodId] = 0
WHERE [RentalPricePeriodId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [RentalPricePeriodId] int NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShoppingCartItem]') and NAME='RentalStartDateUtc')
BEGIN
	ALTER TABLE [ShoppingCartItem]
	ADD [RentalStartDateUtc] datetime NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShoppingCartItem]') and NAME='RentalEndDateUtc')
BEGIN
	ALTER TABLE [ShoppingCartItem]
	ADD [RentalEndDateUtc] datetime NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[OrderItem]') and NAME='RentalStartDateUtc')
BEGIN
	ALTER TABLE [OrderItem]
	ADD [RentalStartDateUtc] datetime NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[OrderItem]') and NAME='RentalEndDateUtc')
BEGIN
	ALTER TABLE [OrderItem]
	ADD [RentalEndDateUtc] datetime NULL
END
GO


--drop 'Purchase order' column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='PurchaseOrderNumber')
BEGIN
	--move existing PurchaseOrderNumber column to CustomValuesXml
	UPDATE [Order]
	SET [CustomValuesXml] = N'<?xml version="1.0" encoding="utf-16"?><DictionarySerializer><item><key>PO Number</key><value>' + [PurchaseOrderNumber] + N'</value></item></DictionarySerializer>'
	WHERE [PaymentMethodSystemName] = N'Payments.PurchaseOrder' and ([CustomValuesXml] is null or [CustomValuesXml] = N'')
		
	EXEC ('ALTER TABLE [Order] DROP COLUMN [PurchaseOrderNumber]')
END
GO




--rename ProductVariantAttributeCombination to ProductAttributeCombination
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductVariantAttributeCombination]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttributeCombination', 'ProductAttributeCombination';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'ProductVariantAttributeCombination_Product'
           AND parent_object_id = Object_id('ProductAttributeCombination')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttributeCombination_Product', 'ProductAttributeCombination_Product';
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='IsTelecommunicationsOrBroadcastingOrElectronicServices')
BEGIN
	ALTER TABLE [Product]
	ADD [IsTelecommunicationsOrBroadcastingOrElectronicServices] bit NULL
END
GO

UPDATE [Product]
SET [IsTelecommunicationsOrBroadcastingOrElectronicServices] = 0
WHERE [IsTelecommunicationsOrBroadcastingOrElectronicServices] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [IsTelecommunicationsOrBroadcastingOrElectronicServices] bit NOT NULL
GO


--rename column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeValue]') and NAME='ProductVariantAttributeId')
BEGIN
	EXEC sp_rename 'ProductVariantAttributeValue.ProductVariantAttributeId', 'ProductAttributeMappingId', 'COLUMN';
END
GO

--rename index
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariantAttributeValue_ProductVariantAttributeId' and object_id=object_id(N'[ProductVariantAttributeValue]'))
BEGIN
	EXEC sp_rename 'ProductVariantAttributeValue.IX_ProductVariantAttributeValue_ProductVariantAttributeId', 'IX_ProductAttributeValue_ProductAttributeMappingId', 'INDEX';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'ProductVariantAttributeValue_ProductVariantAttribute'
           AND parent_object_id = Object_id('ProductVariantAttributeValue')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttributeValue_ProductVariantAttribute', 'ProductAttributeValue_ProductAttributeMapping';
END
GO

--rename table
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductVariantAttributeValue]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttributeValue', 'ProductAttributeValue';
END
GO


IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'ProductVariantAttribute_Product'
           AND parent_object_id = Object_id('Product_ProductAttribute_Mapping')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttribute_Product', 'ProductAttributeMapping_Product';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'ProductVariantAttribute_ProductAttribute'
           AND parent_object_id = Object_id('Product_ProductAttribute_Mapping')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'ProductVariantAttribute_ProductAttribute', 'ProductAttributeMapping_ProductAttribute';
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='AttachedDownloadId')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [AttachedDownloadId] int NULL
END
GO

UPDATE [MessageTemplate]
SET [AttachedDownloadId] = 0
WHERE [AttachedDownloadId] IS NULL
GO

ALTER TABLE [MessageTemplate] ALTER COLUMN [AttachedDownloadId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='AttachedDownloadId')
BEGIN
	ALTER TABLE [QueuedEmail]
	ADD [AttachedDownloadId] int NULL
END
GO

UPDATE [QueuedEmail]
SET [AttachedDownloadId] = 0
WHERE [AttachedDownloadId] IS NULL
GO

ALTER TABLE [QueuedEmail] ALTER COLUMN [AttachedDownloadId] int NOT NULL
GO


--new stored procedure
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
	SELECT [Id] FROM [Customer] c
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
	AND
	--shopping cart items
	((@OnlyWithoutShoppingCart=0) OR (NOT EXISTS(SELECT 1 FROM [ShoppingCartItem] sci inner join [Customer] on sci.[CustomerId]=c.[Id])))
	AND
	--guests only
	(EXISTS(SELECT 1 FROM [Customer_CustomerRole_Mapping] ccrm inner join [Customer] on ccrm.[Customer_Id]=c.[Id] inner join [CustomerRole] cr on cr.[Id]=ccrm.[CustomerRole_Id] WHERE cr.[SystemName] = N'Guests'))
	AND
	--no orders
	(NOT EXISTS(SELECT 1 FROM [Order] o inner join [Customer] on o.[CustomerId]=c.[Id]))
	AND
	--no blog comments
	(NOT EXISTS(SELECT 1 FROM [BlogComment] bc inner join [Customer] on bc.[CustomerId]=c.[Id]))
	AND
	--no news comments
	(NOT EXISTS(SELECT 1 FROM [NewsComment] nc inner join [Customer] on nc.[CustomerId]=c.[Id]))
	AND
	--no product reviews
	(NOT EXISTS(SELECT 1 FROM [ProductReview] pr inner join [Customer] on pr.[CustomerId]=c.[Id]))
	AND
	--no product reviews helpfulness
	(NOT EXISTS(SELECT 1 FROM [ProductReviewHelpfulness] prh inner join [Customer] on prh.[CustomerId]=c.[Id]))
	AND
	--no poll voting
	(NOT EXISTS(SELECT 1 FROM [PollVotingRecord] pvr inner join [Customer] on pvr.[CustomerId]=c.[Id]))
	AND
	--no forum topics 
	(NOT EXISTS(SELECT 1 FROM [Forums_Topic] ft inner join [Customer] on ft.[CustomerId]=c.[Id]))
	AND
	--no forum posts 
	(NOT EXISTS(SELECT 1 FROM [Forums_Post] fp inner join [Customer] on fp.[CustomerId]=c.[Id]))
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

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.richeditorallowjavascript')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.richeditorallowjavascript', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.attachpdfinvoicetoorderpaidemail')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.attachpdfinvoicetoorderpaidemail', N'false', 0)
END
GO