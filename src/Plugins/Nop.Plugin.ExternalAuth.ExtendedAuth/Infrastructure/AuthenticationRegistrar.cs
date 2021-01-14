using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Infrastructure
{
    /// <summary>
    /// Registration of authentication service (plugin)
    /// </summary>
    public class AuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            // facebook
            builder.AddFacebook(AuthenticationDefaults.FacebookAuthenticationScheme, options =>
            {
                var currentstore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                var storeScope = currentstore.Id;
                var _settingService = EngineContext.Current.Resolve<ISettingService>();
                var settings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);
                options.ClientId = settings.FacebookClientId;
                options.ClientSecret = settings.FacebookClientSecret;
                options.SaveTokens = true;

            });

            // Twitter
            builder.AddTwitter(TwitterDefaults.AuthenticationScheme, options =>
              {
                  var currentstore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                  var storeScope = currentstore.Id;
                  var _settingService = EngineContext.Current.Resolve<ISettingService>();
                  var settings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);
                  options.ConsumerKey = settings.TwitterClientId;
                  options.ConsumerSecret = settings.TwitterClientSecret;
                  options.SaveTokens = true;
              });

            // Google
            builder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                var storeScope = EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id;
                var _settingService = EngineContext.Current.Resolve<ISettingService>();
                var settings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);
                options.ClientId = settings.GoogleClientId;
                options.ClientSecret = settings.GoogleClientSecret;
                options.SaveTokens = true;
            });

            // Microsoft
            builder.AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                var storeScope = EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id;
                var _settingService = EngineContext.Current.Resolve<ISettingService>();
                var settings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);
                options.ClientId = settings.MicrosoftClientId;
                options.ClientSecret = settings.MicrosoftClientSecret;
                options.SaveTokens = true;
            });

            // linked in             
            builder.AddOAuth(AuthenticationDefaults.LinkedInAuthenticationScheme, options =>
            {
                var currentstore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                var storeScope = currentstore.Id;
                var _settingService = EngineContext.Current.Resolve<ISettingService>();
                var settings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);
                options.ClientId = settings.LinkedInClientId;
                options.ClientSecret = settings.LinkedInClientSecret;

                options.CallbackPath = "/signin-linkedin";
                options.ClaimsIssuer = AuthenticationDefaults.LinkedInAuthenticationScheme;
                options.SaveTokens = true;

                options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization"; //"https://www.linkedin.com/oauth/v2/authorization";
                options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
                options.UserInformationEndpoint = "https://api.linkedin.com/v2/me"; //"https://api.linkedin.com/v2/me";                
                options.Scope.Add("r_liteprofile");
                options.Scope.Add("r_emailaddress");
                options.Events = new OAuthEvents
                {
                    // The OnCreatingTicket event is called after the user has been authenticated and the OAuth middleware has
                    // created an auth ticket. We need to manually call the UserInformationEndpoint to retrieve the user's information,
                    // parse the resulting JSON to extract the relevant information, and add the correct claims.
                    //OnCreatingTicket = async context => { await CreateAuthTicket(context); }
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Add("x-li-format", "json");

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        var userId = user.Value<string>("id");
                        if (!string.IsNullOrEmpty(userId))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }

                        var formattedName = user.Value<string>("formattedName");
                        if (!string.IsNullOrEmpty(formattedName))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Name, formattedName, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }

                        var email = user.Value<string>("emailAddress");
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String,
                                context.Options.ClaimsIssuer));
                        }
                        var pictureUrl = user.Value<string>("pictureUrl");
                        if (!string.IsNullOrEmpty(pictureUrl))
                        {
                            context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profile-picture", pictureUrl, ClaimValueTypes.String,
                                context.Options.ClaimsIssuer));
                        }

                        string name = string.Empty;
                        var firstName = user.Value<string>("localizedFirstName");
                        if (!string.IsNullOrEmpty(firstName))
                        {
                            context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/firstName", firstName, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            name = firstName;
                        }

                        var lastName = user.Value<string>("localizedLastName");
                        if (!string.IsNullOrEmpty(lastName))
                        {
                            context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/lastName", lastName, ClaimValueTypes.String,
                                context.Options.ClaimsIssuer));
                            name = firstName + " " + lastName;
                        }

                        if (!string.IsNullOrEmpty(name))
                            context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", name, ClaimValueTypes.String, context.Options.ClaimsIssuer));

                        // for emailid we have to make seperate call into linkedin
                        if (string.IsNullOrEmpty(email))
                        {
                            request = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))");
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Add("x-li-format", "json");

                            response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();
                            user = JObject.Parse(await response.Content.ReadAsStringAsync());
                            email = (string)user.SelectToken("elements[0].handle~.emailAddress");
                            if (!string.IsNullOrEmpty(email))
                            {
                                context.Identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String,
                                    context.Options.ClaimsIssuer));
                            }
                        }

                    }
                };
                //https://stackoverflow.com/questions/46016229/addoauth-linkedin-dotnet-core-2-0/46064936
            });
        }
    }    
}



