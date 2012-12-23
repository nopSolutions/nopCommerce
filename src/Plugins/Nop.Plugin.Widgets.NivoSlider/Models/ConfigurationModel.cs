using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.NivoSlider.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture1Id { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text1 { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link1 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture2Id { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text2 { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link2 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture3Id { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text3 { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link3 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture4Id { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text4 { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link4 { get; set; }
    }
}