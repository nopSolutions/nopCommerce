using System;
using AutoMapper;
using System.Collections.Generic;

namespace Nop.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration
    /// </summary>
    public static class AutoMapperConfiguration
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;

        private static readonly List<Action<IMapperConfigurationExpression>> _configurationActions =
            new List<Action<IMapperConfigurationExpression>>();

        /// <summary>
        /// Adds a configuration action for automapper to the initialisation collection
        /// </summary>
        /// <param name="configurationAction">The configuration to run when creating the <see cref="MapperConfiguration"/> </param>
        public static void AddConfiguration(Action<IMapperConfigurationExpression> configurationAction)
        {
            _configurationActions.Add(configurationAction);
        }

        /// <summary>
        /// Initialize mapper
        /// </summary>
        public static void Init()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var configurationExpression in _configurationActions)
                {
                    configurationExpression(cfg);
                }
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        /// <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper
        {
            get
            {
                return _mapper;
            }
        }
        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration
        {
            get
            {
                return _mapperConfiguration;
            }
        }
    }
}