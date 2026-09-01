namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents a customer model to add to the affiliate 
/// </summary>
public partial record AddCustomerToAffiliateModel
{
    #region Properties

    public int CustomerId { get; set; }

    #endregion
}