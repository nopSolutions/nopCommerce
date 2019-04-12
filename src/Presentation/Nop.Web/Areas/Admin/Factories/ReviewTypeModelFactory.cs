using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents a review type model factory implementation
    /// </summary>
    public partial class ReviewTypeModelFactory : IReviewTypeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IReviewTypeService _reviewTypeService;

        #endregion

        #region Ctor

        public ReviewTypeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IReviewTypeService reviewTypeService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _reviewTypeService = reviewTypeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareReviewTypeGridModel(ReviewTypeSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "reviewtypes-grid",
                UrlRead = new DataUrl("List", "ReviewType", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = null;

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(ReviewTypeModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Settings.ReviewType.Fields.Name"),
                    Width = "300"
                },
                new ColumnProperty(nameof(ReviewTypeModel.IsRequired))
                {
                    Title = _localizationService.GetResource("Admin.Settings.ReviewType.Fields.IsRequired"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(ReviewTypeModel.VisibleToAllCustomers))
                {
                    Title = _localizationService.GetResource("Admin.Settings.ReviewType.Fields.VisibleToAllCustomers"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(ReviewTypeModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Settings.ReviewType.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(ReviewTypeModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonEdit(new DataUrl("~/Admin/ReviewType/Edit/"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare review type search model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type search model</returns>
        public virtual ReviewTypeSearchModel PrepareReviewTypeSearchModel(ReviewTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareReviewTypeGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged review type list model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type list model</returns>
        public virtual ReviewTypeListModel PrepareReviewTypeListModel(ReviewTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get review types
            var reviewTypes = _reviewTypeService.GetAllReviewTypes().ToPagedList(searchModel);

            //prepare list model
            var model = new ReviewTypeListModel().PrepareToGrid(searchModel, reviewTypes, () =>
            {
                //fill in model values from the entity
                return reviewTypes.Select(reviewType => reviewType.ToModel<ReviewTypeModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare review type model
        /// </summary>
        /// <param name="model">Review type model</param>
        /// <param name="reviewType">Review type</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Review type model</returns>
        public virtual ReviewTypeModel PrepareReviewTypeModel(ReviewTypeModel model,
            ReviewType reviewType, bool excludeProperties = false)
        {
            Action<ReviewTypeLocalizedModel, int> localizedModelConfiguration = null;

            if (reviewType != null)
            {
                //fill in model values from the entity
                model = model ?? reviewType.ToModel<ReviewTypeModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(reviewType, entity => entity.Name, languageId, false, false);
                    locale.Description = _localizationService.GetLocalized(reviewType, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}
