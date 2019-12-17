using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Attributes
{
    public abstract class BaseValidationAttribute : Attribute
    {
        public abstract void Validate(object instance);
        public  abstract Dictionary<string, string> GetErrors();
    }
}