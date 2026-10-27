using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a contact form attribute value list model
/// </summary>
public partial record ContactFormAttributeValueListModel : BasePagedListModel<ContactFormAttributeValueModel>;