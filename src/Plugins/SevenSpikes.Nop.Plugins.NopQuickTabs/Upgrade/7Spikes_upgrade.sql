-- 7Spikes upgrade scripts from nopCommerce 3.80 to 3.90

-- Upgrade script for MegaMenu
IF(EXISTS (SELECT NULL FROM sys.objects WHERE object_id = OBJECT_ID(N'[SS_MM_MenuItem]') AND type in (N'U')))
BEGIN
	IF( NOT EXISTS (SELECT NULL FROM sys.columns WHERE object_id = object_id(N'[SS_MM_MenuItem]') and NAME='SubjectToAcl'))
	BEGIN
		ALTER TABLE [SS_MM_MenuItem] ADD [SubjectToAcl] BIT NOT NULL DEFAULT 0;
	END
END

-- Upgrade script for Customer Reminders Message Template Description
IF(EXISTS (SELECT NULL FROM sys.objects WHERE object_id = OBJECT_ID(N'[SS_CR_Reminder]') AND type in (N'U')))
BEGIN
	DECLARE @AbandonedShoppingCartRuleId nvarchar(MAX) = 'B35F25F6-CE81-4CD6-A173-EECFCDEFD1C6'
	DECLARE @BirthdayReminderRuleId nvarchar(MAX) = '2289AE95-4F22-45C5-9F3A-2994E2F1358B'
	DECLARE @CompletedOrderRuleId nvarchar(MAX) = '5E9D160A-DC30-4B28-AAC2-EE09215D5E26'
	DECLARE @UnpaidOrderRuleId nvarchar(MAX) = 'AEA3E761-71B5-494E-94C9-149924316931'
	DECLARE @InactiveCustomerRuleId nvarchar(MAX) = 'E06161E2-0331-45FA-94FC-413437222DE7'
	
	DECLARE @BaseMessageTemplateDescriptionResourceName nvarchar(MAX) = 'Admin.ContentManagement.MessageTemplates.Description.'
	DECLARE @MessageTemplateDescriptionName nvarchar(MAX)
	
	DECLARE @CurrentCustomerReminderType nvarchar(MAX)
	DECLARE @AbandonedShoppingCartRuleResource nvarchar(MAX) = 'This message template is used to send message to the customers with abandoned shopping card. The message is received by a customer. You can disable this message from &lt;strong&gt;Administration -> Nop-Templates -> Plugins -> Customer Reminders -> Manage Customer Reminders&lt;/strong&gt;.'
	DECLARE @BirthdayReminderRuleRuleResource nvarchar(MAX) = 'This message template is used to send message congratulating a customer for their birthday. The message is received by a customer. You can disable this message from &lt;strong&gt;Administration -> Nop-Templates -> Plugins -> Customer Reminders -> Manage Customer Reminders&lt;/strong&gt;.'
	DECLARE @CompletedOrderRuleRuleResource nvarchar(MAX) = 'This message template is used to send a message to a customer for a completed order. The message is received by a customer. You can disable this message from &lt;strong&gt;Administration -> Nop-Templates -> Plugins -> Customer Reminders -> Manage Customer Reminders&lt;/strong&gt;.'
	DECLARE @UnpaidOrderRuleRuleResource nvarchar(MAX) = 'This message template is used to send a message customers with unpaid orders. The message is received by the customers with unpaid orders. You can disable this message from &lt;strong&gt;Administration -> Nop-Templates -> Plugins -> Customer Reminders -> Manage Customer Reminders&lt;/strong&gt;.'
	DECLARE @InactiveCustomerRuleRuleResource nvarchar(MAX) = 'This message template is used to send a message to inactive customers. The message is received by the inactive customer. You can disable this message from &lt;strong&gt;Administration -> Nop-Templates -> Plugins -> Customer Reminders -> Manage Customer Reminders&lt;/strong&gt;.'
	
	DECLARE @MtName nvarchar(MAX)
	DECLARE @CrRuleId nvarchar(MAX)
	DECLARE @LanguageId nvarchar(MAX)
	
	DECLARE AllReminders CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT mt.Name, cr.ReminderRuleId  FROM [SS_CR_Reminder] AS cr
	JOIN MessageTemplate AS mt ON cr.MessageTemplateId = mt.Id
	ORDER BY cr.Id ASC

	OPEN AllReminders
	FETCH NEXT FROM AllReminders INTO @MtName, @CrRuleId
	WHILE @@FETCH_STATUS = 0
	BEGIN 		
			SET @MessageTemplateDescriptionName = @BaseMessageTemplateDescriptionResourceName + @MtName				
			
			SET @CurrentCustomerReminderType = 
			(
				CASE
					WHEN @CrRuleId = @AbandonedShoppingCartRuleId THEN @AbandonedShoppingCartRuleResource
					WHEN @CrRuleId = @BirthdayReminderRuleId THEN @BirthdayReminderRuleRuleResource
					WHEN @CrRuleId = @CompletedOrderRuleId THEN @CompletedOrderRuleRuleResource
					WHEN @CrRuleId = @UnpaidOrderRuleId THEN @UnpaidOrderRuleRuleResource
					WHEN @CrRuleId = @InactiveCustomerRuleId THEN @InactiveCustomerRuleRuleResource
					ELSE ''
				END
			);
				
			DECLARE AllLanguages CURSOR 
			LOCAL STATIC READ_ONLY FORWARD_ONLY
			FOR 
			SELECT Id FROM [Language]
		
			OPEN AllLanguages
			FETCH NEXT FROM AllLanguages INTO @LanguageId
			WHILE @@FETCH_STATUS = 0
			BEGIN 											
				INSERT INTO [LocaleStringResource] (LanguageId, ResourceName, ResourceValue) VALUES (@LanguageId, @MessageTemplateDescriptionName, @CurrentCustomerReminderType)			
			
				FETCH NEXT FROM AllLanguages INTO @LanguageId
			END
			CLOSE AllLanguages
			DEALLOCATE AllLanguages		
	
		FETCH NEXT FROM AllReminders INTO @MtName, @CrRuleId
	END
	CLOSE AllReminders
	DEALLOCATE AllReminders	
END