using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Topics;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Security.Permissions;
using Nop.Services.Topics;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class TopicController : BaseNopController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IWebHelper _webHelper;
        #endregion Fields

        #region Constructors

        public TopicController(ITopicService topicService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, IWebHelper webHelper)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._webHelper = webHelper;
        }

        #endregion Constructors
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(Topic topic, TopicModel model)
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
            }
        }
        
        #endregion
        
        #region List / tree

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var topics = _topicService.GetAllTopic();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => x.ToModel()),
                Total = topics.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var topics = _topicService.GetAllTopic();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => x.ToModel()),
                Total = topics.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            var model = new TopicModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {
            //decode description
            model.Body = HttpUtility.HtmlDecode(model.Body);
            foreach (var localized in model.Locales)
                localized.Body = HttpUtility.HtmlDecode(localized.Body);

            if (ModelState.IsValid)
            {
                var topic = model.ToEntity();
                _topicService.InsertTopic(topic);
                //locales
                UpdateLocales(topic, model);

                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                throw new ArgumentException("No topic found with the specified id", "id");
            var model = topic.ToModel();
            //TODO add a method for getting the URL (e.g. SEOHelper.GetEntityUrl)
            model.Url = string.Format("{0}t/{1}", _webHelper.GetStoreLocation(false), topic.SystemName).ToLowerInvariant();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false);
            });

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {
            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                throw new ArgumentException("No topic found with the specified id");

            //decode description
            model.Body = HttpUtility.HtmlDecode(model.Body);
            foreach (var localized in model.Locales)
                localized.Body = HttpUtility.HtmlDecode(localized.Body);

            //TODO add a method for getting the URL (e.g. SEOHelper.GetEntityUrl)
            model.Url = string.Format("{0}t/{1}", _webHelper.GetStoreLocation(false), topic.SystemName).ToLowerInvariant();

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                _topicService.UpdateTopic(topic);
                //locales
                UpdateLocales(topic, model);

                return continueEditing ? RedirectToAction("Edit", topic.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var topic = _topicService.GetTopicById(id);
            _topicService.DeleteTopic(topic);
            return RedirectToAction("List");
        }
        
        #endregion
    }
}
