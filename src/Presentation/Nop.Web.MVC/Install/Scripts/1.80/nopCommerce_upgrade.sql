--upgrade scripts from nopCommerce 1.70 to nopCommerce 1.80

--new locale resources
declare @resources xml
set @resources='
<Language LanguageID="7">
  <LocaleResource Name="Admin.Categories.DisplayOrder">
    <Value>Display Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.HideNewsletterBox">
    <Value>Hide newsletter box:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.HideNewsletterBox.Tooltip">
    <Value>Check if you want to hide the newsletter subscription box</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PublicStore">
    <Value>Public Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LoggedInAs">
    <Value>Logged in as:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Logout">
    <Value>Logout?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ClearCacheButton.Text">
    <Value>Clear Cache</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.EarningRewardPoints.Tooltip1">
    <Value>Each</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.EarningRewardPoints.Tooltip2">
    <Value>spent will earn</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.EarningRewardPoints.Tooltip3">
    <Value>reward points.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Topics.IsPasswordProtected">
    <Value>Password protected</Value>
  </LocaleResource>
  <LocaleResource Name="TopicPage.btnPassword.Text">
    <Value>Enter</Value>
  </LocaleResource>
  <LocaleResource Name="TopicPage.WrongPassword">
    <Value>Wrong password</Value>
  </LocaleResource>
  <LocaleResource Name="TopicPage.EnterPassword">
    <Value>Please enter password to access this page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.IsPasswordProtected">
    <Value>Is password protected:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.IsPasswordProtected.Tooltip">
    <Value>Check if this topic is password proceted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.Password">
    <Value>Password:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.Password.Tooltip">
    <Value>The password to access the content of this topic</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.Url">
    <Value>URL:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.Url.Tooltip">
    <Value>The URL of this topic</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionsHome.NewsletterSubscribers.TitleDescription">
    <Value>Newsletter Subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionsHome.NewsletterSubscribers.Title">
    <Value>Newsletter Subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionsHome.NewsletterSubscribers.Description">
    <Value>Manage the newsletter subscribers here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.Title">
    <Value>Newsletter Subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.DeleteButton.Text">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.Email">
    <Value>Email:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.Email.Tooltip">
    <Value>Enter email to find or leave empty to load all subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.EmailColumn">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.Active">
    <Value>Active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.SubscribedOnColumn">
    <Value>Subscribed on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.NewsletterSubscribersTitle">
    <Value>Newsletter Subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.NewsletterSubscribersDescription">
    <Value>Manage Newsletter Subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Campaigns.ExportEmailsButton.Text">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Campaigns.ExportEmailsButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ExportEmailsButton.Text">
    <Value>Export emails to CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ExportEmailsButton.Tooltip">
    <Value>Export emails subscribed to newsletters to a comma-separated value (CSV) file.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCards.DontWorkWithAutoshipProducts">
    <Value>You cannot use gift cards with auto-ship (recurring) items.</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSubscriptions.InfoColumn">
    <Value>Forum/Topic</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSubscriptions.DeleteSelected">
    <Value>Delete Selected</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ForumSubscriptions">
    <Value>Forum Subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowToManageSubscriptions">
    <Value>Allow customers to manage forum subscriptions:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowToManageSubscriptions.Tooltip">
    <Value>Check if you want to allow customers to manage forum subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerForumSubscriptions.InfoColumn">
    <Value>Forum/Topic</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerForumSubscriptions.DeleteColumn">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerDetails.CustomerForumSubscriptions">
    <Value>Forum Subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BulkEditProducts.PriceColumn.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BulkEditProducts.OldPriceColumn.RangeErrorMessage">
    <Value>The old price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeInfo.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeValues.New.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.DiscountAmount.RangeErrorMessage">
    <Value>The amount must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardInfo.InitialValue.RangeErrorMessage">
    <Value>The initial value must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistInfo.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistInfo.ProductVariants.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.OldPrice.RangeErrorMessage">
    <Value>The old price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductCost.RangeErrorMessage">
    <Value>The product cost must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.MinimumCustomerEnteredPrice.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.MaximumCustomerEnteredPrice.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.AdditionalShippingCharge.RangeErrorMessage">
    <Value>The additional shipping charge must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductPricesByCustomerRole.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductPricesByCustomerRole.New.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.New.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.OldPrice.RangeErrorMessage">
    <Value>The old price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.ProductCost.RangeErrorMessage">
    <Value>The product cost must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.AdditionalShippingCharge.RangeErrorMessage">
    <Value>The additional shipping charge must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantTierPrices.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantTierPrices.New.Price.RangeErrorMessage">
    <Value>The price must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingSettings.ValueOfX.RangeErrorMessage">
    <Value>The value must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Products.QuantityRange">
    <Value>The quantity must be from 1 to 9999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategorySEO.PageSize.RangeErrorMessage">
    <Value>The page size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryProducts.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.MaxNumberOfDownloads.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.StockQuantity.RangeErrorMessage">
    <Value>The stock quantity must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.MinStockQuantity.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.NotifyForQuantityBelow.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.OrderMinimumQuantity.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.OrderMaximumQuantity.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Weight.RangeErrorMessage">
    <Value>The weight must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Length.RangeErrorMessage">
    <Value>The length must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Width.RangeErrorMessage">
    <Value>The width must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Height.RangeErrorMessage">
    <Value>The height must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductCategory.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductManufacturer.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RelatedProducts.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductPictures.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductPictures.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductSpecifications.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductSpecifications.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.StockQuantity.RangeErrorMessage">
    <Value>The stock quantity must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.MinStockQuantity.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.NotifyForQuantityBelow.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.OrderMinimumQuantity.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.OrderMaximumQuantity.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Weight.RangeErrorMessage">
    <Value>The weight must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Length.RangeErrorMessage">
    <Value>The length must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Width.RangeErrorMessage">
    <Value>The width must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Height.RangeErrorMessage">
    <Value>The height must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantTierPrices.Quantity.RangeErrorMessage">
    <Value>The quantity must be from 0 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantTierPrices.New.Quantity.RangeErrorMessage">
    <Value>The quantity must be from 0 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.WeightAdjustment.RangeErrorMessage">
    <Value>The weight adjustment must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.New.WeightAdjustment.RangeErrorMessage">
    <Value>The weight adjustment must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SpecificationAttributeInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerSEO.PageSize.RangeErrorMessage">
    <Value>The page size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerProducts.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistInfo.CacheTime.RangeErrorMessage">
    <Value>The cache time must be from 0 to 64000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.DisplayOrderColumn.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.TopicsPageSize.RangeErrorMessage">
    <Value>The page size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.PostsPageSize.RangeErrorMessage">
    <Value>The page size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumGroupInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductTemplateInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryTemplateInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerTemplateInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.MaxImageSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.ProductThumbSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.ProductDetailSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.ProductVariantSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.CategoryThumbSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.ManufacturerThumbSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.CartThumbSize.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchasedNumber.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CreditCardTypeInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviderInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxCategoryInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryInfo.NumbericISOCode.RangeErrorMessage">
    <Value>The numberic ISO code must be from 1 to 9999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StateProvinceInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.LiveRates.Rate.RangeErrorMessage">
    <Value>The rate must be from 0 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrencyInfo.Rate.RangeErrorMessage">
    <Value>The rate must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrencyInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueue.MaxSendTries.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueueDetails.Priority.RangeErrorMessage">
    <Value>The priority must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueueDetails.SendTries.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddRelatedProduct.DisplayOrderColumn.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCategoryProduct.DisplayOrderColumn.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddManufacturerProduct.DisplayOrderColumn.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.Ratio.RangeErrorMessage">
    <Value>The ratio must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.Ratio.RangeErrorMessage">
    <Value>The ratio must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CycleLength.RangeErrorMessage">
    <Value>The cycle length must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.TotalCycles.RangeErrorMessage">
    <Value>The total cycles must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CycleLength.RangeErrorMessage">
    <Value>The cycle length must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.TotalCycles.RangeErrorMessage">
    <Value>The total cycles must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.CycleLength.RangeErrorMessage">
    <Value>The cycle length must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentInfo.TotalCycles.RangeErrorMessage">
    <Value>The total cycles must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.Combinations.StockQuantity.RangeErrorMessage">
    <Value>The stock quantity must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity.RangeErrorMessage">
    <Value>The stock quantity must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlogSettings.PostsPageSize.RangeErrorMessage">
    <Value>The page size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeInfo.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeValues.New.WeightAdjustment.RangeErrorMessage">
    <Value>The weight adjustment must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeValues.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeInfo.WeightAdjustment.RangeErrorMessage">
    <Value>The weight adjustment must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.PointsForRegistration.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.PointsForPurchases_Amount.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.RewardPoints.PointsForPurchases_Points.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRewardPoints.Add.Points.RangeErrorMessage">
    <Value>The value must be from -999999 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SpecificationAttributeOptions.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SpecificationAttributeOptions.New.DisplayOrder.RangeErrorMessage">
    <Value>The display order must be from -99999 to 99999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DisplayStockQuantity">
    <Value>Display stock quantity:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DisplayStockQuantity.Tooltip">
    <Value>Check to display stock quantity. When enabled, customers will see stock quantity.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayStockQuantity">
    <Value>Display stock quantity:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayStockQuantity.Tooltip">
    <Value>Check to display stock quantity. When enabled, customers will see stock quantity.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.DisplayStockAvailability">
    <Value>Display stock availability:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.DisplayStockAvailability">
    <Value>Display stock availability:</Value>
  </LocaleResource>
  <LocaleResource Name="Products.InStockWithQuantity">
    <Value>{0} in stock</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.EnableUrlRewriting">
    <Value>URL rewriting enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.EnableUrlRewriting.Tooltip">
    <Value>Check to enable URL rewriting</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementSpentAmount">
    <Value>Required spent amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementSpentAmount.Tooltip">
    <Value>Discount will be applied if customer has spent/purchased x.xx amount.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementSpentAmount.RequiredErrorMessage">
    <Value>Enter required spent amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementSpentAmount.RangeErrorMessage">
    <Value>The value must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.AllowOutOfStockOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.AllowOutOfStockOrders.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Backorders">
    <Value>Backorders:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.Backorders.Tooltip">
    <Value>Select backorder mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.AllowOutOfStockOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.AllowOutOfStockOrders.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Backorders">
    <Value>Backorders:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.Backorders.Tooltip">
    <Value>Select backorder mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Backordering">
    <Value>Out of Stock - on backorder and will be despatched once in stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductBackorderMode.NoBackorders">
    <Value>No backorders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductBackorderMode.AllowQtyBelow0">
    <Value>Allow qty below 0</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductBackorderMode.AllowQtyBelow0AndNotifyCustomer">
    <Value>Allow qty below 0 and notify customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreatePosts">
    <Value>Allow guests to create posts:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.AllowGuestsToCreateTopics">
    <Value>Allow guests to create topics:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.GiftCardType">
    <Value>Gift card type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.GiftCardType.Tooltip">
    <Value>Select gift card type. WARNING: not recommended to change in production environment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.GiftCardType">
    <Value>Gift card type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.GiftCardType.Tooltip">
    <Value>Select gift card type. WARNING: not recommended to change in production environment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.ProductType.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.ExchangeRateProvider">
    <Value>Current exchange rate provider:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.ExchangeRateProvider.ToolTip">
    <Value>Select an exchange rate provider that will be used to get live rates.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Title">
    <Value>SMS Providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.SaveButton.Tooltip">
    <Value>Save changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.Title">
    <Value>Clickatell</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.Name">
    <Value>Name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.Name.Tooltip">
    <Value>SMS provider name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.Name.ErrorMessage">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.ClassName">
    <Value>Class name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.ClassName.Tooltip">
    <Value>Enter fully qualified name of SMS provider class</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.ClassName.ErrorMessage">
    <Value>Class name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.SystemKeyword">
    <Value>System keyword:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.SystemKeyword.Tooltip">
    <Value>SMS provider system keywork</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.Active">
    <Value>Active:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviderInfo.Active.Tooltip">
    <Value>Determines whether this SMS provider is active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Title">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Enabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Enabled.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.PhoneNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.PhoneNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.APIID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.APIID.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Username">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Username.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Password">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.Clickatell.Password.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.TestPhone">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.TestPhone.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.SendTestSMSButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SMSAlerts.SendTestSMSButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.PhoneNumber">
    <Value>Phone number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.PhoneNumber.Tooltip">
    <Value>Your phone number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.APIID">
    <Value>API ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.APIID.Tooltip">
    <Value>Clickatell API ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.Username">
    <Value>Username:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.Username.Tooltip">
    <Value>Clickatell username</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.Password">
    <Value>Password:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.Password.Tooltip">
    <Value>Clickatell password</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage">
    <Value>Send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.Text">
    <Value>Message text:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.Text.Tooltip">
    <Value>Text of test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.SendButton">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.SendButton.Tooltip">
    <Value>Click here to send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.Success">
    <Value>Test message was sent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessage.Failed">
    <Value>Test message sending failed</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditSMSProviders">
    <Value>SMS providers settings were changed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.SMSProvidersTitle">
    <Value>SMS Providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.SMSProvidersDescription">
    <Value>Manage SMS Providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatEnabled">
    <Value>EU VAT enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatEnabled.Tooltip">
    <Value>Check to enable EU VAT (the European Union Value Added Tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatShopCountry">
    <Value>Your shop country:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatShopCountry.Tooltip">
    <Value>Select your shop country for VAT calculation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatAllowVATExemption">
    <Value>Allow VAT exemption:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatAllowVATExemption.Tooltip">
    <Value>Check if this store will exempt eligible VAT-registered customers from VAT</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatUseWebService">
    <Value>Use web service:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatUseWebService.Tooltip">
    <Value>Check if you want to use the EU web service to validate VAT numbers. WARNING: If this option is enabled, then DO NOT disable country form field available during registration (public store).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatEmailAdminWhenNewVATSubmitted">
    <Value>Notify admin when a new VAT number is submitted:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatEmailAdminWhenNewVATSubmitted.Tooltip">
    <Value>Check if you want to receive a notification (email) when a new VAT number is submitted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.EUVatShopCountry.SelectCountry">
    <Value>Select country</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VATNumber">
    <Value>VAT number</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VATNumberStatus">
    <Value>(status: {0})</Value>
  </LocaleResource>
  <LocaleResource Name="VatNumberStatus.Unknown">
    <Value>Unknown</Value>
  </LocaleResource>
  <LocaleResource Name="VatNumberStatus.Empty">
    <Value>Empty</Value>
  </LocaleResource>
  <LocaleResource Name="VatNumberStatus.Valid">
    <Value>Valid</Value>
  </LocaleResource>
  <LocaleResource Name="VatNumberStatus.Invalid">
    <Value>Invalid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.SMSProviders.TitleDescription">
    <Value>SMS Providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.SMSProviders.Title">
    <Value>SMS Providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.SMSProviders.Description">
    <Value>Manage SMS notification settings and providers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.VatNumber">
    <Value>VAT number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.VatNumber.Tooltip">
    <Value>Enter company VAT number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.VATNumberStatus">
    <Value>(status: {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.BtnMarkVatNumberAsValid.Text">
    <Value>Mark VAT number as valid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.BtnMarkVatNumberAsInvalid.Text">
    <Value>Mark VAT number as invalid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Countries.SubjectToVAT">
    <Value>Subject to VAT</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryInfo.SubjectToVAT">
    <Value>Subject to VAT:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryInfo.SubjectToVAT.Tooltip">
    <Value>Value indicating whether customers in this country must be charged EU VAT (the European Union Value Added Tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VatNumber">
    <Value>VAT number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.VatNumber.Tooltip">
    <Value>Used VAT number (the European Union Value Added Tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Order.VATNumber">
    <Value>VAT number</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.VATNumber">
    <Value>VAT number: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShippedOn">
    <Value>Shipped on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.Title">
    <Value>Verizon</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.Email">
    <Value>Email:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.Email.Tooltip">
    <Value>Verizon email address(e.g. your_phone_number@vtext.com)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage">
    <Value>Send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage.Text">
    <Value>Text:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessageText.Tooltip">
    <Value>Text of the test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage.SendButton">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage.SendButton.Tooltip">
    <Value>Click to send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage.Failed">
    <Value>Test message sending failed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Verizon.TestMessage.Success">
    <Value>Test message was sent</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportCSV.CSVFile">
    <Value>CSV (tab-delimited) file:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportEmailsButton.Text">
    <Value>Import emails from CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportEmailsButton.Tooltip">
    <Value>Import emails from CSV(tab-delimeted) file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportEmailsButton.Success">
    <Value>{0} emails have been successfully imported</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrentShoppingCarts.Title">
    <Value>Current Shopping Carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrentShoppingCarts.Empty">
    <Value>No records found</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.CurrentShoppingCartsTitle">
    <Value>Current Shopping Carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.CurrentShoppingCartsDescription">
    <Value>Manage Current Shopping Carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.CustomerColumn">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.CustomerColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.NameColumn">
    <Value>Product name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.PriceColumn">
    <Value>Unit price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.QuantityColumn">
    <Value>Quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.TotalColumn">
    <Value>Total</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Sub-TotalDiscount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.OrderDiscount">
    <Value>Discount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUpload">
    <Value>Upload feed to Google FTP server</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPHostname.Text">
    <Value>Hostname:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPHostname.Tooltip">
    <Value>Google FTP server hostname</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUsername.Text">
    <Value>Username:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUsername.Tooltip">
    <Value>Google FTP account username</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPPassword.Text">
    <Value>Password:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPPassword.Tooltip">
    <Value>Google FTP account password</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUploadButton">
    <Value>Upload</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUploadButton.Tooltip">
    <Value>Click here to upload feed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPUploadStatus">
    <Value>Froogle feed upload status: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPFilename.Text">
    <Value>File name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.FTPFilename.Tooltip">
    <Value>Feed file name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.ShoppingCartColumn">
    <Value>Shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.CurrentShoppingCarts.TitleDescription">
    <Value>Current Shopping Carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.CurrentShoppingCarts.Title">
    <Value>Current Shopping Carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.CurrentShoppingCarts.Description">
    <Value>Manage current shopping carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerShoppingCart.CustomerColumn.LastAccess">
    <Value>Last access: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.Pending">
    <Value>Pending</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.Received">
    <Value>Received</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.ReturnAuthorized">
    <Value>Return authorized</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.ItemsRepaired">
    <Value>Item(s) repaired</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.ItemsRefunded">
    <Value>Item(s) refunded</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.RequestRejected">
    <Value>Request rejected</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnStatus.Cancelled">
    <Value>Cancelled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.Title">
    <Value>Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.Enabled">
    <Value>Enable Returns System:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.Enabled.Tooltip">
    <Value>Check if you want to allow customers to submit return requests for items they''ve previously purchased.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.ReturnReasons">
    <Value>Return reasons:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.ReturnReasons.Tooltip">
    <Value>A comma-separated list of reasons a customer will be able to choose when submitting a return request.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.ReturnActions">
    <Value>Return action:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.ReturnRequests.ReturnActions.Tooltip">
    <Value>A comma-separated list of actions a customer will be able to choose when submitting a return request.</Value>
  </LocaleResource>
  <LocaleResource Name="OrderDetails.ReturnItemsButton">
    <Value>Return Item(s)</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.ReturnItems">
    <Value>Return Item(s)</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.Title">
    <Value>Return item(s) from order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.SelectProduct(s)">
    <Value>Which items do you want to return?</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.ProductsGrid.Name">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.ProductsGrid.Price">
    <Value>Unit price</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.ProductsGrid.Quantity">
    <Value>Qty. to return</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.WhyReturning">
    <Value>Why are you returning these items?</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.ReturnReason">
    <Value>Return reason:</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.ReturnAction">
    <Value>Return Action:</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.Comments">
    <Value>Comments:</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.SubmitButton">
    <Value>Submit return request</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.Submitted">
    <Value>Your return request has been submitted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnItems.NoItems">
    <Value>Your return request has not been submitted because you haven''t chosen any items.</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Title">
    <Value>Your Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.RequestTitle">
    <Value>Return #{0} - {1}</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Item">
    <Value>Returned Item: {0} x {1}</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Reason">
    <Value>Return Reason: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Action">
    <Value>Return Action: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Date">
    <Value>Date Requested: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="CustomerReturnRequests.Comments">
    <Value>Your Comments:</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests">
    <Value>Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ReturnRequestsTitle">
    <Value>Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.ReturnRequestsDescription">
    <Value>Manage Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.ReturnRequests.TitleDescription">
    <Value>Manage Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.ReturnRequests.Title">
    <Value>Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesHome.ReturnRequests.Description">
    <Value>The returns system will allow your customers to request a return on items they''ve purchased. These are also known as RMA requests. Manage return requests here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Title">
    <Value>Return Requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.IDColumn">
    <Value>Request ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.NameColumn">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.CustomerColumn">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.CustomerColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.OrderColumn">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.OrderColumn.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.DateColumn">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.StatusColumn">
    <Value>Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Edit">
    <Value>Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.NoRecordsFound">
    <Value>No records found</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditReturnRequest">
    <Value>Edited a return request (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteReturnRequest">
    <Value>Deleted a return request (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.Title">
    <Value>Edit return request</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.BackToReturnRequests">
    <Value>Back to return requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.SaveButton.Tooltip">
    <Value>Save the return request</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.DeleteButton.Tooltip">
    <Value>Delete the return request</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.RequestId">
    <Value>Request ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.RequestId.Tooltip">
    <Value>The indentifier of this return request</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Order">
    <Value>Order:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Order.Tooltip">
    <Value>Order information.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Order.View">
    <Value>View (ID - {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Customer">
    <Value>Customer: </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Customer.Tooltip">
    <Value>Customer information.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Customer.View">
    <Value>View ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Name">
    <Value>Product:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Name.Tooltip">
    <Value>Product information.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Date">
    <Value>Date:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Date.Tooltip">
    <Value>Date when the return request has been submitted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.ReasonForReturn">
    <Value>Reason for return:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.ReasonForReturn.Tooltip">
    <Value>Chosen reason for return</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.RequestedAction">
    <Value>Requested action:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.RequestedAction.Tooltip">
    <Value>Chosen requested action</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.CustomerComments">
    <Value>Customer comments:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.CustomerComments.Tooltip">
    <Value>Entered customer comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.StaffNotes">
    <Value>Staff notes:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.StaffNotes.Tooltip">
    <Value>Entered staff comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Status">
    <Value>Status:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestInfo.Status.Tooltip">
    <Value>Choose request status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Products.ReturnRequests">
    <Value>Return requests: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryDetails.PreviewButton.Text">
    <Value>Preview</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryDetails.PreviewButton.ToolTip">
    <Value>Click to preview the category page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerDetails.PreviewButton.Text">
    <Value>Preview</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerDetails.PreviewButton.ToolTip">
    <Value>Click to preview the manufacturer page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.PreviewButton.Text">
    <Value>Preview</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.PreviewButton.ToolTip">
    <Value>Click to preview the product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantDetails.PreviewButton.Text">
    <Value>Preview</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantDetails.PreviewButton.ToolTip">
    <Value>Click to preview the product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.CurrencyRateAutoUpdateEnabled">
    <Value>Auto update enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Currencies.CurrencyRateAutoUpdateEnabled.ToolTip">
    <Value>Click to enable auto update of currency rates each one hour</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.PendingOrderStatusNotAllowed">
    <Value>Order status could not be set to Pending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingSettings.EstimateShippingEnabled">
    <Value>Estimate shipping enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingSettings.EstimateShippingEnabled.Tooltip">
    <Value>Check to allow customers to estimate shipping on shopping cart page</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShippingButton">
    <Value>Estimate shipping</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.Country">
    <Value>Country:</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.StateProvince">
    <Value>State / province:</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.StateProvinceNonUS">
    <Value>Other (Non US)</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.ZipPostalCode">
    <Value>Zip / postal code:</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.ZipPostalCodeIsRequired">
    <Value>Zip / Postal code is required</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.GetQuoteButton">
    <Value>Get a quote</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.Title">
    <Value>Estimate shipping</Value>
  </LocaleResource>
  <LocaleResource Name="EstimateShipping.Tooltip">
    <Value>Enter your destination to get a shipping estimate</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanCapture">
    <Value>Supports capture:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanCapture.Tooltip">
    <Value>Value indicating whether this payment method supports "Capture"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanRefund">
    <Value>Supports refund:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanRefund.Tooltip">
    <Value>Value indicating whether this payment method supports "Refund"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanVoid">
    <Value>Supports void:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanVoid.Tooltip">
    <Value>Value indicating whether this payment method supports "Void"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.SupportRecurringPayments">
    <Value>Supports recurring payments:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.SupportRecurringPayments.Tooltip">
    <Value>Value indicating whether this payment method supports recurring payments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.DisplayTaxRates">
    <Value>Display all applied tax rates:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxSettings.DisplayTaxRates.Tooltip">
    <Value>A value indicating whether each tax rate should be displayed on separate line (shopping cart page)</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.TaxRate">
    <Value>Tax {0}%</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Totals.TaxRate">
    <Value>Tax {0}%</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.Totals.TaxRate">
    <Value>Tax {0}%:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Totals.TaxRate">
    <Value>Order tax {0}%:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.Totals.TaxRate.Tooltip">
    <Value>Order tax {0}%.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.TaxRates.InPrimaryCurrency">
    <Value>Tax rates in primary currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.TaxRates.InCustomerCurrency">
    <Value>Tax rates in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.Subtotal.InCustomerCurrency">
    <Value>Subtotal in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.Discount.InCustomerCurrency">
    <Value>Discount in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.Shipping.InCustomerCurrency">
    <Value>Shipping in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.PaymentMethodAdditionalFee.InCustomerCurrency">
    <Value>Payment method additional fee in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.Tax.InCustomerCurrency">
    <Value>Tax in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.Total.InCustomerCurrency">
    <Value>Total in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.OnlineCustomersTitle">
    <Value>Online Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.OnlineCustomersDescription">
    <Value>Online Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomersHome.OnlineCustomers.TitleDescription">
    <Value>Online Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomersHome.OnlineCustomers.Title">
    <Value>Online Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomersHome.OnlineCustomers.Description">
    <Value>See how many online users you got on your site in this moment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Title">
    <Value>Online Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Guests">
    <Value>Guests:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Guests.Tooltip">
    <Value>See how many guests you got on your site in this moment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.CustomerInfoColumn">
    <Value>Customer info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.LastVisitColumn">
    <Value>Last visit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Enabled">
    <Value>Module enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Enabled.Tooltip">
    <Value>Check to enable "online customers" module to see how many guests you got on your site in this moment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowShareButton">
    <Value>Show a share button:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DownloadableProductsTab">
    <Value>Show "Downloadable products" tab:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DownloadableProductsTab.Tooltip">
    <Value>Check to show "Downloadable products tab" on My Account page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.FormFields.NewsletterBoxEnabled">
    <Value>''Newsletter'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.FormFields.NewsletterBoxEnabled.Tooltip">
    <Value>Set if ''Newsletter'' is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentStatus.PartiallyRefunded">
    <Value>Partially Refunded</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanPartiallyRefund">
    <Value>Supports partial refund:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.CanPartiallyRefund.Tooltip">
    <Value>Value indicating whether this payment method supports "Partial refund"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SMSProviders.Clickatell.TestMessageText.Tooltip">
    <Value>Text of the test message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundedAmount">
    <Value>Refunded amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.RefundedAmount.Tooltip">
    <Value>The total refunded amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.Title">
    <Value>Partial refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.OrderInfo">
    <Value>Partial refund for order {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.AmountToRefund">
    <Value>Amount to refund:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.AmountToRefund.Tooltip">
    <Value>Enter amount to refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.AmountToRefund.RequiredErrorMessage">
    <Value>The amount to refund is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.AmountToRefund.RangeErrorMessage">
    <Value>The amount to refund be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.MaxRefund">
    <Value>Max amount is {0} {1}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.PartialRefundButton.Text">
    <Value>Partial refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.PartialRefundButton.Tooltip">
    <Value>Partial refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.PartialRefundOfflineButton.Text">
    <Value>Partial refund (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.PartialRefundOfflineButton.Tooltip">
    <Value>Partial refund (Offline)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderPartialRefund.Refund">
    <Value>Refund</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowDiscountCouponBox">
    <Value>Show discount box:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowDiscountCouponBox.Tooltip">
    <Value>Check if you want the discount coupon box to be displayed on shopping cart page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowGiftCardBox">
    <Value>Show gift card box:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowGiftCardBox.Tooltip">
    <Value>Check if you want the gift card coupon box to be displayed on shopping cart page</Value>
  </LocaleResource>
  <LocaleResource Name="Profile.PersonalInfo">
    <Value>Profile info</Value>
  </LocaleResource>
  <LocaleResource Name="Profile.LatestPosts.NoPosts">
    <Value>No posts found</Value>
  </LocaleResource>
  <LocaleResource Name="Order.BillingInformation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.GoDirectlySKU">
    <Value>Go directly to product SKU:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.GoDirectlySKU.Tooltip">
    <Value>Enter product SKU and click Go</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.GoDirectlySKU.ErrorMessage">
    <Value>SKU is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.GoSKUButton.Text">
    <Value>Go</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Manufacturer">
    <Value>Manufacturer: </Value>
  </LocaleResource>
  <LocaleResource Name="Products.Manufacturers">
    <Value>Manufacturers: </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount">
    <Value>Min order amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.Tooltip">
    <Value>Enter minimum order amount here</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.RequiredErrorMessage">
    <Value>Minimum order amount is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.RangeErrorMessage">
    <Value>The minimum order amount must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.MinOrderAmount">
    <Value>Minimum order amount is {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowSKU">
    <Value>Show SKU:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowSKU.Tooltip">
    <Value>Check to show product SKU in public store</Value>
  </LocaleResource>
  <LocaleResource Name="Products.SKU">
    <Value>SKU: </Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ProductsGrid.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Title">
    <Value>Sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Sitemap">
    <Value>Sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Content.Sitemap">
    <Value>Sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CallForPrice">
    <Value>Call for price:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductInfo.CallForPrice.Tooltip">
    <Value>Check to show "Call for Pricing" or "Call for quote" instead of price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CallForPrice">
    <Value>Call for price:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantInfo.CallForPrice.Tooltip">
    <Value>Check to show "Call for Pricing" or "Call for quote" instead of price</Value>
  </LocaleResource>
  <LocaleResource Name="Products.CallForPrice">
    <Value>Call for pricing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlogPostInfo.Tags">
    <Value>Tags:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlogPostInfo.Tags.Tooltip">
    <Value>Tags are keywords that this blog post can also be identified by. Enter a comma separated list of the tags to be associated with this blog post.</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.Tags">
    <Value>Tags:</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.TaggedWith">
    <Value>Blog posts tagged with ''{0}''</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.AdvancedSearch">
    <Value>Advanced search</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchInForum">
    <Value>Search in forum:</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchWithin">
    <Value>Search within:</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious">
    <Value>Limit results to previous:</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.AllResults">
    <Value>All results</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.1day">
    <Value>1 day</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.7days">
    <Value>7 days</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.2weeks">
    <Value>2 weeks</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.1month">
    <Value>1 month</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.3months">
    <Value>3 months</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.6months">
    <Value>6 months</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.LimitResultsToPrevious.1year">
    <Value>1 year</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchWithin.All">
    <Value>Topic titles and post text</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchWithin.TopicTitlesOnly">
    <Value>Topic titles only</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchWithin.PostTextOnly">
    <Value>Post text only</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSearch.SearchInForum.All">
    <Value>All forums</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ImpersonatedAs">
    <Value>Impersonated as {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ImpersonatedAs.Finish">
    <Value>finish session</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ImpersonatedAs.Finish.Tooltip">
    <Value>Click here to finish impersonated session</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerPlaceOrder.PlaceOrder">
    <Value>Place order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerDetails.PlaceOrder">
    <Value>Place order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Title">
    <Value>Google Analytics</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Enabled">
    <Value>Enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Enabled.Tooltip">
    <Value>Check if you want to enable Google Analytics tracking.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.ID">
    <Value>ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.ID.Tooltip">
    <Value>Enter Google Analytics ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.JS">
    <Value>Tracking code:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.JS.Tooltip">
    <Value>Paste the tracking code generated by Google Analytics here. This tracking code will be used to track when a new visitor arrives. You can then login to Google Analytics to view the customer''s details including which site they came from and conversion details.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.LimitationTimes">
    <Value>N times:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.LimitationTimes.Tooltip">
    <Value>Enter the number of times that the discount is limited to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.LimitationTimes.RequiredErrorMessage">
    <Value>The number of times is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.LimitationTimes.RangeErrorMessage">
    <Value>The number of times must be from 1 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.LimitationTimes.Times">
    <Value>times</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.AuthorizationTransactionID">
    <Value>Authorization transaction ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.AuthorizationTransactionID.Tooltip">
    <Value>Authorization transaction identifer received from your payment gateway</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.CaptureTransactionID">
    <Value>Capture transaction ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.CaptureTransactionID.Tooltip">
    <Value>Capture transaction identifer received from your payment gateway</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubscriptionTransactionID">
    <Value>Subscription transaction ID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubscriptionTransactionID.Tooltip">
    <Value>Subscription transaction identifer received from your payment gateway</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayCartAfterAddingProduct">
    <Value>Display cart after adding product:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayCartAfterAddingProduct.Tooltip">
    <Value>If checked a customer will be taken to the Shopping Cart page immediately after adding a product to their cart. If unchecked a customer will stay on the same page that they are adding the product to the cart from.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToTheCart">
    <Value>The product has been added to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditCCButton.Text">
    <Value>Edit credit card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SaveCCButton.Text">
    <Value>Save credit card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.CancelCCButton.Text">
    <Value>Cancel editing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.LastPageVisitedColumn">
    <Value>Last URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.CustomerInfoColumn.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.IPAddressColumn">
    <Value>IP Address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.LocationColumn">
    <Value>Location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.EntryColumn">
    <Value>Entry</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateInfo.AffiliateURL">
    <Value>Affiliate URL:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateInfo.AffiliateURL.Tooltip">
    <Value>When this hyperlink is clicked from the affiliate site, this site looks for an Affiliate ID query string parameter. If one exists, the customer is tagged with that affiliate.</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.Archive">
    <Value>Blog Archive</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.FilteredByMonth">
    <Value>Blog posts of ''{0}'' ''{1}''</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.TagsCloud.Title">
    <Value>Popular blog tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementBillingCountryIs">
    <Value>Required billing country:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementBillingCountryIs.Tooltip">
    <Value>Select required billing country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementShippingCountryIs">
    <Value>Required shipping country:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementShippingCountryIs.Tooltip">
    <Value>Select required shipping country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementBillingCountryIs.SelectCountry">
    <Value>Select billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountInfo.RequirementShippingCountryIs.SelectCountry">
    <Value>Select shipping country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.Product.Tooltip">
    <Value>Mark this product as a cross-sell product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.View">
    <Value>View</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.View.Tooltip">
    <Value>View product details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.AddNewButton.Text">
    <Value>Add new cross-sell product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.AvailableAfterSaving">
    <Value>You need to save the product before you can add cross-sell products for this product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CrossSellProducts.Image">
    <Value>Image</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Title">
    <Value>Add cross-sell product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.ProductName">
    <Value>Product name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.ProductName.Tooltip">
    <Value>A product name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Category">
    <Value>Category:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Category.Tooltip">
    <Value>Search by a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Manufacturer">
    <Value>Manufacturer:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Manufacturer.Tooltip">
    <Value>Search by a specific manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.SearchButton.Tooltip">
    <Value>Search for products based on the criteria below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.ProductColumn">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.ProductColumn.Tooltip">
    <Value>Mark this product as a cross-sell product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.PublishedColumn">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.NoProductsFound">
    <Value>No products found</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.SaveColumn">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.SaveColumn.Tooltip">
    <Value>Save cross-sell products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddCrossSellProduct.Image">
    <Value>Image</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.CrossSellProducts">
    <Value>Cross-sells</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.CrossSellProducts">
    <Value>Cross-sells</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber">
    <Value>Number of ''Cross-Sells'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.Tooltip">
    <Value>The number of ''Cross-Sells'' to display on shopping cart page; 0 if you don''t want to load cross-sells</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.RequiredErrorMessage">
    <Value>Number of ''Cross-Sells'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.RangeErrorMessage">
    <Value>The number of ''Cross-Sells'' must be from 0 to 999999.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CrossSells">
    <Value>Based on your selection, you may be interested in the following items:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.IncludeInSitemap">
    <Value>Include in sitemap:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicInfo.IncludeInSitemap.Tooltip">
    <Value>Check to include this topic in the sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Topics.IncludeInSitemap">
    <Value>Include in sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.DateOfBirth">
    <Value>Date of birth:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.DateOfBirth.Tooltip">
    <Value>Filter by date of birth. Don''t select any value to load all records</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.DateOfBirth.Month">
    <Value>Month</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.DateOfBirth.Day">
    <Value>Day</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueueDetails.EmailAccount">
    <Value>Email account:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueueDetails.EmailAccount.Tooltip">
    <Value>The email account that will be used to send this email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageTemplateDetails.EmailAccount">
    <Value>Email account:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageTemplateDetails.EmailAccount.Tooltip">
    <Value>The email account that will be used to send this message template.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.Title">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.Email">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.Email.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailDisplayName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailDisplayName.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailHost">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailHost.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailPort">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.EmailPort.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.User">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.User.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.Password">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.Password.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SSL">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SSL.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.DefaultCredentials">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.DefaultCredentials.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SendTestEmail">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.TestEmailTo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.TestEmailTo.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SendTestEmailButton.Text">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SendTestEmailButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.MailSettings.SendTestEmailSuccess">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.Email">
    <Value>Email address:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.Email.Tooltip">
    <Value>This is the from address for all outgoing emails from your store e.g. ''sales@yourstore.com''.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailDisplayName">
    <Value>Email display name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailDisplayName.Tooltip">
    <Value>This is the friendly display name for outgoing emails from your store e.g. ''Your Store Sales Department''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailHost">
    <Value>Host:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailHost.Tooltip">
    <Value>This is the host name or IP address of your mail server. You can normally find this out from your ISP or web host.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailPort">
    <Value>Port:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.EmailPort.Tooltip">
    <Value>This is the SMTP port of your mail server. This is usually port 25.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.User">
    <Value>User:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.User.Tooltip">
    <Value>This is the username you use to authenticate to your mail server.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.Password">
    <Value>Password:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.Password.Tooltip">
    <Value>This is the password you use to authenticate to your mail server.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SSL">
    <Value>Enable SSL:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SSL.Tooltip">
    <Value>Check to use Secure Sockets Layer (SSL) to encrypt the SMTP connection.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.DefaultCredentials">
    <Value>Use default credentials:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.DefaultCredentials.Tooltip">
    <Value>Check to use default credentials for the connection</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SendTestEmail">
    <Value>Send Test Email (save settings first by clicking "Save" button)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.TestEmailTo">
    <Value>Send email to:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.TestEmailTo.Tooltip">
    <Value>The email address to which you want to send your test email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SendTestEmailButton.Text">
    <Value>Send Test Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SendTestEmailButton.Tooltip">
    <Value>Send the test email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountInfo.SendTestEmailSuccess">
    <Value>Email has been successfully sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountAdd.Title">
    <Value>Add email account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountAdd.BackTo">
    <Value>back to email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountAdd.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountDetails.Title">
    <Value>Edit email account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountDetails.BackTo">
    <Value>back to email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountDetails.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountDetails.DeleteButton.Text">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.Title">
    <Value>Email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.AddNewButton.Text">
    <Value>Add new</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.AddNewButton.Tooltip">
    <Value>Add new email account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.DisplayName">
    <Value>Display name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.IsDefault">
    <Value>Is default</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.IsDefault.Tooltip">
    <Value>Check to make this email account default one, then click ''Save'' button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccounts.Edit">
    <Value>Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.EmailAccountsTitle">
    <Value>Email Accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.EmailAccountsDescription">
    <Value>Configure Email Accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.EmailAccounts.TitleDescription">
    <Value>Email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.EmailAccounts.Title">
    <Value>Email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ConfigurationHome.EmailAccounts.Description">
    <Value>Manage email accounts and settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderGuid">
    <Value>Order GUID:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderGuid.Tooltip">
    <Value>Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.General.TermsOfService">
    <Value>Terms of service:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.Title">
    <Value>Products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.HidePricesForNonRegistered">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.HidePricesForNonRegistered.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowSKU">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowSKU.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayCartAfterAddingProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayCartAfterAddingProduct.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.EnableDynamicPriceUpdate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.EnableDynamicPriceUpdate.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowProductSorting">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowProductSorting.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowShareButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowShareButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DownloadableProductsTab">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DownloadableProductsTab.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CompareProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CompareProducts.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.WishList">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.WishList.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.NotifyNewProductReviews">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.NotifyNewProductReviews.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ProductReviewsMustBeApproved">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ProductReviewsMustBeApproved.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToReviewProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToReviewProduct.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToSetProductRatings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToSetProductRatings.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProducts.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyViewedProductsNumber.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProducts.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.RecentlyAddedProductsNumber.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePage.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.ShowBestsellersOnHomePageNumber.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchased">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchased.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchasedNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchasedNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchasedNumber.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AlsoPurchasedNumber.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.CrossSellsNumber.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.HidePricesForNonRegistered">
    <Value>Hide prices for non-registered customers:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.HidePricesForNonRegistered.Tooltip">
    <Value>Check to disable product prices for all non-registered customers so that anyone browsing the site cant see prices. And ''Add to cart''/''Add to wishlist'' buttons will be hidden.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowSKU">
    <Value>Show SKU:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowSKU.Tooltip">
    <Value>Check to show product SKU in public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.DisplayCartAfterAddingProduct">
    <Value>Display cart after adding product:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.DisplayCartAfterAddingProduct.Tooltip">
    <Value>If checked a customer will be taken to the Shopping Cart page immediately after adding a product to their cart. If unchecked a customer will stay on the same page that they are adding the product to the cart from.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.EnableDynamicPriceUpdate">
    <Value>Enable dynamic price update:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.EnableDynamicPriceUpdate.Tooltip">
    <Value>Check if you want to enable dynamic price update on product details page in case a product has product attributes with price adjustments.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowProductSorting">
    <Value>Allow product sorting:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowProductSorting.Tooltip">
    <Value>Check to enable product sorting option on category/manufacturer details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowShareButton">
    <Value>Show a share button:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowShareButton.Tooltip">
    <Value>Displays a button from AddThis.com on your product pages that allows customers to share your product with various bookmarking social services</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.DownloadableProductsTab">
    <Value>Show "Downloadable products" tab:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.DownloadableProductsTab.Tooltip">
    <Value>Check to show "Downloadable products tab" on My Account page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CompareProducts">
    <Value>''Compare Products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CompareProducts.Tooltip">
    <Value>Check to allow customers to use the ''Compare Products'' option in your store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.WishList">
    <Value>''Wishlist'' Enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.WishList.Tooltip">
    <Value>Check to enable customer wishlists in your store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.NotifyNewProductReviews">
    <Value>Notify about new product reviews:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.NotifyNewProductReviews.Tooltip">
    <Value>Check to notify store owner about new product reviews.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ProductReviewsMustBeApproved">
    <Value>Product reviews must be approved:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ProductReviewsMustBeApproved.Tooltip">
    <Value>Check if product reviews must be approved by administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowAnonymousUsersToReviewProduct">
    <Value>Allow anonymous users to write product reviews:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowAnonymousUsersToReviewProduct.Tooltip">
    <Value>Check to allow anonymous users to write product reviews.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowAnonymousUsersToSetProductRatings">
    <Value>Allow anonymous users to set product ratings:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AllowAnonymousUsersToSetProductRatings.Tooltip">
    <Value>Check to allow anonymous users to set product ratings.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProducts">
    <Value>''Recently viewed products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProducts.Tooltip">
    <Value>Check to allow customers to use the ''Recently viewed products'' feature in your store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProductsNumber">
    <Value>Number of ''Recently viewed products'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProductsNumber.Tooltip">
    <Value>The number of ''Recently viewed products'' to display when ''Recently viewed products'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProductsNumber.RequiredErrorMessage">
    <Value>Enter a number of ''Recently viewed products'' to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyViewedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProducts">
    <Value>''Recently added products'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProducts.Tooltip">
    <Value>Check to allow customers to use the ''Recently added products'' feature in your store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProductsNumber">
    <Value>Number of ''Recently added products'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProductsNumber.Tooltip">
    <Value>The number of ''Recently added products'' to display when ''Recently added products'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProductsNumber.RequiredErrorMessage">
    <Value>Enter a number of ''Recently added products'' to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.RecentlyAddedProductsNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePage">
    <Value>Show best sellers on home page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePage.Tooltip">
    <Value>Check to show best sellers on home page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePageNumber">
    <Value>Number of best sellers on home page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePageNumber.Tooltip">
    <Value>The number of best sellers on home page to display when ''Show best sellers on home page'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePageNumber.RequiredErrorMessage">
    <Value>Enter a number of best sellers on home page to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowBestsellersOnHomePageNumber.RangeErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchased">
    <Value>''Products also purchased'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchased.Tooltip">
    <Value>Check to allow customers to view a list of products purchased by other customers who purchased the above</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchasedNumber">
    <Value>Number of also purchased products to display:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchasedNumber.Tooltip">
    <Value>The number of products also purchased by other customers to display when ''Products also purchased'' option is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchasedNumber.RequiredErrorMessage">
    <Value>Enter a number of products also purchased by other customers to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.AlsoPurchasedNumber.RangeErrorMessage">
    <Value>The value must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CrossSellsNumber">
    <Value>Number of ''Cross-Sells'':</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CrossSellsNumber.Tooltip">
    <Value>The number of ''Cross-Sells'' to display on shopping cart page; 0 if you don''t want to load cross-sells</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CrossSellsNumber.RequiredErrorMessage">
    <Value>Number of ''Cross-Sells'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.CrossSellsNumber.RangeErrorMessage">
    <Value>The number of ''Cross-Sells'' must be from 0 to 999999.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeDetails.Edit">
    <Value>Edit Checkout Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ConvertNonWesternChars">
    <Value>Convert non-western chars:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ConvertNonWesternChars.Tooltip">
    <Value>Check to take out the accent marks in the letters of SEO names while keeping the letter.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.FaviconRemove">
    <Value>Delete favicon</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.FaviconRemove.Tooltip">
    <Value>Click to delete favicon file.</Value>
  </LocaleResource>
  <LocaleResource Name="CCAvenuePaymentModule.Message">
    <Value>You will be redirected to CCAvenue site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.ShowOnHomePage">
    <Value>Show on home page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.StartDate">
    <Value>Start date:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollInfo.EndDate">
    <Value>End date:</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.ShippingMethod">
    <Value>Shipping method: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="MessageToken.OrderProducts.SKU">
    <Value>SKU: {0}</Value>
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
	
	IF (@ResourceValue is null or @ResourceValue = '')
	BEGIN
		DELETE [Nop_LocaleStringResource]
		WHERE LanguageID=@LanguageID AND ResourceName=@ResourceName
	END
	
	FETCH NEXT FROM cur_localeresource INTO @LanguageID, @ResourceName, @ResourceValue
END
CLOSE cur_localeresource
DEALLOCATE cur_localeresource

DROP TABLE #LocaleStringResourceTmp
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.HideNewsletterBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.HideNewsletterBox', N'False', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='IsPasswordProtected')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [IsPasswordProtected] bit NOT NULL CONSTRAINT [DF_Nop_Topic_IsPasswordProtected] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='Password')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [Password] nvarchar(200) NOT NULL CONSTRAINT [DF_Nop_Topic_Password] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
(
	@Email		nvarchar(200),
	@ShowHidden bit = 0
)
AS
BEGIN
	
	SET @Email = isnull(@Email, '')
	SET @Email = '%' + rtrim(ltrim(@Email)) + '%'


	SET NOCOUNT ON
	SELECT 
		nls.NewsLetterSubscriptionId,
		nls.NewsLetterSubscriptionGuid,
		nls.Email,
		nls.Active,
		nls.CreatedOn
	FROM
		[Nop_NewsLetterSubscription] nls
	LEFT OUTER JOIN 
		Nop_Customer c 
	ON 
		nls.Email=c.Email
	WHERE 
		(patindex(@Email, isnull(nls.Email, '')) > 0) AND
		(nls.Active = 1 OR @ShowHidden = 1) AND 
		(c.CustomerID IS NULL OR (c.Active = 1 AND c.Deleted = 0))
	ORDER BY nls.Email
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Forums.CustomersAllowedToManageSubscriptions')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Forums.CustomersAllowedToManageSubscriptions', N'False', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='DisplayStockQuantity')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD DisplayStockQuantity bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DisplayStockQuantity] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.AllowOutOfStockOrders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEO.EnableUrlRewriting')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEO.EnableUrlRewriting', N'True', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementSpentAmount')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementSpentAmount] money NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementSpentAmount] DEFAULT ((0))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 30)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (30, N'Had spent x.xx amount')
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='Backorders')
BEGIN

	ALTER TABLE [dbo].[Nop_ProductVariant] DROP CONSTRAINT [DF_Nop_ProductVariant_AllowOutOfStockOrders]

	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD Backorders int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_Backorders] DEFAULT ((0))

	EXEC ('UPDATE [dbo].[Nop_ProductVariant] SET [Backorders]=[AllowOutOfStockOrders]')

	ALTER TABLE [dbo].[Nop_ProductVariant] DROP COLUMN AllowOutOfStockOrders
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='GiftCardType')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD GiftCardType int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_GiftCardType] DEFAULT ((0))

END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.GiftCardType,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Product_Nop_ProductType'
           AND parent_obj = Object_id('Nop_Product')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Product
DROP CONSTRAINT FK_Nop_Product_Nop_ProductType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ProductType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ProductType]
ALTER TABLE [dbo].[Nop_Product] DROP CONSTRAINT [DF_Nop_Product_ProductTypeID]
ALTER TABLE [dbo].[Nop_Product] DROP COLUMN ProductTypeID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@Keywords			nvarchar(MAX),	
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder 
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductAlsoPurchasedLoadByProductID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
GO
CREATE PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
(
	@ProductID			int,
	@ShowHidden			bit,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductID int NOT NULL,
		ProductsPurchased int NOT NULL,
	)

	INSERT INTO #PageIndex (ProductID, ProductsPurchased)
	SELECT p.ProductID, SUM(opv.Quantity) as ProductsPurchased
	FROM    
		dbo.Nop_OrderProductVariant opv WITH (NOLOCK)
		INNER JOIN dbo.Nop_ProductVariant pv ON pv.ProductVariantId = opv.ProductVariantId
		INNER JOIN dbo.Nop_Product p ON p.ProductId = pv.ProductId
	WHERE
		opv.OrderID IN 
		(
			/* This inner query should retrieve all orders that have contained the productID */
			SELECT 
				DISTINCT OrderID
			FROM 
				dbo.Nop_OrderProductVariant opv2 WITH (NOLOCK)
				INNER JOIN dbo.Nop_ProductVariant pv2 ON pv2.ProductVariantId = opv2.ProductVariantId
				INNER JOIN dbo.Nop_Product p2 ON p2.ProductId = pv2.ProductId			
			WHERE 
				p2.ProductID = @ProductID
		)
		AND 
			(
				p.ProductId != @ProductID
			)
		AND 
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted = 0
			)
		AND 
			(
				@ShowHidden = 1
				OR
				GETUTCDATE() BETWEEN ISNULL(pv.AvailableStartDateTime, '1/1/1900') AND ISNULL(pv.AvailableEndDateTime, '1/1/2999')
			)
	GROUP BY
		p.ProductId
	ORDER BY 
		ProductsPurchased desc


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex

END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider1.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider1.Classname', N'NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates.EcbExchangeRateProvider, Nop.BusinessLogic', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider2.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider2.Classname', N'NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates.McExchangeRateProvider, Nop.BusinessLogic', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider3.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider3.Classname', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider.Current')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider.Current', N'1', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_SMSProvider]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_SMSProvider](
	[SMSProviderId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[SystemKeyword] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SMSProvider] PRIMARY KEY CLUSTERED 
(
	[SMSProviderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Cache.SMSManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Cache.SMSManager.CacheEnabled', N'True', N'')
END
GO

IF EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.IsEnabled')
BEGIN
	DELETE FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.IsEnabled'
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.Clickatell.PhoneNumber')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Mobile.SMS.Clickatell.PhoneNumber', N'', N'')
END
GO

IF EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.AdminPhoneNumber')
BEGIN
	DELETE FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.AdminPhoneNumber'
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditSMSProviders')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditSMSProviders', N'Edit SMS provider settings', 1)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageSMSProviders')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage SMS Providers', N'ManageSMSProviders', N'',305)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_SMSProvider]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.BusinessLogic.Messages.SMS.ClickatellSMSProvider, Nop.BusinessLogic')
BEGIN
	INSERT [dbo].[Nop_SMSProvider] ([Name], [ClassName], [SystemKeyword], [IsActive]) 
	VALUES (N'Clickatell', N'NopSolutions.NopCommerce.BusinessLogic.Messages.SMS.ClickatellSMSProvider, Nop.BusinessLogic', N'SMSPROVIDERS_CLICKATELL', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Country]') and NAME='SubjectToVAT')
BEGIN
	ALTER TABLE [dbo].[Nop_Country] 
	ADD [SubjectToVAT] bit NOT NULL CONSTRAINT [DF_Nop_Country_SubjectToVAT] DEFAULT ((0))
END
GO

UPDATE		Nop_Country
SET			SubjectToVAT = 1
WHERE		(TwoLetterISOCode IN (N'AT', N'BE', N'BG', N'CY', N'CZ', N'DE', N'DK', N'EE', N'ES', N'FI', N'FR', N'GB', N'GR', N'HU', N'IE', N'IT', N'LT', N'LU', N'LV', N'MT', N'NL', N'PL', N'PT', N'RO', N'SE', N'SI', N'SK'))
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='VatNumber')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [VatNumber] nvarchar(100) NOT NULL CONSTRAINT [DF_Nop_Order_VatNumber] DEFAULT ((''))
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewVATSubmitted.StoreOwnerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewVATSubmitted.StoreOwnerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewVATSubmitted.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'', N'New VAT number is submitted.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />
%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number (%Customer.VatNumber%).
</p>')
	END
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_SMSProvider]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.BusinessLogic.Messages.SMS.VerizonSMSProvider, Nop.BusinessLogic')
BEGIN
	INSERT [dbo].[Nop_SMSProvider] ([Name], [ClassName], [SystemKeyword], [IsActive]) 
	VALUES (N'Verizon', N'NopSolutions.NopCommerce.BusinessLogic.Messages.SMS.VerizonSMSProvider, Nop.BusinessLogic', N'SMSPROVIDERS_VERIZON', 0)
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.Verizon.Email')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Mobile.SMS.Verizon.Email', N'yournumber@vtext.com', N'')
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionLoadNonEmpty]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
DROP PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
END
GO

