using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IPartnerServiceInfoService
    {
        void InsertEntity(PartnerServiceInfo entity);

        void DeleteEntity(PartnerServiceInfo entity, bool delete = false);

        void DeleteEntities(IList<PartnerServiceInfo> entities, bool deleted = false);

        void UpdateEntity(PartnerServiceInfo entity);

        void UpdateEntities(IList<PartnerServiceInfo> entities);

        PartnerServiceInfo GetEntityById(int id);

        PartnerServiceInfo GetEntityByUserId(int wuserId);

        List<PartnerServiceInfo> GetEntitiesByIds(int[] entityIds);
     
        IPagedList<PartnerServiceInfo> GetEntities(
            int userId = 0,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}