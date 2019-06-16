using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using NopBrasil.Plugin.Payments.PagSeguro.Controllers;
using NopBrasil.Plugin.Payments.PagSeguro.Services;
using NopBrasil.Plugin.Payments.PagSeguro.Task;

namespace NopBrasil.Plugin.Payments.PagSeguro.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            builder.RegisterType<PaymentPagSeguroController>().AsSelf();
            builder.RegisterType<PagSeguroService>().As<IPagSeguroService>().InstancePerDependency();
            builder.RegisterType<CheckPaymentTask>().AsSelf().InstancePerDependency();
        }

        public int Order => 2;
    }
}
