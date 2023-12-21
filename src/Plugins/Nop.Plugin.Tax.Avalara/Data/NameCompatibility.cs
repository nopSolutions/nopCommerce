using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Data.Mapping;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public partial class BaseNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new() { { typeof(ItemClassification), "AvalaraItemClassification" } };
    public Dictionary<(Type, string), string> ColumnName => new();
}