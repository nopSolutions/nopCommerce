using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a country list model
/// </summary>
public partial record CountryListModel : BasePagedListModel<CountryModel>
{
}