using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.Entity.Database;
using Nop.Core;
using Nop.Tests;

namespace Nop.Data.Tests
{
    [TestFixture]
    public abstract class PersistenceTest
    {
        const string dbName = "Nop.Data.Tests.Db";
        protected NopObjectContext context;

        [SetUp]
        public void SetUp()
        {
            DbDatabase.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            context = new NopObjectContext(dbName);
            context.Database.Delete();
            context.Database.Create();
        }
        
        
        /// <summary>
        /// Persistance test helper
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        protected T SaveAndLoadEntity<T>(T entity) where T : BaseEntity
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();

            object id = entity.Id;

            context.Dispose();
            context = new NopObjectContext(dbName);

            var fromDb = context.Set<T>().Find(id);
            fromDb.ShouldNotBeNull();
            return fromDb;
        }
    }
}
