using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class StoreThemeModel : BaseNopModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}