using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Topics;
using Nop.Core.Domain.Topics;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Admin.Controllers
{
    public partial class TopicController : BaseAdminController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAclService _aclService;

        #endregion Fields

        #region Constructors

        public TopicController(ITopicService topicService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, 
            ILocalizationService localizationService,
            IPermissionService permissionService, 
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ITopicTemplateService topicTemplateService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            IAclService aclService)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._topicTemplateService = topicTemplateService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._aclService = aclService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void PrepareTemplatesModel(TopicModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _topicTemplateService.GetAllTopicTemplates();
            foreach (var template in templates)
            {
                model.AvailableTopicTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(topic,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = topic.ValidateSeName(localized.SeName, localized.Title, false);
                _urlRecordService.SaveSlug(topic, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareAclModel(TopicModel model, Topic topic, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && topic != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(topic).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveTopicAcl(Topic topic, TopicModel model)
        {
            topic.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(topic);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(topic, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(TopicModel model, Topic topic, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && topic != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(topic).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Topic topic, TopicModel model)
        {
            topic.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(topic);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(topic, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new TopicListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, TopicListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topicModels = _topicService.GetAllTopics(model.SearchStoreId, true, true)
                .Select(x =>x.ToModel())
                .ToList();
            //little hack here:
            //we don't have paging supported for topic list page
            //now ensure that topic bodies are not returned. otherwise, we can get the following error:
            //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
            foreach (var topic in topicModels)
            {
                topic.Body = "";
            }
            var gridModel = new DataSourceResult
            {
                Data = topicModels,
                Total = topicModels.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new TopicModel();
            //templates
            PrepareTemplatesModel(model);
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //locales
            AddLocales(_languageService, model.Locales);
            
            //default values
            model.DisplayOrder = 1;
            model.Published = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                {
                    model.Password = null;
                }

                var topic = model.ToEntity();
                _topicService.InsertTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //ACL (customer roles)
                SaveTopicAcl(topic, model);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));

                //activity log
                _customerActivityService.InsertActivity("AddNewTopic", _localizationService.GetResource("ActivityLog.AddNewTopic"), topic.Title ?? topic.SystemName);

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = topic.Id });
                }
                return RedirectToAction("List");

            }

            //If we got this far, something failed, redisplay form

            //templates
            PrepareTemplatesModel(model);
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            var model = topic.ToModel();
            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //ACL
            PrepareAclModel(model, topic, false);
            //Store
            PrepareStoresMappingModel(model, topic, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = topic.GetSeName(languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                _topicService.UpdateTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //ACL (customer roles)
                SaveTopicAcl(topic, model);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);
                
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                
                //activity log
                _customerActivityService.InsertActivity("EditTopic", _localizationService.GetResource("ActivityLog.EditTopic"), topic.Title ?? topic.SystemName);

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = topic.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form

            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //ACL
            PrepareAclModel(model, topic, true);
            //Store
            PrepareStoresMappingModel(model, topic, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            _topicService.DeleteTopic(topic);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));

            //activity log
            _customerActivityService.InsertActivity("DeleteTopic", _localizationService.GetResource("ActivityLog.DeleteTopic"), topic.Title ?? topic.SystemName);

            return RedirectToAction("List");
        }
        
        #endregion
    }
}
