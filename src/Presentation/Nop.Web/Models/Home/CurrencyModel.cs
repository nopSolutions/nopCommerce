using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Home
{
    public class CurrencyModel : BaseNopEntityModel
    {
        public string Name { get; set; }
    }
}