CREATE PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
AS
BEGIN
	SET NOCOUNT OFF
		
	SELECT
		*
	FROM 
		[Nop_CustomerSession] cs
	WHERE 
		CustomerSessionGUID 
	IN
	(
		SELECT DISTINCT sci.CustomerSessionGUID FROM [Nop_ShoppingCartItem] sci
	)
	ORDER BY cs.LastAccessed desc
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPHostname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPHostname', N'ftp://uploads.google.com', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPFilename')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPFilename', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPUsername')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPUsername', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPPassword')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPPassword', N'', N'')
END
GO

UPDATE [dbo].[Nop_DiscountType] 
SET [Name] = N'Assigned to order total'
WHERE [DiscountTypeID] = 1
GO

--return requests
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ReturnRequest]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_ReturnRequest](
	[ReturnRequestId] [int] IDENTITY(1,1) NOT NULL,
	[OrderProductVariantId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ReasonForReturn] [nvarchar](400) NOT NULL,
	[RequestedAction] [nvarchar](400) NOT NULL,
	[CustomerComments] [nvarchar](MAX) NOT NULL,
	[StaffNotes] [nvarchar](MAX) NOT NULL,
	[ReturnStatusId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ReturnRequest] PRIMARY KEY CLUSTERED 
(
	[ReturnRequestId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ReturnRequest_Nop_OrderProductVariant'
           AND parent_obj = Object_id('Nop_ReturnRequest')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ReturnRequest
DROP CONSTRAINT FK_Nop_ReturnRequest_Nop_OrderProductVariant
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_OrderProductVariant] FOREIGN KEY([OrderProductVariantId])
REFERENCES [dbo].[Nop_OrderProductVariant] ([OrderProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ReturnRequest_Nop_Customer'
           AND parent_obj = Object_id('Nop_ReturnRequest')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ReturnRequest
DROP CONSTRAINT FK_Nop_ReturnRequest_Nop_Customer
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Nop_Customer] ([CustomerId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.Enable')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.Enable', N'True', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.ReturnReasons')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.ReturnReasons', N'Received Wrong Product, Wrong Product Ordered, There Was A Problem With The Product', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.ReturnActions')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.ReturnActions', N'Repair, Replacement, Store Credit', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageReturnRequests')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Return Requests', N'ManageReturnRequests', N'',45)
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditReturnRequest')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditReturnRequest', N'Edit a return request', 1)
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteReturnRequest')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteReturnRequest', N'Delete a return request', 1)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Customer.FormatNameMaxLength')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Customer.FormatNameMaxLength', N'0', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Shipping.EstimateShipping.Enabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Shipping.EstimateShipping.Enabled', N'true', N'')
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@Keywords			nvarchar(MAX),	
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder 
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
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
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductAlsoPurchasedLoadByProductID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
GO
CREATE PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
(
	@ProductID			int,
	@ShowHidden			bit,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductID int NOT NULL,
		ProductsPurchased int NOT NULL,
	)

	INSERT INTO #PageIndex (ProductID, ProductsPurchased)
	SELECT p.ProductID, SUM(opv.Quantity) as ProductsPurchased
	FROM    
		dbo.Nop_OrderProductVariant opv WITH (NOLOCK)
		INNER JOIN dbo.Nop_ProductVariant pv ON pv.ProductVariantId = opv.ProductVariantId
		INNER JOIN dbo.Nop_Product p ON p.ProductId = pv.ProductId
	WHERE
		opv.OrderID IN 
		(
			/* This inner query should retrieve all orders that have contained the productID */
			SELECT 
				DISTINCT OrderID
			FROM 
				dbo.Nop_OrderProductVariant opv2 WITH (NOLOCK)
				INNER JOIN dbo.Nop_ProductVariant pv2 ON pv2.ProductVariantId = opv2.ProductVariantId
				INNER JOIN dbo.Nop_Product p2 ON p2.ProductId = pv2.ProductId			
			WHERE 
				p2.ProductID = @ProductID
		)
		AND 
			(
				p.ProductId != @ProductID
			)
		AND 
			(
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
		AND 
			(
				@ShowHidden = 1
				OR
				GETUTCDATE() BETWEEN ISNULL(pv.AvailableStartDateTime, '1/1/1900') AND ISNULL(pv.AvailableEndDateTime, '1/1/2999')
			)
	GROUP BY
		p.ProductId
	ORDER BY 
		ProductsPurchased desc


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex

END
GO

ALTER TABLE [dbo].[Nop_Order] ALTER COLUMN [CardName] nvarchar(1000) NOT NULL
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='TaxRates')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [TaxRates] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRates] DEFAULT ((''))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='TaxRatesInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [TaxRatesInCustomerCurrency] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRatesInCustomerCurrency] DEFAULT ((''))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'OnlineUserManager.Enabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'OnlineUserManager.Enabled', N'False', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.DownloadableProductsTab')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.DownloadableProductsTab', N'True', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='RefundedAmount')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [RefundedAmount] money NOT NULL CONSTRAINT [DF_Nop_Order_RefundedAmount] DEFAULT ((0))

	exec ('UPDATE [dbo].[Nop_Order]
	SET [RefundedAmount]=[OrderTotal]
	WHERE [PaymentStatusID]=40')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentStatus]
		WHERE [PaymentStatusID] = 35)
