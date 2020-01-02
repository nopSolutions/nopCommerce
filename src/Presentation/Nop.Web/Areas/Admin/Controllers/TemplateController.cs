using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ITemplateModelFactory _templateModelFactory;
        private readonly ITopicTemplateService _topicTemplateService;

        #endregion

        #region Ctor

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPermissionService permissionService,
            IProductTemplateService productTemplateService,
            ITemplateModelFactory templateModelFactory,
            ITopicTemplateService topicTemplateService)
        {
            _categoryTemplateService = categoryTemplateService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _permissionService = permissionService;
            _productTemplateService = productTemplateService;
            _templateModelFactory = templateModelFactory;
            _topicTemplateService = topicTemplateService;
        }

        #endregion

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = _templateModelFactory.PrepareTemplatesModel(new TemplatesModel());

            return View(model);
        }

        #region Category templates        

        [HttpPost]
        public virtual IActionResult CategoryTemplates(CategoryTemplateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _templateModelFactory.PrepareCategoryTemplateListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a category template with the specified id
            var template = _categoryTemplateService.GetCategoryTemplateById(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            _categoryTemplateService.UpdateCategoryTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CategoryTemplateAdd(CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            _categoryTemplateService.InsertCategoryTemplate(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult CategoryTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a category template with the specified id
            var template = _categoryTemplateService.GetCategoryTemplateById(id)
                ?? throw new ArgumentException("No template found with the specified id");

            _categoryTemplateService.DeleteCategoryTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #region Manufacturer templates        

        [HttpPost]
        public virtual IActionResult ManufacturerTemplates(ManufacturerTemplateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _templateModelFactory.PrepareManufacturerTemplateListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a manufacturer template with the specified id
            var template = _manufacturerTemplateService.GetManufacturerTemplateById(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            _manufacturerTemplateService.UpdateManufacturerTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ManufacturerTemplateAdd(ManufacturerTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new ManufacturerTemplate();
            template = model.ToEntity(template);
            _manufacturerTemplateService.InsertManufacturerTemplate(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ManufacturerTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a manufacturer template with the specified id
            var template = _manufacturerTemplateService.GetManufacturerTemplateById(id)
                ?? throw new ArgumentException("No template found with the specified id");

            _manufacturerTemplateService.DeleteManufacturerTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #region Product templates
                
        [HttpPost]
        public virtual IActionResult ProductTemplates(ProductTemplateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _templateModelFactory.PrepareProductTemplateListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductTemplateUpdate(ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a product template with the specified id
            var template = _productTemplateService.GetProductTemplateById(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            _productTemplateService.UpdateProductTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductTemplateAdd(ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new ProductTemplate();
            template = model.ToEntity(template);
            _productTemplateService.InsertProductTemplate(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a product template with the specified id
            var template = _productTemplateService.GetProductTemplateById(id)
                ?? throw new ArgumentException("No template found with the specified id");

            _productTemplateService.DeleteProductTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #region Topic templates
        
        [HttpPost]
        public virtual IActionResult TopicTemplates(TopicTemplateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _templateModelFactory.PrepareTopicTemplateListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult TopicTemplateUpdate(TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a topic template with the specified id
            var template = _topicTemplateService.GetTopicTemplateById(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            _topicTemplateService.UpdateTopicTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult TopicTemplateAdd(TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new TopicTemplate();
            template = model.ToEntity(template);
            _topicTemplateService.InsertTopicTemplate(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult TopicTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a topic template with the specified id
            var template = _topicTemplateService.GetTopicTemplateById(id)
                ?? throw new ArgumentException("No template found with the specified id");

            _topicTemplateService.DeleteTopicTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}