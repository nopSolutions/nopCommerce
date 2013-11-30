using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Templates;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

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

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryTemplates(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _categoryTemplateService.GetAllCategoryTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new GridModel<CategoryTemplateModel>
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryTemplateUpdate(CategoryTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = _categoryTemplateService.GetCategoryTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _categoryTemplateService.UpdateCategoryTemplate(template);

            return CategoryTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryTemplateAdd([Bind(Exclude = "Id")] CategoryTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            _categoryTemplateService.InsertCategoryTemplate(template);

            return CategoryTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryTemplateDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _categoryTemplateService.GetCategoryTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _categoryTemplateService.DeleteCategoryTemplate(template);

            return CategoryTemplates(command);
        }

        #endregion

        #region Manufacturer templates

        public ActionResult ManufacturerTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ManufacturerTemplates(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _manufacturerTemplateService.GetAllManufacturerTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new GridModel<ManufacturerTemplateModel>
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ManufacturerTemplateUpdate(ManufacturerTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = _manufacturerTemplateService.GetManufacturerTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _manufacturerTemplateService.UpdateManufacturerTemplate(template);

            return ManufacturerTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ManufacturerTemplateAdd([Bind(Exclude = "Id")] ManufacturerTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = new ManufacturerTemplate();
            template = model.ToEntity(template);
            _manufacturerTemplateService.InsertManufacturerTemplate(template);

            return ManufacturerTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ManufacturerTemplateDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _manufacturerTemplateService.GetManufacturerTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _manufacturerTemplateService.DeleteManufacturerTemplate(template);

            return ManufacturerTemplates(command);
        }

        #endregion

        #region Product templates

        public ActionResult ProductTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTemplates(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _productTemplateService.GetAllProductTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var model = new GridModel<ProductTemplateModel>
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTemplateUpdate(ProductTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = _productTemplateService.GetProductTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _productTemplateService.UpdateProductTemplate(template);

            return ProductTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTemplateAdd([Bind(Exclude = "Id")] ProductTemplateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = new ProductTemplate();
            template = model.ToEntity(template);
            _productTemplateService.InsertProductTemplate(template);

            return ProductTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTemplateDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _productTemplateService.GetProductTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _productTemplateService.DeleteProductTemplate(template);

            return ProductTemplates(command);
        }

        #endregion
    }
}
