﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll list model
/// </summary>
public partial record PollListModel : BasePagedListModel<PollModel>;