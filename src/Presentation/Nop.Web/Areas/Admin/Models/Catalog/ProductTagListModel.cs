﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product tag list model
/// </summary>
public partial record ProductTagListModel : BasePagedListModel<ProductTagModel>;