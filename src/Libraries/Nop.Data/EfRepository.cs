using System.Linq;
using Nop.Core;
using System;
using System.Data.Entity;
using Nop.Core.Data;
using Nop.Core.Infrastructure;


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