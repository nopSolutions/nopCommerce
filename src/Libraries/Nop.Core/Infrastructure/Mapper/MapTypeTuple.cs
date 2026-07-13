using Mapster.Models;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Represents the configuration for a mapping between a source type and a destination type.
/// </summary>
public partial class MapTypeTuple
{
    public MapTypeTuple(TypeTuple typeTuple)
    {
        SourceType = typeTuple.Source;
        DestinationType = typeTuple.Destination;
    }

    /// <summary>
    /// The source type for the mapping configuration
    /// </summary>
    public Type SourceType { get; }

    /// <summary>
    /// The destination type for the mapping configuration
    /// </summary>
    public Type DestinationType { get; }
}