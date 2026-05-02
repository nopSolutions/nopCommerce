--upgrade scripts from nopCommerce 2.80 to 3.00

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate.Hint">
    <Value>Set the blog post start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate.Hint">
    <Value>Set the blog post end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.StartDate.Hint">
    <Value>Set the poll start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.EndDate.Hint">
    <Value>Set the poll end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.StartDate.Hint">
    <Value>Set the news item start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.EndDate.Hint">
    <Value>Set the news item end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.PageNotFound">
	<Value>Page not found</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Username.Required">
	<Value>Username is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.UsernameIsNotProvided">
	<Value>Username is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.EmailIsNotProvided">
	<Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinimumRewardPointsToUse">
	<Value>Minimum reward points to use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinimumRewardPointsToUse.Hint">
	<Value>Customers won''t be able to use reward points before they have X amount of points. Set to 0 if you do not want to use this setting.</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.MinimumBalance">
	<Value>Minimum balance allowed to use is {0} reward points ({1}).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.AddNew">
	<Value>Add a new store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.BackToList">
	<Value>back to store list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.EditStoreDetails">
	<Value>Edit store details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name">
	<Value>Store name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name.Hint">
	<Value>Enter the name of your store e.g. Your Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DisplayOrder.Hint">
	<Value>The display order for this store. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Added">
	<Value>The new store has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Updated">
	<Value>The store has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Deleted">
	<Value>The store has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.LimitedToStores.Hint">
	<Value>Determines whether the manufacturer is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores.Hint">
	<Value>Select stores for which the manufacturer will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.LimitedToStores.Hint">
	<Value>Determines whether the category is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AvailableStores.Hint">
	<Value>Select stores for which the category will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LimitedToStores.Hint">
	<Value>Determines whether the product is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStores.Hint">
	<Value>Select stores for which the product will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.LimitedToStores.Hint">
	<Value>Determines whether the language is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.AvailableStores.Hint">
	<Value>Select stores for which the language will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.LimitedToStores.Hint">
	<Value>Determines whether the currency is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.AvailableStores.Hint">
	<Value>Select stores for which the currency will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrentCarts.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Store.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Store.Hint">
	<Value>A store name in which this order was placed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Orders.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Hosts">
	<Value>HOST values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Hosts.Hint">
	<Value>The comma separated list of possible HTTP_POST values (for example, "yourstore.com,www.yourstore.com"). This property is required only when you have a multi-store solution to determine the current store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.HTTPHOST">
	<Value>HTTP_HOST</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.HTTPHOST.Hint">
	<Value>HTTP_HOST is used when you have run a multi-store solution to determine the current store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Remove">
	<Value>Remove requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreName">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreName.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreUrl">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreUrl.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Url">
	<Value>Store URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Url.Hint">
	<Value>The URL of your store e.g. http://www.yourstore.com/</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Url.Required">
	<Value>Please provide a store URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.GenerateStaticFileEachMinutes">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.GenerateStaticFileEachMinutes.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskEnabled">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskEnabled.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.TaskRestart">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores.Hint">
	<Value>Determines whether the message template is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AvailableStores.Hint">
	<Value>Select stores for which the message template will be active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Deleted">
	<Value>The message template has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Copy">
	<Value>Copy template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.LimitedToStores.Hint">
	<Value>Determines whether the topic is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AvailableStores.Hint">
	<Value>Select stores for which the topic will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores.Hint">
	<Value>Determines whether the news is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.AvailableStores.Hint">
	<Value>Select stores for which the news will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SslEnabled">
	<Value>SSL enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SslEnabled.Hint">
	<Value>Check if your store will be SSL secured.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SslEnabled.Hint2">
	<Value>WARNING: Do not enable it until you have SSL certificate installed on the server.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SecureUrl">
	<Value>Secure URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.SecureUrl.Hint">
	<Value>The secure URL of your store e.g. https://www.yourstore.com/ or http://sharedssl.yourstore.com/. Leave it empty if you want nopCommerce to detect secure URL automatically.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseSSL">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseSSL.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SharedSSLUrl">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SharedSSLUrl.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.NonSharedSSLUrl">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.NonSharedSSLUrl.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SSLSettings">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SSLSettings.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.ClickHere">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.SuccessResult">
	<Value>Froogle feed has been successfully generated.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Store.Hint">
	<Value>Select the store that will be used to generate the feed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores">
	<Value>All stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.StoreName">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.StoreScope">
	<Value>Multi-store configuration for</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.StoreScope.AllStores">
	<Value>All stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.StoreScope.CheckAll">
	<Value>Check/uncheck all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.StoreScope.CheckAll.Hint">
	<Value>(check boxes if you want to set a custom value for this shop)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CashOnDelivery.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage">
	<Value>Additional fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.EmailAFriend.FriendEmail.Hint">
	<Value>Enter friend''s email</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Topics">
	<Value>Topics</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.OneItemText">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.OneItem">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.SeveralItemsText">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.SeveralItems">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.ItemsText">
	<Value>There are {0} in your cart.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.Items">
	<Value>{0} item(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.PassEditLink">
	<Value>Pass ''edit cart'' link</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.PassEditLink.Hint">
	<Value>Check to pass ''edit cart'' link to Google Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.Fields.IPAddress">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.IPAddress">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.IPAddress.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.Fields.IPAddress">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.HeaderQuantity">
	<Value>({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.HeaderQuantity">
	<Value>({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaKeywords">
	<Value>Meta keywords</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaKeywords.Hint">
	<Value>Meta keywords to be added to blog post page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaDescription">
	<Value>Meta description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaDescription.Hint">
	<Value>Meta description to be added to blog post page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaTitle">
	<Value>Meta title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.MetaTitle.Hint">
	<Value>Override the page title. The default is the title of the blog post.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords">
	<Value>Meta keywords</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords.Hint">
	<Value>Meta keywords to be added to news page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaDescription">
	<Value>Meta description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaDescription.Hint">
	<Value>Meta description to be added to news page header.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaTitle">
	<Value>Meta title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.MetaTitle.Hint">
	<Value>Override the page title. The default is the title of the news.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Info">
	<Value>Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup">
	<Value>Load all locales on startup</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Hint">
	<Value>When enabled, all locale resources will be loaded on application startup. The application start will be slower, but then all pages could be opened much faster.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StoreLastVisitedPage">
	<Value>Store last visited page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StoreLastVisitedPage.Hint">
	<Value>When enabled, the last visited page will be stored. When disabled, it can improved performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Acl">
	<Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Acl">
	<Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Acl">
	<Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores.Hint">
	<Value>Determines whether the blog post is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.AvailableStores.Hint">
	<Value>Select stores for which the blog post will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors">
	<Value>Vendors</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.AddNew">
	<Value>Add a new vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.BackToList">
	<Value>back to vendor list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.EditVendorDetails">
	<Value>Edit vendor details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Added">
	<Value>The new vendor has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Deleted">
	<Value>The vendor has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Updated">
	<Value>The vendor has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Name.Hint">
	<Value>The name of the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Email">
	<Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Email.Hint">
	<Value>Enter email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Email.Required">
	<Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Description">
	<Value>Description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Description.Hint">
	<Value>The description of the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AdminComment">
	<Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AdminComment.Hint">
	<Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Active">
	<Value>Active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Active.Hint">
	<Value>A value indicating whether the vendor is active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Vendor">
	<Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Vendor.Hint">
	<Value>Choose a vendor of this product. This can be useful if you''re using multi-vendor functionality.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Vendor.None">
	<Value>No vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchVendor">
	<Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchVendor.Hint">
	<Value>Search by a specific vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Vendor">
	<Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Vendor.Hint">
	<Value>Search by a specific vendor. You''ll see orders with products from a specified vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Vendor">
	<Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Vendor">
	<Value>Manager of vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Vendor.Hint">
	<Value>Choose a vendor associated to this customer account. When associated this customer will be able to login to the chosen vendor portal and manage his products and orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Vendor.None">
	<Value>Not a vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AssociatedCustomerEmails">
	<Value>Customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AssociatedCustomerEmails.Hint">
	<Value>A list of customer accounts which could be used to manage products and orders of this vendor (have access to the vendor portal). You can associate customers to a vendor on a customer details page. If you don''t want the vendor to have access to the vendor portal, then do not associate any customer account with it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.AssociatedCustomerEmails.None">
	<Value>No customer account associated to this vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AdminCouldNotbeVendor">
	<Value>A customer with a vendor associated could not be in "Administrators" role.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.VendorShouldBeInVendorsRole">
	<Value>Note: if you have a vendor associated with this customer, then also ensure it is in "Vendors" customer role.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Telerik.EditorLocalization.Close">
	<Value>Close</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Telerik.EditorLocalization.Or">
	<Value>or</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Telerik.EditorLocalization.Tooltip">
	<Value>Tooltip</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Telerik.EditorLocalization.WebAddress">
	<Value>Web address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Forums.ForumGroup.Fields.Description">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Forums.ForumGroup.Fields.Description.Hint">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.MaximumShoppingCartItems">
	<Value>The maximum number of distinct products allowed in the cart is {0}.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.MaximumWishlistItems">
	<Value>The maximum number of distinct products allowed in the wishlist is {0}.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated">
	<Value>A customer in the Vendors role should have a vendor account associated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.NoShipments">
	<Value>No shipments selected</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Close">
	<Value>Close</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.UsedDiscounts">
	<Value>Used discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.UsedDiscounts.Hint">
	<Value>A list of used discounts</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.Message.EarnedForRegistration">
	<Value>Registered as customer</Value>
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

--add new "one word" URL to "reservedurlrecordslugs" setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs')
BEGIN
	DECLARE @NewUrlRecord nvarchar(4000)
	SET @NewUrlRecord = N'page-not-found'
	
	DECLARE @reservedurlrecordslugs nvarchar(4000)
	SELECT @reservedurlrecordslugs = [Value] FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs'
	
	IF (CHARINDEX(@NewUrlRecord, @reservedurlrecordslugs) = 0)
	BEGIN
		UPDATE [Setting]
		SET [Value] = @reservedurlrecordslugs + ',' + @NewUrlRecord
		WHERE [name] = N'seosettings.reservedurlrecordslugs'
	END
END
GO


IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Topic]
  WHERE [SystemName] = N'PageNotFound')
BEGIN
	INSERT [dbo].[Topic] ([SystemName], [IncludeInSitemap], [IsPasswordProtected],  [Title], [Body])
	VALUES (N'PageNotFound', 0, 0, N'', N'<p><strong>The page you requested was not found, and we have a fine guess why.</strong>
        <ul>
            <li>If you typed the URL directly, please make sure the spelling is correct.</li>
            <li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li>
        </ul></p>')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.minimumrewardpointstouse')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'rewardpointssettings.minimumrewardpointstouse', N'0')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.multiplethumbdirectories')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.multiplethumbdirectories', N'false')
END
GO



IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Store]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Store](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] nvarchar(400) NOT NULL,
		[Url] nvarchar(400) NOT NULL,
		[SslEnabled] bit NOT NULL,
		[SecureUrl] nvarchar(400) NULL,
		[Hosts] nvarchar(1000) NULL,
		[DisplayOrder] int NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)

	DECLARE @DEFAULT_STORE_NAME nvarchar(400)
	SELECT @DEFAULT_STORE_NAME = [Value] FROM [Setting] WHERE [name] = N'storeinformationsettings.storename' 
	if (@DEFAULT_STORE_NAME is null)
		SET @DEFAULT_STORE_NAME = N'Your store name'
	DECLARE @DEFAULT_STORE_URL nvarchar(400)
	SELECT @DEFAULT_STORE_URL= [Value] FROM [Setting] WHERE [name] = N'storeinformationsettings.storeurl' 
	if (@DEFAULT_STORE_URL is null)
		SET @DEFAULT_STORE_URL = N'http://www.yourstore.com/'

	--create the first store
	INSERT INTO [Store] ([Name], [Url], [SslEnabled], [Hosts], [DisplayOrder])
	VALUES (@DEFAULT_STORE_NAME, @DEFAULT_STORE_URL, 0, N'yourstore.com,www.yourstore.com', 1)

	DELETE FROM [Setting] WHERE [name] = N'storeinformationsettings.storename' 
	DELETE FROM [Setting] WHERE [name] = N'storeinformationsettings.storeurl' 
