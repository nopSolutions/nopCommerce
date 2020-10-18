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
        private async Task PrepareModel(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

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

            model.HideGeneralBlock = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), SendinBlueDefaults.HideGeneralBlock);
            model.HideSynchronizationBlock = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), SendinBlueDefaults.HideSynchronizationBlock);
            model.HideTransactionalBlock = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), SendinBlueDefaults.HideTransactionalBlock);
            model.HideSmsBlock = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), SendinBlueDefaults.HideSmsBlock);
            model.HideMarketingAutomationBlock = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), SendinBlueDefaults.HideMarketingAutomationBlock);

            //prepare nested search models
            model.MessageTemplateSearchModel.SetGridPageSize();
            model.SmsSearchModel.SetGridPageSize();

            //prepare add SMS model
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResource("Plugins.Misc.SendinBlue.MyPhone"), "0"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResource("Plugins.Misc.SendinBlue.CustomerPhone"), "1"));
            model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResource("Plugins.Misc.SendinBlue.BillingAddressPhone"), "2"));
            model.AddSms.DefaultSelectedPhoneTypeId = model.AddSms.AvailablePhoneTypes.First().Value;

            model.AddSms.AvailableMessages = (await _messageTemplateService.GetAllMessageTemplates(storeId)).Select(messageTemplate =>
            {
                var name = messageTemplate.Name;
                if (storeId == 0 && messageTemplate.LimitedToStores)
                {
                    var storeIds = _storeMappingService.GetStoresIdsWithAccess(messageTemplate).Result;
                    var storeNames = _storeService.GetAllStores().Result.Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);
                    name = $"{name} ({string.Join(',', storeNames)})";
                }

                return new SelectListItem(name, messageTemplate.Id.ToString());
            }).ToList();
            var defaultSelectedMessage = model.AddSms.AvailableMessages.FirstOrDefault();
            model.AddSms.DefaultSelectedMessageId = defaultSelectedMessage?.Value ?? "0";

            //check whether email account exists
            if (sendinBlueSettings.UseSmtp && await _emailAccountService.GetEmailAccountById(sendinBlueSettings.EmailAccountId) != null)
                model.UseSmtp = sendinBlueSettings.UseSmtp;

            //get account info
            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _sendinBlueEmailManager.GetAccountInfo();
            model.AccountInfo = accountInfo;
            model.MarketingAutomationKey = maKey;
            model.MarketingAutomationDisabled = !marketingAutomationEnabled;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {accountErrors}");

            //prepare overridable settings
            if (storeId > 0)
            {
                model.ListId_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.ListId, storeId);
                model.UseSmtp_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.UseSmtp, storeId);
                model.SenderId_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.SenderId, storeId);
                model.UseSmsNotifications_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.UseSmsNotifications, storeId);
                model.SmsSenderName_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.SmsSenderName, storeId);
                model.UseMarketingAutomation_OverrideForStore = await _settingService.SettingExists(sendinBlueSettings, settings => settings.UseMarketingAutomation, storeId);
            }

            //check SMTP status
            var (smtpEnabled, smtpErrors) = await _sendinBlueEmailManager.SmtpIsEnabled();
            if (!string.IsNullOrEmpty(smtpErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {smtpErrors}");

            //get available contact lists to synchronize
            var (lists, listsErrors) = await _sendinBlueEmailManager.GetLists();
            model.AvailableLists = lists.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableLists.Insert(0, new SelectListItem("Select list", "0"));
            if (!string.IsNullOrEmpty(listsErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {listsErrors}");

            //get available senders of emails from account
            var (senders, sendersErrors) = await _sendinBlueEmailManager.GetSenders();
            model.AvailableSenders = senders.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
            model.AvailableSenders.Insert(0, new SelectListItem("Select sender", "0"));
            if (!string.IsNullOrEmpty(sendersErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {sendersErrors}");

            //get allowed tokens
            model.AllowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokens());

            //create attributes in account
            var attributesErrors = await _sendinBlueEmailManager.PrepareAttributes();
            if (!string.IsNullOrEmpty(attributesErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {attributesErrors}");

            //try to set account partner
            if (!sendinBlueSettings.PartnerValueSet)
            {
                var partnerSet = await _sendinBlueEmailManager.SetPartner();
                if (partnerSet)
                {
                    sendinBlueSettings.PartnerValueSet = true;
                    await _settingService.SaveSetting(sendinBlueSettings, settings => settings.PartnerValueSet, clearCache: false);
                    await _settingService.ClearCache();
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
            await PrepareModel(model);

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

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            //set API key
            sendinBlueSettings.ApiKey = model.ApiKey;
            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.ApiKey, clearCache: false);
            await _settingService.ClearCache();

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Plugins.Saved"));

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

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            //create webhook for the unsubscribe event
            sendinBlueSettings.UnsubscribeWebhookId = await _sendinBlueEmailManager.GetUnsubscribeWebHookId();
            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.UnsubscribeWebhookId, clearCache: false);

            //set list of contacts to synchronize
            sendinBlueSettings.ListId = model.ListId;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.ListId, model.ListId_OverrideForStore, storeId, false);

            //now clear settings cache
            await _settingService.ClearCache();

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Plugins.Saved"));

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
            var messages = await _sendinBlueEmailManager.Synchronize(false, await _storeContext.GetActiveStoreScopeConfiguration());
            foreach (var message in messages)
            {
                _notificationService.Notification(message.Type, message.Message, false);
            }
            if (!messages.Any(message => message.Type == NotifyType.Error))
            {
                ViewData["synchronizationStart"] = true;
                _notificationService.SuccessNotification(await _localizationService.GetResource("Plugins.Misc.SendinBlue.ImportProcess"));
            }

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<string> GetSynchronizationInfo()
        {
            var res = await _staticCacheManager.Get(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), () => Task.FromResult(string.Empty));
            await _staticCacheManager.Remove(SendinBlueDefaults.SyncKeyCache);

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

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            if (model.UseSmtp)
            {
                //set case invariant for true because tokens are used in uppercase format in SendinBlue's transactional emails
                _messageTemplatesSettings.CaseInvariantReplacement = true;
                await _settingService.SaveSetting(_messageTemplatesSettings, settings => settings.CaseInvariantReplacement, clearCache: false);

                //check whether SMTP enabled on account
                var (smtpIsEnabled, smtpErrors) = await _sendinBlueEmailManager.SmtpIsEnabled();
                if (smtpIsEnabled)
                {
                    //get email account or create new one
                    var (emailAccountId, emailAccountErrors) = await _sendinBlueEmailManager.GetEmailAccountId(model.SenderId, model.SmtpKey);
                    sendinBlueSettings.EmailAccountId = emailAccountId;
                    await _settingService.SaveSetting(sendinBlueSettings, settings => settings.EmailAccountId, storeId, false);
                    if (!string.IsNullOrEmpty(emailAccountErrors))
                        _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {emailAccountErrors}");
                }
                else
                {
                    //need to activate SMTP account
                    _notificationService.WarningNotification(await _localizationService.GetResource("Plugins.Misc.SendinBlue.ActivateSMTP"));
                    model.UseSmtp = false;
                }
                if (!string.IsNullOrEmpty(smtpErrors))
                    _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {smtpErrors}");
            }

            //set whether to use SMTP 
            sendinBlueSettings.UseSmtp = model.UseSmtp;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.UseSmtp, model.UseSmtp_OverrideForStore, storeId, false);

            //set sender of transactional emails
            sendinBlueSettings.SenderId = model.SenderId;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.SenderId, model.SenderId_OverrideForStore, storeId, false);

            //set SMTP key
            sendinBlueSettings.SmtpKey = model.SmtpKey;
            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.SmtpKey, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCache();

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> MessageList(SendinBlueMessageTemplateSearchModel searchModel)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var messageTemplates = (await _messageTemplateService.GetAllMessageTemplates(storeId)).ToPagedList(searchModel);

            //prepare list model
            var model = await new SendinBlueMessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.ToAsyncEnumerable().SelectAwait(async messageTemplate =>
                {
                    //standard template of message is edited in the admin area, SendinBlue template is edited in the SendinBlue account
                    var templateId = await _genericAttributeService.GetAttribute<int?>(messageTemplate, SendinBlueDefaults.TemplateIdAttribute);
                    var stores = (await _storeService.GetAllStores())
                        .Where(store => !messageTemplate.LimitedToStores || _storeMappingService.GetStoresIdsWithAccess(messageTemplate).Result.Contains(store.Id))
                        .Aggregate(string.Empty, (current, next) => $"{current}, {next.Name}").Trim(',');

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

            var message = await _messageTemplateService.GetMessageTemplateById(model.Id);

            //SendinBlue message template
            if (model.UseSendinBlueTemplate)
            {
                var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
                var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

                //get template or create new one
                var currentTemplateId = await _genericAttributeService.GetAttribute<int?>(message, SendinBlueDefaults.TemplateIdAttribute);
                var templateId = await _sendinBlueEmailManager.GetTemplateId(currentTemplateId, message,
                    await _emailAccountService.GetEmailAccountById(sendinBlueSettings.EmailAccountId));
                await _genericAttributeService.SaveAttribute(message, SendinBlueDefaults.TemplateIdAttribute, templateId);
                model.EditLink = $"{string.Format(SendinBlueDefaults.EditMessageTemplateUrl, templateId)}";
            }
            else
            {
                //standard message template
                await _genericAttributeService.SaveAttribute<int?>(message, SendinBlueDefaults.TemplateIdAttribute, null);
                model.EditLink = Url.Action("Edit", "MessageTemplate", new { id = model.Id, area = AreaNames.Admin });
            }

            //update message template
            if (model.IsActive == message.IsActive)
                return new NullJsonResult();

            message.IsActive = model.IsActive;
            await _messageTemplateService.UpdateMessageTemplate(message);

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

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            sendinBlueSettings.UseSmsNotifications = model.UseSmsNotifications;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.UseSmsNotifications, model.UseSmsNotifications_OverrideForStore, storeId, false);
            sendinBlueSettings.SmsSenderName = model.SmsSenderName;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.SmsSenderName, model.SmsSenderName_OverrideForStore, storeId, false);
            sendinBlueSettings.StoreOwnerPhoneNumber = model.StoreOwnerPhoneNumber;
            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.StoreOwnerPhoneNumber, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCache();

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> SMSList(SmsSearchModel searchModel)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();

            //TODO: may be deleted
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            //get message templates which are sending in SMS
            var messageTemplates = (await _messageTemplateService.GetAllMessageTemplates(storeId))
                .Where(messageTemplate => _genericAttributeService.GetAttribute<bool>(messageTemplate, SendinBlueDefaults.UseSmsAttribute).Result)
                .ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SmsListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.ToAsyncEnumerable().SelectAwait(async messageTemplate =>
                {
                    var phoneTypeID = await _genericAttributeService.GetAttribute<int>(messageTemplate, SendinBlueDefaults.PhoneTypeAttribute);
                    var smsModel = new SmsModel
                    {
                        Id = messageTemplate.Id,
                        MessageId = messageTemplate.Id,
                        Name = messageTemplate.Name,
                        PhoneTypeId = phoneTypeID,
                        Text = await _genericAttributeService.GetAttribute<string>(messageTemplate, SendinBlueDefaults.SmsTextAttribute)
                    };

                    if (storeId == 0)
                    {
                        if (storeId == 0 && messageTemplate.LimitedToStores)
                        {
                            var storeIds = await _storeMappingService.GetStoresIdsWithAccess(messageTemplate);
                            var storeNames = (await _storeService.GetAllStores()).Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);
                            smsModel.Name = $"{smsModel.Name} ({string.Join(',', storeNames)})";
                        }
                    }

                    //choose phone number to send SMS
                    //currently supported: "my phone" (filled on the configuration page), customer phone, phone of the billing address
                    switch (phoneTypeID)
                    {
                        case 0:
                            smsModel.PhoneType = await _localizationService.GetResource("Plugins.Misc.SendinBlue.MyPhone");
                            break;
                        case 1:
                            smsModel.PhoneType = await _localizationService.GetResource("Plugins.Misc.SendinBlue.CustomerPhone");
                            break;
                        case 2:
                            smsModel.PhoneType = await _localizationService.GetResource("Plugins.Misc.SendinBlue.BillingAddressPhone");
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

            var message = await _messageTemplateService.GetMessageTemplateById(model.MessageId);
            if (message != null)
            {
                await _genericAttributeService.SaveAttribute(message, SendinBlueDefaults.UseSmsAttribute, true);
                await _genericAttributeService.SaveAttribute(message, SendinBlueDefaults.SmsTextAttribute, model.Text);
                await _genericAttributeService.SaveAttribute(message, SendinBlueDefaults.PhoneTypeAttribute, model.PhoneTypeId);
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
            var message = await _messageTemplateService.GetMessageTemplateById(model.Id);
            if (message != null)
            {
                await _genericAttributeService.SaveAttribute<bool?>(message, SendinBlueDefaults.UseSmsAttribute, null);
                await _genericAttributeService.SaveAttribute<string>(message, SendinBlueDefaults.SmsTextAttribute, null);
                await _genericAttributeService.SaveAttribute<int?>(message, SendinBlueDefaults.PhoneTypeAttribute, null);
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

            var campaignErrors = await _sendinBlueEmailManager.SendSMSCampaign(model.CampaignListId, model.CampaignSenderName, model.CampaignText);
            if (!string.IsNullOrEmpty(campaignErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {campaignErrors}");
            else
                _notificationService.SuccessNotification(await _localizationService.GetResource("Plugins.Misc.SendinBlue.SMS.Campaigns.Sent"));

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

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sendinBlueSettings = await _settingService.LoadSetting<SendinBlueSettings>(storeId);

            sendinBlueSettings.UseMarketingAutomation = model.UseMarketingAutomation;
            await _settingService.SaveSettingOverridablePerStore(sendinBlueSettings, settings => settings.UseMarketingAutomation, model.UseMarketingAutomation_OverrideForStore, storeId, false);

            var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _sendinBlueEmailManager.GetAccountInfo();
            sendinBlueSettings.MarketingAutomationKey = maKey;
            if (!string.IsNullOrEmpty(accountErrors))
                _notificationService.ErrorNotification($"{SendinBlueDefaults.NotificationMessage} {accountErrors}");

            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.MarketingAutomationKey, clearCache: false);
            sendinBlueSettings.TrackingScript = model.TrackingScript;
            await _settingService.SaveSetting(sendinBlueSettings, settings => settings.TrackingScript, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCache();

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> ImportContacts(BaseNopModel model, IFormCollection form)
        {
            try
            {
                //logging info
                var logInfo = string.Format("SendinBlue synchronization: New emails {1},{0} Existing emails {2},{0} Invalid emails {3},{0} Duplicates emails {4}{0}",
                    Environment.NewLine, form["new_emails"], form["emails_exists"], form["invalid_email"], form["duplicates_email"]);
                await _logger.Information(logInfo);

                //display info on configuration page in case of the manually synchronization
                await _staticCacheManager.Set(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), logInfo);
            }
            catch (Exception ex)
            {
                await _logger.Error(ex.Message, ex);
                await _staticCacheManager.Set(_staticCacheManager.PrepareKeyForDefaultCache(SendinBlueDefaults.SyncKeyCache), ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UnsubscribeWebHook()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                await _sendinBlueEmailManager.UnsubscribeWebhook(streamReader.ReadToEnd());
            }
            catch (Exception ex)
            {
                await _logger.Error(ex.Message, ex);
            }

            return Ok();
        }

        #endregion
    }
}