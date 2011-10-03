using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using Nop.Core;
using Nop.Data.Mapping.Localization;

namespace Nop.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class NopObjectContext : DbContext, IDbContext
    {
        public NopObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //dynamically load all configuration
            System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            var typesToRegister = Assembly.GetAssembly(configType).GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
        
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //HACK: Entity Framework Code First doesn't support doesn't support output parameters
            //That's why we have to manually create command and execute it.
            //just wait until EF Code First starts support them
            //
            //More info: http://weblogs.asp.net/dwahlin/archive/2011/09/23/using-entity-framework-code-first-with-stored-procedures-that-have-output-parameters.aspx

            bool hasOutputParameters = false;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var outputP = p as DbParameter;
                    if (outputP == null)
                        continue;

                    if (outputP.Direction == ParameterDirection.InputOutput ||
                        outputP.Direction == ParameterDirection.Output)
                        hasOutputParameters = true;
                }
            }



            var context = ((IObjectContextAdapter)(this)).ObjectContext;
            if (!hasOutputParameters)
            {
                //no output parameters
                var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
                foreach (var entity in result)
                    Set<TEntity>().Attach(entity);
                return result;
                
                //var result = context.ExecuteStoreQuery<TEntity>(commandText, parameters).ToList();
                //foreach (var entity in result)
                //    Set<TEntity>().Attach(entity);
                //return result;
            }
            else
            {

                //var connection = context.Connection;
                var connection = this.Database.Connection;
                //Don't close the connection after command execution


                //open the connection for use
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                //create a command object
                using (var cmd = connection.CreateCommand())
                {
                    //command to execute
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // move parameters to command object
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    //database call
                    var reader = cmd.ExecuteReader();
                    //return reader.DataReaderToObjectList<TEntity>();
                    var result = context.Translate<TEntity>(reader).ToList();
                    foreach (var entity in result)
                        Set<TEntity>().Attach(entity);
                    //close up the reader, we're done saving results
                    reader.Close();
                    return result;
                }

            }
        }
    }
}