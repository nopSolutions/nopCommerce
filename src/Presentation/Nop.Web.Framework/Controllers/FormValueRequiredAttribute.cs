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
        private readonly string[] _submitButtonNames;

        public FormValueRequiredAttribute(params string[] submitButtonNames)
        {
            //at least one submit button should be found
            this._submitButtonNames = submitButtonNames;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (string buttonName in _submitButtonNames)
            {
                var value = controllerContext.HttpContext.Request.Form[buttonName];
                if (!String.IsNullOrEmpty(value))
                    return true;
            }
            return false;
        }
    }
}
