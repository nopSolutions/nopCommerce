using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IMarketingAdvertWayService
    {
        void InsertEntity(MarketingAdvertWay entity);

        void DeleteEntity(MarketingAdvertWay entity, bool delete = false);

        void DeleteEntities(IList<MarketingAdvertWay> entities, bool deleted = false);

        void UpdateEntity(MarketingAdvertWay entity);

        void UpdateEntities(IList<MarketingAdvertWay> entities);

        MarketingAdvertWay GetEntityById(int id);

        string GetEntinyNameById(int id);

        List<MarketingAdvertWay> GetEntitiesByIds(int[] entityIds);
        
        IPagedList<MarketingAdvertWay> GetEntities(
            string name = "",
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}