using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
{
    var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
    builder.Configuration.AddJsonFile(path, true, true);
}
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<CookiePolicyOptions>(options => { options.CheckConsentNeeded = context => true; options.MinimumSameSitePolicy = SameSiteMode.None; options.Secure = CookieSecurePolicy.Always; });
//load application settings
builder.Services.ConfigureApplicationSettings(builder);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
               .AddCookie(options =>
               {
                   options.CookieManager = new ChunkingCookieManager();
                   options.Cookie.SameSite = SameSiteMode.None;
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
               });

//pwa
builder.Services.AddProgressiveWebApp();

var appSettings = Singleton<AppSettings>.Instance;
var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;

if (useAutofac)
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
else
    builder.Host.UseDefaultServiceProvider(options =>
    {
        //we don't validate the scopes, since at the app start and the initial configuration we need 
        //to resolve some services (registered as "scoped") through the root container
        options.ValidateScopes = false;
        options.ValidateOnBuild = true;
    });

//add services to the application and configure service provider
builder.Services.ConfigureApplicationServices(builder);



var app = builder.Build();

//configure the application HTTP request pipeline
app.ConfigureRequestPipeline();
app.StartEngine();
app.UseAuthentication();
app.UseCookiePolicy();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.Run();
