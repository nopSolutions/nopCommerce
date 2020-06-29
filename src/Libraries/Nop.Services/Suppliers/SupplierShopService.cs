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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// WConfig Service
    /// </summary>
    public partial class SupplierShopService : ISupplierShopService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<SupplierShop> _supplierShopRepository;

        #endregion

        #region Ctor

        public SupplierShopService(IEventPublisher eventPublisher,
            IRepository<SupplierShop> supplierShopRepository)
        {
            _eventPublisher = eventPublisher;
            _supplierShopRepository = supplierShopRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(SupplierShop supplierShop)
        {
            if (supplierShop == null)
                throw new ArgumentNullException(nameof(supplierShop));

            _supplierShopRepository.Insert(supplierShop);

            //event notification
            _eventPublisher.EntityInserted(supplierShop);
        }

        public virtual void DeleteEntity(SupplierShop supplierShop, bool delete = false)
        {
            if (supplierShop == null)
                throw new ArgumentNullException(nameof(supplierShop));

            if (delete)
            {
                _supplierShopRepository.Delete(supplierShop);
            }
            else
            {
                supplierShop.Deleted = true;
                UpdateEntity(supplierShop);
            }

            //event notification
            _eventPublisher.EntityDeleted(supplierShop);
        }

        public virtual void DeleteEntities(IList<SupplierShop> supplierShops, bool deleted = false)
        {
            if (supplierShops == null)
                throw new ArgumentNullException(nameof(supplierShops));

            if (deleted)
            {
                _supplierShopRepository.Delete(supplierShops);
            }
            else
            {
                foreach (var supplierShop in supplierShops)
                {
                    supplierShop.Deleted = true;
                }
                //delete wUser
                UpdateEntities(supplierShops);
            }

            foreach (var supplierShop in supplierShops)
            {
                //event notification
                _eventPublisher.EntityDeleted(supplierShop);
            }
        }

        public virtual void UpdateEntity(SupplierShop supplierShop)
        {
            if (supplierShop == null)
                throw new ArgumentNullException(nameof(supplierShop));

            _supplierShopRepository.Update(supplierShop);

            //event notification
            _eventPublisher.EntityUpdated(supplierShop);
        }

        public virtual void UpdateEntities(IList<SupplierShop> supplierShops)
        {
            if (supplierShops == null)
                throw new ArgumentNullException(nameof(supplierShops));

            //update
            _supplierShopRepository.Update(supplierShops);

            //event notification
            foreach (var supplierShop in supplierShops)
            {
                _eventPublisher.EntityUpdated(supplierShop);
            }
        }

        public virtual SupplierShop GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _supplierShopRepository.ToCachedGetById(id);
        }

        public virtual List<SupplierShop> GetEntitiesByIds(int[] entityIds)
        {
            if (entityIds is null)
                return new List<SupplierShop>();

            var query = from t in _supplierShopRepository.Table
                        where entityIds.Contains(t.Id) &&
                        !t.Deleted &&
                        t.Published
                        select t;

            return query.ToList();
        }

        public virtual List<SupplierShop> GetEntitiesBySupplierId(int supplierId)
        {
            if (supplierId <= 0)
                return new List<SupplierShop>();

            var query = from t in _supplierShopRepository.Table
                        where t.SupplierId == supplierId &&
                        !t.Deleted &&
                        t.Published
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<SupplierShop> GetEntities(
            int supplierId = 0,
            string name = "",
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _supplierShopRepository.Table;

            if (supplierId > 0)
                query = query.Where(q => q.SupplierId == supplierId);
            if (!string.IsNullOrEmpty(name))
                query = query.Where(q => q.Name.Contains(name));
            if (published.HasValue)
                query = query.Where(q => q.Published==published);
            if (deleted.HasValue)
                query = query.Where(q => q.Deleted == deleted);

            return new PagedList<SupplierShop>(query, pageIndex, pageSize);
        }

        #endregion
    }
}