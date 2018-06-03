using System;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions
{
    /// <summary>
    /// Represents the extensions to map entity to model and vise versa
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Execute a mapping from the source object to a new destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>Mapped destination object, same instance as the passed destination object</returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #region Model-Entity mapping

        /// <summary>
        /// Execute a mapping from the entity to the model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="entity">Entity to map from</param>
        /// <param name="model">Model to map into; pass null to map to the new model</param>
        /// <returns>Mapped model</returns>
        public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model = null) 
            where TEntity : BaseEntity where TModel : BaseNopEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            return model == null ? entity.Map<TEntity, TModel>() : entity.MapTo(model);
        }

        /// <summary>
        /// Execute a mapping from the model to the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model to map from</param>
        /// <param name="entity">Entity to map into; pass null to map to the new entity</param>
        /// <returns>Mapped entity</returns>
        public static TEntity ToEntity<TEntity, TModel>(this TModel model, TEntity entity = null) 
            where TEntity : BaseEntity where TModel : BaseNopEntityModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
            return entity == null ? model.Map<TModel, TEntity>() : model.MapTo(entity);
        }

        #endregion

        #region Model-Settings mapping

        /// <summary>
        /// Execute a mapping from the settings to the model
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="settings">Settings to map from</param>
        /// <param name="model">Model to map into; pass null to map to the new model</param>
        /// <returns>Mapped model</returns>
        public static TModel ToSettingsModel<TSettings, TModel>(this TSettings settings, TModel model = null)
            where TSettings : class, ISettings where TModel : BaseNopModel
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return model == null ? settings.Map<TSettings, TModel>() : settings.MapTo(model);
        }

        /// <summary>
        /// Execute a mapping from the model to the settings
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model to map from</param>
        /// <param name="settings">Settings to map into; pass null to map to the new settings</param>
        /// <returns>Mapped settings</returns>
        public static TSettings ToSettings<TSettings, TModel>(this TModel model, TSettings settings = null)
            where TSettings : class, ISettings where TModel : BaseNopModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return settings == null ? model.Map<TModel, TSettings>() : model.MapTo(settings);
        }

        #endregion

        #region Model-Plugin mapping

        /// <summary>
        /// Execute a mapping from the plugin to the model
        /// </summary>
        /// <typeparam name="TPlugin">Plugin type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="plugin">Plugin to map from</param>
        /// <param name="model">Model to map into; pass null to map to the new model</param>
        /// <returns>Mapped model</returns>
        public static TModel ToPluginModel<TPlugin, TModel>(this TPlugin plugin, TModel model = null)
            where TPlugin : class, IPlugin where TModel : BaseNopModel
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            return model == null ? plugin.Map<TPlugin, TModel>() : plugin.MapTo(model);
        }

        /// <summary>
        /// Execute a mapping from the plugin descriptor to the model
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to map from</param>
        /// <param name="model">Model to map into; pass null to map to the new model</param>
        /// <returns>Mapped model</returns>
        public static PluginModel ToPluginModel(this PluginDescriptor pluginDescriptor, PluginModel model = null)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            return model == null ? pluginDescriptor.Map<PluginDescriptor, PluginModel>() : pluginDescriptor.MapTo(model);
        }

        #endregion
    }
}