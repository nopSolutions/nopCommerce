using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
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
        /// <param name="builder">Builder</param>
        /// <param name="contextName">Context name</param>
        public static void RegisterPluginDataContext<TContext>(this ContainerBuilder builder, string contextName) where TContext : DbContext, IDbContext
        {
            //register named context
            builder.Register(context => (IDbContext)Activator.CreateInstance(typeof(TContext), new[] { context.Resolve<DbContextOptions<TContext>>() }))
                .Named<IDbContext>(contextName).InstancePerLifetimeScope();
        }
    }
}