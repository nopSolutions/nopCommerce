--upgrade scripts from nopCommerce 3.00 to 3.10

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientKeyIdentifier">
    <Value>App ID/API Key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint">
    <Value>Enter your app ID/API key here. You can find it on your FaceBook application page.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientSecret">
    <Value>App Secret</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientSecret.Hint">
    <Value>Enter your app secret here. You can find it on your FaceBook application page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn">
    <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn.Hint">
    <Value>Date and time when this product was created.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn">
    <Value>Updated on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn.Hint">
    <Value>Date and time when this product was updated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.PageShareCode">
    <Value>Share button code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.PageShareCode.Hint">
    <Value>A page share button code. By default, we''re using AddThis service.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.NumberOfAssociatedProducts">
    <Value>Number of associated products</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.HeaderQuantity.Mobile">
    <Value>({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.HeaderQuantity.Mobile">
    <Value>({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Compare.NoItems">
    <Value>You have no items to compare.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.HideAdminMenuItemsBasedOnPermissions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.HideAdminMenuItemsBasedOnPermissions.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Products.Breadcrumb.Top">
    <Value>Home</Value>
  </LocaleResource>
  <LocaleResource Name="Categories.Breadcrumb.Top">
    <Value>Home</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Store.All">
    <Value>All stores</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Store.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate.EnterUtc">
    <Value>Date and time should entered in Coordinated Universal Time (UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.DeliveryDate.EnterUtc">
    <Value>Date and time should entered in Coordinated Universal Time (UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.LimitedToStores">
    <Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.LimitedToStores.Hint">
    <Value>Determines whether the plugin is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AvailableStores">
    <Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AvailableStores.Hint">
    <Value>Select stores for which the plugin will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Stores">
    <Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Info">
    <Value>Plugin Info</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CannotLoadProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.AddNewForProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Name.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Published">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Published.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.BackToProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.EditProductVariantDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Info">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Added">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Deleted">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Updated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.SaveBeforeEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Store">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Store.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.CustomerRole">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.CustomerRole.AllRoles">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Quantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.TierPrices.Fields.Price">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices">
    <Value>Tier prices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.SaveBeforeEdit">
    <Value>You need to save the product before you can add tier prices for this product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Store.All">
    <Value>All stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.CustomerRole">
    <Value>Customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.CustomerRole.All">
    <Value>All customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Quantity">
    <Value>Quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Discounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Discounts.NoDiscounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Discounts">
    <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Discounts.NoDiscounts">
    <Value>No discounts available. Create at least one discount before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.FirstVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ProductName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ProductName.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ProductName.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.SaveBeforeEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.NoAttributesAvailable">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.AddTitle">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Attributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Sku">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Sku.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.GTIN">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.GTIN.Hint">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes">
    <Value>Product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.SaveBeforeEdit">
    <Value>You need to save the product before you can add attributes for this page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.NoAttributesAvailable">
    <Value>No product attributes available. Create at least one product attribute before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations">
    <Value>Attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.AddTitle">
    <Value>Select new combination and enter details below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.AddNew">
    <Value>Add combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Description">
    <Value>Attribute combinations are useful when your ''Manage inventory method'' is set to ''Track inventory by product attributes''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders">
    <Value>Allow out of stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders.Hint">
    <Value>A value indicating whether to allow orders when out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity.Hint">
    <Value>The current stock quantity of this combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Sku">
    <Value>Sku</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.Sku.Hint">
    <Value>Product stock keeping unit (SKU). Your internal unique identifier that can be used to track this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber">
    <Value>Manufacturer part number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber.Hint">
    <Value>The manufacturer''s part number for this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.GTIN">
    <Value>GTIN</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.GTIN.Hint">
    <Value>Enter global trade item number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Fields.Attribute">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Fields.TextPrompt">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Fields.IsRequired">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Fields.AttributeControlType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.BackToProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.EditValueDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.Name.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.Name.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.EditAttributeDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.ViewLink">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes">
    <Value>Attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.Attribute">
    <Value>Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.TextPrompt">
    <Value>Text prompt</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.IsRequired">
    <Value>Is Required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.AttributeControlType">
    <Value>Control type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Fields.DisplayOrder">
    <Value>Display order></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values">
    <Value>Values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.AddNew">
    <Value>Add a new value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.BackToProduct">
    <Value>back to product details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.EditValueDetails">
    <Value>Edit value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name.Hint">
    <Value>The attribute value name e.g. ''Blue'' for Color attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb">
    <Value>RGB color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value>Choose color to be used with the color squares attribute control.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment">
    <Value>Price adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.PriceAdjustment.Hint">
    <Value>The price adjustment applied when choosing this attribute value e.g. ''10'' to add 10 dollars.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment">
    <Value>Weight adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.WeightAdjustment.Hint">
    <Value>The weight adjustment applied when choosing this attribute value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected">
    <Value>Is pre-selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre selected for the customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.DisplayOrder.Hint">
    <Value>The display order of the attribute value. 1 represents the first item in attribute value list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.EditAttributeDetails">
    <Value>Add/Edit values for [{0}] attribute. Product: {1}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.ViewLink">
    <Value>View/Edit value (Total: {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AdditionalShippingCharge">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AdditionalShippingCharge.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AdminComment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AdminComment.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowBackInStockSubscriptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowBackInStockSubscriptions.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowedQuantities">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowedQuantities.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AutomaticallyAddRequiredProductVariants">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AutomaticallyAddRequiredProductVariants.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableEndDateTime">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableEndDateTime.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableForPreOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableForPreOrder.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableStartDateTime">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableStartDateTime.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.BackorderMode">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.BackorderMode.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.CallForPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.CallForPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.CustomerEntersPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.CustomerEntersPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Description.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableBuyButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableBuyButton.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableWishlistButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableWishlistButton.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayOrder.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayStockAvailability">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayStockAvailability.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayStockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisplayStockQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DownloadActivationType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DownloadActivationType.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DownloadExpirationDays">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DownloadExpirationDays.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Download">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Download.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.GiftCardType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.GiftCardType.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.GTIN">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.GTIN.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.HasSampleDownload">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.HasSampleDownload.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.HasUserAgreement">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.HasUserAgreement.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Height">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Height.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsDownload">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsDownload.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsFreeShipping">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsFreeShipping.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsGiftCard">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsGiftCard.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsRecurring">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsRecurring.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsShipEnabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsShipEnabled.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsTaxExempt">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.IsTaxExempt.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Length">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Length.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.LowStockActivity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.LowStockActivity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ManageInventoryMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ManageInventoryMethod.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ManufacturerPartNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ManufacturerPartNumber.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MaximumCustomerEnteredPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MaximumCustomerEnteredPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MaxNumberOfDownloads">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MaxNumberOfDownloads.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MinimumCustomerEnteredPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MinimumCustomerEnteredPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MinStockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.MinStockQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Name.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.NotifyAdminForQuantityBelow">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OldPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OldPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMaximumQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMaximumQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMinimumQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMinimumQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Picture">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Picture.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Price">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Price.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ProductCost">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.ProductCost.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Published">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Published.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequiredProductVariantIds">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequiredProductVariantIds.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequireOtherProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequireOtherProducts.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SampleDownload">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SampleDownload.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Sku">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Sku.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPriceStartDateTimeUtc">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPriceStartDateTimeUtc.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPriceEndDateTimeUtc">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.SpecialPriceEndDateTimeUtc.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.StockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.StockQuantity.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringTotalCycles">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringTotalCycles.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringCycleLength">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringCycleLength.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringCyclePeriod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RecurringCyclePeriod.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.TaxCategory">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.TaxCategory.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.UserAgreementText">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.UserAgreementText.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.UnlimitedDownloads">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.UnlimitedDownloads.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Weight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Weight.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Width">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Width.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AdditionalShippingCharge">
    <Value>Additional shipping charge</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AdditionalShippingCharge.Hint">
    <Value>The additional shipping charge.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowBackInStockSubscriptions">
    <Value>Allow back in stock subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowBackInStockSubscriptions.Hint">
    <Value>Allow customers to subscribe to a notification list for a product that has gone out of stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowedQuantities">
    <Value>Allowed quantities</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowedQuantities.Hint">
    <Value>Enter a comma separated list of quantities you want this product to be restricted to. Instead of a quantity textbox that allows them to enter any quantity, they will receive a dropdown list of the values you enter here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AutomaticallyAddRequiredProducts">
    <Value>Automatically add these products to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AutomaticallyAddRequiredProducts.Hint">
    <Value>Check to automatically add these products to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableEndDateTime">
    <Value>Available end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableEndDateTime.Hint">
    <Value>The end of the product''s availability in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableForPreOrder">
    <Value>Available for pre-order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableForPreOrder.Hint">
    <Value>Check if this item is available for Pre-Order. It also displays "Pre-order" button instead of "Add to cart".</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStartDateTime">
    <Value>Available start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStartDateTime.Hint">
    <Value>The start of the product''s availability in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BackorderMode">
    <Value>Backorders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.BackorderMode.Hint">
    <Value>Select backorder mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CallForPrice">
    <Value>Call for price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CallForPrice.Hint">
    <Value>Check to show "Call for Pricing" or "Call for quote" instead of price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CustomerEntersPrice">
    <Value>Customer enters price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CustomerEntersPrice.Hint">
    <Value>An option indicating whether customer should enter price.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisableBuyButton">
    <Value>Disable buy button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisableBuyButton.Hint">
    <Value>Check to disable the buy button for this product. This may be necessary for products that are ''available upon request''.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisableWishlistButton">
    <Value>Disable wishlist button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisableWishlistButton.Hint">
    <Value>Check to disable the wishlist button for this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisplayStockAvailability">
    <Value>Display stock availability</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisplayStockAvailability.Hint">
    <Value>Check to display stock availability. When enabled, customers will see stock availability.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisplayStockQuantity">
    <Value>Display stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisplayStockQuantity.Hint">
    <Value>Check to display stock quantity. When enabled, customers will see stock quantity.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DownloadActivationType">
    <Value>Download activation type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DownloadActivationType.Hint">
    <Value>A value indicating when download links will be enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DownloadExpirationDays">
    <Value>Number of days</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DownloadExpirationDays.Hint">
    <Value>The number of days during customers keeps access to the file (e.g. 14). Leave this field blank to allow continuous downloads.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Download">
    <Value>Download file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Download.Hint">
    <Value>The download file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.GiftCardType">
    <Value>Gift card type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.GiftCardType.Hint">
    <Value>Select gift card type. WARNING: not recommended to change in production environment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.GTIN">
    <Value>GTIN (global trade item number)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.GTIN.Hint">
    <Value>Enter global trade item number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasSampleDownload">
    <Value>Has sample download file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasSampleDownload.Hint">
    <Value>Check if this product has a sample download file that can be downloaded before checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasUserAgreement">
    <Value>Has user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasUserAgreement.Hint">
    <Value>Check if the product has a user agreement.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Height">
    <Value>Height</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Height.Hint">
    <Value>The height of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID.Hint">
    <Value>The product identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsDownload">
    <Value>Downloadable product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsDownload.Hint">
    <Value>Check if this product is a downloadable product. When a customer purchases a download product, they can download the item direct from your store by viewing their completed order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsFreeShipping">
    <Value>Free shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsFreeShipping.Hint">
    <Value>Check if this product comes with FREE shipping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsGiftCard">
    <Value>Is gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsGiftCard.Hint">
    <Value>Check if it is a gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsRecurring">
    <Value>Recurring product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsRecurring.Hint">
    <Value>Check if this product is a recurring product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsShipEnabled">
    <Value>Shipping enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsShipEnabled.Hint">
    <Value>Determines whether the product can be shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTaxExempt">
    <Value>Tax exempt</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTaxExempt.Hint">
    <Value>Determines whether this product is tax exempt (tax will not be applied to this product at checkout).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Length">
    <Value>Length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Length.Hint">
    <Value>The length of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LowStockActivity">
    <Value>Low stock activity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LowStockActivity.Hint">
    <Value>Action to be taken when your current stock quantity falls below the ''Minimum stock quantity''.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManageInventoryMethod">
    <Value>Manage inventory method</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManageInventoryMethod.Hint">
    <Value>Select manage inventory method. When enabled, stock quantities are automatically adjusted when a customer makes a purchase. You can also set low stock activity actions and receive notifications.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManufacturerPartNumber">
    <Value>Manufacturer part number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManufacturerPartNumber.Hint">
    <Value>The manufacturer''s part number for this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MaximumCustomerEnteredPrice">
    <Value>Maximum amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MaximumCustomerEnteredPrice.Hint">
    <Value>Enter a maximum amount.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MaxNumberOfDownloads">
    <Value>Max. downloads</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MaxNumberOfDownloads.Hint">
    <Value>The maximum number of downloads.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MinimumCustomerEnteredPrice">
    <Value>Minimum amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MinimumCustomerEnteredPrice.Hint">
    <Value>Enter a minimum amount.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MinStockQuantity">
    <Value>Minimum stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MinStockQuantity.Hint">
    <Value>If you have enabled ''Manage Stock'' you can perform a number of different actions when the current stock quantity falls below your minimum stock quantity.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.NotifyAdminForQuantityBelow">
    <Value>Notify admin for quantity below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value>When the current stock quantity falls below this quantity, the storekeeper (admin) will receive a notification.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OldPrice">
    <Value>Old price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OldPrice.Hint">
    <Value>The old price of the product. If you set an old price, this will display alongside the current price on the product page to show the difference in price.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMaximumQuantity">
    <Value>Maximum cart quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMaximumQuantity.Hint">
    <Value>Set the maximum quantity allowed in a customer''s shopping cart e.g. set to 5 to only allow customers to purchase 5 of this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMinimumQuantity">
    <Value>Minimum cart quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMinimumQuantity.Hint">
    <Value>Set the minimum quantity allowed in a customer''s shopping cart e.g. set to 3 to only allow customers to purchase 3 or more of this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Price.Hint">
    <Value>The price of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductCost">
    <Value>Product cost</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductCost.Hint">
    <Value>The product cost is the cost of all the different components which make up the product. This may either be the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds">
    <Value>Required product IDs</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds.Hint">
    <Value>Specify comma separated list of required product IDs. NOTE: Ensure that there are no circular references (for example, A requires B, and B requires A).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequireOtherProducts">
    <Value>Require other products added to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequireOtherProducts.Hint">
    <Value>Check if this product requires that other products are added to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SampleDownload">
    <Value>Sample download file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SampleDownload.Hint">
    <Value>The sample download file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Sku">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Sku.Hint">
    <Value>Product stock keeping unit (SKU). Your internal unique identifier that can be used to track this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPrice">
    <Value>Special price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPrice.Hint">
    <Value>Set a special price for the product. New price will be valid between start and end dates. Leave empty to ignore field.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceStartDateTimeUtc">
    <Value>Special price start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceStartDateTimeUtc.Hint">
    <Value>The start date of the special price in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceEndDateTimeUtc">
    <Value>Special price end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceEndDateTimeUtc.Hint">
    <Value>The end date of the special price in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.StockQuantity.Hint">
    <Value>The current stock quantity of this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringTotalCycles">
    <Value>Total cycles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringTotalCycles.Hint">
    <Value>Enter total cycles.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCycleLength">
    <Value>Cycle length</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCycleLength.Hint">
    <Value>Enter cycle length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCyclePeriod">
    <Value>Cycle period</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCyclePeriod.Hint">
    <Value>Select cycle period.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.TaxCategory">
    <Value>Tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.TaxCategory.Hint">
    <Value>The tax classification for this product. You can manage product tax classifications from Configuration : Tax : Tax Classes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UserAgreementText">
    <Value>User agreement text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UserAgreementText.Hint">
    <Value>The text of the user agreement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UnlimitedDownloads">
    <Value>Unlimited downloads</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UnlimitedDownloads.Hint">
    <Value>When a customer purchases a download product, they can download the item unlimited number of times.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Weight">
    <Value>Weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Weight.Hint">
    <Value>The weight of the product. Can be used in shipping calculations.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Width">
    <Value>Width</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Width.Hint">
    <Value>The width of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants.NoRecords">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts">
    <Value>Assigned to products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts.Hint">
    <Value>A list of products to which the discount is to be applied. You can assign this discount on a product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProducts.NoRecords">
    <Value>No products selected</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.ProductVariants">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.ProductVariants.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products">
    <Value>Restricted products</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products.Hint">
    <Value>The comma-separated list of product identifiers (e.g. 77, 123, 156). You can find a product ID on its details page. You can also specify the comma-separated list of product identifiers with quantities ({Product ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product identifiers with quantity range ({Product ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products">
    <Value>Restricted products</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products.Hint">
    <Value>The comma-separated list of product identifiers (e.g. 77, 123, 156). You can find a product ID on its details page. You can also specify the comma-separated list of product identifiers with quantities ({Product ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product identifiers with quantity range ({Product ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductType">
    <Value>Product type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductType.Hint">
    <Value>Choose your product type.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.Nop.Core.Domain.Catalog.ProductType.SimpleProduct">
    <Value>Simple product</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.Nop.Core.Domain.Catalog.ProductType.GroupedProduct">
    <Value>Grouped product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit">
    <Value>Bulk edit products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.Manage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Note1">
    <Value>Click on interested product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountType.Hint">
    <Value>The type of discount.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Discounts.DiscountType.AssignedToSkus">
    <Value>Assigned to products</Value>
  </LocaleResource>
  <LocaleResource Name="PDFProductCatalog.UnnamedProductVariant">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTemplate.Hint">
    <Value>Choose a product template. This template defines how this product will be displayed in public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductVariantPictureSize">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductVariantPictureSize.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.AssociatedProductPictureSize">
    <Value>Associated product image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.AssociatedProductPictureSize.Hint">
    <Value>The default size (pixels) for associated product images (part of ''grouped'' products).</Value>
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

--more indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Category_LimitedToStores' and object_id=object_id(N'[Category]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Category_LimitedToStores] ON [Category] ([LimitedToStores] ASC)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Manufacturer_LimitedToStores' and object_id=object_id(N'[Manufacturer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Manufacturer_LimitedToStores] ON [Manufacturer] ([LimitedToStores] ASC)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_LimitedToStores' and object_id=object_id(N'[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_LimitedToStores] ON [Product] ([LimitedToStores] ASC)
END
GO




IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Category_SubjectToAcl' and object_id=object_id(N'[Category]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Category_SubjectToAcl] ON [Category] ([SubjectToAcl] ASC)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Manufacturer_SubjectToAcl' and object_id=object_id(N'[Manufacturer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Manufacturer_SubjectToAcl] ON [Manufacturer] ([SubjectToAcl] ASC)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_SubjectToAcl' and object_id=object_id(N'[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_SubjectToAcl] ON [Product] ([SubjectToAcl] ASC)
END
GO


--recaptcha theme name
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchatheme')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'captchasettings.recaptchatheme', N'', 0)
END
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
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
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


		--product variant name
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Name]) > 0 '


		--SKU
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Sku], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Sku]) > 0 '


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


			--product variant description
			SET @sql = @sql + '
			UNION
			SELECT pv.ProductId
			FROM ProductVariant pv with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pv.[Description], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Description]) > 0 '


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
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
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


--remove obsolete setting
DELETE FROM [Setting]
WHERE [name] = N'SecuritySettings.HideAdminMenuItemsBasedOnPermissions'
GO

--new column 
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[TierPrice]') and NAME='StoreId')
BEGIN
	ALTER TABLE [TierPrice]
	ADD [StoreId] int NULL
END
GO

UPDATE [TierPrice]
SET [StoreId] = 0
WHERE [StoreId] IS NULL
GO

ALTER TABLE [TierPrice] ALTER COLUMN [StoreId] int NOT NULL
GO



--shipping by weight plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [StoreId] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''StoreId'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [StoreId] int NULL

		exec(''UPDATE [ShippingByWeight] SET [StoreId] = 0'')
		
		EXEC (''ALTER TABLE [ShippingByWeight] ALTER COLUMN [StoreId] int NOT NULL'')
	END')
END
GO

--rename ShipmentOrderProductVariant to ShipmentItem
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipment_OrderProductVariant]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC sp_rename 'Shipment_OrderProductVariant', 'ShipmentItem';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'ShipmentOrderProductVariant_Shipment'
           AND parent_object_id = Object_id('ShipmentItem')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'ShipmentOrderProductVariant_Shipment', 'ShipmentItem_Shipment';
END
GO

--rename OrderProductVariant to OrderItem
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[GiftCard]') and NAME='PurchasedWithOrderProductVariantId')
BEGIN
	EXEC sp_rename 'GiftCard.PurchasedWithOrderProductVariantId', 'PurchasedWithOrderItemId', 'COLUMN';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'GiftCard_PurchasedWithOrderProductVariant'
           AND parent_object_id = Object_id('GiftCard')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'GiftCard_PurchasedWithOrderProductVariant', 'GiftCard_PurchasedWithOrderItem';
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[OrderProductVariant]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC sp_rename 'OrderProductVariant', 'OrderItem';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'OrderProductVariant_Order'
           AND parent_object_id = Object_id('OrderItem')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'OrderProductVariant_Order', 'OrderItem_Order';
END
GO

IF EXISTS (SELECT 1
           FROM sys.objects
           WHERE name = 'OrderProductVariant_ProductVariant'
           AND parent_object_id = Object_id('OrderItem')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	EXEC sp_rename 'OrderProductVariant_ProductVariant', 'OrderItem_ProductVariant';
END
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_OrderProductVariant_OrderId' and object_id=object_id(N'[OrderItem]'))
BEGIN
	EXEC sp_rename 'OrderItem.IX_OrderProductVariant_OrderId', 'IX_OrderItem_OrderId', 'INDEX';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ReturnRequest]') and NAME='OrderProductVariantId')
BEGIN
	EXEC sp_rename 'ReturnRequest.OrderProductVariantId', 'OrderItemId', 'COLUMN';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShipmentItem]') and NAME='OrderProductVariantId')
BEGIN
	EXEC sp_rename 'ShipmentItem.OrderProductVariantId', 'OrderItemId', 'COLUMN';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[OrderItem]') and NAME='OrderProductVariantGuid')
BEGIN
	EXEC sp_rename 'OrderItem.OrderProductVariantGuid', 'OrderItemGuid', 'COLUMN';
END
GO

--revise product/product variant logic
DELETE FROM [ActivityLogType]
WHERE [SystemKeyword] = N'AddNewProductVariant'
GO

DELETE FROM [ActivityLogType]
WHERE [SystemKeyword] = N'DeleteProductVariant'
GO

DELETE FROM [ActivityLogType]
WHERE [SystemKeyword] = N'EditProductVariant'
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
	@ParentProductId	int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
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



		--SKU
		SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Sku], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Sku]) > 0 '


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
	
	--filter by parent product identifer
	IF @ParentProductId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.ParentProductId = ' + CAST(@ParentProductId AS nvarchar(max))
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
	IF @PriceMin > 0
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
	IF @PriceMax > 0
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

--remove obsolete setting
DELETE FROM [Setting]
WHERE [name] = N'MediaSettings.ProductVariantPictureSize'
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.associatedproductpicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.associatedproductpicturesize', N'125', 0)
END
GO

--update some message template tokens
UPDATE [MessageTemplate]
SET [Subject] = REPLACE([Subject], 'ProductVariant.ID', 'Product.ID'),
[Body] = REPLACE([Body], 'ProductVariant.ID', 'Product.ID')
GO

UPDATE [MessageTemplate]
SET [Subject] = REPLACE([Subject], 'ProductVariant.FullProductName', 'Product.Name'),
[Body] = REPLACE([Body], 'ProductVariant.FullProductName', 'Product.Name')
GO

UPDATE [MessageTemplate]
SET [Subject] = REPLACE([Subject], 'ProductVariant.StockQuantity', 'Product.StockQuantity'),
[Body] = REPLACE([Body], 'ProductVariant.StockQuantity', 'Product.StockQuantity')
GO