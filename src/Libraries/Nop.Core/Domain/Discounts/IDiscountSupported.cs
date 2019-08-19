using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents an entity which supports discounts
    /// </summary>
    public partial interface IDiscountSupported<T> where T : DiscountMapping
    {
        
    }
}