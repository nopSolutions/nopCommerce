using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using Nop.Core.Infrastructure;

namespace Nop.Web.MVC.Infrastructure
{
    public class NopValidatorFactory : AttributedValidatorFactory
    {
        public override FluentValidation.IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                ValidatorAttribute attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if ((attribute != null) && (attribute.ValidatorType != null))
                {
                    return EngineContext.Current.ContainerManager.ResolveUnregistered(attribute.ValidatorType) as IValidator;
                }
            }
            return null;

        }
    }
}