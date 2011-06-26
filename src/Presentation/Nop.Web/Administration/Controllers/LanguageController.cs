using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Localization;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class LanguageController : BaseNopController
	{
		#region Fields

		private readonly ILanguageService _languageService;
		private readonly ILocalizationService _localizationService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
		#endregion

		#region Constructors

		public LanguageController(ILanguageService languageService, ILocalizationService localizationService,
            IExportManager exportManager, IImportManager importManager)
		{
			this._localizationService = localizationService;
            this._languageService = languageService;
            this._exportManager = exportManager;
            this._importManager = importManager;
		}

		#endregion 

		#region Languages

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
		{
			var languages = _languageService.GetAllLanguages(true);
			var gridModel = new GridModel<LanguageModel>
			{
				Data = languages.Select(x => x.ToModel()),
				Total = languages.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
		{
			var languages = _languageService.GetAllLanguages(true);
			var gridModel = new GridModel<LanguageModel>
			{
				Data = languages.Select(x => x.ToModel()),
				Total = languages.Count()
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        public ActionResult Create()
        {
            var model = new LanguageModel();
            //default values
            model.Published = true;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public ActionResult Edit(int id)
		{
			var language = _languageService.GetLanguageById(id);
			if (language == null) 
                throw new ArgumentException("No language found with the specified id", "id");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;
            
			return View(language.ToModel());
		}
        
        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
		public ActionResult Edit(LanguageModel model, bool continueEditing)
        {
            var language = _languageService.GetLanguageById(model.Id);
            if (ModelState.IsValid)
            {
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
		{
			var language = _languageService.GetLanguageById(id);
			_languageService.DeleteLanguage(language);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
			return RedirectToAction("List");
		}

		#endregion

		#region Resources

		public ActionResult Resources(int languageId)
		{
			ViewBag.AllLanguages = _languageService.GetAllLanguages(true)
                .Select(x => new DropDownItem
                {
                    Selected = (x.Id.Equals(languageId)),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
		    var language = _languageService.GetLanguageById(languageId);
		    ViewBag.LanguageId = languageId;
		    ViewBag.LanguageName = language.Name;

			var resources = _localizationService.GetAllResourcesByLanguageId(languageId);
			var gridModel = new GridModel<LanguageResourceModel>
			{
				Data = resources.Take(20).Select(x => x.Value.ToModel()),
				Total = resources.Count
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult Resources(int languageId, GridCommand command)
		{
		    var resources = _localizationService.GetAllResourcesByLanguageId(languageId).Select(x => x.Value)
		        .Select(x => x.ToModel())
		        .ForCommand(command);

            var model = new GridModel<LanguageResourceModel>
            {
                Data = resources.PagedForCommand(command),
                Total = resources.Count()
            };
		    return new JsonResult
			{
				Data = model
			};
		}


        [GridAction(EnableCustomBinding=true)]
        public ActionResult ResourceUpdate(LanguageResourceModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult { Data = "error" };
            }

            var resource = _localizationService.GetLocaleStringResourceById(model.Id);
            resource = model.ToEntity(resource);
            _localizationService.UpdateLocaleStringResource(resource);

            return Resources(model.LanguageId, command);
        }
        
        [GridAction(EnableCustomBinding = true)]
        public ActionResult ResourceAdd(int id, LanguageResourceModel resourceModel, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult {Data = "error"};
            }

            var resource = new LocaleStringResource {LanguageId = id};
            resource = resourceModel.ToEntity(resource);
            _localizationService.InsertLocaleStringResource(resource);


            return Resources(id, command);
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult ResourceDelete(int id, int languageId, GridCommand command)
        {
            var resource = _localizationService.GetLocaleStringResourceById(id);
            _localizationService.DeleteLocaleStringResource(resource);


            return Resources(languageId, command);
        }

        #endregion
        
        #region Export / Import

        public ActionResult ExportXml(int id)
        {
            var language = _languageService.GetLanguageById(id);
            if (language == null)
                throw new ArgumentException("No language found with the specified id", "id");

            try
            {
                var fileName = string.Format("language_{0}.xml", id);
                var xml = _exportManager.ExportLanguageToXml(language);
                return new XmlDownloadResult(xml, fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportXml(int id, FormCollection form)
        {
            var language = _languageService.GetLanguageById(id);
            if (language == null)
                throw new ArgumentException("No language found with the specified id", "id");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                var file = Request.Files["importxmlfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
                        _importManager.ImportLanguageFromXml(language, content);
                    }

                }
                else
                {
                    ErrorNotification("Please upload a file");
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }

        }

        #endregion
    }
}
