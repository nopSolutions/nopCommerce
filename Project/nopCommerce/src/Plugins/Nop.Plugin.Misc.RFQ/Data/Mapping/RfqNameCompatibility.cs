using Nop.Data.Mapping;
using Nop.Plugin.Misc.RFQ.Domains;

namespace Nop.Plugin.Misc.RFQ.Data.Mapping;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class RfqNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        [typeof(RequestQuote)] = "RFQRequestQuote",
        [typeof(RequestQuoteItem)] = "RFQRequestQuoteItem",
        [typeof(Quote)] = "RFQQuote",
        [typeof(QuoteItem)] = "RFQQuoteItem",
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}