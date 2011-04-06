using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Nop.Web.Framework.Controllers
{
    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _submitButtonName;

        public FormValueRequiredAttribute(string submitButtonName)
        {
            _submitButtonName = submitButtonName;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var value = controllerContext.HttpContext.Request.Form[_submitButtonName];

            return !string.IsNullOrEmpty(value);

        }
    }
}
