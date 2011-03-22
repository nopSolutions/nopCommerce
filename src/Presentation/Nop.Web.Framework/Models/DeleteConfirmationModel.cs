using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework;

namespace Nop.Web.Framework.Models
{
    public class DeleteConfirmationModel : BaseNopEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}