using Nop.Core.Infrastructure;
using Nop.Plugin.Theme.KungFu.Infrastructure;

namespace Nop.Plugin.Theme.KungFu.Seeding.Category;

public class CategorySeeds(INopFileProvider fileProvider) : ICategorySeeds
{
    private const string CATEGORY_SEED_FILE_NAME = "categories.json";
    
    public async Task<Core.Domain.Catalog.Category[]> GetCategoryiesForSeedAsync()
    {
        var json = ThemeKungFuDefaults.CategorySeedingVirtualPath + "/" + CATEGORY_SEED_FILE_NAME;
        var filePath = fileProvider.MapPath(json);
        
        if (!fileProvider.FileExists(filePath))
            return [];
        
        var fileContent = await fileProvider.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
        var categories = System.Text.Json.JsonSerializer.Deserialize<Core.Domain.Catalog.Category[]>(fileContent, new System.Text.Json.JsonSerializerOptions
        {
            Converters = { new JsonBooleanConverter() }
        });
        return categories ?? [];
        
    }
    
}