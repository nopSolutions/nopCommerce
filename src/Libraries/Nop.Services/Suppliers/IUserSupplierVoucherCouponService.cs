using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IUserSupplierVoucherCouponService
    {
        void InsertEntity(UserSupplierVoucherCoupon entity);

        void DeleteEntity(UserSupplierVoucherCoupon entity);

        void DeleteEntities(IList<UserSupplierVoucherCoupon> entities);

        void UpdateEntity(UserSupplierVoucherCoupon entity);

    }
}