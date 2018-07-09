using System;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory implementation
    /// </summary>
    public partial class TemplateModelFactory : ITemplateModelFactory
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ITopicTemplateService _topicTemplateService;

        #endregion

        #region Ctor

        public TemplateModelFactory(ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IProductTemplateService productTemplateService,
            ITopicTemplateService topicTemplateService)
        {
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._productTemplateService = productTemplateService;
            this._topicTemplateService = topicTemplateService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>Templates model</returns>
        public virtual TemplatesModel PrepareTemplatesModel(TemplatesModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare nested search models
            PrepareCategoryTemplateSearchModel(model.TemplatesCategory);
            PrepareManufacturerTemplateSearchModel(model.TemplatesManufacturer);
            PrepareProductTemplateSearchModel(model.TemplatesProduct);
            PrepareTopicTemplateSearchModel(model.TemplatesTopic);

            return model;
        }

        /// <summary>
        /// Prepare category template search model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>Category template search model</returns>
        public virtual CategoryTemplateSearchModel PrepareCategoryTemplateSearchModel(CategoryTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category template list model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>Category template list model</returns>
        public virtual CategoryTemplateListModel PrepareCategoryTemplateListModel(CategoryTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get category templates
            var categoryTemplates = _categoryTemplateService.GetAllCategoryTemplates();

            //prepare grid model
            var model = new CategoryTemplateListModel
            {
                //fill in model values from the entity
                Data = categoryTemplates.PaginationByRequestModel(searchModel).Select(template => template.ToModel<CategoryTemplateModel>()),
                Total = categoryTemplates.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare manufacturer template search model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>Manufacturer template search model</returns>
        public virtual ManufacturerTemplateSearchModel PrepareManufacturerTemplateSearchModel(ManufacturerTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged manufacturer template list model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>Manufacturer template list model</returns>
        public virtual ManufacturerTemplateListModel PrepareManufacturerTemplateListModel(ManufacturerTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturer templates
            var manufacturerTemplates = _manufacturerTemplateService.GetAllManufacturerTemplates();

            //prepare grid model
            var model = new ManufacturerTemplateListModel
            {
                //fill in model values from the entity
                Data = manufacturerTemplates.PaginationByRequestModel(searchModel)
                    .Select(template => template.ToModel<ManufacturerTemplateModel>()),
                Total = manufacturerTemplates.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare product template search model
        /// </summary>
        /// <param name="searchModel">Product template search model</param>
        /// <returns>Product template search model</returns>
        public virtual ProductTemplateSearchModel PrepareProductTemplateSearchModel(ProductTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product template list model
        /// </summary>
        /// <param name="searchModel">Product template search model</param>
        /// <returns>Product template list model</returns>
        public virtual ProductTemplateListModel PrepareProductTemplateListModel(ProductTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product templates
            var productTemplates = _productTemplateService.GetAllProductTemplates();

            //prepare grid model
            var model = new ProductTemplateListModel
            {
                //fill in model values from the entity
                Data = productTemplates.PaginationByRequestModel(searchModel).Select(template => template.ToModel<ProductTemplateModel>()),
                Total = productTemplates.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare topic template search model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>Topic template search model</returns>
        public virtual TopicTemplateSearchModel PrepareTopicTemplateSearchModel(TopicTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged topic template list model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>Topic template list model</returns>
        public virtual TopicTemplateListModel PrepareTopicTemplateListModel(TopicTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get topic templates
            var topicTemplates = _topicTemplateService.GetAllTopicTemplates();

            //prepare grid model
            var model = new TopicTemplateListModel
            {
                //fill in model values from the entity
                Data = topicTemplates.PaginationByRequestModel(searchModel).Select(template => template.ToModel<TopicTemplateModel>()),
                Total = topicTemplates.Count
            };

            return model;
        }

        #endregion
    }
}