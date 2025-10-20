﻿using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;

namespace Nop.Services.Shipping.Caching;

/// <summary>
/// Represents a delivery date cache event consumer
/// </summary>
public partial class DeliveryDateCacheEventConsumer : CacheEventConsumer<DeliveryDate>;