#if NET451
using System;
using System.Collections.Generic;
using AutoMapper;
using Nop.Web.Areas.Admin.Infrastructure.Mapper;
using Nop.Core.Infrastructure.Mapper;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
            var adminMapper = new AdminMapperConfiguration();
            configurationActions.Add(adminMapper.GetConfiguration());
            AutoMapperConfiguration.Init(configurationActions);
            AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
        }
    }
}
#endif