BEGIN
	INSERT [dbo].[Nop_PaymentStatus] ([PaymentStatusID], [Name])
	VALUES (35, N'PartiallyRefunded')
END
GO

DELETE FROM [dbo].[Nop_Setting] 
WHERE [Name] = N'Tax.TaxProvider.FixedRate.Rate'
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Checkout.DiscountCouponBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Checkout.DiscountCouponBox', N'True', N'')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Checkout.GiftCardBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Checkout.GiftCardBox', N'True', N'')
END
GO

--"call for price" option

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='CallForPrice')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD [CallForPrice] bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CallForPrice] DEFAULT ((0))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.GiftCardType,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.CallForPrice,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_BlogPost]') and NAME='Tags')
BEGIN
	ALTER TABLE [dbo].[Nop_BlogPost] 
	ADD [Tags] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_BlogPost_Tags] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_BlogPostLoadAll]
(
	@LanguageID	int,
	@PageSize			int = 2147483644,
	@PageIndex			int = 0, 
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		BlogPostID int NOT NULL,
	)

	INSERT INTO #PageIndex (BlogPostID)
	SELECT
		bp.BlogPostID
	FROM 
	    Nop_BlogPost bp 
	WITH 
		(NOLOCK)
	WHERE
		@LanguageID IS NULL OR @LanguageID = 0 OR LanguageID = @LanguageID
	ORDER BY 
		CreatedOn 
	DESC


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		bp.[BlogPostId],
		bp.[LanguageId],
		bp.[BlogPostTitle],
		bp.[BlogPostBody],
		bp.[BlogPostAllowComments],
		bp.[Tags],
		bp.[CreatedById],
		bp.[CreatedOn]
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_BlogPost bp on bp.BlogPostID = [pi].BlogPostID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_TopicLoadAll]
(
	@ForumID			int,
	@UserID				int,
	@Keywords			nvarchar(MAX),	
	@SearchType			int = 0,
	@LimitDate			datetime,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		TopicID int NOT NULL,
		TopicTypeID int NOT NULL,
		LastPostTime datetime NULL,
	)

	INSERT INTO #PageIndex (TopicID, TopicTypeID, LastPostTime)
	SELECT DISTINCT
		ft.TopicID, ft.TopicTypeID, ft.LastPostTime
	FROM Nop_Forums_Topic ft with (NOLOCK) 
	LEFT OUTER JOIN Nop_Forums_Post fp with (NOLOCK) ON ft.TopicID = fp.TopicID
	WHERE  (
				@ForumID IS NULL OR @ForumID=0
				OR (ft.ForumID=@ForumID)
			)
		AND (
				@UserID IS NULL OR @UserID=0
				OR (ft.UserID=@UserID)
			)
		AND	(
				((@SearchType = 0 or @SearchType = 10) and patindex(@Keywords, ft.Subject) > 0)
				OR
				((@SearchType = 0 or @SearchType = 20) and patindex(@Keywords, fp.Text) > 0)
			)
		AND (
				@LimitDate IS NULL
				OR (DATEDIFF(second, @LimitDate, ft.LastPostTime) > 0)
			)
	ORDER BY ft.TopicTypeID desc, ft.LastPostTime desc, ft.TopicID desc

	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		ft.TopicID as ForumTopicId,
		ft.ForumId,
		ft.UserId,
		ft.TopicTypeId,
		ft.Subject,
		ft.NumPosts,
		ft.Views,
		ft.LastPostId,
		ft.LastPostUserId,
		ft.LastPostTime,
		ft.CreatedOn,
		ft.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Forums_Topic ft on ft.TopicID = [pi].TopicID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostDelete]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_PostDelete]
