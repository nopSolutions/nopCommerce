using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NivoSlider.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture1Id { get; set; }
        public bool Picture1Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        public string Text1 { get; set; }
        public bool Text1_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        public string Link1 { get; set; }
        public bool Link1_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.AltText")]
        public string AltText1 { get; set; }
        public bool AltText1_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture2Id { get; set; }
        public bool Picture2Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        public string Text2 { get; set; }
        public bool Text2_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        public string Link2 { get; set; }
        public bool Link2_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.AltText")]
        public string AltText2 { get; set; }
        public bool AltText2_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture3Id { get; set; }
        public bool Picture3Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        public string Text3 { get; set; }
        public bool Text3_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        public string Link3 { get; set; }
        public bool Link3_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.AltText")]
        public string AltText3 { get; set; }
        public bool AltText3_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture4Id { get; set; }
        public bool Picture4Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        public string Text4 { get; set; }
        public bool Text4_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        public string Link4 { get; set; }
        public bool Link4_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.AltText")]
        public string AltText4 { get; set; }
        public bool AltText4_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int Picture5Id { get; set; }
        public bool Picture5Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        public string Text5 { get; set; }
        public bool Text5_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        public string Link5 { get; set; }
        public bool Link5_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.AltText")]
        public string AltText5 { get; set; }
        public bool AltText5_OverrideForStore { get; set; }
    }
}