using System;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions
{
    /// <summary>
    /// Represents the extensions to map entity to model and vise versa
    /// </summary>
    public static class MappingExtensions
    {
        #region Utilities

        /// <summary>
        /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object
        /// </summary>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        private static TDestination Map<TDestination>(this object source)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }
        
        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>Mapped destination object, same instance as the passed destination object</returns>
        private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        
        #endregion

        #region Methods

        #region Model-Entity mapping

        /// <summary>
        /// Execute a mapping from the entity to a new model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="entity">Entity to map from</param>
        /// <returns>Mapped model</returns>
        public static TModel ToModel<TModel>(this BaseEntity entity) where TModel : BaseNopEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.Map<TModel>();
        }

        /// <summary>
        /// Execute a mapping from the entity to the existing model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="entity">Entity to map from</param>
        /// <param name="model">Model to map into</param>
        /// <returns>Mapped model</returns>
        public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model) 
            where TEntity : BaseEntity where TModel : BaseNopEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return entity.MapTo(model);
        }

        /// <summary>
        /// Execute a mapping from the model to a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="model">Model to map from</param>
        /// <returns>Mapped entity</returns>
        public static TEntity ToEntity<TEntity>(this BaseNopEntityModel model) where TEntity : BaseEntity
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model.Map<TEntity>();
        }

        /// <summary>
        /// Execute a mapping from the model to the existing entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model to map from</param>
        /// <param name="entity">Entity to map into</param>
        /// <returns>Mapped entity</returns>
        public static TEntity ToEntity<TEntity, TModel>(this TModel model, TEntity entity)
            where TEntity : BaseEntity where TModel : BaseNopEntityModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return model.MapTo(entity);
        }

        #endregion

        #region Model-Settings mapping
        
        /// <summary>
        /// Execute a mapping from the settings to a new settings model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="settings">Settings to map from</param>
        /// <returns>Mapped model</returns>
        public static TModel ToSettingsModel<TModel>(this ISettings settings) where TModel : BaseNopModel, ISettingsModel
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return settings.Map<TModel>();
        }

        /// <summary>
        /// Execute a mapping from the model to the existing settings
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model to map from</param>
        /// <param name="settings">Settings to map into</param>
        /// <returns>Mapped settings</returns>
        public static TSettings ToSettings<TSettings, TModel>(this TModel model, TSettings settings)
            where TSettings : class, ISettings where TModel : BaseNopModel, ISettingsModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return model.MapTo(settings);
        }

        #endregion

        #region Model-Plugin mapping

        /// <summary>
        /// Execute a mapping from the plugin to a new plugin model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="plugin">Plugin to map from</param>
        /// <returns>Mapped model</returns>
        public static TModel ToPluginModel<TModel>(this IPlugin plugin) where TModel : BaseNopModel, IPluginModel
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            return plugin.Map<TModel>();
        }

        /// <summary>
        /// Execute a mapping from the plugin descriptor to the plugin model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="pluginDescriptor">Plugin descriptor to map from</param>
        /// <param name="model">Model to map into; pass null to map to the new model</param>
        /// <returns>Mapped model</returns>
        public static TModel ToPluginModel<TModel>(this PluginDescriptor pluginDescriptor, TModel model = null) 
            where TModel : BaseNopModel, IPluginModel
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            return model == null ? pluginDescriptor.Map<TModel>() : pluginDescriptor.MapTo(model);
        }

        #endregion

        #endregion
    }
}