END
GO

--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageStores')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Stores', N'ManageStores', N'Configuration')

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

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StoreMapping]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[StoreMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityName] nvarchar(400) NOT NULL,
	[StoreId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_StoreMapping_EntityId_EntityName' and object_id=object_id(N'[StoreMapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
END
GO

--Store mapping for manufacturers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Manufacturer]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Manufacturer]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Manufacturer]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Manufacturer] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO


--Store mapping for categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Category]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Category]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Category]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Category] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO



--Store mapping for products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Product]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Product]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [LimitedToStores] bit NOT NULL
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
					FROM [AclRecord] acl
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm
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
					FROM Product_SpecificationAttribute_Mapping psam
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
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
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


--Store mapping for languages
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Language]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Language]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Language]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Language] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO



--Store mapping for currencies
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Currency]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Currency]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Currency]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Currency] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO


--drop some constraints
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Customer_Currency'
           AND parent_object_id = Object_id('Customer')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Customer]
	DROP CONSTRAINT Customer_Currency
	
	EXEC ('UPDATE [Customer] SET [CurrencyId] = 0 WHERE [CurrencyId] IS NULL')

	EXEC ('ALTER TABLE [Customer] ALTER COLUMN [CurrencyId] int NOT NULL')
END
GO



IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Customer_Language'
           AND parent_object_id = Object_id('Customer')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Customer]
	DROP CONSTRAINT Customer_Language
		
	EXEC ('UPDATE [Customer] SET [LanguageId] = 0 WHERE [LanguageId] IS NULL')

	EXEC ('ALTER TABLE [Customer] ALTER COLUMN [LanguageId] int NOT NULL')	
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Customer_Affiliate'
           AND parent_object_id = Object_id('Customer')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Customer]
	DROP CONSTRAINT Customer_Affiliate
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Affiliate_AffiliatedCustomers'
           AND parent_object_id = Object_id('Customer')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Customer]
	DROP CONSTRAINT Affiliate_AffiliatedCustomers
