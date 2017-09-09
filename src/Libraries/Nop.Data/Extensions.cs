using System;
using System.Data.Entity.Core.Objects;
using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Data
{
    public static class Extensions
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
            var type = entity is IEntityForCaching ? 
               ((IEntityForCaching) entity).GetType().BaseType :
               ObjectContext.GetObjectType(entity.GetType());
            if (type == null)
                throw new Exception("Original entity type cannot be loaded");

            return type;
        }
    }
}
