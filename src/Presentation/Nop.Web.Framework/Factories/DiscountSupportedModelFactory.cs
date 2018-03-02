using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents the base discount supported model factory implementation
    /// </summary>
    public partial class DiscountSupportedModelFactory : IDiscountSupportedModelFactory
    {
        #region Methods

        /// <summary>
        /// Prepare selected and all available discounts for the passed model
        /// </summary>
        /// <typeparam name="TModel">Discount supported model type</typeparam>
        /// <typeparam name="TEntity">Discount supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="availableDiscounts">List of all available discounts</param>
        /// <param name="ignoreAppliedDiscounts">Whether to ignore existing applied discounts</param>
        public virtual void PrepareDiscountModel<TModel, TEntity>(TModel model, TEntity entity, 
            IList<Discount> availableDiscounts, bool ignoreAppliedDiscounts)
            where TModel : IDiscountSupportedModel where TEntity : BaseEntity, IDiscountSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //try to get already applied discounts
            if (!ignoreAppliedDiscounts && entity != null)
                model.SelectedDiscountIds = entity.AppliedDiscounts.Select(discount => discount.Id).ToList();

            //prepare available discounts
            model.AvailableDiscounts = availableDiscounts.Select(discount => new SelectListItem
            {
                Text = discount.Name,
                Value = discount.Id.ToString(),
                Selected = model.SelectedDiscountIds.Contains(discount.Id)
            }).ToList();
        }

        #endregion
    }
}