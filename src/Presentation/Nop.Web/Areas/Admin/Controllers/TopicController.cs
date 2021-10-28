using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Topics;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TopicController : BaseAdminController
    {
        #region Fields

        protected IAclService AclService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected ITopicModelFactory TopicModelFactory { get; }
        protected ITopicService TopicService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }


        #endregion Fields

        #region Ctor

        public TopicController(IAclService aclService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ITopicModelFactory topicModelFactory,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            AclService = aclService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            TopicModelFactory = topicModelFactory;
            TopicService = topicService;
            UrlRecordService = urlRecordService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(topic, localized.SeName, localized.Title, false);
                await UrlRecordService.SaveSlugAsync(topic, seName, localized.LanguageId);
            }
        }

        protected virtual async Task SaveTopicAclAsync(Topic topic, TopicModel model)
        {
            topic.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await TopicService.UpdateTopicAsync(topic);

            var existingAclRecords = await AclService.GetAclRecordsAsync(topic);
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await AclService.InsertAclRecordAsync(topic, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await AclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Topic topic, TopicModel model)
        {
            topic.LimitedToStores = model.SelectedStoreIds.Any();
            await TopicService.UpdateTopicAsync(topic);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(topic);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(topic, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await TopicModelFactory.PrepareTopicSearchModelAsync(new TopicSearchModel());

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TopicSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TopicModelFactory.PrepareTopicListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await TopicModelFactory.PrepareTopicModelAsync(new TopicModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TopicModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                    model.Password = null;

                var topic = model.ToEntity<Topic>();
                await TopicService.InsertTopicAsync(topic);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await UrlRecordService.SaveSlugAsync(topic, model.SeName, 0);

                //ACL (customer roles)
                await SaveTopicAclAsync(topic, model);

                //stores
                await SaveStoreMappingsAsync(topic, model);

                //locales
                await UpdateLocalesAsync(topic, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Topics.Added"));

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewTopic",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await TopicModelFactory.PrepareTopicModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await TopicService.GetTopicByIdAsync(id);
            if (topic == null)
                return RedirectToAction("List");

            //prepare model
            var model = await TopicModelFactory.PrepareTopicModelAsync(null, topic);

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TopicModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await TopicService.GetTopicByIdAsync(model.Id);
            if (topic == null)
                return RedirectToAction("List");

            if (!model.IsPasswordProtected)
                model.Password = null;

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                await TopicService.UpdateTopicAsync(topic);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await UrlRecordService.SaveSlugAsync(topic, model.SeName, 0);

                //ACL (customer roles)
                await SaveTopicAclAsync(topic, model);

                //stores
                await SaveStoreMappingsAsync(topic, model);

                //locales
                await UpdateLocalesAsync(topic, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Topics.Updated"));

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditTopic",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await TopicModelFactory.PrepareTopicModelAsync(model, topic, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await TopicService.GetTopicByIdAsync(id);
            if (topic == null)
                return RedirectToAction("List");

            await TopicService.DeleteTopicAsync(topic);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Topics.Deleted"));

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteTopic",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteTopic"), topic.Title ?? topic.SystemName), topic);

            return RedirectToAction("List");
        }

        #endregion
    }
}