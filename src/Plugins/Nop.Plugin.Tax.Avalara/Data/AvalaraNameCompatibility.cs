using Nop.Data.Mapping;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Data;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class AvalaraNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new() { { typeof(ItemClassification), "AvalaraItemClassification" } };
    public Dictionary<(Type, string), string> ColumnName => new();
}