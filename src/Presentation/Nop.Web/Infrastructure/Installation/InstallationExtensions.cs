using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Infrastructure.Installation {
    public static class InstallationExtensions {
        public static SelectList ToInstallSelectList<TEnum>(this TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct {

            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            //var localizationService = EngineContext.Current.Resolve<IInstallationLocalizationService>();
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = CommonHelper.ConvertEnum(enumValue.ToString()) };
            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);

        }
    }
}