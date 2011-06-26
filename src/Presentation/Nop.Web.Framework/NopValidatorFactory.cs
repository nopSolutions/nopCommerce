using System;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Internal;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework
{
    public class NopValidatorFactory : AttributedValidatorFactory
    {
        private readonly InstanceCache _cache = new InstanceCache();

        public override IValidator GetValidator(Type type)
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