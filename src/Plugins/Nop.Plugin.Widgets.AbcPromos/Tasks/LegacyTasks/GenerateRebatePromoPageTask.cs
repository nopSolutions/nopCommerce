using Nop.Services.Tasks;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Core.Domain.Topics;
using Nop.Services.Topics;
using Nop.Services.Seo;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System.Linq;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Catalog;
using System.Text;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Services.Logging;
using Nop.Plugin.Widgets.AbcPromos;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Widgets.AbcPromos.Tasks.LegacyTasks
{
    public class GenerateRebatePromoPageTask : IScheduleTask
    {
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductAbcDescription> _productAbcDescriptionRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

        private readonly IAbcPromoService _abcPromoService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;

        private readonly ILogger _logger;

        private readonly MediaSettings _mediaSettings;
        private readonly AbcPromosSettings _settings;

        public GenerateRebatePromoPageTask(
            IRepository<Topic> topicRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAbcDescription> productAbcDescriptionRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IAbcPromoService abcPromoService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IProductService productService,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            MediaSettings mediaSettings,
            AbcPromosSettings settings
        )
        {
            _topicRepository = topicRepository;
            _productRepository = productRepository;
            _productAbcDescriptionRepository = productAbcDescriptionRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _abcPromoService = abcPromoService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _productService = productService;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _logger = logger;
            _mediaSettings = mediaSettings;
            _settings = settings;
        }

        public async Task ExecuteAsync()
        {
            var rebatePromoTopicName = "Rebates and Promos";
            var topic = await _topicService.GetTopicBySystemNameAsync(rebatePromoTopicName);
            if (topic == null)
            {
                topic = new Topic
                {
                    SystemName = rebatePromoTopicName,
                    IncludeInFooterColumn1 = true,
                    LimitedToStores = false,
                    Title = rebatePromoTopicName,
                    TopicTemplateId = 1,
                    Published = true
                };
                await _topicService.InsertTopicAsync(topic);
                await _urlRecordService.SaveSlugAsync(topic, await _urlRecordService.ValidateSeNameAsync(topic, rebatePromoTopicName, topic.Title, true), 0);
            }

            topic.Body = await GetRebatePromoHtmlAsync(topic);
            await _topicService.UpdateTopicAsync(topic);
        }

        private async System.Threading.Tasks.Task<string> GetRebatePromoHtmlAsync(Topic rootTopic)
        {
            var html = $"<h2 class=\"abc-rebate-promo-title\">" +
                        "Promos</h2><div class=\"abc-container abc-promo-container\">";

            var promos = _settings.IncludeExpiredPromosOnRebatesPromosPage ?
                            (await _abcPromoService.GetActivePromosAsync()).Union(await _abcPromoService.GetExpiredPromosAsync()) :
                            await _abcPromoService.GetActivePromosAsync();
            foreach (var promo in promos)
            {
                var publishedPromoProducts = await _abcPromoService.GetPublishedProductsByPromoIdAsync(promo.Id);
                if (!publishedPromoProducts.Any())
                {
                    continue;
                }

                var promoDescription = promo.ManufacturerId != null ?
                                $"{(await _manufacturerService.GetManufacturerByIdAsync(promo.ManufacturerId.Value)).Name} - {promo.Description}" :
                                promo.Description;
                html += $"<div class=\"abc-item abc-promo-item\"> " +
                        $"<a href=\"/promos/{await _urlRecordService.GetActiveSlugAsync(promo.Id, "AbcPromo", 0)}\"> " +
                        $"{promoDescription}</a><br />" +
                        $"Expires {promo.EndDate.ToString("MM-dd-yy")}" +
                        "</div>";
            }

            html += "</div>";

            return html;
        }
    }
}