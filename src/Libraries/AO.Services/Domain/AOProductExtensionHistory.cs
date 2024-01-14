using Nop.Core;
using System;

namespace AO.Services.Domain
{
    public partial class AOProductExtensionHistory : BaseEntity
    {
        public int ProductId { get; set; }

        public int OldStatusId { get; set; }

        public int NewStatusId { get; set; }

        public string Comment { get; set; }

        public DateTime InsertDate { get; set; }
    }
}
