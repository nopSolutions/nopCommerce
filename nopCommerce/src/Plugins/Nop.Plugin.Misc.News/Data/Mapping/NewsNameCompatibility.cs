using Nop.Data.Mapping;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Data.Mapping;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class NewsNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new() { [typeof(NewsItem)] = "News" };
    public Dictionary<(Type, string), string> ColumnName => [];
}
