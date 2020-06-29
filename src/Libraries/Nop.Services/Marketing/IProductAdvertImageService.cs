using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductActivitiesThemeMappingService
    {
        void InsertEntity(ProductActivitiesThemeMapping entity);

        void DeleteEntity(ProductActivitiesThemeMapping entity);

        void DeleteEntities(IList<ProductActivitiesThemeMapping> entities);

        void UpdateEntity(ProductActivitiesThemeMapping entity);

        void UpdateEntities(IList<ProductActivitiesThemeMapping> entities);

        ProductActivitiesThemeMapping GetEntityById(int productId, int activitiesThemeId);

        List<ProductActivitiesThemeMapping> GetEntitiesByProductId(int productId);

        List<ProductActivitiesThemeMapping> GetEntitiesByActivitiesThemeId(int activitiesThemeId);
        
        IPagedList<ProductActivitiesThemeMapping> GetEntities(
            int productId = 0,
            int activitiesThemeId = 0,
            bool? published = null,
            int pageIndex = 0, int pageSize = int.MaxValue);


    }
}