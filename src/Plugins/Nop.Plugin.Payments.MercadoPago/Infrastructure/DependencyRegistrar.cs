using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.MercadoPago.Controllers;
using Nop.Plugin.Payments.MercadoPago.FuraFila;

namespace Nop.Plugin.Payments.MercadoPago.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 2;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PaymentMercadoPagoController>().AsSelf();
            builder.RegisterType<MPPaymentService>().As<IMPPaymentService>().InstancePerDependency();
        }
    }
}