(
	@PostID int
)
AS
BEGIN
	SET NOCOUNT ON

	declare @UserID int
	declare @ForumID int
	declare @TopicID int
	SELECT 
		@UserID = fp.UserID,
		@ForumID = ft.ForumID,
		@TopicID = ft.TopicID
	FROM
		[Nop_Forums_Topic] ft
		INNER JOIN 
		[Nop_Forums_Post] fp
		ON ft.TopicID=fp.TopicID
	WHERE
		fp.PostID = @PostID 
	
	DELETE
	FROM [Nop_Forums_Post]
	WHERE
		PostID = @PostID

	--update stats/info
	exec [dbo].[Nop_Forums_TopicUpdateCounts] @TopicID
	exec [dbo].[Nop_Forums_ForumUpdateCounts] @ForumID
	exec [dbo].[Nop_CustomerUpdateCounts] @UserID
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 15)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name])
	VALUES (15, N'N Times Only')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 25)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name])
	VALUES (25, N'N Times Per Customer')
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='LimitationTimes')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [LimitationTimes] int NOT NULL CONSTRAINT [DF_Nop_Discount_LimitationTimes] DEFAULT ((1))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@RelatedToProductID	int = 0,
	@Keywords			nvarchar(MAX),
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
		[DisplayOrder3] int,
		[CreatedOn] datetime
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2], [DisplayOrder3], [CreatedOn])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder, rp.DisplayOrder, p.CreatedOn
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_RelatedProduct rp with (NOLOCK) ON p.ProductID=rp.ProductID2
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
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
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				@RelatedToProductID IS NULL OR @RelatedToProductID=0
				OR rp.ProductID1=@RelatedToProductID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @RelatedToProductID IS NOT NULL AND @RelatedToProductID > 0
		THEN rp.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOn END DESC

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Products.DisplayCartAfterAddingProduct')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Products.DisplayCartAfterAddingProduct', N'True', N'')
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_BlogPostLoadAll]
(
	@LanguageID	int,
	@DateFrom			datetime,
	@DateTo				datetime,
	@PageSize			int = 2147483644,
	@PageIndex			int = 0, 
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		BlogPostID int NOT NULL,
	)

	INSERT INTO #PageIndex (BlogPostID)
	SELECT
		bp.BlogPostID
	FROM 
	    Nop_BlogPost bp 
	WITH 
		(NOLOCK)
	WHERE
		(@LanguageID IS NULL OR @LanguageID = 0 OR bp.LanguageID = @LanguageID)
		AND
		(@DateFrom is NULL or @DateFrom <= bp.CreatedOn)
		AND
		(@DateTo is NULL or @DateTo >= bp.CreatedOn)
	ORDER BY 
		bp.CreatedOn 
	DESC


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		bp.[BlogPostId],
		bp.[LanguageId],
		bp.[BlogPostTitle],
		bp.[BlogPostBody],
		bp.[BlogPostAllowComments],
		bp.[Tags],
		bp.[CreatedById],
		bp.[CreatedOn]
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_BlogPost bp on bp.BlogPostID = [pi].BlogPostID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 60)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (60, N'Billing country is')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 70)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (70, N'Shipping country is')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementBillingCountryIs')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementBillingCountryIs] int NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementBillingCountryIs] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementShippingCountryIs')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementShippingCountryIs] int NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementShippingCountryIs] DEFAULT ((0))
END
GO

