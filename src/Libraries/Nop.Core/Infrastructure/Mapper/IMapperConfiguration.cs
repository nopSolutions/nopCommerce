using System;
using AutoMapper;

namespace Nop.Core.Infrastructure.Mapper
{
    /// <summary>
    /// Mapper configuration registrar interface
    /// </summary>
    public interface IMapperConfiguration
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Mapper configuration action</returns>
        Action<IMapperConfigurationExpression> GetConfiguration();

        /// <summary>
        /// Gets order of this configuration implementation (more is better) 
        /// </summary>
        int Order { get; }
    }
}
