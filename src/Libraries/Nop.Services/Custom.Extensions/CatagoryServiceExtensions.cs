using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial interface ICategoryService
    {
        Task<IList<Category>> GetAllCategoriesAsync(List<string> categoryNames, int storeId = 0, bool showHidden = false);
    }

    public partial class CategoryService
    {
        public virtual async Task<IList<Category>> GetAllCategoriesAsync(List<string> categoryNames, int storeId = 0, bool showHidden = false)
        {
            var query = _categoryRepository.Table;

            if (!showHidden)
                query = query.Where(c => c.Published);

            query = query.Where(o => categoryNames.Contains(o.Name));
            var categories = await query.ToListAsync();
            return categories;
        }
    }
}
