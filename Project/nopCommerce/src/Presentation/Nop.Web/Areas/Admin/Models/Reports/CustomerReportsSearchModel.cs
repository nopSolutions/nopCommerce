using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a customer reports search model
/// </summary>
public partial record CustomerReportsSearchModel : BaseSearchModel
{
    #region Ctor

    public CustomerReportsSearchModel()
    {
        BestCustomersByOrderTotal = new BestCustomersReportSearchModel();
        BestCustomersByNumberOfOrders = new BestCustomersReportSearchModel();
        RegisteredCustomers = new RegisteredCustomersReportSearchModel();
    }

    #endregion

    #region Properties

    public BestCustomersReportSearchModel BestCustomersByOrderTotal { get; set; }

    public BestCustomersReportSearchModel BestCustomersByNumberOfOrders { get; set; }

    public RegisteredCustomersReportSearchModel RegisteredCustomers { get; set; }

    #endregion
}