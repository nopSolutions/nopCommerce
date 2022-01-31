using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Middleware;

namespace Nop.Web
{
	/// <summary>
	/// Represents startup class of application
	/// </summary>
	public class Startup
	{
		#region Fields

		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _webHostEnvironment;

		#endregion

		#region Ctor

		public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
		{
			_configuration = configuration;
			_webHostEnvironment = webHostEnvironment;
		}

		#endregion

		/// <summary>
		/// Add services to the application and configure service provider
		/// </summary>
		/// <param name="services">Collection of service descriptors</param>
		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
			services.AddSwaggerGen(swagger =>
			{
				//This is to generate the Default UI of Swagger Documentation  
				swagger.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "JWT Token Authentication API",
					Description = "ASP.NET Core 3.1 Web API"
				});
				// To Enable authorization using Swagger (JWT)  
				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
				});
				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
						new string[] {}
					}
				});
			});

			services
                .AddAuthentication(option => {
				    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
				    options.TokenValidationParameters = new TokenValidationParameters {
					    ValidateIssuer = true,
					    ValidateAudience = true,
					    ValidateLifetime = false,
					    ValidateIssuerSigningKey = true,
					    ValidIssuer = _configuration["Jwt:Issuer"],
					    ValidAudience = _configuration["Jwt:Issuer"],
					    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
				    };
			    });
			services.AddTokenAuthentication(_configuration);

			// configure strongly typed settings object
			services.Configure<JwtSettings>(_configuration.GetSection("Jwt"));		
        }

		/// <summary>
		/// Configure the application HTTP request pipeline
		/// </summary>
		/// <param name="application">Builder for configuring an application's request pipeline</param>
		public void Configure(IApplicationBuilder application)
		{
			application.UseSwagger();
			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			application.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Snacks LLC API");
				c.RoutePrefix = "swagger";
			});
			 // global cors policy
			application.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());

            // custom jwt auth middleware
            application.UseMiddleware<JwtMiddleware>();
            application.UseStatusCodePages(context => {
                var response = context.HttpContext.Response;
                if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                    response.StatusCode == (int)HttpStatusCode.Forbidden)
                    response.Redirect("/Login");
                return Task.CompletedTask;
            });
            application.ConfigureRequestPipeline();
			application.StartEngine();
		}
	}
}