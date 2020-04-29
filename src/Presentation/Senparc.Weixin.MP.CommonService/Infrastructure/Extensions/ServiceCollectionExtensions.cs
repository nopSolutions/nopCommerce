using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Core.Redis;
using Nop.Core.Security;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure.Extensions;
using Senparc.Weixin.RegisterServices;
using Senparc.CO2NET;
using Senparc.Weixin.Entities;
using Senparc.WebSocket;
using Senparc.Weixin.MP.CommonService.MessageHandlers.WebSocket;

namespace Senparc.Weixin.MP.CommonService.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="webHostEnvironment">Hosting environment</param>
        /// <returns>Configured service provider</returns>
        public static void ConfigureWeixinServices(this IServiceCollection services, IConfiguration configuration)
        {
            //add SenparcSetting parameters
            services.ConfigureStartupConfig<SenparcSetting>(configuration.GetSection("SenparcSetting"));

            //add SenparcWeixinSetting parameters
            services.ConfigureStartupConfig<SenparcWeixinSetting>(configuration.GetSection("SenparcWeixinSetting")); 
        }

        /// <summary>
        /// Adds services required for Weixin support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddWeixin(this IServiceCollection services, IConfiguration configuration)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            services.AddSenparcWeixinServices(configuration); //Senparc.Weixin 注册（必须）
        }

        /// <summary>
        /// Adds services required for WebSocket support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddWebSocket(this IServiceCollection services)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            //Senparc.WebSocket 注册（按需）
            services.AddSenparcWebSocket<CustomWebSocketMessageHandler>();
        }
    }
}