--Cross-sells
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_CrossSellProduct]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_CrossSellProduct](
	[CrossSellProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId1] [int] NOT NULL,
	[ProductId2] [int] NOT NULL,
 CONSTRAINT [PK_CrossSellProduct] PRIMARY KEY CLUSTERED 
(
	[CrossSellProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_CrossSellProduct_Nop_Product'
           AND parent_obj = Object_id('Nop_CrossSellProduct')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_CrossSellProduct
DROP CONSTRAINT FK_Nop_CrossSellProduct_Nop_Product
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CrossSellProduct_Nop_Product] FOREIGN KEY([ProductId1])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'IX_Nop_CrossSellProduct_Unique'
           AND parent_obj = Object_id('Nop_CrossSellProduct')
           AND Objectproperty(id,N'IsConstraint') = 1)
ALTER TABLE dbo.Nop_CrossSellProduct
DROP CONSTRAINT IX_Nop_CrossSellProduct_Unique
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct]  WITH CHECK ADD CONSTRAINT [IX_Nop_CrossSellProduct_Unique]  UNIQUE NONCLUSTERED
(
	[ProductId1] ASC,
	[ProductId2] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='IncludeInSitemap')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [IncludeInSitemap] bit NOT NULL CONSTRAINT [DF_Nop_Topic_IncludeInSitemap] DEFAULT ((0))
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Sitemap.IncludeTopics' 
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'SEO.Sitemaps.IncludeTopics'
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Customer]') and NAME='DateOfBirth')
BEGIN
	ALTER TABLE [dbo].[Nop_Customer] 
	ADD [DateOfBirth] datetime
