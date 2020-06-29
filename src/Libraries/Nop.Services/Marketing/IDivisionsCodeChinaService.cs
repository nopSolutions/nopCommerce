using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IDivisionsCodeChinaService
    {
        void InsertEntity(DivisionsCodeChina entity);

        void DeleteEntity(DivisionsCodeChina entity);

        void DeleteEntities(IList<DivisionsCodeChina> entities);

        void UpdateEntity(DivisionsCodeChina entity);

        void UpdateEntities(IList<DivisionsCodeChina> entities);

        DivisionsCodeChina GetEntityById(int id);

        DivisionsCodeChina GetEntityByAreaCode(string areaCode);

        List<DivisionsCodeChina> GetEntitiesByAreaCode(string areaCode, int areaLevel); //通过areaLevel，切分不同长度的areaCode进行contain查找

        List<DivisionsCodeChina> GetEntitiesByProductId(int productId, int top = 1, bool asc = true);

        List<DivisionsCodeChina> GetEntitiesBySupplierVoucherCouponId(int supplierVoucherCouponId, int top = 1, bool asc = true);

        
        IPagedList<DivisionsCodeChina> GetEntities(
            string areaCode = "",
            string areaName = "",
            int areaLevel = 0,
            bool? published = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}