using AutoMapper;
using AutoMapper.Configuration;

namespace Nop.Plugin.Api.AutoMapper
{
    public static class AutoMapperApiConfiguration
    {
        private static MapperConfigurationExpression s_mapperConfigurationExpression;
        private static IMapper s_mapper;
        private static readonly object s_mapperLockObject = new object();

        public static MapperConfigurationExpression MapperConfigurationExpression =>
            s_mapperConfigurationExpression ??
            (s_mapperConfigurationExpression = new MapperConfigurationExpression());

        public static IMapper Mapper
        {
            get
            {
                if (s_mapper == null)
                {
                    lock (s_mapperLockObject)
                    {
                        if (s_mapper == null)
                        {
                            var mapperConfiguration = new MapperConfiguration(MapperConfigurationExpression);

                            s_mapper = mapperConfiguration.CreateMapper();
                        }
                    }
                }

                return s_mapper;
            }
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}
