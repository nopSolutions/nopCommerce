using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IUserAdvertChannelAnalysisService
    {
        void InsertEntity(UserAdvertChannelAnalysis entity);

        void DeleteEntity(UserAdvertChannelAnalysis entity);

        void DeleteEntities(IList<UserAdvertChannelAnalysis> entities);

        void UpdateEntity(UserAdvertChannelAnalysis entity);

        void UpdateEntities(IList<UserAdvertChannelAnalysis> entities);

        UserAdvertChannelAnalysis GetEntityById(int id);

        UserAdvertChannelAnalysis GetEntitiesByUserId(int wuserId);

        List<UserAdvertChannelAnalysis> GetEntitiesByProductId(int productId);

        List<UserAdvertChannelAnalysis> GetEntitiesBySupplierId(int supplierId, int supplierShopId);

        
        IPagedList<UserAdvertChannelAnalysis> GetEntities(
            int userId = 0,
            int supplierId = 0,
            int supplierShopId = 0,
            int productId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

    }
}