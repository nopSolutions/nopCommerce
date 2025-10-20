﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic list model
/// </summary>
public partial record TopicListModel : BasePagedListModel<TopicModel>;