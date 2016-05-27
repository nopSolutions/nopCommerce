using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// If form name exists, then specified "actionParameterName" will be set to "true"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)] 
    public class ParameterBasedOnFormNameAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _name;
        private readonly string _actionParameterName;

        public ParameterBasedOnFormNameAttribute(string name, string actionParameterName)
        {
            this._name = name;
            this._actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //we check "name" only. uncomment the code below if you want to check whether "value" attribute is specified
            //var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name];
            //filterContext.ActionParameters[_actionParameterName] = !string.IsNullOrEmpty(formValue);
            filterContext.ActionParameters[_actionParameterName] = filterContext.RequestContext
                .HttpContext.Request.Form.AllKeys.Any(x => x.Equals(_name));
        }
    }
}
