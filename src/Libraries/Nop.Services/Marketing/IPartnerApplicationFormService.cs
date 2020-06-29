using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IPartnerApplicationFormService
    {
        void InsertEntity(PartnerApplicationForm entity);

        void DeleteEntity(PartnerApplicationForm entity, bool delete = false);

        void DeleteEntities(IList<PartnerApplicationForm> entities, bool deleted = false);

        void UpdateEntity(PartnerApplicationForm entity);

        void UpdateEntities(IList<PartnerApplicationForm> entities);

        PartnerApplicationForm GetEntityById(int id);

        PartnerApplicationForm GetEntityByUserId(int wuserId);

        List<PartnerApplicationForm> GetEntitiesByIds(int[] entityIds);

        IPagedList<PartnerApplicationForm> GetEntities(
            string telephoneNumber = "",
            int wUserId = 0,
            DateTime? endDateTimeUtc = null,
            bool? approved = null,
            bool? locked = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}