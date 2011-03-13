using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.MVC.Areas.Admin.Infrastructure
{
    public class TelerikResourceService : Telerik.Web.Mvc.Infrastructure.ILocalizationService
    {
        public IDictionary<string, string> All()
        {
            throw new NotImplementedException();
        }

        public bool IsDefault
        {
            get { return true; }
        }

        public string One(string key)
        {
            throw new NotImplementedException();
        }
    }
}