END
GO


CREATE TABLE #DateOfBirthImportTmp
	(
		[CustomerID] [int] NOT NULL,
		[DateOfBirthXml] xml NOT NULL
	)



INSERT INTO #DateOfBirthImportTmp (CustomerId, DateOfBirthXml)
SELECT	ca.CustomerId, ca.[Value]
FROM	Nop_CustomerAttribute ca
WHERE	ca.[Key] = N'DateOfBirth'

DECLARE @CustomerID int
DECLARE @DateOfBirthXml xml
DECLARE cur_attributes CURSOR FOR
SELECT CustomerID, DateOfBirthXml
FROM #DateOfBirthImportTmp
OPEN cur_attributes
FETCH NEXT FROM cur_attributes INTO @CustomerID, @DateOfBirthXml
WHILE @@FETCH_STATUS = 0
BEGIN

BEGIN TRY
--limit to 10 chars (datetime only)
UPDATE Nop_Customer
SET [DateOfBirth] = CAST(@DateOfBirthXml.value('dateTime[1]', 'nvarchar(10)') as datetime)
WHERE CustomerID = @CustomerID
END TRY
BEGIN CATCH
SELECT ERROR_MESSAGE()
END CATCH

FETCH NEXT FROM cur_attributes INTO @CustomerID, @DateOfBirthXml
END
CLOSE cur_attributes
DEALLOCATE cur_attributes

