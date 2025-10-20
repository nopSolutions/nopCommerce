﻿using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a category template cache event consumer
/// </summary>
public partial class CategoryTemplateCacheEventConsumer : CacheEventConsumer<CategoryTemplate>;