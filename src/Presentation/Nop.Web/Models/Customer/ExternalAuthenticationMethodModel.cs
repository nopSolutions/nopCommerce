using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer
{
    public partial record ExternalAuthenticationMethodModel : BaseNopModel
    {
        public Type ViewComponentType { get; set; }
    }
}