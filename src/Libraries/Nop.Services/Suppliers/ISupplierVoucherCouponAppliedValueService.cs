using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierVoucherCouponAppliedValueService
    {
        void InsertEntity(SupplierVoucherCouponAppliedValue entity);

        void DeleteEntity(SupplierVoucherCouponAppliedValue entity);

        void DeleteEntities(IList<SupplierVoucherCouponAppliedValue> entities);

        void UpdateEntity(SupplierVoucherCouponAppliedValue entity);

        void UpdateEntities(IList<SupplierVoucherCouponAppliedValue> entities);

        SupplierVoucherCouponAppliedValue GetEntityById(int id);
    }
}