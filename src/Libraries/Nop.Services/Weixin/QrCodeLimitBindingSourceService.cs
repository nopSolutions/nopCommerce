using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.Weixin;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WQrCodeLimitUser Service
    /// </summary>
    public partial class QrCodeLimitBindingSourceService : IQrCodeLimitBindingSourceService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<QrCodeLimitBindingSource> _qrCodeLimitBindingSourceRepository;

        #endregion

        #region Ctor

        public QrCodeLimitBindingSourceService(IEventPublisher eventPublisher,
            IRepository<QrCodeLimitBindingSource> qrCodeLimitBindingSourceRepository)
        {
            _eventPublisher = eventPublisher;
            _qrCodeLimitBindingSourceRepository = qrCodeLimitBindingSourceRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(QrCodeLimitBindingSource entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _qrCodeLimitBindingSourceRepository.Insert(entity);

            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public virtual void DeleteEntity(QrCodeLimitBindingSource entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _qrCodeLimitBindingSourceRepository.Delete(entity);

            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void DeleteEntities(IList<QrCodeLimitBindingSource> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _qrCodeLimitBindingSourceRepository.Delete(entities);

            foreach (var entity in entities)
            {
                //event notification
                _eventPublisher.EntityDeleted(entity);
            }
        }

        public virtual void UpdateEntity(QrCodeLimitBindingSource entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _qrCodeLimitBindingSourceRepository.Update(entity);

            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        public virtual void UpdateEntities(IList<QrCodeLimitBindingSource> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            //update
            _qrCodeLimitBindingSourceRepository.Update(entities);

            //event notification
            foreach (var entity in entities)
            {
                _eventPublisher.EntityUpdated(entity);
            }
        }

        public virtual QrCodeLimitBindingSource GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _qrCodeLimitBindingSourceRepository.GetById(id);
        }

        public virtual QrCodeLimitBindingSource GetEntityByQrcodeLimitId(int qrCodeLimitId)
        {
            if (qrCodeLimitId == 0)
                return null;

            var query = from t in _qrCodeLimitBindingSourceRepository.Table
                        where t.QrCodeLimitId == qrCodeLimitId &&
                        t.Published
                        select t;

            return query.FirstOrDefault();
        }

        public virtual List<QrCodeLimitBindingSource> GetEntitiesByIds(int[] wEntityIds)
        {
            if (wEntityIds is null)
                return new List<QrCodeLimitBindingSource>();

            var query = from t in _qrCodeLimitBindingSourceRepository.Table
                        where wEntityIds.Contains(t.Id) &&
                        t.Published
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<QrCodeLimitBindingSource> GetEntities(
            int qrCodeLimitId = 0, 
            int supplierId = 0, 
            int supplierShopId = 0, 
            int productId = 0, 
            bool? published = null, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _qrCodeLimitBindingSourceRepository.Table;

            if (qrCodeLimitId > 0)
                query = query.Where(v => v.QrCodeLimitId == qrCodeLimitId);
            if (supplierId > 0)
                query = query.Where(v => v.SupplierId == supplierId);
            if (supplierShopId > 0)
                query = query.Where(v => v.SupplierShopId == supplierShopId);
            if (productId > 0)
                query = query.Where(v => v.ProductId == productId);

            if (published.HasValue)
                query = query.Where(v => v.Published == published);

            query = query.OrderBy(v => v.Id);

            return new PagedList<QrCodeLimitBindingSource>(query, pageIndex, pageSize);
        }

        #endregion
    }
}