DROP TABLE #DateOfBirthImportTmp
GO

DELETE FROM Nop_CustomerAttribute
WHERE [Key] = N'DateOfBirth'
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerLoadAll]
(
	@StartTime				datetime = NULL,
	@EndTime				datetime = NULL,
	@Email					nvarchar(200),
	@Username				nvarchar(200),
	@DontLoadGuestCustomers	bit = 0,
	@DateOfBirthMonth		int = 0,
	@DateOfBirthDay			int = 0,
	@PageSize				int = 2147483644,
	@PageIndex				int = 0, 
	@TotalRecords			int = null OUTPUT
)
AS
BEGIN

	SET @Email = isnull(@Email, '')
	SET @Email = '%' + rtrim(ltrim(@Email)) + '%'

	SET @Username = isnull(@Username, '')
	SET @Username = '%' + rtrim(ltrim(@Username)) + '%'


	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	DECLARE @TotalThreads int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		CustomerID int NOT NULL,
		RegistrationDate datetime NOT NULL,
	)

	INSERT INTO #PageIndex (CustomerID, RegistrationDate)
	SELECT DISTINCT
		c.CustomerID, c.RegistrationDate
	FROM [Nop_Customer] c with (NOLOCK)
	WHERE 
		(@StartTime is NULL or @StartTime <= c.RegistrationDate) and
		(@EndTime is NULL or @EndTime >= c.RegistrationDate) and 
		(patindex(@Email, c.Email) > 0) and
		(patindex(@Username, c.Username) > 0) and
		(@DontLoadGuestCustomers = 0 or c.IsGuest = 0) and 
		(@DateOfBirthMonth = 0 or (c.DateOfBirth is not null and DATEPART(month, c.DateOfBirth) = @DateOfBirthMonth)) and 
		(@DateOfBirthDay = 0 or (c.DateOfBirth is not null and DATEPART(day, c.DateOfBirth) = @DateOfBirthDay)) and 
		c.deleted=0
	order by c.RegistrationDate desc 

	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		c.CustomerId,
		c.CustomerGuid,
		c.Email,
		c.Username,
		c.PasswordHash,
		c.SaltKey,
		c.AffiliateId,
		c.BillingAddressId,
		c.ShippingAddressId,
		c.LastPaymentMethodId,
		c.LastAppliedCouponCode,
		c.GiftCardCouponCodes,
		c.CheckoutAttributes,
		c.LanguageId,
		c.CurrencyId,
		c.TaxDisplayTypeId,
		c.IsTaxExempt,
		c.IsAdmin,
		c.IsGuest,
		c.IsForumModerator,
		c.TotalForumPosts,
		c.Signature,
		c.AdminComment,
		c.Active,
		c.Deleted,
		c.RegistrationDate,
		c.TimeZoneId,
		c.AvatarId,
		c.DateOfBirth
	FROM
		#PageIndex [pi]
		INNER JOIN [Nop_Customer] c on c.CustomerID = [pi].CustomerID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
	
