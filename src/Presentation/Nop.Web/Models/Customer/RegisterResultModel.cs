using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class RegisterResultModel : BaseNopModel
    {
        public string Result { get; set; }
    }
}