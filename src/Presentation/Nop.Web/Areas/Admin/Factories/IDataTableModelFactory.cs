namespace Nop.Web.Areas.Admin.Factories
{
    using Nop.Web.Areas.Admin.Models.Orders;
    using Nop.Web.Framework.Models.DataTables;

    public interface IDataTableModelFactory
    {
        DataTablesModel PrepareOrderListDataTablesModel(OrderSearchModel model);
    }
}
