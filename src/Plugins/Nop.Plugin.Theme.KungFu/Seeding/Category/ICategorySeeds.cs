namespace Nop.Plugin.Theme.KungFu.Seeding.Category;

public interface ICategorySeeds
{
    Task<Core.Domain.Catalog.Category[]> GetCategoryiesForSeedAsync();
}