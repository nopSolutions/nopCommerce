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
        private readonly IDbContext context;
        private readonly IDbSet<T> entities;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context) {
            this.context = context;
            this.entities = context.Set<T>();
        }
        
        public T GetById(object id) {
            return this.entities.Find(id);
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.entities.Add(entity);

            this.context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //if (!this._context.IsAttached(entity))
            //    this._entities.Attach(entity);

            this.context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //if (!this._context.IsAttached(entity))
            //    this._entities.Attach(entity);

            this.entities.Remove(entity);

            this.context.SaveChanges();
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.entities;
            }
        }
    }
}