END
GO

UPDATE [Customer]
SET [AffiliateId] = 0
WHERE [AffiliateId] IS NULL
GO

ALTER TABLE [Customer] ALTER COLUMN [AffiliateId] int NOT NULL
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Order_Affiliate'
           AND parent_object_id = Object_id('Order')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Order]
	DROP CONSTRAINT Order_Affiliate
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'Affiliate_AffiliatedOrders'
           AND parent_object_id = Object_id('Order')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[Order]
	DROP CONSTRAINT Affiliate_AffiliatedOrders
END
GO

UPDATE [Order]
SET [AffiliateId] = 0
WHERE [AffiliateId] IS NULL
GO

ALTER TABLE [Order] ALTER COLUMN [AffiliateId] int NOT NULL
GO


--Store mapping to shopping cart items
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShoppingCartItem]') and NAME='StoreId')
BEGIN
	ALTER TABLE [ShoppingCartItem]
	ADD [StoreId] int NULL
END
GO

DECLARE @DEFAULT_STORE_ID int
SELECT @DEFAULT_STORE_ID = [Id] FROM [Store] ORDER BY [DisplayOrder]
UPDATE [ShoppingCartItem]
SET [StoreId] = @DEFAULT_STORE_ID
WHERE [StoreId] IS NULL
GO

ALTER TABLE [ShoppingCartItem] ALTER COLUMN [StoreId] int NOT NULL
GO


--Store mapping to orders
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='StoreId')
BEGIN
	ALTER TABLE [Order]
	ADD [StoreId] int NULL
END
GO

DECLARE @DEFAULT_STORE_ID int
SELECT @DEFAULT_STORE_ID = [Id] FROM [Store] ORDER BY [DisplayOrder]
UPDATE [Order]
SET [StoreId] = @DEFAULT_STORE_ID
WHERE [StoreId] IS NULL
GO

ALTER TABLE [Order] ALTER COLUMN [StoreId] int NOT NULL
GO

--Store mapping to return requests
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ReturnRequest]') and NAME='StoreId')
BEGIN
	ALTER TABLE [ReturnRequest]
	ADD [StoreId] int NULL
END
GO

DECLARE @DEFAULT_STORE_ID int
SELECT @DEFAULT_STORE_ID = [Id] FROM [Store] ORDER BY [DisplayOrder]
UPDATE [ReturnRequest]
SET [StoreId] = @DEFAULT_STORE_ID
WHERE [StoreId] IS NULL
GO

ALTER TABLE [ReturnRequest] ALTER COLUMN [StoreId] int NOT NULL
GO

DELETE FROM [ScheduleTask]
WHERE [Type] like N'Nop.Plugin.Feed.Froogle.StaticFileGenerationTask, Nop.Plugin.Feed.Froogle'

--Store mapping to message templates
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [MessageTemplate]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [MessageTemplate] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO


--Store mapping for topics
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Topic]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Topic]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO




--Store mapping for news
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [News]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [News]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [News] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO


--Store mapping to BackInStockSubscription
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BackInStockSubscription]') and NAME='StoreId')
BEGIN
	ALTER TABLE [BackInStockSubscription]
	ADD [StoreId] int NULL
END
GO

DECLARE @DEFAULT_STORE_ID int
SELECT @DEFAULT_STORE_ID = [Id] FROM [Store] ORDER BY [DisplayOrder]
UPDATE [BackInStockSubscription]
SET [StoreId] = @DEFAULT_STORE_ID
WHERE [StoreId] IS NULL
GO

ALTER TABLE [BackInStockSubscription] ALTER COLUMN [StoreId] int NOT NULL
GO

--Store mapping to Forums_PrivateMessage
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Forums_PrivateMessage]') and NAME='StoreId')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD [StoreId] int NULL
END
GO

DECLARE @DEFAULT_STORE_ID int
SELECT @DEFAULT_STORE_ID = [Id] FROM [Store] ORDER BY [DisplayOrder]
UPDATE [Forums_PrivateMessage]
SET [StoreId] = @DEFAULT_STORE_ID
WHERE [StoreId] IS NULL
GO

ALTER TABLE [Forums_PrivateMessage] ALTER COLUMN [StoreId] int NOT NULL
GO


--GenericAttributes cuold be limited to some specific store name
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[GenericAttribute]') and NAME='StoreId')
BEGIN
	ALTER TABLE [GenericAttribute]
	ADD [StoreId] int NULL
END
GO

UPDATE [GenericAttribute]
SET [StoreId] = 0
WHERE [StoreId] IS NULL
GO

ALTER TABLE [GenericAttribute] ALTER COLUMN [StoreId] int NOT NULL
GO

--delete generic attributes which depends on a specific store now
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'NotifiedAboutNewPrivateMessages' and [StoreId] = 0
GO
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'WorkingDesktopThemeName' and [StoreId] = 0
GO
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'DontUseMobileVersion' and [StoreId] = 0
GO
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'LastContinueShoppingPage' and [StoreId] = 0
GO
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'LastShippingOption' and [StoreId] = 0
GO
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] =N'Customer' and [Key]=N'OfferedShippingOptions' and [StoreId] = 0
GO

