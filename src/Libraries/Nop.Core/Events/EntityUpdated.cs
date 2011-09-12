using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Events
{
    /// <summary>
    /// A containe for entities that are udpated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityUpdated<T> where T : BaseEntity
    {
        private readonly T _entity;

        public EntityUpdated(T entity)
        {
            _entity = entity;
        }

        public T Entity { get { return _entity; } }
    }
}
