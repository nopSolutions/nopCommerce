using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Converters
{
    public interface IApiTypeConverter
    {
        DateTime? ToUtcDateTimeNullable(string value);
        int ToInt(string value);
        int? ToIntNullable(string value);
        IList<int> ToListOfInts(string value);
        bool? ToStatus(string value);
        object ToEnumNullable(string value, Type type);
    }
}