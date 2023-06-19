using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Brevo.Models;
using Nop.Plugin.Misc.Brevo.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Brevo.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class BrevoController : BasePluginController
    {
        #region Fields

        protected readonly BrevoManager _brevoEmailManager;
        protected readonly IEmailAccountService _emailAccountService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILogger _logger;
        protected readonly IMessageTemplateService _messageTemplateService;
        protected readonly IMessageTokenProvider _messageTokenProvider;
        protected readonly INotificationService _notificationService;
        protected readonly ISettingService _settingService;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreContext _storeContext;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IStoreService _storeService;
        protected readonly IWorkContext _workContext;
        protected readonly MessageTemplatesSettings _messageTemplatesSettings;

        #endregion

        #region Ctor

        public BrevoController(BrevoManager brevoEmailManager,
            IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            INotificationService notificationService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IWorkContext workContext,
            MessageTemplatesSettings messageTemplatesSettings)
        {
            _brevoEmailManager = brevoEmailManager;
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _notificationService = notificationService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _messageTemplatesSettings = messageTemplatesSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare ConfigurationModel
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task PrepareModelAsync(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            //whether plugin is configured
            if (string.IsNullOrEmpty(brevoSettings.ApiKey))
                return;

            //prepare common properties
            model.ActiveStoreScopeConfiguration = storeId;
            model.ApiKey = brevoSettings.ApiKey;
            model.ListId = brevoSettings.ListId;
            model.SmtpKey = brevoSettings.SmtpKey;
            model.SenderId = brevoSettings.SenderId;
            model.UseSmsNotifications = brevoSettings.UseSmsNotifications;
            model.SmsSenderName = brevoSettings.SmsSenderName;
            model.StoreOwnerPhoneNumber = brevoSettings.StoreOwnerPhoneNumber;
            model.UseMarketingAutomation = brevoSettings.UseMarketingAutomation;
            model.TrackingScript = brevoSettings.TrackingScript;

            var customer = await _workContext.GetCurrentCustomerAsync();
            model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideGeneralBlock);
            model.HideSynchronizationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideSynchronizationBlock);
            model.HideTransactionalBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideTransactionalBlock);
            model.HideSmsBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideSmsBlock);
            model.HideMarketingAutomationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideMarketingAutomationBlock);

            //prepare nested search models
            model.MessageTemplateSearchModel.SetGridPageSize();
            model.SmsSearchModel.SetGridPageSize();

            //prepare add SMS model
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.MyPhone"), "0"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.CustomerPhone"), "1"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.BillingAddressPhone"), "2"));
            model.AddSms.DefaultSelectedPhoneTypeId = model.AddSms.AvailablePhoneTypes.First().Value;

            var stores = await _storeService.GetAllStoresAsync();
            var messageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(storeId);
            model.AddSms.AvailableMessages = await messageTemplates.SelectAwait(async messageTemplate =>
            {
                var name = messageTemplate.Name;
                if (storeId == 0 && messageTemplate.LimitedToStores)
                {
                    var storeIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
                    var storeNames = stores.Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);
                    name = $"{name} ({string.Join(',', storeNames)})";
                }

                return new SelectListItem(name, messageTemplate.Id.ToString());
            }).ToListAsync();
            var defaultSelectedMessage = model.AddSms.AvailableMessages.FirstOrDefault();
            model.AddSms.DefaultSelectedMessageId = defaultSelectedMessage?.Value ?? "0";

            //check whether email account exists
            if (brevoSettings.UseSmtp && await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId) != null)
                model.UseSmtp = brevoSettings.UseSmtp;

            //get account info
            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _brevoEmailManager.GetAccountInfoAsync();
            model.AccountInfo = accountInfo;
            model.MarketingAutomationKey = maKey;
            model.MarketingAutomationDisabled = !marketingAutomationEnabled;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {accountErrors}");

            //prepare overridable settings
            if (storeId > 0)
            {
                model.ListId_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.ListId, storeId);
                model.UseSmtp_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseSmtp, storeId);
                model.SenderId_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.SenderId, storeId);
                model.UseSmsNotifications_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseSmsNotifications, storeId);
                model.SmsSenderName_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.SmsSenderName, storeId);
                model.UseMarketingAutomation_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseMarketingAutomation, storeId);
            }

            //check SMTP status
            var (smtpEnabled, smtpErrors) = await _brevoEmailManager.SmtpIsEnabledAsync();
            if (!string.IsNullOrEmpty(smtpErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {smtpErrors}");

            //get available contact lists to synchronize
            var (lists, listsErrors) = await _brevoEmailManager.GetListsAsync();
            model.AvailableLists = lists.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableLists.Insert(0, new SelectListItem("Select list", "0"));
            if (!string.IsNullOrEmpty(listsErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {listsErrors}");

            //get available senders of emails from account
            var (senders, sendersErrors) = await _brevoEmailManager.GetSendersAsync();
            model.AvailableSenders = senders.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableSenders.Insert(0, new SelectListItem("Select sender", "0"));
            if (!string.IsNullOrEmpty(sendersErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {sendersErrors}");

            //get allowed tokens
            model.AllowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync());

            //create attributes in account
            var attributesErrors = await _brevoEmailManager.PrepareAttributesAsync();
            if (!string.IsNullOrEmpty(attributesErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {attributesErrors}");
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);

            return View("~/Plugins/Misc.Brevo/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            //set API key
            brevoSettings.ApiKey = model.ApiKey;
            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.ApiKey, clearCache: false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("saveSync")]
        public async Task<IActionResult> SaveSynchronization(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            //create webhook for the unsubscribe event
            brevoSettings.UnsubscribeWebhookId = await _brevoEmailManager.GetUnsubscribeWebHookIdAsync();
            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.UnsubscribeWebhookId, clearCache: false);

            //set list of contacts to synchronize
            brevoSettings.ListId = model.ListId;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.ListId, model.ListId_OverrideForStore, storeId, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("sync")]
        public async Task<IActionResult> Synchronization(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            //synchronize contacts of selected store
            var messages = await _brevoEmailManager.SynchronizeAsync(false, await _storeContext.GetActiveStoreScopeConfigurationAsync());
            foreach (var message in messages)
            {
                _notificationService.Notification(message.Type, message.Message, false);
            }
            if (!messages.Any(message => message.Type == NotifyType.Error))
            {
                ViewData["synchronizationStart"] = true;
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.ImportProcess"));
            }

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetSynchronizationInfo()
        {
            var res = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), () => string.Empty);
            await _staticCacheManager.RemoveAsync(BrevoDefaults.SyncKeyCache);

            return res;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("saveSMTP")]
        public async Task<IActionResult> ConfigureSMTP(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            if (model.UseSmtp)
            {
                //set case invariant for true because tokens are used in uppercase format in Brevo's transactional emails
                _messageTemplatesSettings.CaseInvariantReplacement = true;
                await _settingService.SaveSettingAsync(_messageTemplatesSettings, settings => settings.CaseInvariantReplacement, clearCache: false);

                //check whether SMTP enabled on account
                var (smtpIsEnabled, smtpErrors) = await _brevoEmailManager.SmtpIsEnabledAsync();
                if (smtpIsEnabled)
                {
                    //get email account or create new one
                    var (emailAccountId, emailAccountErrors) = await _brevoEmailManager.GetEmailAccountIdAsync(model.SenderId, model.SmtpKey);
                    brevoSettings.EmailAccountId = emailAccountId;
                    await _settingService.SaveSettingAsync(brevoSettings, settings => settings.EmailAccountId, storeId, false);
                    if (!string.IsNullOrEmpty(emailAccountErrors))
                        _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {emailAccountErrors}");
                }
                else
                {
                    //need to activate SMTP account
                    _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.ActivateSMTP"));
                    model.UseSmtp = false;
                }
                if (!string.IsNullOrEmpty(smtpErrors))
                    _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {smtpErrors}");
            }

            //set whether to use SMTP 
            brevoSettings.UseSmtp = model.UseSmtp;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseSmtp, model.UseSmtp_OverrideForStore, storeId, false);

            //set sender of transactional emails
            brevoSettings.SenderId = model.SenderId;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.SenderId, model.SenderId_OverrideForStore, storeId, false);

            //set SMTP key
            brevoSettings.SmtpKey = model.SmtpKey;
            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.SmtpKey, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> MessageList(BrevoMessageTemplateSearchModel searchModel)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var messageTemplates = (await _messageTemplateService.GetAllMessageTemplatesAsync(storeId)).ToPagedList(searchModel);

            //prepare list model
            var model = await new BrevoMessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.SelectAwait(async messageTemplate =>
                {
                    //standard template of message is edited in the admin area, Brevo template is edited in the Brevo account
                    var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, BrevoDefaults.TemplateIdAttribute);
                    var stores = (await (await _storeService.GetAllStoresAsync())
                        .WhereAwait(async store => !messageTemplate.LimitedToStores
                            || (await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate)).Contains(store.Id))
                        .AggregateAsync(string.Empty, (current, next) => $"{current}, {next.Name}"))
                        .Trim(',');

                    return new BrevoMessageTemplateModel
                    {
                        Id = messageTemplate.Id,
                        Name = messageTemplate.Name,
                        IsActive = messageTemplate.IsActive,
                        ListOfStores = stores,
                        UseBrevoTemplate = templateId.HasValue,
                        EditLink = templateId.HasValue
                            ? $"{string.Format(BrevoDefaults.EditMessageTemplateUrl, templateId.Value)}"
                            : Url.Action("Edit", "MessageTemplate", new { id = messageTemplate.Id, area = AreaNames.Admin })
                    };
                });
            });

            return Json(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> MessageUpdate(BrevoMessageTemplateModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors().ToString());

            var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);

            //Brevo message template
            if (model.UseBrevoTemplate)
            {
                var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

                //get template or create new one
                var currentTemplateId = await _genericAttributeService.GetAttributeAsync<int?>(message, BrevoDefaults.TemplateIdAttribute);
                var templateId = await _brevoEmailManager.GetTemplateIdAsync(currentTemplateId, message,
                    await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId));
                await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.TemplateIdAttribute, templateId);
                model.EditLink = $"{string.Format(BrevoDefaults.EditMessageTemplateUrl, templateId)}";
            }
            else
            {
                //standard message template
                await _genericAttributeService.SaveAttributeAsync<int?>(message, BrevoDefaults.TemplateIdAttribute, null);
                model.EditLink = Url.Action("Edit", "MessageTemplate", new { id = model.Id, area = AreaNames.Admin });
            }

            //update message template
            if (model.IsActive == message.IsActive)
                return new NullJsonResult();

            message.IsActive = model.IsActive;
            await _messageTemplateService.UpdateMessageTemplateAsync(message);

            return new NullJsonResult();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("saveSMS")]
        public async Task<IActionResult> ConfigureSMS(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            brevoSettings.UseSmsNotifications = model.UseSmsNotifications;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseSmsNotifications, model.UseSmsNotifications_OverrideForStore, storeId, false);
            brevoSettings.SmsSenderName = model.SmsSenderName;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.SmsSenderName, model.SmsSenderName_OverrideForStore, storeId, false);
            brevoSettings.StoreOwnerPhoneNumber = model.StoreOwnerPhoneNumber;
            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.StoreOwnerPhoneNumber, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> SMSList(SmsSearchModel searchModel)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            //get message templates which are sending in SMS
            var allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(storeId);
            var messageTemplates = await allMessageTemplates

                .WhereAwait(async messageTemplate => await _genericAttributeService.GetAttributeAsync<bool>(messageTemplate, BrevoDefaults.UseSmsAttribute))
                .ToPagedListAsync(searchModel);

            //prepare list model
            var model = await new SmsListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.SelectAwait(async messageTemplate =>
                {
                    var phoneTypeID = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, BrevoDefaults.PhoneTypeAttribute);

                    var smsModel = new SmsModel
                    {
                        Id = messageTemplate.Id,
                        MessageId = messageTemplate.Id,
                        Name = messageTemplate.Name,
                        PhoneTypeId = phoneTypeID,

                        Text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, BrevoDefaults.SmsTextAttribute)
                    };

                    if (storeId == 0)
                    {
                        if (storeId == 0 && messageTemplate.LimitedToStores)
                        {
                            var storeIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
                            var storeNames = (await _storeService.GetAllStoresAsync()).Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);

                            smsModel.Name = $"{smsModel.Name} ({string.Join(',', storeNames)})";
                        }
                    }

                    //choose phone number to send SMS
                    //currently supported: "my phone" (filled on the configuration page), customer phone, phone of the billing address
                    switch (phoneTypeID)
                    {
                        case 0:
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.MyPhone");
                            break;
                        case 1:
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.CustomerPhone");
                            break;
                        case 2:
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.BillingAddressPhone");
                            break;
                        default:
                            break;
                    }

                    return smsModel;
                });
            });

            return Json(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> SMSAdd(SmsModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.MessageId);
            if (message != null)
            {
                await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.UseSmsAttribute, true);
                await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.SmsTextAttribute, model.Text);
                await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.PhoneTypeAttribute, model.PhoneTypeId);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> SMSDelete(SmsModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //delete generic attributes
            var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
            if (message != null)
            {
                await _genericAttributeService.SaveAttributeAsync<bool?>(message, BrevoDefaults.UseSmsAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<string>(message, BrevoDefaults.SmsTextAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<int?>(message, BrevoDefaults.PhoneTypeAttribute, null);
            }

            return new NullJsonResult();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("submitCampaign")]
        public async Task<IActionResult> SubmitCampaign(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var campaignErrors = await _brevoEmailManager.SendSMSCampaignAsync(model.CampaignListId, model.CampaignSenderName, model.CampaignText);
            if (!string.IsNullOrEmpty(campaignErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {campaignErrors}");
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.SMS.Campaigns.Sent"));

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("saveMA")]
        public async Task<IActionResult> ConfigureMA(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

            brevoSettings.UseMarketingAutomation = model.UseMarketingAutomation;
            await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseMarketingAutomation, model.UseMarketingAutomation_OverrideForStore, storeId, false);

            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _brevoEmailManager.GetAccountInfoAsync();
            brevoSettings.MarketingAutomationKey = maKey;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {accountErrors}");

            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.MarketingAutomationKey, clearCache: false);
            brevoSettings.TrackingScript = model.TrackingScript;
            await _settingService.SaveSettingAsync(brevoSettings, settings => settings.TrackingScript, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> ImportContacts(BaseNopModel model, IFormCollection form)
        {
            try
            {
                //logging info
                var logInfo = string.Format("Brevo synchronization: New emails {1},{0} Existing emails {2},{0} Invalid emails {3},{0} Duplicates emails {4}{0}",
                    Environment.NewLine, form["new_emails"], form["emails_exists"], form["invalid_email"], form["duplicates_email"]);
                await _logger.InformationAsync(logInfo);

                //display info on configuration page in case of the manually synchronization
                await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), logInfo);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), ex.Message);
            }

            return Ok();
        }

        #endregion
    }
}