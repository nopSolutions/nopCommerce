using System.Linq;
using System.Linq.Dynamic;
using FluentValidation;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Base class for validators
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseNopValidator<T> : AbstractValidator<T> where T : class
    {
        /// <summary>
        /// Ctor
        /// </summary>
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
        /// Sets validation rule(s) to appropriate database model
        /// </summary>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="filterStringPropertyNames">Properties to skip</param>
        protected virtual void SetDatabaseValidationRules<TObject>(IDbContext dbContext, params string[] filterStringPropertyNames)
        {
            SetStringPropertiesMaxLength<TObject>(dbContext, filterStringPropertyNames);
            SetDecimalMaxValue<TObject>(dbContext);
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength<TObject>(IDbContext dbContext, params string[] filterPropertyNames)
        {
            if (dbContext == null)
                return;

            var dbObjectType = typeof(TObject);

            var names = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && !filterPropertyNames.Contains(p.Name))
                .Select(p => p.Name).ToArray();

            var maxLength = dbContext.GetColumnsMaxLength(dbObjectType.Name, names);
            var expression = maxLength.Keys.ToDictionary(name => name, name => DynamicExpression.ParseLambda<T, string>(name, null));

            foreach (var expr in expression)
            {
                RuleFor(expr.Value).Length(0, maxLength[expr.Key]);
            }
        }

        /// <summary>
        /// Sets max value validation rule(s) to decimal properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="dbContext">Database context</param>
        protected virtual void SetDecimalMaxValue<TObject>(IDbContext dbContext)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            if (dbContext == null)
                return;

            var dbObjectType = typeof(TObject);

            var names = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(decimal))
                .Select(p => p.Name).ToArray();

            var maxValues = dbContext.GetDecimalMaxValue(dbObjectType.Name, names);
            var expression = maxValues.Keys.ToDictionary(name => name, name => DynamicExpression.ParseLambda<T, decimal>(name, null));

            foreach (var expr in expression)
            {
                RuleFor(expr.Value).IsDecimal(maxValues[expr.Key]).WithMessage(string.Format(localizationService.GetResource("Nop.Web.Framework.Validators.MaxDecimal"), maxValues[expr.Key] - 1));
            }
        }
    }
}