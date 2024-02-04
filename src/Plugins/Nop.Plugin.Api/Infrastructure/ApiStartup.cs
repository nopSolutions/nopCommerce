using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Authorization.Policies;
using Nop.Plugin.Api.Authorization.Requirements;
using Nop.Plugin.Api.Configuration;
using Nop.Plugin.Api.Delta;
using Nop.Web.Framework.Infrastructure.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nop.Plugin.Api.Infrastructure
{
	class DeltaSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (context.Type is Type type &&
				type.IsGenericType &&
				!type.IsGenericTypeDefinition &&
				type.GetGenericTypeDefinition() == typeof(Delta<>))
			{
				var underlyingType = type.GetGenericArguments()[0];
				var attribute = underlyingType.GetCustomAttributes(typeof(JsonObjectAttribute), inherit: false).Cast<JsonObjectAttribute>().FirstOrDefault();
				string title = attribute?.Title;
				if (title is not null)
				{
					string key = nameof(Delta<object>.Dto);
					if (schema.Properties.TryGetValue(key, out var dtoTypeSchema))
					{
						schema.Properties.Remove(key);
						schema.Properties[title] = dtoTypeSchema;
					}
				}
			}
		}
	}

	public class ApiStartup : INopStartup
	{
		public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{

			var apiConfigSection = configuration.GetSection("Api");

			if (apiConfigSection != null)
			{
				var apiConfig = Singleton<AppSettings>.Instance.Get<ApiConfiguration>();


				if (!string.IsNullOrEmpty(apiConfig.SecurityKey))
				{
					services.AddAuthentication(options =>
					{
						options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
						options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					})
							.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
							{
								jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
								{
									ValidateIssuerSigningKey = true,
									IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiConfig.SecurityKey)),
									ValidateIssuer = false, // ValidIssuer = "The name of the issuer",
									ValidateAudience = false, // ValidAudience = "The name of the audience",
									ValidateLifetime = true, // validate the expiration and not before values in the token
									ClockSkew = TimeSpan.FromMinutes(apiConfig.AllowedClockSkewInMinutes)
								};
							});

					JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
					AddAuthorizationPipeline(services);
					//services.AddHostedService<ApplicationPartsLogger>();

				}
			}
			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});
			services.Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});
			services.Configure<MvcNewtonsoftJsonOptions>(options =>
			{
				options.SerializerSettings.Converters.Add(new StringEnumConverter());
			});
			services.AddSwaggerGen(options =>
			{
				// api description >>
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Nop API", Version = "v1" });
				// auth definition >>
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter into field the word 'Bearer' following by space and JWT",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
				// custom type mappings >>
				options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" }); // correct currency typings
				options.SchemaFilter<DeltaSchemaFilter>();
				// TODO: options.UseAllOfToExtendReferenceSchemas(); // https://github.com/stepanbenes/api-for-nopcommerce/issues/16
			});
			services.AddSwaggerGenNewtonsoftSupport();


		}

		public void Configure(IApplicationBuilder app)
		{
			var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

			var rewriteOptions = new RewriteOptions().AddRewrite("api/token", "/token", true);
			app.UseRewriter(rewriteOptions);

			app.UseCors(x => x
							 .AllowAnyOrigin()
							 .AllowAnyMethod()
							 .AllowAnyHeader());

			// Need to enable rewind so we can read the request body multiple times
			//This should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream.
			//app.UseWhen(x => x.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
			//            builder =>
			//            {
			//                builder.Use(async (context, next) =>
			//                {
			//                    Console.WriteLine("API Call");
			//                    context.Request.EnableBuffering();
			//                    await next();
			//                });
			//            });

			app.MapWhen(context => context.Request.Path.StartsWithSegments(new PathString("/api")),
				a =>
				{
					if (environment.IsDevelopment())
					{
						a.UseDeveloperExceptionPage();
					}

					a.Use(async (context, next) =>
					{
						// API Call
						context.Request.EnableBuffering();
						await next();
					});

					a.UseExceptionHandler(a => a.Run(async context =>
					{
						var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
						var exception = exceptionHandlerPathFeature.Error;
						await context.Response.WriteAsJsonAsync(new { error = exception.Message });
					}));

					a.UseRouting();
					a.UseAuthentication();
					a.UseAuthorization();
					a.UseEndpoints(endpoints =>
					{
						endpoints.MapControllers();
					});

					// swagger configuration
					{
						// http://eatcodelive.com/2017/05/19/change-default-swagger-route-in-an-asp-net-core-web-api/

						a.UseSwagger(options => options.RouteTemplate = "api/swagger/{documentName}/swagger.json");
						a.UseSwaggerUI(c =>
						{
							c.RoutePrefix = "api/swagger";
							c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Nop.Plugin.Api v4.40");
							c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
						});
					}
				}
			);
		}

		public int Order => 1;

		private static void AddAuthorizationPipeline(IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
								  policy =>
								  {
									  policy.Requirements.Add(new ActiveApiPluginRequirement());
									  policy.Requirements.Add(new AuthorizationSchemeRequirement());
									  policy.Requirements.Add(new CustomerRoleRequirement());
									  policy.RequireAuthenticatedUser();
								  });
			});

			services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
			services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
			services.AddSingleton<IAuthorizationHandler, CustomerRoleAuthorizationPolicy>();

		}
	}
}
