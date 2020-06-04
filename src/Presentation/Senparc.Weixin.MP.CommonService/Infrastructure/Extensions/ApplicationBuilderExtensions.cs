using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Plugins;
using Nop.Web.Framework.Globalization;
using Nop.Web.Framework.Mvc.Routing;
using WebMarkupMin.AspNetCore3;

using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Cache.Memcached;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.MessageHandlers;
using Senparc.WebSocket;
using Senparc.Weixin.Cache.CsRedis;
using Senparc.Weixin.Cache.Memcached;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
//using Senparc.Weixin.MP.MessageHandlers.Middleware;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.CommonService.MessageHandlers.WebSocket;
using Senparc.Weixin.MP.CommonService.WorkMessageHandlers;
using Senparc.Weixin.MP.CommonService.WxOpenMessageHandler;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.RegisterServices;

using Senparc.Weixin.TenPay;
using Senparc.Weixin.Work;
//using Senparc.Weixin.Work.MessageHandlers.Middleware;
using Senparc.Weixin.WxOpen;
//using Senparc.Weixin.WxOpen.MessageHandlers.Middleware;

namespace Senparc.Weixin.MP.CommonService.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add senparc weixin global
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseWeixin(this IApplicationBuilder application)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            var senparcSetting = EngineContext.Current.Resolve<SenparcSetting>();
            var senparcWeixinSetting = EngineContext.Current.Resolve<SenparcWeixinSetting>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            // 启动 CO2NET 全局注册（必须）
            // 关于 UseSenparcGlobal() 的更多用法见 CO2NET Demo：https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore3/Startup.cs
            application.UseSenparcGlobal(webHostEnvironment, senparcSetting, globalRegister =>
            {
                #region CO2NET 全局配置

                #region 全局缓存配置（按需）

                //当同一个分布式缓存同时服务于多个网站（应用程序池）时，可以使用命名空间将其隔离（非必须）
                //globalRegister.ChangeDefaultCacheNamespace("DefaultCO2NETCache");

                #region 配置和使用 Redis / Memcached

                //配置全局使用 Redis / Memcached 缓存（按需，独立）
                if (UseRedis(senparcSetting, out var redisConfigurationStr))
                {
                    /* 说明：
                     * 1、Redis 的连接字符串信息会从 Config.SenparcSetting.Cache_Redis_Configuration 自动获取并注册，如不需要修改，下方方法可以忽略
                    /* 2、如需手动修改，可以通过下方 SetConfigurationOption 方法手动设置 Redis 链接信息（仅修改配置，不立即启用）
                     */
                    Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(redisConfigurationStr);

                    //以下会立即将全局缓存设置为 Redis
                    Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）
                    //Senparc.CO2NET.Cache.CsRedis.Register.UseHashRedisNow();//HashSet储存格式的缓存策略

                    //也可以通过以下方式自定义当前需要启用的缓存策略
                    //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//键值对
                    //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisHashSetObjectCacheStrategy.Instance);//HashSet

                    #region 注册 StackExchange.Redis

                    /* 如果需要使用 StackExchange.Redis，则可以使用 Senparc.CO2NET.Cache.Redis 库
                     * 注意：这一步注册和上述 CsRedis 库两选一即可，本 Sample 需要同时演示两个库，因此才都进行注册
                     */

                    //Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);
                    //Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）

                    #endregion
                }
                else if (UseMemcached(senparcSetting, out var memcachedConfigurationStr))
                {
                    application.UseEnyimMemcached();

                    /* 说明：
                    * 1、Memcached 的连接字符串信息会从 Config.SenparcSetting.Cache_Memcached_Configuration 自动获取并注册，如不需要修改，下方方法可以忽略
                    * 2、如需手动修改，可以通过下方 SetConfigurationOption 方法手动设置 Memcached 链接信息（仅修改配置，不立即启用）
                    */
                    Senparc.CO2NET.Cache.Memcached.Register.SetConfigurationOption(memcachedConfigurationStr);

                    //以下会立即将全局缓存设置为 Memcached
                    Senparc.CO2NET.Cache.Memcached.Register.UseMemcachedNow();

                    //也可以通过以下方式自定义当前需要启用的缓存策略
                    CacheStrategyFactory.RegisterObjectCacheStrategy(() => MemcachedObjectCacheStrategy.Instance);
                }
                //如果这里不进行 Redis/Memcach 缓存启用，则目前还是默认使用内存缓存 

                #endregion

                #endregion

                #region 注册日志（按需，建议）

                globalRegister.RegisterTraceLog(ConfigTraceLog); //配置TraceLog

                #endregion

                #region APM 系统运行状态统计记录配置

                //测试APM缓存过期时间（默认情况下可以不用设置）
                CO2NET.APM.Config.EnableAPM = false; //默认已经为开启，如果需要关闭，则设置为 false
                CO2NET.APM.Config.DataExpire = TimeSpan.FromMinutes(60);

                #endregion

                #endregion
            },

            #region 扫描自定义扩展缓存

            //自动扫描自定义扩展缓存
            false

            //自动扫描自定义扩展缓存（二选一）
            //autoScanExtensionCacheStrategies: true //默认为 true，可以不传入

            //指定自定义扩展缓存（二选一）
            //autoScanExtensionCacheStrategies: false, extensionCacheStrategiesFunc: () => GetExCacheStrategies(senparcSetting.Value)

            #endregion

            ).UseSenparcWeixin(senparcWeixinSetting, weixinRegister =>  //使用 Senparc.Weixin SDK
            {
                #region 微信相关配置

                /* 微信配置开始，建议按照以下顺序进行注册，尤其须将缓存放在第一位！ */

                #region 微信缓存（按需，必须放在配置开头，以确保其他可能依赖到缓存的注册过程使用正确的配置）
                //注意：如果使用非本地缓存，而不执行本块注册代码，将会收到“当前扩展缓存策略没有进行注册”的异常

                //微信的 Redis/Memcached 缓存，如果不使用则注释掉（开启前必须保证配置有效，否则会抛错）
                if (UseRedis(senparcSetting, out _))
                {
                    weixinRegister.UseSenparcWeixinCacheCsRedis();//CsRedis，两选一
                    //weixinRegister.UseSenparcWeixinCacheRedis();//StackExchange.Redis，两选一
                }
                else if (UseMemcached(senparcSetting, out _))
                {
                    application.UseEnyimMemcached();
                    weixinRegister.UseSenparcWeixinCacheMemcached();
                }

                #endregion

                #region 注册公众号或小程序（按需）

                //注册公众号（可注册多个）
                if (true)
                {
                    weixinRegister.RegisterMpAccount(senparcWeixinSetting, "NOP-公众号");
                }
                
                //注册多个公众号或小程序（可注册多个）
                if(true)
                {
                    //weixinRegister.RegisterWxOpenAccount(senparcWeixinSetting, "NOP-小程序");
                }

                //除此以外，仍然可以在程序任意地方注册公众号或小程序：
                //AccessTokenContainer.Register(appId, appSecret, name); //命名空间：Senparc.Weixin.MP.Containers
                #endregion

                #region 注册企业号（按需）

                //注册企业微信（可注册多个）
                if (false)
                {
                    weixinRegister.RegisterWorkAccount(senparcWeixinSetting, "NOP-企业微信");
                }
                
                //除此以外，仍然可以在程序任意地方注册企业微信：
                //AccessTokenContainer.Register(corpId, corpSecret, name);//命名空间：Senparc.Weixin.Work.Containers
                #endregion

                #region 注册微信支付（按需）

                if(true)
                {
                    //注册旧微信支付版本（V2）（可注册多个）
                    weixinRegister.RegisterTenpayOld(senparcWeixinSetting, "NOP-公众号"); //这里的 name 和第一个 RegisterMpAccount() 中的一致，会被记录到同一个 SenparcWeixinSettingItem 对象中

                    //注册最新微信支付版本（V3）（可注册多个）
                    weixinRegister.RegisterTenpayV3(senparcWeixinSetting, "NOP-公众号"); //记录到同一个 SenparcWeixinSettingItem 对象中

                    /* 特别注意：
                     * 在 services.AddSenparcWeixinServices() 代码中，已经自动为当前的 
                     * senparcWeixinSetting  对应的TenpayV3 配置进行了 Cert 证书配置，
                     * 如果此处注册的微信支付信息和默认 senparcWeixinSetting 信息不同，
                     * 请在 ConfigureServices() 方法中使用 services.AddCertHttpClient() 
                     * 添加对应证书。
                     */
                }


                #endregion

                #region 注册微信第三方平台（按需）

                if (false)
                {
                    //注册第三方平台（可注册多个）
                    weixinRegister.RegisterOpenComponent(senparcWeixinSetting,
                            //getComponentVerifyTicketFunc
                            async componentAppId =>
                            {
                                var dir = fileProvider.Combine(fileProvider.MapPath("~/App_Data/OpenTicket"));
                                if (!fileProvider.DirectoryExists(dir))
                                {
                                    fileProvider.CreateDirectory(dir);
                                }

                                var file = fileProvider.Combine(dir, string.Format("{0}.txt", componentAppId));

                                using var fs = new FileStream(file, FileMode.Open);
                                using var sr = new StreamReader(fs);
                                var ticket = await sr.ReadToEndAsync();
                                sr.Close();
                                return ticket;


                            },

                            //getAuthorizerRefreshTokenFunc
                            async (componentAppId, auhtorizerId) =>
                            {
                                var dir = fileProvider.Combine(ServerUtility.ContentRootMapPath("~/App_Data/AuthorizerInfo/" + componentAppId));
                                if (!fileProvider.DirectoryExists(dir))
                                {
                                    fileProvider.CreateDirectory(dir);
                                }

                                var file = fileProvider.Combine(dir, string.Format("{0}.bin", auhtorizerId));
                                if (!fileProvider.FileExists(file))
                                {
                                    return null;
                                }

                                using Stream fs = new FileStream(file, FileMode.Open);

                                var binFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                                var result = (RefreshAuthorizerTokenResult)binFormat.Deserialize(fs);
                                fs.Close();
                                return result.authorizer_refresh_token;

                            },

                            //authorizerTokenRefreshedFunc
                            (componentAppId, auhtorizerId, refreshResult) =>
                            {
                                var dir = fileProvider.Combine(fileProvider.MapPath("~/App_Data/AuthorizerInfo/" + componentAppId));
                                if (!fileProvider.DirectoryExists(dir))
                                {
                                    fileProvider.CreateDirectory(dir);
                                }

                                var file = fileProvider.Combine(dir, string.Format("{0}.bin", auhtorizerId));
                                using Stream fs = new FileStream(file, FileMode.Create);

                                //这里存了整个对象，实际上只存RefreshToken也可以，有了RefreshToken就能刷新到最新的AccessToken
                                var binFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                                binFormat.Serialize(fs, refreshResult);
                                fs.Flush();
                                fs.Close();

                            }, "【盛派网络】开放平台");
                }
                
                //除此以外，仍然可以在程序任意地方注册开放平台：
                //ComponentContainer.Register();//命名空间：Senparc.Weixin.Open.Containers

                #endregion

                /* 微信配置结束 */

                #endregion
            });
        }

        /// <summary>
        /// Add Senparc MessageHandler Middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMessageHandlerMiddleware(this IApplicationBuilder application)
        {

        }

        /// <summary>
        /// 配置微信跟踪日志（演示，按需）
        /// </summary>
        private static void ConfigTraceLog()
        {
            //这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭

            //如果全局的IsDebug（Senparc.CO2NET.Config.IsDebug）为false，此处可以单独设置true，否则自动为true
            CO2NET.Trace.SenparcTrace.SendCustomLog("系统日志", "系统启动"); //只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            //全局自定义日志记录回调
            CO2NET.Trace.SenparcTrace.OnLogFunc = () =>
            {
                //加入每次触发Log后需要执行的代码
            };

            //当发生基于WeixinException的异常时触发
            WeixinTrace.OnWeixinExceptionFunc = async ex =>
            {
                //加入每次触发WeixinExceptionLog后需要执行的代码

                //发送模板消息给管理员
                //TODO (TERRY)EventService发送模板消息时，管理员openid错误导致循环发送消息，日志记录不断增长的BUG
                //var eventService = new Senparc.Weixin.MP.CommonService.EventService();
                //await eventService.ConfigOnWeixinExceptionFunc(ex);
            };
        }

        /// <summary>
        /// 判断当前配置是否满足使用 Redis（根据是否已经修改了默认配置字符串判断）
        /// </summary>
        /// <param name="senparcSetting"></param>
        /// <returns></returns>
        private static bool UseRedis(SenparcSetting senparcSetting, out string redisConfigurationStr)
        {
            redisConfigurationStr = senparcSetting.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != "#{Cache_Redis_Configuration}#"/*默认值，不启用*/;
            return useRedis;
        }

        /// <summary>
        /// 初步判断当前配置是否满足使用 Memcached（根据是否已经修改了默认配置字符串判断）
        /// </summary>
        /// <param name="senparcSetting"></param>
        /// <returns></returns>
        private static bool UseMemcached(SenparcSetting senparcSetting, out string memcachedConfigurationStr)
        {
            memcachedConfigurationStr = senparcSetting.Cache_Memcached_Configuration;
            var useMemcached = !string.IsNullOrEmpty(memcachedConfigurationStr) && memcachedConfigurationStr != "#{Cache_Memcached_Configuration}#";
            return useMemcached;
        }

        /// <summary>
        /// 获取扩展缓存策略
        /// </summary>
        /// <returns></returns>
        private static IList<IDomainExtensionCacheStrategy> GetExCacheStrategies(SenparcSetting senparcSetting)
        {
            var exContainerCacheStrategies = new List<IDomainExtensionCacheStrategy>();
            senparcSetting ??= new SenparcSetting();

            //注意：以下两个 if 判断仅作为演示，方便大家添加自定义的扩展缓存策略，

            #region 演示扩展缓存注册方法

            /*
            //判断Redis是否可用
            var redisConfiguration = senparcSetting.Cache_Redis_Configuration;
            if ((!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置"))
            {
                exContainerCacheStrategies.Add(RedisContainerCacheStrategy.Instance);//自定义的扩展缓存
            }
            //判断Memcached是否可用
            var memcachedConfiguration = senparcSetting.Cache_Memcached_Configuration;
            if ((!string.IsNullOrEmpty(memcachedConfiguration) && memcachedConfiguration != "Memcached配置"))
            {
                exContainerCacheStrategies.Add(MemcachedContainerCacheStrategy.Instance);//TODO:如果没有进行配置会产生异常
            }
            */

            #endregion

            //扩展自定义的缓存策略

            return exContainerCacheStrategies;
        }

    }
}
