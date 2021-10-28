using System;
using System.Threading.Tasks;
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

        protected ICategoryTemplateService CategoryTemplateService { get; }
        protected IManufacturerTemplateService ManufacturerTemplateService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductTemplateService ProductTemplateService { get; }
        protected ITemplateModelFactory TemplateModelFactory { get; }
        protected ITopicTemplateService TopicTemplateService { get; }

        #endregion

        #region Ctor

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPermissionService permissionService,
            IProductTemplateService productTemplateService,
            ITemplateModelFactory templateModelFactory,
            ITopicTemplateService topicTemplateService)
        {
            CategoryTemplateService = categoryTemplateService;
            ManufacturerTemplateService = manufacturerTemplateService;
            PermissionService = permissionService;
            ProductTemplateService = productTemplateService;
            TemplateModelFactory = templateModelFactory;
            TopicTemplateService = topicTemplateService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await TemplateModelFactory.PrepareTemplatesModelAsync(new TemplatesModel());

            return View(model);
        }

        #region Category templates        

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplates(CategoryTemplateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TemplateModelFactory.PrepareCategoryTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a category template with the specified id
            var template = await CategoryTemplateService.GetCategoryTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await CategoryTemplateService.UpdateCategoryTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateAdd(CategoryTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            await CategoryTemplateService.InsertCategoryTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a category template with the specified id
            var template = await CategoryTemplateService.GetCategoryTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await CategoryTemplateService.DeleteCategoryTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region Manufacturer templates        

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplates(ManufacturerTemplateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TemplateModelFactory.PrepareManufacturerTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a manufacturer template with the specified id
            var template = await ManufacturerTemplateService.GetManufacturerTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await ManufacturerTemplateService.UpdateManufacturerTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateAdd(ManufacturerTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new ManufacturerTemplate();
            template = model.ToEntity(template);
            await ManufacturerTemplateService.InsertManufacturerTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a manufacturer template with the specified id
            var template = await ManufacturerTemplateService.GetManufacturerTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await ManufacturerTemplateService.DeleteManufacturerTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region Product templates
                
        [HttpPost]
        public virtual async Task<IActionResult> ProductTemplates(ProductTemplateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TemplateModelFactory.PrepareProductTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTemplateUpdate(ProductTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a product template with the specified id
            var template = await ProductTemplateService.GetProductTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await ProductTemplateService.UpdateProductTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTemplateAdd(ProductTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new ProductTemplate();
            template = model.ToEntity(template);
            await ProductTemplateService.InsertProductTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTemplateDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a product template with the specified id
            var template = await ProductTemplateService.GetProductTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await ProductTemplateService.DeleteProductTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region Topic templates
        
        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplates(TopicTemplateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TemplateModelFactory.PrepareTopicTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateUpdate(TopicTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a topic template with the specified id
            var template = await TopicTemplateService.GetTopicTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await TopicTemplateService.UpdateTopicTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateAdd(TopicTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new TopicTemplate();
            template = model.ToEntity(template);
            await TopicTemplateService.InsertTopicTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //try to get a topic template with the specified id
            var template = await TopicTemplateService.GetTopicTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await TopicTemplateService.DeleteTopicTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}