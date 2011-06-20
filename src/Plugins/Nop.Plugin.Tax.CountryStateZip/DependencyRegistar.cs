using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Tax.CountryStateZip.Data;
using Nop.Plugin.Tax.CountryStateZip.Domain;
using Nop.Plugin.Tax.CountryStateZip.Services;
using Nop.Services.Configuration;


namespace Nop.Plugin.Tax.CountryStateZip
{
    public class DependencyRegistar : IDependencyRegistar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<TaxRateService>().As<ITaxRateService>().InstancePerHttpRequest();

            //data layer
            var dataProviderManager = new DataProviderManager();
            var dataProviderSettings = dataProviderManager.LoadSettings();
            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //register named context
                builder.Register<IDbContext>(c => new TaxRateObjectContext(dataProviderSettings.DataConnectionString))
                    .Named<IDbContext>("nop_object_context_tax_country_state_zip")
                    .InstancePerHttpRequest();

                builder.Register<TaxRateObjectContext>(c => new TaxRateObjectContext(dataProviderSettings.DataConnectionString))
                    .InstancePerHttpRequest();
            }
            else
            {
                //register named context
                builder.Register<IDbContext>(c => new TaxRateObjectContext(new DataProviderManager().LoadSettings().DataConnectionString))
                    .Named<IDbContext>("nop_object_context_tax_country_state_zip")
                    .InstancePerHttpRequest();

                builder.Register<TaxRateObjectContext>(c => new TaxRateObjectContext(new DataProviderManager().LoadSettings().DataConnectionString))
                    .InstancePerHttpRequest();
            }

            //override required repository with our custom context
            builder.RegisterType<EfRepository<TaxRate>>()
                .As<IRepository<TaxRate>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_tax_country_state_zip"))
                .InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
