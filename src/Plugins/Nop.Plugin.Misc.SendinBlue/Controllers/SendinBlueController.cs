using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.SendinBlue.Models;
using Nop.Plugin.Misc.SendinBlue.Services;
using Nop.Services.Caching;
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

namespace Nop.Plugin.Misc.SendinBlue.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class SendinBlueController : BasePluginController
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _messageTemplatesSettings;
        private readonly SendinBlueManager _sendinBlueEmailManager;

        #endregion

        #region Ctor

        public SendinBlueController(IEmailAccountService emailAccountService,
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
            MessageTemplatesSettings messageTemplatesSettings,
            SendinBlueManager sendinBlueEmailManager)
        {
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
            _sendinBlueEmailManager = sendinBlueEmailManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare SendinBlueModel
        /// </summary>
        /// <param name="model">Model</param>
        private async Task PrepareModelAsync(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            //whether plugin is configured
            if (string.IsNullOrEmpty(sendinBlueSettings.ApiKey))
                return;

            //prepare common properties
            model.ActiveStoreScopeConfiguration = storeId;
            model.ApiKey = sendinBlueSettings.ApiKey;
            model.ListId = sendinBlueSettings.ListId;
            model.SmtpKey = sendinBlueSettings.SmtpKey;
            model.SenderId = sendinBlueSettings.SenderId;
            model.UseSmsNotifications = sendinBlueSettings.UseSmsNotifications;
            model.SmsSenderName = sendinBlueSettings.SmsSenderName;
            model.StoreOwnerPhoneNumber = sendinBlueSettings.StoreOwnerPhoneNumber;
            model.UseMarketingAutomation = sendinBlueSettings.UseMarketingAutomation;
            model.TrackingScript = sendinBlueSettings.TrackingScript;

            model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), SendinBlueDefaults.HideGeneralBlock);
            model.HideSynchronizationBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), SendinBlueDefaults.HideSynchronizationBlock);
            model.HideTransactionalBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), SendinBlueDefaults.HideTransactionalBlock);
            model.HideSmsBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), SendinBlueDefaults.HideSmsBlock);
            model.HideMarketingAutomationBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), SendinBlueDefaults.HideMarketingAutomationBlock);

            //prepare nested search models
            model.MessageTemplateSearchModel.SetGridPageSize();
            model.SmsSearchModel.SetGridPageSize();

            //prepare add SMS model
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.MyPhone"), "0"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.CustomerPhone"), "1"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.BillingAddressPhone"), "2"));
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
            if (sendinBlueSettings.UseSmtp && await _emailAccountService.GetEmailAccountByIdAsync(sendinBlueSettings.EmailAccountId) != null)
                model.UseSmtp = sendinBlueSettings.UseSmtp;

            //get account info
            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _sendinBlueEmailManager.GetAccountInfoAsync();
            model.AccountInfo = accountInfo;
            model.MarketingAutomationKey = maKey;
            model.MarketingAutomationDisabled = !marketingAutomationEnabled;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {accountErrors}");

            //prepare overridable settings
            if (storeId > 0)
            {
                model.ListId_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.ListId, storeId);
                model.UseSmtp_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.UseSmtp, storeId);
                model.SenderId_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.SenderId, storeId);
                model.UseSmsNotifications_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.UseSmsNotifications, storeId);
                model.SmsSenderName_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.SmsSenderName, storeId);
                model.UseMarketingAutomation_OverrideForStore = await _settingService.SettingExistsAsync(sendinBlueSettings, settings => settings.UseMarketingAutomation, storeId);
            }

            //check SMTP status
            var (smtpEnabled, smtpErrors) = await _sendinBlueEmailManager.SmtpIsEnabledAsync();
            if (!string.IsNullOrEmpty(smtpErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {smtpErrors}");

            //get available contact lists to synchronize
            var (lists, listsErrors) = await _sendinBlueEmailManager.GetListsAsync();
            model.AvailableLists = lists.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableLists.Insert(0, new SelectListItem("Select list", "0"));
            if (!string.IsNullOrEmpty(listsErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {listsErrors}");

            //get available senders of emails from account
            var (senders, sendersErrors) = await _sendinBlueEmailManager.GetSendersAsync();
            model.AvailableSenders = senders.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableSenders.Insert(0, new SelectListItem("Select sender", "0"));
            if (!string.IsNullOrEmpty(sendersErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {sendersErrors}");

            //get allowed tokens
            model.AllowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync());

            //create attributes in account
            var attributesErrors = await _sendinBlueEmailManager.PrepareAttributesAsync();
            if (!string.IsNullOrEmpty(attributesErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {attributesErrors}");

            //try to set account partner
            if (!sendinBlueSettings.PartnerValueSet)
            {
                var partnerSet = await _sendinBlueEmailManager.SetPartnerAsync();
                if (partnerSet)
                {
                    sendinBlueSettings.PartnerValueSet = true;
                    await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.PartnerValueSet, clearCache: false);
                    await _settingService.ClearCacheAsync();
                }
            }
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);

            return View("~/Plugins/Misc.SendinBlue/Views/Configure.cshtml", model);
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
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            //set API key
            sendinBlueSettings.ApiKey = model.ApiKey;
            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.ApiKey, clearCache: false);
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
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            //create webhook for the unsubscribe event
            sendinBlueSettings.UnsubscribeWebhookId = await _sendinBlueEmailManager.GetUnsubscribeWebHookIdAsync();
            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.UnsubscribeWebhookId, clearCache: false);

            //set list of contacts to synchronize
            sendinBlueSettings.ListId = model.ListId;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.ListId, model.ListId_OverrideForStore, storeId, false);

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
            var messages = await _sendinBlueEmailManager.SynchronizeAsync(false, await _storeContext.GetActiveStoreScopeConfigurationAsync());
            foreach (var message in messages)
            {
                _notificationService.Notification(message.Type, message.Message, false);
            }
            if (!messages.Any(message => message.Type == NotifyType.Error))
            {
                ViewData["synchronizationStart"] = true;
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.ImportProcess"));
            }

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<string> GetSynchronizationInfo()
        {
            var res = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), () => string.Empty);
            await _staticCacheManager.RemoveAsync(SendinBlueDefaults.SyncKeyCache);

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
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            if (model.UseSmtp)
            {
                //set case invariant for true because tokens are used in uppercase format in SendinBlue's transactional emails
                _messageTemplatesSettings.CaseInvariantReplacement = true;
                await _settingService.SaveSettingAsync(_messageTemplatesSettings, settings => settings.CaseInvariantReplacement, clearCache: false);

                //check whether SMTP enabled on account
                var (smtpIsEnabled, smtpErrors) = await _sendinBlueEmailManager.SmtpIsEnabledAsync();
                if (smtpIsEnabled)
                {
                    //get email account or create new one
                    var (emailAccountId, emailAccountErrors) = await _sendinBlueEmailManager.GetEmailAccountIdAsync(model.SenderId, model.SmtpKey);
                    sendinBlueSettings.EmailAccountId = emailAccountId;
                    await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.EmailAccountId, storeId, false);
                    if (!string.IsNullOrEmpty(emailAccountErrors))
                        _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {emailAccountErrors}");
                }
                else
                {
                    //need to activate SMTP account
                    _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.ActivateSMTP"));
                    model.UseSmtp = false;
                }
                if (!string.IsNullOrEmpty(smtpErrors))
                    _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {smtpErrors}");
            }

            //set whether to use SMTP 
            sendinBlueSettings.UseSmtp = model.UseSmtp;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.UseSmtp, model.UseSmtp_OverrideForStore, storeId, false);

            //set sender of transactional emails
            sendinBlueSettings.SenderId = model.SenderId;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.SenderId, model.SenderId_OverrideForStore, storeId, false);

            //set SMTP key
            sendinBlueSettings.SmtpKey = model.SmtpKey;
            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.SmtpKey, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> MessageList(SendinBlueMessageTemplateSearchModel searchModel)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var messageTemplates = (await _messageTemplateService.GetAllMessageTemplatesAsync(storeId)).ToPagedList(searchModel);

            //prepare list model
            var model = await new SendinBlueMessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.SelectAwait(async messageTemplate =>
                {
                    //standard template of message is edited in the admin area, SendinBlue template is edited in the SendinBlue account
                    var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, SendinBlueDefaults.TemplateIdAttribute);
                    var stores = (await (await _storeService.GetAllStoresAsync())
                        .WhereAwait(async store => !messageTemplate.LimitedToStores
                            || (await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate)).Contains(store.Id))
                        .AggregateAsync(string.Empty, (current, next) => $"{current}, {next.Name}"))
                        .Trim(',');

                    return new SendinBlueMessageTemplateModel
                    {
                        Id = messageTemplate.Id,
                        Name = messageTemplate.Name,
                        IsActive = messageTemplate.IsActive,
                        ListOfStores = stores,
                        UseSendinBlueTemplate = templateId.HasValue,
                        EditLink = templateId.HasValue
                            ? $"{string.Format(SendinBlueDefaults.EditMessageTemplateUrl, templateId.Value)}"
                            : Url.Action("Edit", "MessageTemplate", new { id = messageTemplate.Id, area = AreaNames.Admin })
                    };
                });
            });

            return Json(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> MessageUpdate(SendinBlueMessageTemplateModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors().ToString());

            var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);

            //SendinBlue message template
            if (model.UseSendinBlueTemplate)
            {
                var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

                //get template or create new one
                var currentTemplateId = await _genericAttributeService.GetAttributeAsync<int?>(message, SendinBlueDefaults.TemplateIdAttribute);
                var templateId = await _sendinBlueEmailManager.GetTemplateIdAsync(currentTemplateId, message,
                    await _emailAccountService.GetEmailAccountByIdAsync(sendinBlueSettings.EmailAccountId));
                await _genericAttributeService.SaveAttributeAsync(message, SendinBlueDefaults.TemplateIdAttribute, templateId);
                model.EditLink = $"{string.Format(SendinBlueDefaults.EditMessageTemplateUrl, templateId)}";
            }
            else
            {
                //standard message template
                await _genericAttributeService.SaveAttributeAsync<int?>(message, SendinBlueDefaults.TemplateIdAttribute, null);
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
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            sendinBlueSettings.UseSmsNotifications = model.UseSmsNotifications;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.UseSmsNotifications, model.UseSmsNotifications_OverrideForStore, storeId, false);
            sendinBlueSettings.SmsSenderName = model.SmsSenderName;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.SmsSenderName, model.SmsSenderName_OverrideForStore, storeId, false);
            sendinBlueSettings.StoreOwnerPhoneNumber = model.StoreOwnerPhoneNumber;
            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.StoreOwnerPhoneNumber, clearCache: false);

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
                
                .WhereAwait(async messageTemplate => await _genericAttributeService.GetAttributeAsync<bool>(messageTemplate, SendinBlueDefaults.UseSmsAttribute))
                .ToPagedListAsync(searchModel);

            //prepare list model
            var model = await new SmsListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.SelectAwait(async messageTemplate =>
                {
                    var phoneTypeID = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, SendinBlueDefaults.PhoneTypeAttribute);

                    var smsModel = new SmsModel
                    {
                        Id = messageTemplate.Id,
                        MessageId = messageTemplate.Id,
                        Name = messageTemplate.Name,
                        PhoneTypeId = phoneTypeID,

                        Text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, SendinBlueDefaults.SmsTextAttribute)
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
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.MyPhone");
                            break;
                        case 1:
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.CustomerPhone");
                            break;
                        case 2:
                            smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.BillingAddressPhone");
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
                await _genericAttributeService.SaveAttributeAsync(message, SendinBlueDefaults.UseSmsAttribute, true);
                await _genericAttributeService.SaveAttributeAsync(message, SendinBlueDefaults.SmsTextAttribute, model.Text);
                await _genericAttributeService.SaveAttributeAsync(message, SendinBlueDefaults.PhoneTypeAttribute, model.PhoneTypeId);
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
                await _genericAttributeService.SaveAttributeAsync<bool?>(message, SendinBlueDefaults.UseSmsAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<string>(message, SendinBlueDefaults.SmsTextAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<int?>(message, SendinBlueDefaults.PhoneTypeAttribute, null);
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

            var campaignErrors = await _sendinBlueEmailManager.SendSMSCampaignAsync(model.CampaignListId, model.CampaignSenderName, model.CampaignText);
            if (!string.IsNullOrEmpty(campaignErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {campaignErrors}");
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.SendinBlue.SMS.Campaigns.Sent"));

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
            var sendinBlueSettings = await _settingService.LoadSettingAsync<SendinBlueSettings>(storeId);

            sendinBlueSettings.UseMarketingAutomation = model.UseMarketingAutomation;
            await _settingService.SaveSettingOverridablePerStoreAsync(sendinBlueSettings, settings => settings.UseMarketingAutomation, model.UseMarketingAutomation_OverrideForStore, storeId, false);

            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _sendinBlueEmailManager.GetAccountInfoAsync();
            sendinBlueSettings.MarketingAutomationKey = maKey;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {accountErrors}");

            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.MarketingAutomationKey, clearCache: false);
            sendinBlueSettings.TrackingScript = model.TrackingScript;
            await _settingService.SaveSettingAsync(sendinBlueSettings, settings => settings.TrackingScript, clearCache: false);

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
                var logInfo = string.Format("SendinBlue synchronization: New emails {1},{0} Existing emails {2},{0} Invalid emails {3},{0} Duplicates emails {4}{0}",
                    Environment.NewLine, form["new_emails"], form["emails_exists"], form["invalid_email"], form["duplicates_email"]);
                await _logger.InformationAsync(logInfo);

                //display info on configuration page in case of the manually synchronization
                await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), logInfo);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UnsubscribeWebHook()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                await _sendinBlueEmailManager.UnsubscribeWebhookAsync(await streamReader.ReadToEndAsync());
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return Ok();
        }

        #endregion
    }
}