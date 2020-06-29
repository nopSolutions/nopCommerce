using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductSupplierVoucherCouponMappingService
    {
        void InsertEntity(ProductSupplierVoucherCouponMapping entity);

        void DeleteEntity(ProductSupplierVoucherCouponMapping entity);

        void DeleteEntities(IList<ProductSupplierVoucherCouponMapping> entities);

        void UpdateEntity(ProductSupplierVoucherCouponMapping entity);

        void UpdateEntities(IList<ProductSupplierVoucherCouponMapping> entities);

        ProductSupplierVoucherCouponMapping GetEntityById(int id);


        List<ProductSupplierVoucherCouponMapping> GetEntitiesByIds(int[] entityIds);

        IPagedList<ProductSupplierVoucherCouponMapping> GetEntities(
            int productId=0,
            int? productAttributeValueId=0,
            int supplierVoucherCouponId=0,
            int? customerRoleId=0,
            int storeId=0,
            DateTime? startDateTimeUtc=null,
            DateTime? endDateTimeUtc=null,
            int pageIndex = 0, int pageSize = int.MaxValue);

    }
}