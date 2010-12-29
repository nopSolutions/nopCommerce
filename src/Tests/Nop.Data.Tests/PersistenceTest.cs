using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public abstract class PersistenceTest
    {
        protected NopObjectContext context;

        [SetUp]
        public void SetUp()
        {
            DbDatabase.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            context = new NopObjectContext(GetTestDbName());
            context.Database.Delete();
            context.Database.Create();
        }

        protected string GetTestDbName()
        {
            string testDbName = "Data Source=" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)) + @"\\Nop.Data.Tests.Db.sdf;Persist Security Info=False";
            return testDbName;
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
            context = new NopObjectContext(GetTestDbName());

            var fromDb = context.Set<T>().Find(id);
            fromDb.ShouldNotBeNull();
            return fromDb;
        }
    }
}
