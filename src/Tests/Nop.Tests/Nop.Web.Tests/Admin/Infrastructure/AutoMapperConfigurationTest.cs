﻿using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Infrastructure.Mapper;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Infrastructure;


public class AutoMapperConfigurationTest
{

    public void ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(typeof(AdminMapperConfiguration));
        });

        AutoMapperConfiguration.Init(config);
        AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
    }
}
