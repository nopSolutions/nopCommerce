﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart;

/// <summary>
/// Represents a shopping cart list model
/// </summary>
public partial record ShoppingCartListModel : BasePagedListModel<ShoppingCartModel>;