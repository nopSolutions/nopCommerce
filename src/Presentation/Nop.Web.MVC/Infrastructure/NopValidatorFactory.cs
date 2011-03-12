using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Internal;
using Nop.Core.Infrastructure;

namespace Nop.Web.MVC.Infrastructure
{
    public class NopValidatorFactory : AttributedValidatorFactory
    {
        private readonly InstanceCache _cache = new InstanceCache();

        public override FluentValidation.IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if ((attribute != null) && (attribute.ValidatorType != null))
                {
                    var instance = _cache.GetOrCreateInstance(attribute.ValidatorType,
                                               x =>
                                               EngineContext.Current.ContainerManager.ResolveUnregistered(x));
                    return instance as IValidator;
                }
            }
            return null;

        }
    }
}