--Moved several properties from [Customer] to [GenericAtrribute]
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='TaxDisplayTypeId')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [TaxDisplayTypeId]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='SelectedPaymentMethodSystemName')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [SelectedPaymentMethodSystemName]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='UseRewardPointsDuringCheckout')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [UseRewardPointsDuringCheckout]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='CurrencyId')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [CurrencyId]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='LanguageId')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [LanguageId]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='VatNumber')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [VatNumber]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='VatNumberStatusId')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [VatNumberStatusId]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='TimeZoneId')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [TimeZoneId]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='DiscountCouponCode')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [DiscountCouponCode]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='GiftCardCouponCodes')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [GiftCardCouponCodes]
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='CheckoutAttributes')
BEGIN
	ALTER TABLE [Customer]
	DROP COLUMN [CheckoutAttributes]
END
GO


--Store mapping to Setting
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Setting]') and NAME='StoreId')
BEGIN
	ALTER TABLE [Setting]
	ADD [StoreId] int NULL
END
GO

UPDATE [Setting]
SET [StoreId] = 0
WHERE [StoreId] IS NULL
GO

ALTER TABLE [Setting] ALTER COLUMN [StoreId] int NOT NULL
GO

--built-in user record for background tasks
IF NOT EXISTS (SELECT 1 FROM [Customer] WHERE [SystemName] = N'BackgroundTask')
BEGIN
	INSERT [Customer] ([CustomerGuid], [Email], [PasswordFormatId], [AdminComment], [IsTaxExempt], [AffiliateId], [Active], [Deleted], [IsSystemAccount], [SystemName], [CreatedOnUtc], [LastActivityDateUtc]) 
	VALUES (NEWID(), N'builtin@background-task-record.com', 0, N'Built-in system record used for background tasks.', 0, 0, 1, 0, 1, N'BackgroundTask',GETUTCDATE(),GETUTCDATE())
END
GO

