using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc.Infrastructure;

namespace Nop.Web.MVC.Areas.Admin.Infrastructure
{
    public class TelerikLocalizationServiceFactory : ILocalizationServiceFactory
    {
        public ILocalizationService Create(string resourceName, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}