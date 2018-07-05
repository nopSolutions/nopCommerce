using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Cms;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the widget model factory implementation
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        private readonly IWidgetService _widgetService;

        #endregion

        #region Ctor

        public WidgetModelFactory(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare widget search model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>Widget search model</returns>
        public virtual WidgetSearchModel PrepareWidgetSearchModel(WidgetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged widget list model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>Widget list model</returns>
        public virtual WidgetListModel PrepareWidgetListModel(WidgetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get widgets
            var widgets = _widgetService.LoadAllWidgets();

            //prepare grid model
            var model = new WidgetListModel
            {
                Data = widgets.PaginationByRequestModel(searchModel).Select(widget =>
                {
                    //fill in model values from the entity
                    var widgetMethodModel = widget.ToPluginModel<WidgetModel>();

                    //fill in additional values (not existing in the entity)
                    widgetMethodModel.IsActive = _widgetService.IsWidgetActive(widget);
                    widgetMethodModel.ConfigurationUrl = widget.GetConfigurationPageUrl();

                    return widgetMethodModel;
                }),
                Total = widgets.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare render widget models
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>List of render widget models</returns>
        public virtual IList<RenderWidgetModel> PrepareRenderWidgetModels(string widgetZone, object additionalData = null)
        {
            //add widget zone to view component arguments
            additionalData = new RouteValueDictionary
            {
                { "widgetZone", widgetZone },
                { "additionalData", additionalData }
            };

            //get active widgets by widget zone
            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);

            //prepare models
            var models = widgets.Select(widget => new RenderWidgetModel
            {
                WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
                WidgetViewComponentArguments = additionalData
            }).ToList();

            return models;
        }

        #endregion
    }
}