--move records from CustomerContent to NewsComment
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[NewsComment]') and NAME='CreatedOnUtc')
BEGIN
	ALTER TABLE [NewsComment]
	ADD [CreatedOnUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[NewsComment]') and NAME='CustomerId')
BEGIN
	ALTER TABLE [NewsComment]
	ADD [CustomerId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DECLARE @ExistingNewsCommentID int
	DECLARE cur_existingcomment CURSOR FOR
	SELECT [ID]
	FROM [NewsComment]
	OPEN cur_existingcomment
	FETCH NEXT FROM cur_existingcomment INTO @ExistingNewsCommentID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @CustomerID int
		SET @CustomerID = null -- clear cache (variable scope)
		
		DECLARE @CreatedOnUtc datetime
		SET @CreatedOnUtc = null -- clear cache (variable scope)
		
		DECLARE @sql nvarchar(4000)
		SET @sql = 'SELECT @CustomerID = cc.[CustomerId], @CreatedOnUtc = cc.[CreatedOnUtc] FROM [CustomerContent] cc WHERE cc.[Id]=' + ISNULL(CAST(@ExistingNewsCommentID AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@CustomerID int OUTPUT, @CreatedOnUtc datetime OUTPUT',@CustomerID OUTPUT,@CreatedOnUtc OUTPUT
		
		UPDATE [NewsComment] 
		SET [CustomerId] = @CustomerID,
		[CreatedOnUtc] = @CreatedOnUtc
		WHERE [Id]=@ExistingNewsCommentID
		
		--fetch next language identifier
		FETCH NEXT FROM cur_existingcomment INTO @ExistingNewsCommentID
	END
	CLOSE cur_existingcomment
	DEALLOCATE cur_existingcomment
END
GO

ALTER TABLE [NewsComment] ALTER COLUMN [CustomerId] int NOT NULL
GO

ALTER TABLE [NewsComment] ALTER COLUMN [CreatedOnUtc] datetime NOT NULL
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'NewsComment_Customer'
           AND parent_object_id = Object_id('NewsComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[NewsComment] WITH CHECK ADD CONSTRAINT [NewsComment_Customer] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customer] ([Id])
	ON DELETE CASCADE
END
GO


--move records from CustomerContent to BlogComment
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogComment]') and NAME='CreatedOnUtc')
BEGIN
	ALTER TABLE [BlogComment]
	ADD [CreatedOnUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogComment]') and NAME='CustomerId')
BEGIN
	ALTER TABLE [BlogComment]
	ADD [CustomerId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DECLARE @ExistingBlogCommentID int
	DECLARE cur_existingcomment CURSOR FOR
	SELECT [ID]
	FROM [BlogComment]
	OPEN cur_existingcomment
	FETCH NEXT FROM cur_existingcomment INTO @ExistingBlogCommentID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @CustomerID int
		SET @CustomerID = null -- clear cache (variable scope)
		
		DECLARE @CreatedOnUtc datetime
		SET @CreatedOnUtc = null -- clear cache (variable scope)
		
		DECLARE @sql nvarchar(4000)
		SET @sql = 'SELECT @CustomerID = cc.[CustomerId], @CreatedOnUtc = cc.[CreatedOnUtc] FROM [CustomerContent] cc WHERE cc.[Id]=' + ISNULL(CAST(@ExistingBlogCommentID AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@CustomerID int OUTPUT, @CreatedOnUtc datetime OUTPUT',@CustomerID OUTPUT,@CreatedOnUtc OUTPUT
		
		UPDATE [BlogComment] 
		SET [CustomerId] = @CustomerID,
		[CreatedOnUtc] = @CreatedOnUtc
		WHERE [Id]=@ExistingBlogCommentID
		
		--fetch next language identifier
		FETCH NEXT FROM cur_existingcomment INTO @ExistingBlogCommentID
	END
	CLOSE cur_existingcomment
	DEALLOCATE cur_existingcomment
END
GO

ALTER TABLE [BlogComment] ALTER COLUMN [CustomerId] int NOT NULL
GO

ALTER TABLE [BlogComment] ALTER COLUMN [CreatedOnUtc] datetime NOT NULL
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'BlogComment_Customer'
           AND parent_object_id = Object_id('BlogComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[BlogComment] WITH CHECK ADD CONSTRAINT [BlogComment_Customer] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customer] ([Id])
	ON DELETE CASCADE
END
GO


--move records from CustomerContent to ProductReview
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='CreatedOnUtc')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [CreatedOnUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='IsApproved')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [IsApproved] bit NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='CustomerId')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [CustomerId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DECLARE @ExistingProductReviewID int
	DECLARE cur_existingcomment CURSOR FOR
	SELECT [ID]
	FROM [ProductReview]
	OPEN cur_existingcomment
	FETCH NEXT FROM cur_existingcomment INTO @ExistingProductReviewID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @CustomerID int
		SET @CustomerID = null -- clear cache (variable scope)
		
		DECLARE @IsApproved bit
		SET @IsApproved = null -- clear cache (variable scope)
		
		DECLARE @CreatedOnUtc datetime
		SET @CreatedOnUtc = null -- clear cache (variable scope)
		
		DECLARE @sql nvarchar(4000)
		SET @sql = 'SELECT @CustomerID = cc.[CustomerId], @IsApproved = cc.[IsApproved], @CreatedOnUtc = cc.[CreatedOnUtc] FROM [CustomerContent] cc WHERE cc.[Id]=' + ISNULL(CAST(@ExistingProductReviewID AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@CustomerID int OUTPUT, @IsApproved bit OUTPUT, @CreatedOnUtc datetime OUTPUT',@CustomerID OUTPUT,@IsApproved OUTPUT,@CreatedOnUtc OUTPUT
		
		UPDATE [ProductReview] 
		SET [CustomerId] = @CustomerID,
		[IsApproved] = @IsApproved,
		[CreatedOnUtc] = @CreatedOnUtc
		WHERE [Id]=@ExistingProductReviewID
		
		--fetch next language identifier
		FETCH NEXT FROM cur_existingcomment INTO @ExistingProductReviewID
	END
	CLOSE cur_existingcomment
	DEALLOCATE cur_existingcomment
END
GO

ALTER TABLE [ProductReview] ALTER COLUMN [CustomerId] int NOT NULL
GO

ALTER TABLE [ProductReview] ALTER COLUMN [IsApproved] bit NOT NULL
GO

ALTER TABLE [ProductReview] ALTER COLUMN [CreatedOnUtc] datetime NOT NULL
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReview_Customer'
           AND parent_object_id = Object_id('ProductReview')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[ProductReview] WITH CHECK ADD CONSTRAINT [ProductReview_Customer] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customer] ([Id])
	ON DELETE CASCADE
END
GO



--move records from CustomerContent to ProductReviewHelpfulness
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReviewHelpfulness]') and NAME='CustomerId')
BEGIN
	ALTER TABLE [ProductReviewHelpfulness]
	ADD [CustomerId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DECLARE @ExistingProductReviewHelpfulnessID int
	DECLARE cur_existingcomment CURSOR FOR
	SELECT [ID]
	FROM [ProductReviewHelpfulness]
	OPEN cur_existingcomment
	FETCH NEXT FROM cur_existingcomment INTO @ExistingProductReviewHelpfulnessID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @CustomerID int
		SET @CustomerID = null -- clear cache (variable scope)
		
		DECLARE @sql nvarchar(4000)
		SET @sql = 'SELECT @CustomerID = cc.[CustomerId] FROM [CustomerContent] cc WHERE cc.[Id]=' + ISNULL(CAST(@ExistingProductReviewHelpfulnessID AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@CustomerID int OUTPUT',@CustomerID OUTPUT
		
		UPDATE [ProductReviewHelpfulness] 
		SET [CustomerId] = @CustomerID
		WHERE [Id]=@ExistingProductReviewHelpfulnessID
		
		--fetch next language identifier
		FETCH NEXT FROM cur_existingcomment INTO @ExistingProductReviewHelpfulnessID
	END
	CLOSE cur_existingcomment
	DEALLOCATE cur_existingcomment
END
GO

ALTER TABLE [ProductReviewHelpfulness] ALTER COLUMN [CustomerId] int NOT NULL
GO



--move records from CustomerContent to PollVotingRecord
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[PollVotingRecord]') and NAME='CustomerId')
BEGIN
	ALTER TABLE [PollVotingRecord]
	ADD [CustomerId] int NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[PollVotingRecord]') and NAME='CreatedOnUtc')
BEGIN
	ALTER TABLE [PollVotingRecord]
	ADD [CreatedOnUtc] datetime NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DECLARE @ExistingPollVotingRecordID int
	DECLARE cur_existingcomment CURSOR FOR
	SELECT [ID]
	FROM [PollVotingRecord]
	OPEN cur_existingcomment
	FETCH NEXT FROM cur_existingcomment INTO @ExistingPollVotingRecordID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @CustomerID int
		SET @CustomerID = null -- clear cache (variable scope)
		
		DECLARE @CreatedOnUtc datetime
		SET @CreatedOnUtc = null -- clear cache (variable scope)
		
		DECLARE @sql nvarchar(4000)
		SET @sql = 'SELECT @CustomerID = cc.[CustomerId], @CreatedOnUtc = cc.[CreatedOnUtc] FROM [CustomerContent] cc WHERE cc.[Id]=' + ISNULL(CAST(@ExistingPollVotingRecordID AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@CustomerID int OUTPUT, @CreatedOnUtc datetime OUTPUT',@CustomerID OUTPUT, @CreatedOnUtc OUTPUT
		
		UPDATE [PollVotingRecord] 
		SET [CustomerId] = @CustomerID,
		[CreatedOnUtc] = @CreatedOnUtc
		WHERE [Id]=@ExistingPollVotingRecordID
		
		--fetch next language identifier
		FETCH NEXT FROM cur_existingcomment INTO @ExistingPollVotingRecordID
	END
	CLOSE cur_existingcomment
	DEALLOCATE cur_existingcomment
END
GO

ALTER TABLE [PollVotingRecord] ALTER COLUMN [CustomerId] int NOT NULL
GO

UPDATE [PollVotingRecord]
SET [CreatedOnUtc] = GETUTCDATE()
WHERE [CreatedOnUtc] is null
GO 

ALTER TABLE [PollVotingRecord] ALTER COLUMN [CreatedOnUtc] datetime NOT NULL
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PollVotingRecord_Customer'
           AND parent_object_id = Object_id('PollVotingRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[PollVotingRecord] WITH CHECK ADD CONSTRAINT [PollVotingRecord_Customer] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customer] ([Id])
	ON DELETE CASCADE
END
GO

--remove CustomerContent table 
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'BlogComment_TypeConstraint_From_CustomerContent_To_BlogComment'
           AND parent_object_id = Object_id('BlogComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[BlogComment]
	DROP CONSTRAINT BlogComment_TypeConstraint_From_CustomerContent_To_BlogComment
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReview_TypeConstraint_From_CustomerContent_To_ProductReview'
           AND parent_object_id = Object_id('ProductReview')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReview]
	DROP CONSTRAINT ProductReview_TypeConstraint_From_CustomerContent_To_ProductReview
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReviewHelpfulness_TypeConstraint_From_CustomerContent_To_ProductReviewHelpfulness'
           AND parent_object_id = Object_id('ProductReviewHelpfulness')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReviewHelpfulness]
	DROP CONSTRAINT ProductReviewHelpfulness_TypeConstraint_From_CustomerContent_To_ProductReviewHelpfulness
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'NewsComment_TypeConstraint_From_CustomerContent_To_NewsComment'
           AND parent_object_id = Object_id('NewsComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[NewsComment]
	DROP CONSTRAINT NewsComment_TypeConstraint_From_CustomerContent_To_NewsComment
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PollVotingRecord_TypeConstraint_From_CustomerContent_To_PollVotingRecord'
           AND parent_object_id = Object_id('PollVotingRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[PollVotingRecord]
	DROP CONSTRAINT PollVotingRecord_TypeConstraint_From_CustomerContent_To_PollVotingRecord
END
GO

IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'CustomerContent_Customer'
           AND parent_object_id = Object_id('CustomerContent')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[CustomerContent]
	DROP CONSTRAINT CustomerContent_Customer
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerContent]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC('DROP TABLE [CustomerContent]')
END
GO

--now we should add IDENTITY to the primary keys of these tables (moved from CustomerContent)
--1. Product reviews
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReview_Customer'
           AND parent_object_id = Object_id('ProductReview')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReview]
	DROP CONSTRAINT ProductReview_Customer
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReview_Product1'
           AND parent_object_id = Object_id('ProductReview')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReview]
	DROP CONSTRAINT ProductReview_Product1
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReview_Product'
           AND parent_object_id = Object_id('ProductReview')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReview]
	DROP CONSTRAINT ProductReview_Product
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReviewHelpfulness_ProductReview1'
           AND parent_object_id = Object_id('ProductReviewHelpfulness')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReviewHelpfulness]
	DROP CONSTRAINT ProductReviewHelpfulness_ProductReview1
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReviewHelpfulness_ProductReview'
           AND parent_object_id = Object_id('ProductReviewHelpfulness')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReviewHelpfulness]
	DROP CONSTRAINT ProductReviewHelpfulness_ProductReview
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tmp_ProductReview]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Tmp_ProductReview](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[ReviewText] [nvarchar](max) NULL,
	[Rating] [int] NOT NULL,
	[HelpfulYesTotal] [int] NOT NULL,
	[HelpfulNoTotal] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT dbo.Tmp_ProductReview ON
GO
IF EXISTS(SELECT TOP 1 * FROM dbo.ProductReview)
EXEC('INSERT INTO dbo.Tmp_ProductReview ([Id],[ProductId],[Title],[ReviewText],[Rating],[HelpfulYesTotal],[HelpfulNoTotal],[CreatedOnUtc],[CustomerId],[IsApproved])
SELECT [Id],[ProductId],[Title],[ReviewText],[Rating],[HelpfulYesTotal],[HelpfulNoTotal],[CreatedOnUtc],[CustomerId],[IsApproved] FROM dbo.ProductReview')
GO
SET IDENTITY_INSERT dbo.Tmp_ProductReview OFF
GO
DROP TABLE dbo.ProductReview
GO
EXECUTE sp_rename N'dbo.Tmp_ProductReview', N'ProductReview', 'OBJECT'
GO
ALTER TABLE [dbo].[ProductReview]  WITH CHECK ADD  CONSTRAINT [ProductReview_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductReview]  WITH CHECK ADD  CONSTRAINT [ProductReview_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductReviewHelpfulness]  WITH CHECK ADD  CONSTRAINT [ProductReviewHelpfulness_ProductReview] FOREIGN KEY([ProductReviewId])
REFERENCES [dbo].[ProductReview] ([Id])
ON DELETE CASCADE
GO





--2. News comment
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'NewsComment_Customer'
           AND parent_object_id = Object_id('NewsComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[NewsComment]
	DROP CONSTRAINT NewsComment_Customer
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'NewsComment_NewsItem1'
           AND parent_object_id = Object_id('NewsComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[NewsComment]
	DROP CONSTRAINT NewsComment_NewsItem1
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'NewsComment_NewsItem'
           AND parent_object_id = Object_id('NewsComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[NewsComment]
	DROP CONSTRAINT NewsComment_NewsItem
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tmp_NewsComment]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Tmp_NewsComment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommentTitle] [nvarchar](max) NULL,
	[CommentText] [nvarchar](max) NULL,
	[NewsItemId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT dbo.Tmp_NewsComment ON
GO
IF EXISTS(SELECT TOP 1 * FROM dbo.NewsComment)
EXEC('INSERT INTO dbo.Tmp_NewsComment ([Id],[CommentTitle],[CommentText],[NewsItemId],[CustomerId],[CreatedOnUtc])
SELECT [Id],[CommentTitle],[CommentText],[NewsItemId],[CustomerId],[CreatedOnUtc] FROM dbo.NewsComment')
GO
SET IDENTITY_INSERT dbo.Tmp_NewsComment OFF
GO
DROP TABLE dbo.NewsComment
GO
EXECUTE sp_rename N'dbo.Tmp_NewsComment', N'NewsComment', 'OBJECT'
GO
ALTER TABLE [dbo].[NewsComment]  WITH CHECK ADD  CONSTRAINT [NewsComment_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[NewsComment]  WITH CHECK ADD  CONSTRAINT [NewsComment_NewsItem] FOREIGN KEY([NewsItemId])
REFERENCES [dbo].[News] ([Id])
ON DELETE CASCADE
GO


--3. Blog comment
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'BlogComment_Customer'
           AND parent_object_id = Object_id('BlogComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[BlogComment]
	DROP CONSTRAINT BlogComment_Customer
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'BlogComment_BlogPost1'
           AND parent_object_id = Object_id('BlogComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[BlogComment]
	DROP CONSTRAINT BlogComment_BlogPost1
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'BlogComment_BlogPost'
           AND parent_object_id = Object_id('BlogComment')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[BlogComment]
	DROP CONSTRAINT BlogComment_BlogPost
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tmp_BlogComment]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Tmp_BlogComment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommentText] [nvarchar](max) NULL,
	[BlogPostId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT dbo.Tmp_BlogComment ON
GO
IF EXISTS(SELECT TOP 1 * FROM dbo.BlogComment)
EXEC('INSERT INTO dbo.Tmp_BlogComment ([Id],[CommentText],[BlogPostId],[CustomerId],[CreatedOnUtc])
SELECT [Id],[CommentText],[BlogPostId],[CustomerId],[CreatedOnUtc] FROM dbo.BlogComment')
GO
SET IDENTITY_INSERT dbo.Tmp_BlogComment OFF
GO
DROP TABLE dbo.BlogComment
GO
EXECUTE sp_rename N'dbo.Tmp_BlogComment', N'BlogComment', 'OBJECT'
GO
ALTER TABLE [dbo].[BlogComment]  WITH CHECK ADD  CONSTRAINT [BlogComment_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlogComment]  WITH CHECK ADD  CONSTRAINT [BlogComment_BlogPost] FOREIGN KEY([BlogPostId])
REFERENCES [dbo].[BlogPost] ([Id])
ON DELETE CASCADE
GO



--4. Product review helpfulness
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReviewHelpfulness_ProductReview'
           AND parent_object_id = Object_id('ProductReviewHelpfulness')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReviewHelpfulness]
	DROP CONSTRAINT ProductReviewHelpfulness_ProductReview
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductReviewHelpfulness_ProductReview1'
           AND parent_object_id = Object_id('ProductReviewHelpfulness')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[ProductReviewHelpfulness]
	DROP CONSTRAINT ProductReviewHelpfulness_ProductReview1
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tmp_ProductReviewHelpfulness]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Tmp_ProductReviewHelpfulness](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductReviewId] [int] NOT NULL,
	[WasHelpful] [bit] NOT NULL,
	[CustomerId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT dbo.Tmp_ProductReviewHelpfulness ON
GO
IF EXISTS(SELECT TOP 1 * FROM dbo.ProductReviewHelpfulness)
EXEC('INSERT INTO dbo.Tmp_ProductReviewHelpfulness ([Id],[ProductReviewId],[WasHelpful],[CustomerId])
SELECT [Id],[ProductReviewId],[WasHelpful],[CustomerId] FROM dbo.ProductReviewHelpfulness')
GO
SET IDENTITY_INSERT dbo.Tmp_ProductReviewHelpfulness OFF
GO
DROP TABLE dbo.ProductReviewHelpfulness
GO
EXECUTE sp_rename N'dbo.Tmp_ProductReviewHelpfulness', N'ProductReviewHelpfulness', 'OBJECT'
GO
ALTER TABLE [dbo].[ProductReviewHelpfulness]  WITH CHECK ADD  CONSTRAINT [ProductReviewHelpfulness_ProductReview] FOREIGN KEY([ProductReviewId])
REFERENCES [dbo].[ProductReview] ([Id])
ON DELETE CASCADE
GO



--5. Poll voting record
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PollVotingRecord_Customer'
           AND parent_object_id = Object_id('PollVotingRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[PollVotingRecord]
	DROP CONSTRAINT PollVotingRecord_Customer
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PollVotingRecord_PollAnswer1'
           AND parent_object_id = Object_id('PollVotingRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[PollVotingRecord]
	DROP CONSTRAINT PollVotingRecord_PollAnswer1
END
GO
IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'PollVotingRecord_PollAnswer'
           AND parent_object_id = Object_id('PollVotingRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE dbo.[PollVotingRecord]
	DROP CONSTRAINT PollVotingRecord_PollAnswer
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tmp_PollVotingRecord]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Tmp_PollVotingRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PollAnswerId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT dbo.Tmp_PollVotingRecord ON
GO
IF EXISTS(SELECT TOP 1 * FROM dbo.PollVotingRecord)
EXEC('INSERT INTO dbo.Tmp_PollVotingRecord ([Id],[PollAnswerId],[CustomerId],[CreatedOnUtc])
SELECT [Id],[PollAnswerId],[CustomerId],[CreatedOnUtc] FROM dbo.PollVotingRecord')
GO
SET IDENTITY_INSERT dbo.Tmp_PollVotingRecord OFF
GO
DROP TABLE dbo.PollVotingRecord
GO
EXECUTE sp_rename N'dbo.Tmp_PollVotingRecord', N'PollVotingRecord', 'OBJECT'
GO
ALTER TABLE [dbo].[PollVotingRecord]  WITH CHECK ADD  CONSTRAINT [PollVotingRecord_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PollVotingRecord]  WITH CHECK ADD  CONSTRAINT [PollVotingRecord_PollAnswer] FOREIGN KEY([PollAnswerId])
REFERENCES [dbo].[PollAnswer] ([Id])
ON DELETE CASCADE
GO


--drop [ApprovedCommentCount] and [NotApprovedCommentCount] columns
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='ApprovedCommentCount')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [CommentCount] int NULL
	
	EXEC ('UPDATE [BlogPost] SET [CommentCount] = [ApprovedCommentCount]')

	ALTER TABLE [BlogPost] ALTER COLUMN [CommentCount] int NOT NULL
	
	EXEC ('ALTER TABLE [BlogPost] DROP COLUMN [ApprovedCommentCount]')
	
	EXEC ('ALTER TABLE [BlogPost] DROP COLUMN [NotApprovedCommentCount]')
END
GO

--drop [ApprovedCommentCount] and [NotApprovedCommentCount] columns
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='ApprovedCommentCount')
BEGIN
	ALTER TABLE [News]
	ADD [CommentCount] int NULL
	
	EXEC ('UPDATE [News] SET [CommentCount] = [ApprovedCommentCount]')

	ALTER TABLE [News] ALTER COLUMN [CommentCount] int NOT NULL
	
	EXEC ('ALTER TABLE [News] DROP COLUMN [ApprovedCommentCount]')
	
	EXEC ('ALTER TABLE [News] DROP COLUMN [NotApprovedCommentCount]')
END
GO


--SEO settings for blog
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='MetaKeywords')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [MetaKeywords] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='MetaTitle')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [MetaTitle] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='MetaDescription')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [MetaDescription] nvarchar(MAX) NULL
END
GO


--SEO settings for news
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='MetaKeywords')
BEGIN
	ALTER TABLE [News]
	ADD [MetaKeywords] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='MetaTitle')
BEGIN
	ALTER TABLE [News]
	ADD [MetaTitle] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='MetaDescription')
BEGIN
	ALTER TABLE [News]
	ADD [MetaDescription] nvarchar(MAX) NULL
END
GO


--delete abondoned records from [GenericAttribute] table
DELETE FROM [GenericAttribute]
WHERE [EntityId] NOT IN (SELECT c.[Id] FROM [Customer] c) and [KeyGroup]=N'Customer'
GO



--Store mapping for blog posts
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [BlogPost]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [BlogPost] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO



--vendor support
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Vendor]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Vendor](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] nvarchar(400) NOT NULL,
		[Email] nvarchar(400) NOT NULL,
		[Description] nvarchar(MAX) NULL,
		[AdminComment] nvarchar(MAX) NULL,
		[Active] [bit] NOT NULL,
		[Deleted] [bit] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageVendors')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Vendors', N'ManageVendors', N'Customers')

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

--Product-vendor mapping
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='VendorId')
BEGIN
	ALTER TABLE [Product]
	ADD [VendorId] int NULL
END
GO

UPDATE [Product]
SET [VendorId] = 0
WHERE [VendorId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [VendorId] int NOT NULL
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
					FROM [AclRecord] acl
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm
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
					FROM Product_SpecificationAttribute_Mapping psam
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
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
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

--Customer-vendor mapping (managers)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='VendorId')
BEGIN
	ALTER TABLE [Customer]
	ADD [VendorId] int NULL
END
GO

UPDATE [Customer]
SET [VendorId] = 0
WHERE [VendorId] IS NULL
GO

ALTER TABLE [Customer] ALTER COLUMN [VendorId] int NOT NULL
GO

--new "Vendors" customer role
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[CustomerRole]
		WHERE [SystemName] = N'Vendors' and [IsSystemRole]=1)
BEGIN
	INSERT [dbo].[CustomerRole] ([Name], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [SystemName])
	VALUES (N'Vendors', 0, 0, 1, 1, N'Vendors')
	
	DECLARE @VendorsCustomerRoleId INT 
	SET @VendorsCustomerRoleId = @@IDENTITY
	
	DECLARE @AccessAdminPanelPermissionRecordId INT 
	SELECT @AccessAdminPanelPermissionRecordId = [Id] FROM [PermissionRecord]
	WHERE [SystemName] = N'AccessAdminPanel' 

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@AccessAdminPanelPermissionRecordId, @VendorsCustomerRoleId)
	
	
	DECLARE @ManageOrdersPermissionRecordId INT 
	SELECT @ManageOrdersPermissionRecordId = [Id] FROM [PermissionRecord]
	WHERE [SystemName] = N'ManageOrders' 

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@ManageOrdersPermissionRecordId, @VendorsCustomerRoleId)
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'AccessAdminPanel')
BEGIN
	DECLARE @PermissionRecordId INT 
	SELECT @PermissionRecordId = [Id] FROM [PermissionRecord]
	WHERE [SystemName] = N'AccessAdminPanel' 

	--add it to vendors role
	DECLARE @VendorsCustomerRoleId int
	SELECT @VendorsCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Vendors'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @VendorsCustomerRoleId)
