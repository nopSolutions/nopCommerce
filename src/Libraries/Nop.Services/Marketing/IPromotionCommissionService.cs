using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IPromotionCommissionService
    {
        void InsertEntity(PromotionCommission entity);

        void DeleteEntity(PromotionCommission entity, bool delete = false);

        void DeleteEntities(IList<PromotionCommission> entities, bool deleted = false);

        void UpdateEntity(PromotionCommission entity);

        void UpdateEntities(IList<PromotionCommission> entities);

        PromotionCommission GetEntityById(int id);

        PromotionCommission GetEntitiesByProductId(int productId, int productAttributeValueId = 0);

        List<PromotionCommission> GetEntitiesByIds(int[] entityIds);

        IPagedList<PromotionCommission> GetEntities(
            int productId = 0,
            int productAttributeValueId = 0,
            bool? usePercentage = null,
            DateTime? startDateTimeUtc = null,
            DateTime? endDateTimeUtc = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

    }
}