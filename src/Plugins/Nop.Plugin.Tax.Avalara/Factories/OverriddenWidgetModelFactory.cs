using System;
using System.Linq;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Tax.Avalara.Factories
{
    /// <summary>
    /// Represents the widget model factory implementation
    /// </summary>
    public class OverriddenWidgetModelFactory : WidgetModelFactory
    {
        #region Fields

        private readonly IWidgetPluginManager _widgetPluginManager;

        #endregion

        #region Ctor

        public OverriddenWidgetModelFactory(IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext) : base(widgetPluginManager, workContext)
        {
            _widgetPluginManager = widgetPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare paged widget list model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>Widget list model</returns>
        public override WidgetListModel PrepareWidgetListModel(WidgetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //exclude Avalara tax provider from the widget list
            var widgets = _widgetPluginManager.LoadAllPlugins()
                .Where(widget => !widget.PluginDescriptor.SystemName.Equals(AvalaraTaxDefaults.SystemName)).ToList()
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

        #endregion
    }
}