using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nop.Plugin.Api.Validators
{
    public class FieldsValidator : IFieldsValidator
    {
        private static IEnumerable<string> GetPropertiesIntoList(string fields)
        {
            var properties = fields.ToLowerInvariant()
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }

        public Dictionary<string, bool> GetValidFields(string fields, Type type)
        {
            // This check ensures that the fields won't be null, because it can couse exception.
            fields = fields ?? string.Empty;
            // This is needed in case you pass the fields as you see them in the json representation of the objects.
            // By specification if the property consists of several words, each word should be separetate from the others with underscore.
            fields = fields.Replace("_", string.Empty);

            var validFields = new Dictionary<string, bool>();
            var fieldsAsList = GetPropertiesIntoList(fields); 
            
            foreach (var field in fieldsAsList)
            {
                var propertyExists = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;

                if (propertyExists)
                {
                    validFields.Add(field, true);
                }
            }

            return validFields;
        }
    }
}