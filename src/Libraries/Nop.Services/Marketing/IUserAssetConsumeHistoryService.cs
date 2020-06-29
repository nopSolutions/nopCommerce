using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IUserAssetConsumeHistoryService
    {
        void InsertEntity(UserAssetConsumeHistory entity);

        void DeleteEntity(UserAssetConsumeHistory entity, bool delete = false);

        void DeleteEntities(IList<UserAssetConsumeHistory> entities, bool deleted = false);

        void UpdateEntity(UserAssetConsumeHistory entity);

        void UpdateEntities(IList<UserAssetConsumeHistory> entities);

        UserAssetConsumeHistory GetEntityById(int id);

        List<UserAssetConsumeHistory> GetEntitiesByUserId(int userId, bool completed, bool isInvalid);

        List<UserAssetConsumeHistory> GetEntitiesByUserAssetIncomeHistoryId(int userAssetIncomeHistoryId, bool completed, bool isInvalid);

        IPagedList<UserAssetConsumeHistory> GetEntities(
            int ownerUserId = 0,
            int? userAssetIncomeHistoryId = 0,
            AssetConsumType? assetConsumType = null,
            bool? completed = null,
            bool? isInvalid = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}