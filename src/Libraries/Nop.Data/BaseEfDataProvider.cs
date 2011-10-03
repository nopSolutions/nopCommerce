using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Data
{
    public abstract class BaseEfDataProvider:IEfDataProvider
    {
        public abstract IDbConnectionFactory GetConnectionFactory();

        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = GetConnectionFactory();
        }

        public abstract void SetDatabaseInitializer();

        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        public abstract bool StoredProceduredSupported { get; }
    }
}
