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
