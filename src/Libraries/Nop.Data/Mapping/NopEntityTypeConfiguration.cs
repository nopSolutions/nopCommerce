using System.Data.Entity.ModelConfiguration;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// Entity configuration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NopEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected NopEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}