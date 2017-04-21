using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Data;
using Nop.Web.Framework.FluentValidation;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add anf configure MVC for the application
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //whether database is already installed
            var databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            //add basic MVC feature
            var mvcBuilder = services.AddMvc();

            //add custom display metadata provider
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider()));

#if NET451
            //add themeable view engine
            if (databaseInstalled)
            {
                mvcBuilder.AddViewOptions(options =>
                {
                    options.ViewEngines.Clear();
                    options.ViewEngines.Add(new ThemeableRazorViewEngine());
                });
            }
#endif

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration => configuration.ValidatorFactoryType = typeof(NopValidatorFactory));

            return mvcBuilder;
        }
    }
}
