using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Services.Common
{

    public partial interface IGenericAttributeService
    {
        Task<IList<GenericAttribute>> GetAttributesAsync(string keyGroup, string key, string value = null);
    }

    public partial class GenericAttributeService
    {
        public virtual async Task<IList<GenericAttribute>> GetAttributesAsync(string keyGroup, string key, string value = null)
        {
            var query = from ga in _genericAttributeRepository.Table
                        where ga.Key == key &&
                              ga.KeyGroup == keyGroup
                        select ga;

            if (!string.IsNullOrEmpty(value))
                query = query.Where(ga => ga.Value == value);

            var attributes = await query.ToListAsync();

            return attributes;
        }

    }
}