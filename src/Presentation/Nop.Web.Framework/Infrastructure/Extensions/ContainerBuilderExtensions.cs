using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Data;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of Autofac ContainerBuilder
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Register data context for a plugin
        /// </summary>
        /// <typeparam name="TContext">DB Context type</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="contextName">Context name</param>
        public static void RegisterPluginDataContext<TContext>(this IServiceCollection services, string contextName) where TContext : DbContext, IDbContext
        {
            //register named context
            services.AddScoped(provider => (IDbContext)Activator.CreateInstance(typeof(TContext), new[] { provider.GetService<DbContextOptions<TContext>>() }));
        }
    }
}