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
        public static void RegisterPluginDataContext<TContext>(this ContainerBuilder builder, string contextName) where TContext : DbContext
        {
            //register named context
            //TODO: 239 try to implement or delete
        }
    }
}