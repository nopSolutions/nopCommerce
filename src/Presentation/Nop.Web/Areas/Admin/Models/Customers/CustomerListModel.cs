﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer list model
/// </summary>
public partial record CustomerListModel : BasePagedListModel<CustomerModel>;