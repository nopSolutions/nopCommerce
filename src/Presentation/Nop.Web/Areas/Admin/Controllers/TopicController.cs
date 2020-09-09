using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Topics;
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

        private readonly IAclService _aclService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITopicModelFactory _topicModelFactory;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;

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
            IUrlRecordService urlRecordService)
        {
            _aclService = aclService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _topicModelFactory = topicModelFactory;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocales(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(topic,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(topic,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(topic,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(topic,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(topic,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeName(topic, localized.SeName, localized.Title, false);
                await _urlRecordService.SaveSlug(topic, seName, localized.LanguageId);
            }
        }

        protected virtual async Task SaveTopicAcl(Topic topic, TopicModel model)
        {
            topic.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await _topicService.UpdateTopic(topic);

            var existingAclRecords = await _aclService.GetAclRecords(topic);
            var allCustomerRoles = await _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await _aclService.InsertAclRecord(topic, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappings(Topic topic, TopicModel model)
        {
            topic.LimitedToStores = model.SelectedStoreIds.Any();
            await _topicService.UpdateTopic(topic);

            var existingStoreMappings = await _storeMappingService.GetStoreMappings(topic);
            var allStores = await _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMapping(topic, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicSearchModel(new TopicSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TopicSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicModel(new TopicModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TopicModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                    model.Password = null;

                var topic = model.ToEntity<Topic>();
                await _topicService.InsertTopic(topic);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await _urlRecordService.SaveSlug(topic, model.SeName, 0);

                //ACL (customer roles)
                await SaveTopicAcl(topic, model);

                //stores
                await SaveStoreMappings(topic, model);

                //locales
                await UpdateLocales(topic, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Topics.Added"));

                //activity log
                await _customerActivityService.InsertActivity("AddNewTopic",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await _topicModelFactory.PrepareTopicModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicById(id);
            if (topic == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _topicModelFactory.PrepareTopicModel(null, topic);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TopicModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicById(model.Id);
            if (topic == null)
                return RedirectToAction("List");

            if (!model.IsPasswordProtected)
                model.Password = null;

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                await _topicService.UpdateTopic(topic);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await _urlRecordService.SaveSlug(topic, model.SeName, 0);

                //ACL (customer roles)
                await SaveTopicAcl(topic, model);

                //stores
                await SaveStoreMappings(topic, model);

                //locales
                await UpdateLocales(topic, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));

                //activity log
                await _customerActivityService.InsertActivity("EditTopic",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await _topicModelFactory.PrepareTopicModel(model, topic, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicById(id);
            if (topic == null)
                return RedirectToAction("List");

            await _topicService.DeleteTopic(topic);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));

            //activity log
            await _customerActivityService.InsertActivity("DeleteTopic",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteTopic"), topic.Title ?? topic.SystemName), topic);

            return RedirectToAction("List");
        }

        #endregion
    }
}