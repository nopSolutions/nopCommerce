using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Framework.Mvc.Routing;

using Senparc.Weixin.MP.CommonService.Infrastructure.Extensions;

namespace Senparc.Weixin.MP.CommonService.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring weixin features and middleware on application startup
    /// </summary>
    public class NopWeixinStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //配置微信
            services.ConfigureWeixinServices(configuration); //TODO: 待检查是否重复赋值

            //Senparc.Weixin 注册（必须）
            services.AddWeixin(configuration);

            //Senparc.WebSocket 注册（按需）
            //services.AddWebSocket();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //Use Senparc Global（必须）
            application.UseWeixin();

            //Use Senparc MessageHandler Middleware（按须）
            //application.UseMessageHandlerMiddleware();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 101;
    }
}