END
GO

--Multiple email senders
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_EmailAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_EmailAccount](
	[EmailAccountId] int IDENTITY(1,1) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[DisplayName] nvarchar(255) NOT NULL,
	[Host] nvarchar(255) NOT NULL,
	[Port] int NOT NULL,
	[Username] nvarchar(255) NOT NULL,
	[Password] nvarchar(255) NOT NULL,
	[EnableSSL] bit NOT NULL,
	[UseDefaultCredentials] bit NOT NULL,
 CONSTRAINT [PK_EmailAccount] PRIMARY KEY CLUSTERED 
(
	[EmailAccountId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--migrate default email accont details
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_EmailAccount])
BEGIN

declare @Email nvarchar(255)
declare @DisplayName nvarchar(255)
declare @Host nvarchar(255)
declare @Port int
declare @Username nvarchar(255)
declare @Password nvarchar(255)


SELECT @Email = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailAddress'
SELECT @DisplayName = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailDisplayName'
SELECT @Host = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailHost'
SELECT @Port = isnull([Value], 25) FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPort'
SELECT @Username = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUser'
SELECT @Password = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPassword'

INSERT INTO [Nop_EmailAccount]([Email], [DisplayName], [Host], [Port], [Username], [Password], [EnableSSL], [UseDefaultCredentials])
VALUES (isnull(@Email, N''), isnull(@DisplayName, N''), isnull(@Host, N''), isnull(@Port, 25), isnull(@Username, N''), isnull(@Password, N''), 0, 0)

DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailAddress'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailDisplayName'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailHost'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPort'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUser'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPassword'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUseDefaultCredentials'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailEnableSsl'

END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_MessageTemplateLocalized]') and NAME='EmailAccountId')
BEGIN
	ALTER TABLE [dbo].[Nop_MessageTemplateLocalized] 
	ADD [EmailAccountId] int NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_EmailAccountId] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_QueuedEmail]') and NAME='EmailAccountId')
BEGIN
	ALTER TABLE [dbo].[Nop_QueuedEmail] 
	ADD [EmailAccountId] int NOT NULL CONSTRAINT [DF_Nop_QueuedEmail_EmailAccountId] DEFAULT ((0))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEONames.ConvertNonWesternChars')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEONames.ConvertNonWesternChars', N'True', N'')
END
GO

--CCAvenue payment module
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentMethod]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.Payment.Methods.CCAvenue.CCAvenuePaymentProcessor, Nop.Payment.CCAvenue')
BEGIN
	INSERT [dbo].[Nop_PaymentMethod] ([Name], [VisibleName], [Description], [ConfigureTemplatePath], [UserTemplatePath], [ClassName], [SystemKeyword], [IsActive], [DisplayOrder]) 
	VALUES (N'CCAvenue', N'CCAvenue', N'', N'Payment\CCAvenue\ConfigurePaymentMethod.ascx', N'~\Templates\Payment\CCAvenue\PaymentModule.ascx', N'NopSolutions.NopCommerce.Payment.Methods.CCAvenue.CCAvenuePaymentProcessor, Nop.Payment.CCAvenue', N'CCAVENUE', 0, 290)
END
GO

ALTER TABLE [dbo].[Nop_Address] ALTER COLUMN [ZipPostalCode] nvarchar(30) NOT NULL
GO

ALTER TABLE [dbo].[Nop_Affiliate] ALTER COLUMN [ZipPostalCode] nvarchar(30) NOT NULL
GO

ALTER TABLE [dbo].[Nop_Order] ALTER COLUMN [BillingZipPostalCode] nvarchar(30) NOT NULL
GO

ALTER TABLE [dbo].[Nop_Order] ALTER COLUMN [ShippingZipPostalCode] nvarchar(30) NOT NULL
GO

ALTER TABLE [dbo].[Nop_Warehouse] ALTER COLUMN [ZipPostalCode] nvarchar(30) NOT NULL
GO



--update current version
UPDATE [dbo].[Nop_Setting] 
SET [Value]='1.80'
WHERE [Name]='Common.CurrentVersion'
GO