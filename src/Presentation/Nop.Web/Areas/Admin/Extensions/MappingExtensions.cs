using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Extensions
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

        public static void PrepareCustomAddressAttributes(this AddressModel model,
            Address address,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser)
        {
            //this method is very similar to the same one in Nop.Web project
            if (addressAttributeService == null)
                throw new ArgumentNullException(nameof(addressAttributeService));

            if (addressAttributeParser == null)
                throw new ArgumentNullException(nameof(addressAttributeParser));

            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressModel.AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressModel.AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address != null ? address.CustomAttributes : null;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }
    }
}