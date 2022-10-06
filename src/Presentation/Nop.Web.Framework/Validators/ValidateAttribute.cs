using System;
using FluentValidation.AspNetCore;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Represents attribute that used to mark model for the forced validation. 
    /// Without this attribute, the model passed in the parameter will not be validated. It's used to prevent auto-validation of child models.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed partial class ValidateAttribute : CustomizeValidatorAttribute
    {
        public ValidateAttribute()
        {
            //specify rule set
            RuleSet = NopValidationDefaults.ValidationRuleSet;
        }
    }
}