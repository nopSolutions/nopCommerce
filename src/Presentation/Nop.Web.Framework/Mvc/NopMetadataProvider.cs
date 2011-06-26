using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;

namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// This MetadataProvider adds some functionality on top of the default DataAnnotationsModelMetadataProvider.
    /// It adds custom attributes (implementing IModelAttribute) to the AdditionalValues property of the model's metadata
    /// so that it can be retrieved later.
    /// </summary>
    public class NopMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            var additionalValues = attributes.OfType<IModelAttribute>().ToList();
            foreach (var additionalValue in additionalValues)
            {
                if (metadata.AdditionalValues.ContainsKey(additionalValue.Name))
                    throw new NopException("There is already an attribute with the name of \"" + additionalValue.Name +
                                           "\" on this model.");
                metadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
            }
            return metadata;
        }
    }
}