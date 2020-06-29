using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IActivitiesThemeService
    {
        void InsertEntity(ActivitiesTheme entity);

        void DeleteEntity(ActivitiesTheme entity, bool delete = false);

        void DeleteEntities(IList<ActivitiesTheme> entities, bool delete = false);

        void UpdateEntity(ActivitiesTheme entity);

        void UpdateEntities(IList<ActivitiesTheme> entities);

        ActivitiesTheme GetEntityById(int id);

        IPagedList<ActivitiesTheme> GetEntities(
            string title = "",
            int customerRoleId = 0,
            int storeId = 0,
            DateTime? startDateTimeUtc = null,
            DateTime? endDateTimeUtc = null,
            bool? sysActivities = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}