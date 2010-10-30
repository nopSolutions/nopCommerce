//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Warehouses
{
    /// <summary>
    /// Warehouse manager
    /// </summary>
    public partial class WarehouseManager : IWarehouseManager
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public WarehouseManager(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks a warehouse as deleted
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        public void MarkWarehouseAsDeleted(int warehouseId)
        {
            var warehouse = GetWarehouseById(warehouseId);
            if (warehouse != null)
            {
                warehouse.Deleted = true;
                UpdateWarehouse(warehouse);
            }
        }

        /// <summary>
        /// Gets all warehouses
        /// </summary>
        /// <returns>Warehouse collection</returns>
        public List<Warehouse> GetAllWarehouses()
        {
            
            var query = from w in _context.Warehouses
                        orderby w.Name
                        where !w.Deleted
                        select w;
            var warehouses = query.ToList();

            return warehouses;
        }

        /// <summary>
        /// Gets a warehouse
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <returns>Warehouse</returns>
        public Warehouse GetWarehouseById(int warehouseId)
        {
            if (warehouseId == 0)
                return null;

            
            var query = from w in _context.Warehouses
                        where w.WarehouseId == warehouseId
                        select w;
            var warehouse = query.SingleOrDefault();
            return warehouse;
        }

        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public void InsertWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException("warehouse");
            
            warehouse.Name = CommonHelper.EnsureNotNull(warehouse.Name);
            warehouse.Name = CommonHelper.EnsureMaximumLength(warehouse.Name, 255);
            warehouse.PhoneNumber = CommonHelper.EnsureNotNull(warehouse.PhoneNumber);
            warehouse.PhoneNumber = CommonHelper.EnsureMaximumLength(warehouse.PhoneNumber, 50);
            warehouse.Email = CommonHelper.EnsureNotNull(warehouse.Email);
            warehouse.Email = CommonHelper.EnsureMaximumLength(warehouse.Email, 255);
            warehouse.FaxNumber = CommonHelper.EnsureNotNull(warehouse.FaxNumber);
            warehouse.FaxNumber = CommonHelper.EnsureMaximumLength(warehouse.FaxNumber, 50);
            warehouse.Address1 = CommonHelper.EnsureNotNull(warehouse.Address1);
            warehouse.Address1 = CommonHelper.EnsureMaximumLength(warehouse.Address1, 100);
            warehouse.Address2 = CommonHelper.EnsureNotNull(warehouse.Address2);
            warehouse.Address2 = CommonHelper.EnsureMaximumLength(warehouse.Address2, 100);
            warehouse.City = CommonHelper.EnsureNotNull(warehouse.City);
            warehouse.City = CommonHelper.EnsureMaximumLength(warehouse.City, 100);
            warehouse.StateProvince = CommonHelper.EnsureNotNull(warehouse.StateProvince);
            warehouse.StateProvince = CommonHelper.EnsureMaximumLength(warehouse.StateProvince, 100);
            warehouse.ZipPostalCode = CommonHelper.EnsureNotNull(warehouse.ZipPostalCode);
            warehouse.ZipPostalCode = CommonHelper.EnsureMaximumLength(warehouse.ZipPostalCode, 30);

            

            _context.Warehouses.AddObject(warehouse);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public void UpdateWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException("warehouse");

            warehouse.Name = CommonHelper.EnsureNotNull(warehouse.Name);
            warehouse.Name = CommonHelper.EnsureMaximumLength(warehouse.Name, 255);
            warehouse.PhoneNumber = CommonHelper.EnsureNotNull(warehouse.PhoneNumber);
            warehouse.PhoneNumber = CommonHelper.EnsureMaximumLength(warehouse.PhoneNumber, 50);
            warehouse.Email = CommonHelper.EnsureNotNull(warehouse.Email);
            warehouse.Email = CommonHelper.EnsureMaximumLength(warehouse.Email, 255);
            warehouse.FaxNumber = CommonHelper.EnsureNotNull(warehouse.FaxNumber);
            warehouse.FaxNumber = CommonHelper.EnsureMaximumLength(warehouse.FaxNumber, 50);
            warehouse.Address1 = CommonHelper.EnsureNotNull(warehouse.Address1);
            warehouse.Address1 = CommonHelper.EnsureMaximumLength(warehouse.Address1, 100);
            warehouse.Address2 = CommonHelper.EnsureNotNull(warehouse.Address2);
            warehouse.Address2 = CommonHelper.EnsureMaximumLength(warehouse.Address2, 100);
            warehouse.City = CommonHelper.EnsureNotNull(warehouse.City);
            warehouse.City = CommonHelper.EnsureMaximumLength(warehouse.City, 100);
            warehouse.StateProvince = CommonHelper.EnsureNotNull(warehouse.StateProvince);
            warehouse.StateProvince = CommonHelper.EnsureMaximumLength(warehouse.StateProvince, 100);
            warehouse.ZipPostalCode = CommonHelper.EnsureNotNull(warehouse.ZipPostalCode);
            warehouse.ZipPostalCode = CommonHelper.EnsureMaximumLength(warehouse.ZipPostalCode, 30);

            
            if (!_context.IsAttached(warehouse))
                _context.Warehouses.Attach(warehouse);

            _context.SaveChanges();
        }

        #endregion
    }
}
