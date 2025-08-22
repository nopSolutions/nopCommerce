using Mapster;
using MapsterMapper;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Mapster configuration
/// </summary>
public static class MapsterConfiguration
{
    /// <summary>
    /// Mapper
    /// </summary>
    public static IMapper Mapper { get; private set; }

    /// <summary>
    /// TypeAdapterConfig
    /// </summary>
    public static TypeAdapterConfig Config { get; private set; }

    /// <summary>
    /// Initialize mapper
    /// </summary>
    /// <param name="config">Mapper configuration</param>
    public static void Init(TypeAdapterConfig config)
    {
        Config = config;
        Mapper = new MapsterMapper.Mapper(config);
    }
}