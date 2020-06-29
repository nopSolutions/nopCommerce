using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IMarketingAdvertAddressService
    {
        void InsertEntity(MarketingAdvertAddress entity);

        void DeleteEntity(MarketingAdvertAddress entity, bool delete = false);

        void DeleteEntities(IList<MarketingAdvertAddress> entities, bool deleted = false);

        void UpdateEntity(MarketingAdvertAddress entity);

        void UpdateEntities(IList<MarketingAdvertAddress> entities);

        MarketingAdvertAddress GetEntityById(int id);

        MarketingAdvertAddress GetEntityByAddress(string address);

        string GetAddressById(int id);

        IPagedList<MarketingAdvertAddress> GetEntities(
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}