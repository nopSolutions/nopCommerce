using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseNopEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}