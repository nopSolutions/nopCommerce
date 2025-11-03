using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Api.Converters
{
    public class ApiTypeConverter : IApiTypeConverter
    {
        /// <summary>
        ///     Converts the value, which should be in ISO 8601 format to UTC time or null if not valid
        /// </summary>
        /// <param name="value">The time format in ISO 8601. If no timezone or offset specified we assume it is in UTC</param>
        /// <returns>The time in UTC or null if the time is not valid</returns>
        public DateTime? ToUtcDateTimeNullable(string value)
        {
            DateTime result;

            var formats = new[]
                          {
                              "yyyy", "yyyy-MM", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"
                          };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
            {
                // only if parsed in Local time then we need to convert it to UTC
                if (result.Kind == DateTimeKind.Local)
                {
                    return result.ToUniversalTime();
                }

                return result;
            }

            return null;
        }

        public int ToInt(string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        public int? ToIntNullable(string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public IList<int> ToListOfInts(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var stringIds = value.Split(new[]
                                            {
                                                ','
                                            }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var intIds = new List<int>();

                foreach (var id in stringIds)
                {
                    int intId;
                    if (int.TryParse(id, out intId))
                    {
                        intIds.Add(intId);
                    }
                }

                intIds = intIds.Distinct().ToList();
                return intIds.Count > 0
                           ? intIds
                           : null;
            }

            return null;
        }

        public IList<string> ToListOfStrings(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var stringValues = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return stringValues;
            }
            return null;
        }

        public bool? ToBoolean(string value)
        {
            if (bool.TryParse(value, out bool result)) // should be case-insensitive
            {
                return result;
            }
            return null;
        }

        public object ToEnumNullable(string value, Type type)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var enumType = Nullable.GetUnderlyingType(type);

                var enumNames = enumType.GetEnumNames();

                if (enumNames.Any(x => x.ToLowerInvariant().Equals(value.ToLowerInvariant())))
                {
                    return Enum.Parse(enumType, value, true);
                }
            }

            return null;
        }
    }
}