END
GO
--split "Manage catalog" permission to several permissions
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageProducts')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Products', N'ManageProducts', N'Catalog')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admin role by default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)


	--add it to vendors role by default
	DECLARE @VendorsCustomerRoleId int
	SELECT @VendorsCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Vendors'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @VendorsCustomerRoleId)
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageCategories')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Categories', N'ManageCategories', N'Catalog')

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
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageManufacturers')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Manufacturers', N'ManageManufacturers', N'Catalog')

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
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageProductReviews')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Product Reviews', N'ManageProductReviews', N'Catalog')

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
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageProductTags')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Product Tags', N'ManageProductTags', N'Catalog')

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
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageAttributes')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Attributes', N'ManageAttributes', N'Catalog')

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
--delete obsolete permission
DELETE FROM [dbo].[PermissionRecord]
WHERE [SystemName] = N'ManageCatalog'
GO
--enable "Hide admin menu items based on permissions" setting
UPDATE [Setting]
SET [Value] = N'true'
WHERE [name] = N'securitysettings.hideadminmenuitemsbasedonpermissions'
--delete obsolete permission
DELETE FROM [dbo].[PermissionRecord]
WHERE [SystemName] = N'UploadPictures'
GO

--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageCurrentCarts')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Current Carts', N'ManageCurrentCarts', N'Orders')

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

