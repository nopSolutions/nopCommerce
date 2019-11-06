using FluentValidation;
using Nop.Core;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Base class for validators
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    public abstract class BaseNopValidator<TModel> : AbstractValidator<TModel> where TModel : class
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
        /// <param name="filterStringPropertyNames">Properties to skip</param>
        protected virtual void SetDatabaseValidationRules<TEntity>(params string[] filterStringPropertyNames)
            where TEntity : BaseEntity
        {
            SetStringPropertiesMaxLength<TEntity>(filterStringPropertyNames);
            SetDecimalMaxValue<TEntity>();
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength<TEntity>(params string[] filterPropertyNames)
            where TEntity : BaseEntity
        {
            //TODO: 239 try to implement
        }

        /// <summary>
        /// Sets max value validation rule(s) to decimal properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        protected virtual void SetDecimalMaxValue<TEntity>() where TEntity : BaseEntity
        {
            //TODO: 239 try to implement
        }

        #endregion
    }
}