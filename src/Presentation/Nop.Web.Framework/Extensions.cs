using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        public static IEnumerable<T> PagedForCommand<T>(this IEnumerable<T> current, DataSourceRequest command)
        {
            return current.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, 
            bool markCurrentAsSelected = true, int[] valuesToExclude = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = enumValue.GetLocalizedEnum(localizationService, workContext) };
            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source)
        {
            return RelativeFormat(source, string.Empty);
        }
        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source, string defaultFormat)
        {
            return RelativeFormat(source, false, defaultFormat);
        }
        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="convertToUserTime">A value indicating whether we should convet DateTime instance to user local time (in case relative formatting is not applied)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source,
            bool convertToUserTime, string defaultFormat)
        {
            string result = "";

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            double delta = ts.TotalSeconds;

            if (delta > 0)
            {
                if (delta < 60) // 60 (seconds)
                {
                    result = ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
                }
                else if (delta < 120) //2 (minutes) * 60 (seconds)
                {
                    result = "a minute ago";
                }
                else if (delta < 2700) // 45 (minutes) * 60 (seconds)
                {
                    result = ts.Minutes + " minutes ago";
                }
                else if (delta < 5400) // 90 (minutes) * 60 (seconds)
                {
                    result = "an hour ago";
                }
                else if (delta < 86400) // 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int hours = ts.Hours;
                    if (hours == 1)
                        hours = 2;
                    result = hours + " hours ago";
                }
                else if (delta < 172800) // 48 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = "yesterday";
                }
                else if (delta < 2592000) // 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = ts.Days + " days ago";
                }
                else if (delta < 31104000) // 12 (months) * 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    result = months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    result = years <= 1 ? "one year ago" : years + " years ago";
                }
            }
            else
            {
                DateTime tmp1 = source;
                if (convertToUserTime)
                {
                    tmp1 = EngineContext.Current.Resolve<IDateTimeHelper>().ConvertToUserTime(tmp1, DateTimeKind.Utc);
                }
                //default formatting
                if (!String.IsNullOrEmpty(defaultFormat))
                {
                    result = tmp1.ToString(defaultFormat);
                }
                else
                {
                    result = tmp1.ToString();
                }
            }
            return result;
        }
    }
}
