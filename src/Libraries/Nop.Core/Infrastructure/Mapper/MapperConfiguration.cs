using System.Collections.Concurrent;
using System.Linq.Expressions;
using Mapster;
using Mapster.Models;
using MapsterMapper;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Mapper configuration
/// </summary>
public static class MapperConfiguration
{
    public static ConcurrentDictionary<TypeTuple, TypeAdapterSetter> RuleMap { get; } = new();

    /// <summary>
    /// Gets the mapper
    /// </summary>
    public static IMapper Mapper { get; private set; }

    /// <summary>
    /// Gets the type adapter configuration
    /// </summary>
    public static TypeAdapterConfig TypeAdapterConfig { get; } = new();

    /// <summary>
    /// Initialize mapper
    /// </summary>
    public static void Init(IOrderedEnumerable<IOrderedMapperProfile> profiles)
    {
        foreach (var orderedMapperProfile in profiles)
            orderedMapperProfile.Init();

        Mapper = new MapsterMapper.Mapper(TypeAdapterConfig);
    }

    /// <summary>
    /// Configures the mapping for a specific member
    /// </summary>
    /// <param name="settings">The type adapter setter</param>
    /// <param name="member">The member to configure</param>
    /// <param name="options">The mapping options</param>
    /// <returns>The type adapter setter</returns>
    public static TypeAdapterSetter<TSource, TDestination> ForMember<TSource, TDestination>(this TypeAdapterSetter<TSource, TDestination> settings, Expression<Func<TDestination, object>> member, Action<MapOptions> options)
    {
        var option = new MapOptions();
        options(option);

        if (option.IsIgnored)
            settings.Ignore(member);
        else if (!string.IsNullOrEmpty(member.Name))
            settings.Map(member.Name, option.Source);

        return settings;
    }

    /// <summary>
    /// Configures the mapping for a specific member
    /// </summary>
    /// <param name="settings">The type adapter setter</param>
    /// <param name="memberName">The member name to configure</param>
    /// <param name="options">The mapping options</param>
    /// <returns>The type adapter setter</returns>
    public static TypeAdapterSetter ForMember(this TypeAdapterSetter settings, string memberName, Action<MapOptions> options)
    {
        var option = new MapOptions();
        options(option);

        if (option.IsIgnored)
            settings.Ignore(memberName);
        else
            settings.Map(memberName, option.Source);

        return settings;
    }
}