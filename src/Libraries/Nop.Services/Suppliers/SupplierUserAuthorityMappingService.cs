using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// WConfig Service
    /// </summary>
    public partial class SupplierUserAuthorityMappingService : ISupplierUserAuthorityMappingService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<SupplierUserAuthorityMapping> _supplierUserAuthorityMappingRepository;

        #endregion

        #region Ctor

        public SupplierUserAuthorityMappingService(IEventPublisher eventPublisher,
            IRepository<SupplierUserAuthorityMapping> supplierUserAuthorityMappingRepository)
        {
            _eventPublisher = eventPublisher;
            _supplierUserAuthorityMappingRepository = supplierUserAuthorityMappingRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(SupplierUserAuthorityMapping entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _supplierUserAuthorityMappingRepository.Insert(entity);

            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public virtual void DeleteEntity(SupplierUserAuthorityMapping entity, bool delete = false)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (delete)
            {
                _supplierUserAuthorityMappingRepository.Delete(entity);
            }
            else
            {
                entity.Deleted = true;
                UpdateEntity(entity);
            }

            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void DeleteEntities(IList<SupplierUserAuthorityMapping> entities, bool deleted = false)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (deleted)
            {
                _supplierUserAuthorityMappingRepository.Delete(entities);
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

        public virtual void UpdateEntity(SupplierUserAuthorityMapping entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _supplierUserAuthorityMappingRepository.Update(entity);

            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        public virtual void UpdateEntities(IList<SupplierUserAuthorityMapping> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            //update
            _supplierUserAuthorityMappingRepository.Update(entities);

            //event notification
            foreach (var entity in entities)
            {
                _eventPublisher.EntityUpdated(entity);
            }
        }

        public virtual SupplierUserAuthorityMapping GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _supplierUserAuthorityMappingRepository.ToCachedGetById(id);
        }

        public virtual SupplierUserAuthorityMapping GetEntityByUserId(int userId)
        {
            if (userId == 0)
                return null;

            var query = from t in _supplierUserAuthorityMappingRepository.Table
                        where t.WUserId == userId &&
                        !t.Deleted &&
                        t.Published
                        select t;

            return query.FirstOrDefault();
        }

        public virtual List<SupplierUserAuthorityMapping> GetEntitiesByIds(int[] entityIds)
        {
            if (entityIds is null)
                return new List<SupplierUserAuthorityMapping>();

            var query = from t in _supplierUserAuthorityMappingRepository.Table
                        where entityIds.Contains(t.Id) &&
                        !t.Deleted &&
                        t.Published
                        select t;

            return query.ToList();
        }

        public virtual List<SupplierUserAuthorityMapping> GetEntitiesBySupplierId(int supplierId, int supplierShopId = 0)
        {
            if (supplierId == 0)
                return new List<SupplierUserAuthorityMapping>();

            var query = _supplierUserAuthorityMappingRepository.Table;
            query = query.Where(q => q.SupplierId == supplierId);

            if (supplierShopId > 0)
                query = query.Where(q => q.SupplierShopId == supplierShopId);

            query = query.Where(q => !q.Deleted);

            return query.ToList();
        }

        public virtual IPagedList<SupplierUserAuthorityMapping> GetEntities(
            int supplierId = 0,
            int supplierShopId = 0,
            bool? financialManager = null,
            bool? verifyManager = null,
            bool? orderConfirmer = null,
            bool? productPulisher = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _supplierUserAuthorityMappingRepository.Table;

            query = query.Where(q => q.SupplierId == supplierId);
            if (supplierShopId > 0)
                query = query.Where(q => q.SupplierShopId == supplierShopId);

            if (financialManager.HasValue)
                query = query.Where(q => q.FinancialManager == financialManager);
            if (verifyManager.HasValue)
                query = query.Where(q => q.VerifyManager == verifyManager);
            if (orderConfirmer.HasValue)
                query = query.Where(q => q.OrderConfirmer == orderConfirmer);
            if (productPulisher.HasValue)
                query = query.Where(q => q.ProductPulisher == productPulisher);
            if (published.HasValue)
                query = query.Where(q => q.Published == published);
            if (deleted.HasValue)
                query = query.Where(q => q.Deleted == deleted);

            return new PagedList<SupplierUserAuthorityMapping>(query, pageIndex, pageSize);
        }

        #endregion
    }
}