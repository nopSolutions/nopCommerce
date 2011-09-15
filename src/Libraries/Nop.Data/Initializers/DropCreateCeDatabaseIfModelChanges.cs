using System;
using System.Data.Entity;
using System.Transactions;

namespace Nop.Data.Initializers
{

    /// <summary>
    /// An implementation of IDatabaseInitializer that will <b>DELETE</b>, recreate, and optionally re-seed the
    /// database only if the model has changed since the database was created.  This is achieved by writing a
    /// hash of the store model to the database when it is created and then comparing that hash with one
    /// generated from the current model.
    /// To seed the database, create a derived class and override the Seed method.
    /// </summary>
    public class DropCreateCeDatabaseIfModelChanges<TContext> : SqlCeInitializer<TContext> where TContext : DbContext
    {
        #region Strategy implementation

        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void InitializeDatabase(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var replacedContext = ReplaceSqlCeConnection(context);

            bool databaseExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                databaseExists = replacedContext.Database.Exists();
            }

            if (databaseExists)
            {
                if (context.Database.CompatibleWithModel(throwIfNoMetadata: true))
                {
                    return;
                }

                replacedContext.Database.Delete();
            }

            // Database didn't exist or we deleted it, so we now create it again.
            context.Database.Create();

            Seed(context);
            context.SaveChanges();
        }

        #endregion

        #region Seeding methods

        /// <summary>
        /// A that should be overridden to actually add data to the context for seeding. 
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected virtual void Seed(TContext context)
        {
        }

        #endregion
    }

}