--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageRecurringPayments')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Recurring Payments', N'ManageRecurringPayments', N'Orders')

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


--'Order placed for vendor' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderPlaced.VendorNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'OrderPlaced.VendorNotification', null, N'%Store.Name%. Order placed', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just placed on order. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%</p>', 0, 0, 0)
END
GO

DELETE FROM [Setting] 
WHERE [name] = N'GoogleAnalyticsSettings.WidgetZone' 
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.breadcrumbdelimiter')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.breadcrumbdelimiter', N'/', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.compareproductsnumber')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.compareproductsnumber', N'4', 0)
END
GO

--do not store product tag count
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductTag]') and NAME='ProductCount')
BEGIN
	ALTER TABLE [ProductTag]
	DROP COLUMN [ProductCount]
END
GO



--stored procedure to load product tags
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE OBJECT_ID = OBJECT_ID(N'[ProductTagCountLoadAll]') AND OBJECTPROPERTY(OBJECT_ID,N'IsProcedure') = 1)
DROP PROCEDURE [ProductTagCountLoadAll]
GO
CREATE PROCEDURE [dbo].[ProductTagCountLoadAll]
(
	@StoreId int
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT pt.Id as [ProductTagId], COUNT(p.Id) as [ProductCount]
	FROM ProductTag pt with (NOLOCK)
	LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON pt.[Id] = pptm.[ProductTag_Id]
	LEFT JOIN Product p with (NOLOCK) ON pptm.[Product_Id] = p.[Id]
	WHERE
		p.[Deleted] = 0
		AND p.Published = 1
		AND (@StoreId = 0 or (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = 'Product' and [sm].StoreId=@StoreId
			)))
	GROUP BY pt.Id
	ORDER BY pt.Id
END
GO

--forum group do not need description
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Forums_Group]') and NAME='Description')
BEGIN
	ALTER TABLE [Forums_Group]
	DROP COLUMN [Description]
END
GO
