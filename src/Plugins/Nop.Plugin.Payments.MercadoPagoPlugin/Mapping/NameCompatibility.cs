using Nop.Data.Mapping;

namespace Nop.Plugin.Payments.MercadoPagoPlugin.Mapping;
public partial class NameCompatibility : INameCompatibility
{
    /// <summary>
    /// Gets table name for mapping with the type
    /// </summary>
    public Dictionary<Type, string> TableNames => new();

    /// <summary>
    ///  Gets column name for mapping with the entity's property and type
    /// </summary>
    public Dictionary<(Type, string), string> ColumnName => new();
}