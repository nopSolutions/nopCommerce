using System.Linq.Dynamic.Core;
using FluentValidation;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Base class for validators
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    public abstract partial class BaseNopValidator<TModel> : AbstractValidator<TModel> where TModel : class
    {
        #region Ctor

        protected BaseNopValidator()
        {
            PostInitialize();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
        }

        /// <summary>
        /// Sets validation rule(s) to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="mappingEntityAccessor">An object implements accessor methods to mapped entities</param>
        /// <param name="filterStringPropertyNames">Properties to skip</param>
        protected virtual void SetDatabaseValidationRules<TEntity>(IMappingEntityAccessor mappingEntityAccessor, params string[] filterStringPropertyNames)
            where TEntity : BaseEntity
        {
            if (mappingEntityAccessor is null)
                throw new ArgumentNullException(nameof(mappingEntityAccessor));

            var entityDescriptor = mappingEntityAccessor.GetEntityDescriptor(typeof(TEntity));

            SetStringPropertiesMaxLength(entityDescriptor, filterStringPropertyNames);
            SetDecimalMaxValue(entityDescriptor);
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <param name="entityDescriptor">Entity descriptor</param>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength(NopEntityDescriptor entityDescriptor, params string[] filterPropertyNames)
        {
            if (entityDescriptor is null)
                return;

            //filter model properties for which need to get max lengths
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(string) && !filterPropertyNames.Contains(property.Name))
                .Select(property => property.Name).ToList();

            //get max length of these properties
            var columnsMaxLengths = entityDescriptor.Fields.Where(field =>
                modelPropertyNames.Contains(field.Name) && field.Type == typeof(string) && field.Size.HasValue);

            //create expressions for the validation rules
            var maxLengthExpressions = columnsMaxLengths.Select(field => new
            {
                MaxLength = field.Size.Value,
                // We must using identifiers of the form @SomeName to avoid problems with parsing fields that match reserved words https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions#substitution-values
                Expression = DynamicExpressionParser.ParseLambda<TModel, string>(null, false, "@" + field.Name)
            }).ToList();

            //define string length validation rules
            foreach (var expression in maxLengthExpressions)
            {
                RuleFor(expression.Expression).Length(0, expression.MaxLength);
            }
        }

        /// <summary>
        /// Sets max value validation rule(s) to decimal properties according to appropriate database model
        /// </summary>
        /// <param name="entityDescriptor">Entity descriptor</param>
        protected virtual void SetDecimalMaxValue(NopEntityDescriptor entityDescriptor)
        {
            if (entityDescriptor is null)
                return;

            //filter model properties for which need to get max values
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(decimal))
                .Select(property => property.Name).ToList();

            //get max values of these properties
            var decimalColumnsMaxValues = entityDescriptor.Fields.Where(field =>
                modelPropertyNames.Contains(field.Name) &&
                field.Type == typeof(decimal) && field.Size.HasValue && field.Precision.HasValue);

            if (!decimalColumnsMaxValues.Any())
                return;

            //create expressions for the validation rules
            var maxValueExpressions = decimalColumnsMaxValues.Select(field => new
            {
                MaxValue = (decimal)Math.Pow(10, field.Size.Value - field.Precision.Value),
                Expression = DynamicExpressionParser.ParseLambda<TModel, decimal>(null, false, field.Name)
            }).ToList();

            //define decimal validation rules
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            foreach (var expression in maxValueExpressions)
            {
                RuleFor(expression.Expression).IsDecimal(expression.MaxValue)
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.Validation.Decimal.Max"), expression.MaxValue - 1);
            }
        }

        #endregion
    }
}