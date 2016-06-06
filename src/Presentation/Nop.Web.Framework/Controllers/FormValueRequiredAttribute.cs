using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Attribute to validate whether a certain form name (or value) was submitted
    /// </summary>
    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly string[] _submitButtonNames;
        private readonly FormValueRequirement _requirement;
        private readonly bool _validateNameOnly;

        public FormValueRequiredAttribute(params string[] submitButtonNames):
            this(FormValueRequirement.Equal, submitButtonNames)
        {
        }
        public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames):
            this(requirement, true, submitButtonNames)
        {
        }
        public FormValueRequiredAttribute(FormValueRequirement requirement, bool validateNameOnly, params string[] submitButtonNames)
        {
            //at least one submit button should be found
            this._submitButtonNames = submitButtonNames;
            this._validateNameOnly = validateNameOnly;
            this._requirement = requirement;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (string buttonName in _submitButtonNames)
            {
                try
                {
                    switch (this._requirement)
                    {
                        case FormValueRequirement.Equal:
                            {
                                if (_validateNameOnly)
                                {
                                    //"name" only
                                    if (controllerContext.HttpContext.Request.Form.AllKeys.Any(x => x.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    //do not iterate because "Invalid request" exception can be thrown
                                    string value = controllerContext.HttpContext.Request.Form[buttonName];
                                    if (!String.IsNullOrEmpty(value))
                                        return true;
                                }
                            }
                            break;
                        case FormValueRequirement.StartsWith:
                            {
                                if (_validateNameOnly)
                                {
                                    //"name" only
                                    if (controllerContext.HttpContext.Request.Form.AllKeys.Any(x => x.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    foreach (var formValue in controllerContext.HttpContext.Request.Form.AllKeys)
                                        if (formValue.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase))
                                        { 
                                            var value = controllerContext.HttpContext.Request.Form[formValue];
                                            if (!String.IsNullOrEmpty(value))
                                                return true;
                                        }
                                }
                            }
                            break;
                    }
                }
                catch (Exception exc)
                {
                    //try-catch to ensure that no exception is throw
                    Debug.WriteLine(exc.Message);
                }
            }
            return false;
        }
    }

    public enum FormValueRequirement
    {
        Equal,
        StartsWith
    }
}
