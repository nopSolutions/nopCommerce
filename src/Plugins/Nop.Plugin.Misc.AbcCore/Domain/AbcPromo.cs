using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class AbcPromo : BaseEntity, ISlugSupported
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ManufacturerId { get; set; }

        public bool IsActive()
        {
            return StartDate <= DateTime.Now && DateTime.Now <= EndDate.AddDays(1);
        }

        public bool IsUpcoming()
        {
            return StartDate > DateTime.Now;
        }

        public bool IsExpired()
        {
            return DateTime.Now > EndDate.AddDays(1);
        }

        public string GetPdfPath()
        {
            return $"/promotion_forms/{Name}.pdf";
        }

        public string GetPopupCommand()
        {
            return $"javascript:win = window.open('{GetPdfPath()}', 'Promo', 'height=500,width=750,top=25,left=25,resizable=yes'); win.focus()";
        }

        public async Task<string> GetPromoBannerUrlAsync()
        {
            var bannerToFind = Name;
            var imageUrls = Directory.GetFiles(AbcPromosHelpers.GetPromoBannersFolder(), $"{bannerToFind}.*");

            if (!imageUrls.Any()) { return null; }
            if (imageUrls.Length > 1)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                await logger.WarningAsync($"Widgets.AbcPromos: Multiple banners found for promo {bannerToFind}, using first found.");
            }

            // Doing some cleanup for IIS and Linux based systems.
            return $"/promo_banners/{imageUrls[0].Split('/').Last().Replace("promo_banners\\", "")}";
        }
    }
}