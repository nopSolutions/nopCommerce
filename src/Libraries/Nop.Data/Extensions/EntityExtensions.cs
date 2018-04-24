using System;
using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Get unproxied entity type
        /// </summary>
        /// <remarks> If your Entity Framework context is proxy-enabled, 
        /// the runtime will create a proxy instance of your entities, 
        /// i.e. a dynamically generated class which inherits from your entity class 
        /// and overrides its virtual properties by inserting specific code useful for example 
        /// for tracking changes and lazy loading.
        /// </remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
#if EF6
            var type = entity is IEntityForCaching ? 
               ((IEntityForCaching) entity).GetType().BaseType :
               ObjectContext.GetObjectType(entity.GetType());
            if (type == null)
                throw new Exception("Original entity type cannot be loaded");

            return type;
#else
            return entity.GetType();
#endif
        }
    }
}