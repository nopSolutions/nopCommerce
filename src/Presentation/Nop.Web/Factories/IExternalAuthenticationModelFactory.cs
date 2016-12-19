using System.Collections.Generic;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    public partial interface IExternalAuthenticationModelFactory
    {
        List<ExternalAuthenticationMethodModel> PrepareExternalMethodsModel();
    }
}
