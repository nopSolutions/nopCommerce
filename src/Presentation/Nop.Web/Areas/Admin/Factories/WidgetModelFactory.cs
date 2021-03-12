using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the widget model factory implementation
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare widget search model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget search model
        /// </returns>
        public virtual Task<WidgetSearchModel> PrepareWidgetSearchModelAsync(WidgetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged widget list model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget list model
        /// </returns>
        public virtual async Task<WidgetListModel> PrepareWidgetListModelAsync(WidgetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get widgets
            var widgets = (await _widgetPluginManager.LoadAllPluginsAsync())
                .Where(widget => !widget.HideInWidgetList).ToList()
                .ToPagedList(searchModel);

            //prepare grid model
            var model = new WidgetListModel().PrepareToGrid(searchModel, widgets, () =>
            {
                return widgets.Select(widget =>
                {
                    //fill in model values from the entity
                    var widgetMethodModel = widget.ToPluginModel<WidgetModel>();

                    //fill in additional values (not existing in the entity)
                    widgetMethodModel.IsActive = _widgetPluginManager.IsPluginActive(widget);
                    widgetMethodModel.ConfigurationUrl = widget.GetConfigurationPageUrl();

                    return widgetMethodModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare render widget models
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of render widget models
        /// </returns>
        public virtual async Task<IList<RenderWidgetModel>> PrepareRenderWidgetModelsAsync(string widgetZone, object additionalData = null)
        {
            //get active widgets by widget zone
            var widgets = await _widgetPluginManager.LoadActivePluginsAsync(await _workContext.GetCurrentCustomerAsync(), widgetZone: widgetZone);

            //prepare models
            var models = widgets.Select(widget => new RenderWidgetModel
            {
                WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
            }).ToList();

            return models;
        }

        #endregion
    }
}