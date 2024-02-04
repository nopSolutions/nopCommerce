using System;
using System.Collections;
using System.Collections.Generic;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Maps;

namespace Nop.Plugin.Api.Delta
{
    public class Delta<TDto> where TDto : class, new()
    {
        private readonly Dictionary<string, object> _changedJsonPropertyNames;

        private readonly IJsonPropertyMapper _jsonPropertyMapper;

        private readonly IMappingHelper _mappingHelper = new MappingHelper();
        private TDto _dto;

        private Dictionary<string, object> _propertyValuePairs;

        public Dictionary<object, object> ObjectPropertyNameValuePairs = new Dictionary<object, object>();

        public Delta(Dictionary<string, object> passedChangedJsonPropertyValuePairs)
        {
            _jsonPropertyMapper = EngineContext.Current.Resolve<IJsonPropertyMapper>();
            _changedJsonPropertyNames = passedChangedJsonPropertyValuePairs;

            _mappingHelper.SetValues(PropertyValuePairs, Dto, typeof(TDto), ObjectPropertyNameValuePairs, true);
        }

        private Dictionary<string, object> PropertyValuePairs =>
            _propertyValuePairs ?? (_propertyValuePairs = GetPropertyValuePairs(typeof(TDto), _changedJsonPropertyNames));

        public TDto Dto => _dto ?? (_dto = new TDto());

        public void Merge<TEntity>(TEntity entity, bool mergeComplexTypeCollections = true)
        {
            _mappingHelper.SetValues(PropertyValuePairs, entity, entity.GetType(), null, mergeComplexTypeCollections);
        }

        public void Merge<TEntity>(object dto, TEntity entity, bool mergeComplexTypeCollections = true)
        {
            if (dto != null && ObjectPropertyNameValuePairs.ContainsKey(dto))
            {
                var propertyValuePairs = ObjectPropertyNameValuePairs[dto] as Dictionary<string, object>;
                _mappingHelper.SetValues(propertyValuePairs, entity, entity.GetType(), null, mergeComplexTypeCollections);
            }
        }

        private Dictionary<string, object> GetPropertyValuePairs(Type type, Dictionary<string, object> changedJsonPropertyNames)
        {
            var propertyValuePairs = new Dictionary<string, object>();

            if (changedJsonPropertyNames == null)
            {
                return propertyValuePairs;
            }

            var typeMap = _jsonPropertyMapper.GetMap(type);

            foreach (var changedProperty in changedJsonPropertyNames)
            {
                var jsonName = changedProperty.Key;

                if (typeMap.ContainsKey(jsonName))
                {
                    var propertyNameAndType = typeMap[jsonName];

                    var propertyName = propertyNameAndType.Item1;
                    var propertyType = propertyNameAndType.Item2;

                    // Handle system types
                    // This is also the recursion base
                    if (propertyType.Namespace == "System")
                    {
                        propertyValuePairs.Add(propertyName, changedProperty.Value);
                    }
                    else if (propertyType.IsEnum) // handle enums
                    {
                        propertyValuePairs.Add(propertyName, changedProperty.Value);
                    }
                    else if (propertyType.GetInterface(typeof(IEnumerable).FullName) != null) // Handle collections
                    {
                        // skip any collections that are passed as null
                        // we can handle only empty collection, which will delete any items if exist
                        // or collections that has some elements which need to be updated/added/deleted.
                        if (changedProperty.Value == null)
                        {
                            continue;
                        }

                        var collection = changedProperty.Value as IEnumerable<object>;
                        var collectionElementsType = propertyType.GetGenericArguments()[0];
                        var resultCollection = new List<object>();

                        foreach (var item in collection)
                        {
                            // Simple types in collection
                            if (collectionElementsType.Namespace == "System")
                            {
                                resultCollection.Add(item);
                            }
                            // Complex types in collection
                            else
                            {
                                // the complex type could be null so we try a defensive cast
                                var itemDictionary =
                                    item as Dictionary<string, object>;

                                resultCollection.Add(GetPropertyValuePairs(collectionElementsType, itemDictionary));
                            }
                        }

                        propertyValuePairs.Add(propertyName, resultCollection);
                    }
                    // Handle nested properties
                    else
                    {
                        // the complex type could be null so we try a defensive cast
                        var changedPropertyValueDictionary =
                            changedProperty.Value as Dictionary<string, object>;

                        var resultedNestedObject = GetPropertyValuePairs(propertyType, changedPropertyValueDictionary);

                        propertyValuePairs.Add(propertyName, resultedNestedObject);
                    }
                }
            }

            return propertyValuePairs;
        }
    }
}
