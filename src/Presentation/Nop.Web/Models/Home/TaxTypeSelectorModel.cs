using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Home;

namespace Nop.Web.Models.Home
{
    public class TaxTypeSelectorModel : BaseNopModel
    {
        public bool Enabled { get; set; }

        public TaxDisplayType CurrentTaxType { get; set; }
    }
}