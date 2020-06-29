using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IUserAssetService
    {
        void InsertEntity(UserAsset entity);

        void DeleteEntity(UserAsset entity, bool delete = false);

        void DeleteEntities(IList<UserAsset> entities, bool deleted = false);

        void UpdateEntity(UserAsset entity);

        void UpdateEntities(IList<UserAsset> entities);

        UserAsset GetEntityById(int id);

        UserAsset GetEntityByUserId(int wuserId);

    }
}