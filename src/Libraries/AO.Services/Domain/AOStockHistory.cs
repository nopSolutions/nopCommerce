using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace AO.Services.Domain
{
    public class AOStockHistory : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public decimal CostPrice { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
