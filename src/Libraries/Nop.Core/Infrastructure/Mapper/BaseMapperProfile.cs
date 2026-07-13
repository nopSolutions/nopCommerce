using Mapster;
using Mapster.Models;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Base mapper profile class that can be used to configure mapping rules for specific types.
/// It provides methods to create mappings and apply actions to all mappings
/// </summary>
public abstract class BaseMapperProfile : IOrderedMapperProfile
{
    /// <summary>
    /// Creates a mapping configuration between the source type <typeparamref name="TSource"/> and the destination type <typeparamref name="TDestination"/>.
    /// </summary>
    /// <remarks>
    /// Please, use this method in the Init to create mapping configurations instead of directly using the Mapster API.
    /// This ensures that all mapping configurations are registered in the <see cref="MapperConfiguration.RuleMap"/> and can be accessed later for further customization or inspection.
    /// </remarks>
    /// <returns>
    /// A <see cref="TypeAdapterSetter{TSource, TDestination}"/> instance representing the mapping configuration
    /// </returns>
    protected TypeAdapterSetter<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var setter = MapperConfiguration.TypeAdapterConfig.NewConfig<TSource, TDestination>();
        var key = new TypeTuple(typeof(TSource), typeof(TDestination));

        MapperConfiguration.RuleMap.AddOrUpdate(key, _ => setter, (_, _) => setter);

        return setter;
    }

    /// <summary>
    /// Applies the specified <paramref name="action"/> to all mapping configurations defined in the <see cref="MapperConfiguration.RuleMap"/>.
    /// </summary>
    /// <param name="action"></param>
    protected void ForAllMaps(Action<MapTypeTuple, TypeAdapterSetter> action)
    {
        var rm = MapperConfiguration.TypeAdapterConfig.RuleMap;

        foreach (var (tt, _) in rm)
        {
            if (!MapperConfiguration.RuleMap.TryGetValue(tt, out var typeAdapterSetter))
                continue;

            action(new MapTypeTuple(tt), typeAdapterSetter);
        }
    }

    /// <summary>
    /// Gets order of this configuration implementation
    /// </summary>
    /// <remarks>Works only with Init method implemented</remarks>
    public int Order => 0;

    /// <summary>
    /// Initializes the mapping configuration for this profile. This method is called during the application startup to register the mapping rules defined in the profile
    /// </summary>
    public virtual void Init() { }
}