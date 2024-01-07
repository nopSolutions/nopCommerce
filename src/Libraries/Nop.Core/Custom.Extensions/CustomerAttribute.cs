using System;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents a customer attribute
/// </summary>
public partial class CustomerAttribute
{
    public string HelpText { get; set; }

    public bool ShowOnRegisterPage { get; set; }
}
