using System.Linq;
using FluentValidation;
using Nop.Core;
using Nop.Data;

namespace Nop.Web.Framework.Validators
{
    public abstract class BaseNopValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseNopValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength<TObject>(IDbContext dbContext, params string[] filterPropertyNames)
        {
            if(dbContext==null)
                return;

            var dbObjectType = typeof(TObject);

            var names = typeof (T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && !filterPropertyNames.Contains(p.Name))
                .Select(p => p.Name).ToArray();

            var maxLength = dbContext.GetColumnsMaxLength(dbObjectType.Name, names);
            var expression = maxLength.Keys.ToDictionary(name => name, name => DynamicExpression.ParseLambda<T, string>(name, null));

            foreach (var expr in expression)
            {
                RuleFor(expr.Value).Length(0, maxLength[expr.Key]);
            }
        }
    }
}