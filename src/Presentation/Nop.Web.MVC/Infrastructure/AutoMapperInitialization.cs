using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Localization;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Infrastructure
{
    public static class AutoMapperInitialization
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<Language, LanguageModel>();
        }
    }
}