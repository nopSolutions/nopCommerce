using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;
using Nop.Services.Topics;
using Nop.Web.Framework.Themes;
using Nop.Web.Infrastructure.Installations;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests;

[TestFixture]
public class KungFuThemeStartupTests
{
    [Test]
    public void Should_seed_kung_fu_topics_when_theme_is_active()
    {
        var originalDataSettings = Singleton<DataConfig>.Instance;
        var originalAppSettings = Singleton<AppSettings>.Instance;
        Singleton<DataConfig>.Instance = new DataConfig
        {
            ConnectionString = "ConnectionString"
        };
        Singleton<AppSettings>.Instance = new AppSettings(new List<IConfig>
        {
            new DataConfig
            {
                ConnectionString = "ConnectionString"
            }
        });

        var fileProvider = new Mock<INopFileProvider>();
        fileProvider.Setup(m => m.MapPath(It.IsAny<string>()))
            .Returns((string path) => path);
        fileProvider.Setup(m => m.Combine(It.IsAny<string[]>()))
            .Returns((string[] paths) => string.Join('/', paths));
        fileProvider.Setup(m => m.FileExists(It.Is<string>(p => p.Contains("Themes/KungFu/Content/topics"))))
            .Returns(true);
        fileProvider.Setup(m => m.FileExists(It.Is<string>(p => !p.Contains("Themes/KungFu/Content/topics"))))
            .Returns(false);
        fileProvider.Setup(m => m.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<Encoding>()))
            .ReturnsAsync("body");

        var originalFileProvider = CommonHelper.DefaultFileProvider;
        CommonHelper.DefaultFileProvider = fileProvider.Object;

        try
        {
            var topicService = new Mock<ITopicService>();
            topicService.Setup(m => m.GetTopicBySystemNameAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Topic)null);
            topicService.Setup(m => m.InsertTopicAsync(It.IsAny<Topic>()))
                .Returns(Task.CompletedTask);

            var topicTemplateService = new Mock<ITopicTemplateService>();
            topicTemplateService.Setup(m => m.GetAllTopicTemplatesAsync())
                .ReturnsAsync(new List<TopicTemplate> { new() { Id = 3, DisplayOrder = 1 } });

            var themeContext = new Mock<IThemeContext>();
            themeContext.Setup(m => m.GetWorkingThemeNameAsync()).ReturnsAsync("KungFu");

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddScoped(_ => fileProvider.Object);
            services.AddScoped(_ => topicService.Object);
            services.AddScoped(_ => topicTemplateService.Object);
            services.AddScoped(_ => themeContext.Object);
            services.AddScoped(_ => Mock.Of<ILogger<KungFuTopicSeeder>>());
            services.AddScoped<KungFuTopicSeeder>();

            var serviceProvider = services.BuildServiceProvider();
            var applicationBuilder = new ApplicationBuilder(serviceProvider);

            var startup = new KungFuThemeStartup();
            startup.Configure(applicationBuilder);

            topicService.Verify(m => m.InsertTopicAsync(It.IsAny<Topic>()), Times.AtLeastOnce);
        }
        finally
        {
            Singleton<DataConfig>.Instance = originalDataSettings;
            Singleton<AppSettings>.Instance = originalAppSettings;
            CommonHelper.DefaultFileProvider = originalFileProvider;
        }
    }

    [Test]
    public void Should_reload_data_settings_before_seeding_topics()
    {
        var originalDataSettings = Singleton<DataConfig>.Instance;
        var originalAppSettings = Singleton<AppSettings>.Instance;
        Singleton<DataConfig>.Instance = new DataConfig
        {
            ConnectionString = string.Empty
        };
        Singleton<AppSettings>.Instance = new AppSettings(new List<IConfig>
        {
            new DataConfig
            {
                ConnectionString = "ConnectionString"
            }
        });

        var fileProvider = new Mock<INopFileProvider>();
        fileProvider.Setup(m => m.MapPath(It.IsAny<string>()))
            .Returns((string path) => path);
        fileProvider.Setup(m => m.Combine(It.IsAny<string[]>()))
            .Returns((string[] paths) => string.Join('/', paths));
        fileProvider.Setup(m => m.FileExists(It.Is<string>(p => p.Contains("Themes/KungFu/Content/topics"))))
            .Returns(true);
        fileProvider.Setup(m => m.FileExists(It.Is<string>(p => !p.Contains("Themes/KungFu/Content/topics"))))
            .Returns(false);
        fileProvider.Setup(m => m.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<Encoding>()))
            .ReturnsAsync("body");

        var originalFileProvider = CommonHelper.DefaultFileProvider;
        CommonHelper.DefaultFileProvider = fileProvider.Object;

        try
        {
            var topicService = new Mock<ITopicService>();
            topicService.Setup(m => m.GetTopicBySystemNameAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Topic)null);
            topicService.Setup(m => m.InsertTopicAsync(It.IsAny<Topic>()))
                .Returns(Task.CompletedTask);

            var topicTemplateService = new Mock<ITopicTemplateService>();
            topicTemplateService.Setup(m => m.GetAllTopicTemplatesAsync())
                .ReturnsAsync(new List<TopicTemplate> { new() { Id = 3, DisplayOrder = 1 } });

            var themeContext = new Mock<IThemeContext>();
            themeContext.Setup(m => m.GetWorkingThemeNameAsync()).ReturnsAsync("KungFu");

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddScoped(_ => fileProvider.Object);
            services.AddScoped(_ => topicService.Object);
            services.AddScoped(_ => topicTemplateService.Object);
            services.AddScoped(_ => themeContext.Object);
            services.AddScoped(_ => Mock.Of<ILogger<KungFuTopicSeeder>>());
            services.AddScoped<KungFuTopicSeeder>();

            var serviceProvider = services.BuildServiceProvider();
            var applicationBuilder = new ApplicationBuilder(serviceProvider);

            var startup = new KungFuThemeStartup();
            startup.Configure(applicationBuilder);

            topicService.Verify(m => m.InsertTopicAsync(It.IsAny<Topic>()), Times.AtLeastOnce);
        }
        finally
        {
            Singleton<DataConfig>.Instance = originalDataSettings;
            Singleton<AppSettings>.Instance = originalAppSettings;
            CommonHelper.DefaultFileProvider = originalFileProvider;
        }
    }
}
