using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        void SetValues(
            Dictionary<string, object> propertyNameValuePairs, object objectToBeUpdated, Type objectToBeUpdatedType,
            Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections = false);

        void Merge(object source, object destination);
    }
}
