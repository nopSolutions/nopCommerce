using System;
using System.Collections.Generic;
using Humanizer;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductUserFollowMappingService
    {
        void InsertEntity(ProductUserFollowMapping entity);

        void DeleteEntity(ProductUserFollowMapping entity);

        void DeleteEntities(IList<ProductUserFollowMapping> entities);

        void UpdateEntity(ProductUserFollowMapping entity);

        void UpdateEntities(IList<ProductUserFollowMapping> entities);

        ProductUserFollowMapping GetEntityById(int id);

        List<ProductUserFollowMapping> GetEntitiesByIds(int[] entityIds);

        List<ProductUserFollowMapping> GetEntitiesByProductId(int productId);

        List<ProductUserFollowMapping> GetEntitiesByUserId(int userId);

        int GetUserFollowCount(int productId);

        List<int> GetFollowUserIdByProductId(int productId);

        List<int> GetFollowProductIdByUserId(int wuserId);

        IPagedList<ProductUserFollowMapping> GetEntities(
            int productId = 0,
            int userId = 0,
            bool? subscribe = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}