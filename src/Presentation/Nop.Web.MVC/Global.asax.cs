using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Web.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DbDatabase.SetInitializer<NopObjectContext>(new DatabaseInitializer());



            using (Nop.Data.NopObjectContext context = new Data.NopObjectContext("NopSqlConnection"))
            {
                Nop.Data.EfRepository<Nop.Core.Domain.Language> repo1 = new Data.EfRepository<Core.Domain.Language>(context);
                Nop.Data.EfRepository<Nop.Core.Domain.LocaleStringResource> repo2 = new Data.EfRepository<Core.Domain.LocaleStringResource>(context);
                Nop.Data.EfRepository<Nop.Core.Domain.Log> repo3 = new Data.EfRepository<Core.Domain.Log>(context);

                var cacheManager = new Nop.Core.Caching.NopNullCache();

                Nop.Services.LanguageService service1 = new Services.LanguageService(cacheManager, repo1);
                Nop.Services.LocalizationService service2 = new Services.LocalizationService(cacheManager, repo2);
                Nop.Services.Logging.ILogger logger = new Nop.Services.Logging.DefaultLogger(repo3);

                var list = service1.GetAllLanguages();
                var l1 = service1.GetLanguageById(7);
                var l2 = service1.GetLanguageById(8);
                var l3 = new Nop.Core.Domain.Language()
                {
                    Name = "test1",
                    LanguageCulture = "en-US",
                    FlagImageFileName = "1",
                    DisplayOrder = 2,
                    Published = true
                };
                service1.InsertLanguage(l3);

                var lsr1 = new Nop.Core.Domain.LocaleStringResource()
                {
                    Language = l3,
                    ResourceName = "1",
                    ResourceValue = "2"
                };
                service2.InsertLocaleStringResource(lsr1);

                logger.Debug("test");
                logger.Error("test");
                logger.GetAllLogs(null, null, string.Empty, null, 0, 10);

            }
        }
    }
}