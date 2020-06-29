using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// WConfig Service
    /// </summary>
    public partial class UserAssetService : IUserAssetService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<UserAsset> _userAssetRepository;

        #endregion

        #region Ctor

        public UserAssetService(IEventPublisher eventPublisher,
            IRepository<UserAsset> userAssetRepository)
        {
            _eventPublisher = eventPublisher;
            _userAssetRepository = userAssetRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(UserAsset entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _userAssetRepository.Insert(entity);

            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public virtual void DeleteEntity(UserAsset entity, bool delete = false)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (delete)
            {
                _userAssetRepository.Delete(entity);
            }
            else
            {
                entity.Deleted = true;
                UpdateEntity(entity);
            }

            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void DeleteEntities(IList<UserAsset> entities, bool deleted = false)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (deleted)
            {
                _userAssetRepository.Delete(entities);
            }
            else
            {
                foreach (var entity in entities)
                {
                    entity.Deleted = true;
                }
                //delete wUser
                UpdateEntities(entities);
            }

            foreach (var entity in entities)
            {
                //event notification
                _eventPublisher.EntityDeleted(entity);
            }
        }

        public virtual void UpdateEntity(UserAsset entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _userAssetRepository.Update(entity);

            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        public virtual void UpdateEntities(IList<UserAsset> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            //update
            _userAssetRepository.Update(entities);

            //event notification
            foreach (var entity in entities)
            {
                _eventPublisher.EntityUpdated(entity);
            }
        }

        public virtual UserAsset GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _userAssetRepository.ToCachedGetById(id);
        }

        public virtual UserAsset GetEntityByUserId(int wuserId)
        {
            if (wuserId == 0)
                return null;

            var query = from t in _userAssetRepository.Table
                        where t.OwnerUserId==wuserId
                        select t;

            return query.FirstOrDefault();
        }
        #endregion
    }
}