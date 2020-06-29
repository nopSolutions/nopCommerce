using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductAdvertImageService
    {
        void InsertEntity(ProductAdvertImage entity);

        void DeleteEntity(ProductAdvertImage entity, bool delete = false);

        void DeleteEntities(IList<ProductAdvertImage> entities, bool deleted = false);

        void UpdateEntity(ProductAdvertImage entity);

        void UpdateEntities(IList<ProductAdvertImage> entities);

        ProductAdvertImage GetEntityById(int id);

        List<ProductAdvertImage> GetEntitiesByIds(int[] entityIds);

        List<ProductAdvertImage> GetEntitiesByProductId(int productId, int top = 1, bool asc = true);

        List<ProductAdvertImage> GetEntitiesBySupplierVoucherCouponId(int supplierVoucherCouponId, int top = 1, bool asc = true);

        
        IPagedList<ProductAdvertImage> GetEntities(
            string title = "",
            int productId = 0,
            bool? isDiscountAdver = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}