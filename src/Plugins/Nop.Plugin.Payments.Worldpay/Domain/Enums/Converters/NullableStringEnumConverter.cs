using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters
{
    /// <summary>
    /// Represents StringEnumConverter that allows to deserialize unknown enum values as null
    /// </summary>
    class NullableStringEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                //try to read JSON as usual
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException exception)
            {
                //if object type is nullable enum, set unknown value as null
                if (Nullable.GetUnderlyingType(objectType)?.IsEnum ?? false)
                    return null;

                throw exception;
            }
        }
    }
}