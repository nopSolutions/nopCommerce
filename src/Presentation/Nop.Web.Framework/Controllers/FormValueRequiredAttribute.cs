using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="submitButtonNames">Submit button names</param>
        public FormValueRequiredAttribute(params string[] submitButtonNames):
            this(FormValueRequirement.Equal, submitButtonNames)
        {
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requirement">Requirement</param>
        /// <param name="submitButtonNames">Submit button names</param>
        public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames):
            this(requirement, true, submitButtonNames)
        {
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requirement">Requirement</param>
        /// <param name="validateNameOnly">A value indicating whether we should check "name" attribute only (not "value")</param>
        /// <param name="submitButtonNames">Submit button names</param>
        public FormValueRequiredAttribute(FormValueRequirement requirement, bool validateNameOnly, params string[] submitButtonNames)
        {
            //at least one submit button should be found
            _submitButtonNames = submitButtonNames;
            _validateNameOnly = validateNameOnly;
            _requirement = requirement;
        }


        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="routeContext">Route context</param>
        /// <param name="action">Action descriptor</param>
        /// <returns>Result</returns>
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (routeContext.HttpContext.Request.Method != WebRequestMethods.Http.Post)
                return false;

            foreach (var buttonName in _submitButtonNames)
            {
                try
                {
                    switch (_requirement)
                    {
                        case FormValueRequirement.Equal:
                            {
                                if (_validateNameOnly)
                                {
                                    //"name" only
                                    if (routeContext.HttpContext.Request.Form.Keys.Any(x => x.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    //do not iterate because "Invalid request" exception can be thrown
                                    string value = routeContext.HttpContext.Request.Form[buttonName];
                                    if (!string.IsNullOrEmpty(value))
                                        return true;
                                }
                            }
                            break;
                        case FormValueRequirement.StartsWith:
                            {
                                if (_validateNameOnly)
                                {
                                    //"name" only
                                    if (routeContext.HttpContext.Request.Form.Keys.Any(x => x.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    foreach (var formValue in routeContext.HttpContext.Request.Form.Keys)
                                        if (formValue.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase))
                                        { 
                                            var value = routeContext.HttpContext.Request.Form[formValue];
                                            if (!string.IsNullOrEmpty(value))
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

    /// <summary>
    /// Requirement
    /// </summary>
    public enum FormValueRequirement
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal,
        /// <summary>
        /// Starts with
        /// </summary>
        StartsWith
    }
}