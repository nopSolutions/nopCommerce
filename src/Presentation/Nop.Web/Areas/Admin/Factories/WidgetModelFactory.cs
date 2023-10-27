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

        protected readonly IWidgetPluginManager _widgetPluginManager;
        protected readonly IWorkContext _workContext;

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

        #endregion
    }
}