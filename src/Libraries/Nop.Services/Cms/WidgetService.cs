using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Events;
using Nop.Core.Plugins;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Widget service
    /// </summary>
    public partial class WidgetService : IWidgetService
    {
        #region Constants
        private const string WIDGETS_BY_ID_KEY = "Nop.widget.id-{0}";
        private const string WIDGETS_ALL_KEY = "Nop.widget.all";
        private const string WIDGETS_PATTERN_KEY = "Nop.widget.";

        #endregion

        #region Fields

        private readonly IRepository<Widget> _widgetRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="widgetRepository">Widget repository</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher"></param>
        public WidgetService(ICacheManager cacheManager,
            IRepository<Widget> widgetRepository, 
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _widgetRepository = widgetRepository;
            _pluginFinder = pluginFinder;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods



        /// <summary>
        /// Load widget plugin provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget plugin</returns>
        public virtual IWidgetPlugin LoadWidgetPluginBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IWidgetPlugin>();

            return null;
        }

        /// <summary>
        /// Load all widget plugins
        /// </summary>
        /// <returns>widget plugins</returns>
        public virtual IList<IWidgetPlugin> LoadAllWidgetPlugins()
        {
            return _pluginFinder.GetPlugins<IWidgetPlugin>().ToList();
        }









        /// <summary>
        /// Delete widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void DeleteWidget(Widget widget)
        {
            if (widget == null)
                throw new ArgumentNullException("widget");

            _widgetRepository.Delete(widget);

            _cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(widget);
        }

        /// <summary>
        /// Gets all widgets
        /// </summary>
        /// <returns>Widgets</returns>
        public virtual IList<Widget> GetAllWidgets()
        {
            string key = WIDGETS_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from w in _widgetRepository.Table
                            orderby w.DisplayOrder
                            select w;
                return query.ToList();
            });
        }

        /// <summary>
        /// Gets all widgets by zone
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <returns>Widgets</returns>
        public virtual IList<Widget> GetAllWidgetsByZone(WidgetZone widgetZone)
        {
            var allWidgets = GetAllWidgets();
            var widgets = allWidgets
                .Where(w => w.WidgetZone == widgetZone)
                .OrderBy(w => w.DisplayOrder)
                .ToList();
            return widgets;
        }

        /// <summary>
        /// Gets a widget
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <returns>Widget</returns>
        public virtual Widget GetWidgetById(int widgetId)
        {
            if (widgetId == 0)
                return null;

            string key = string.Format(WIDGETS_BY_ID_KEY, widgetId);
            return _cacheManager.Get(key, () =>
            {
                var widget = _widgetRepository.GetById(widgetId);
                return widget;
            });
        }

        /// <summary>
        /// Inserts widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void InsertWidget(Widget widget)
        {
            if (widget == null)
                throw new ArgumentNullException("widget");

            _widgetRepository.Insert(widget);

            //cache
            _cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(widget);
        }

        /// <summary>
        /// Updates the widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void UpdateWidget(Widget widget)
        {
            if (widget == null)
                throw new ArgumentNullException("widget");

            _widgetRepository.Update(widget);

            //cache
            _cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(widget);
        }
        
        #endregion
    }
}
