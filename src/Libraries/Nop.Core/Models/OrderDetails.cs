using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Models;

public class OrderDetails
{
    public int Id { get; set; }
    public decimal OrderTotal { get; set; }
    public CustomerDetails Customer { get; set; }
    public List<ProductDetails> Products { get; set; }
}