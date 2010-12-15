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

using System.Linq;
using Nop.Core;
using System;
using System.Data.Entity;


namespace Nop.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private readonly IDbSet<T> _entities;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context) {
            this._context = context;
            this._entities = context.Set<T>();
        }
        
        public T GetById(object id) {
            //TODO: use this._entities.Where(e => e.Id == id);
            //this._entities.Find(id) returns cached entity
            return this._entities.Find(id);
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this._entities.Add(entity);

            this._context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //if (!this._context.IsAttached(entity))
            //    this._entities.Attach(entity);

            this._context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //if (!this._context.IsAttached(entity))
            //    this._entities.Attach(entity);

            this._entities.Remove(entity);

            this._context.SaveChanges();
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this._entities;
            }
        }

        //TODO implement IDisposable interface
    }
}