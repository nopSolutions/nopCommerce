using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Templates;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class TemplateController : BaseNopController
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IProductTemplateService productTemplateService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._productTemplateService = productTemplateService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Category templates

        public ActionResult CategoryTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult CategoryTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _categoryTemplateService.GetAllCategoryTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        public ActionResult CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = _categoryTemplateService.GetCategoryTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _categoryTemplateService.UpdateCategoryTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult CategoryTemplateAdd([Bind(Exclude = "Id")] CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            _categoryTemplateService.InsertCategoryTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult CategoryTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _categoryTemplateService.GetCategoryTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _categoryTemplateService.DeleteCategoryTemplate(template);

            return Json(null);
        }

        #endregion

        #region Manufacturer templates

        public ActionResult ManufacturerTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult ManufacturerTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _manufacturerTemplateService.GetAllManufacturerTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        public ActionResult ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = _manufacturerTemplateService.GetManufacturerTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _manufacturerTemplateService.UpdateManufacturerTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult ManufacturerTemplateAdd([Bind(Exclude = "Id")] ManufacturerTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = new ManufacturerTemplate();
            template = model.ToEntity(template);
            _manufacturerTemplateService.InsertManufacturerTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult ManufacturerTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _manufacturerTemplateService.GetManufacturerTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _manufacturerTemplateService.DeleteManufacturerTemplate(template);

            return Json(null);
        }

        #endregion

        #region Product templates

        public ActionResult ProductTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult ProductTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _productTemplateService.GetAllProductTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        public ActionResult ProductTemplateUpdate(ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = _productTemplateService.GetProductTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _productTemplateService.UpdateProductTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult ProductTemplateAdd([Bind(Exclude = "Id")] ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var template = new ProductTemplate();
            template = model.ToEntity(template);
            _productTemplateService.InsertProductTemplate(template);

            return Json(null);
        }

        [HttpPost]
        public ActionResult ProductTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _productTemplateService.GetProductTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _productTemplateService.DeleteProductTemplate(template);

            return Json(null);
        }

        #endregion
    }
}
