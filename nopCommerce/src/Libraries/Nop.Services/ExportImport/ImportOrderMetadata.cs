using Nop.Core.Domain.Orders;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class ImportOrderMetadata
{
    public int EndRow { get; internal set; }

    public PropertyManager<Order> Manager { get; internal set; }

    public IList<PropertyByName<Order>> Properties { get; set; }

    public int CountOrdersInFile { get; set; }

    public PropertyManager<OrderItem> OrderItemManager { get; internal set; }

    public List<Guid> AllOrderGuids { get; set; }

    public List<Guid> AllCustomerGuids { get; set; }
}