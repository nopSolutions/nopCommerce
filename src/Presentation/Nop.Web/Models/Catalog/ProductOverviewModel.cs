using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record ProductOverviewModel : BaseNopEntityModel
{
    public ProductOverviewModel()
    {
        ProductPrice = new ProductPriceModel();
        PictureModels = new List<PictureModel>();
        ProductSpecificationModel = new ProductSpecificationModel();
        ReviewOverviewModel = new ProductReviewOverviewModel();
    }

    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public string SeName { get; set; }

    public string Sku { get; set; }

    public ProductType ProductType { get; set; }

    public bool MarkAsNew { get; set; }

    //price
    public ProductPriceModel ProductPrice { get; set; }
    //pictures
    public IList<PictureModel> PictureModels { get; set; }
    //specification attributes
    public ProductSpecificationModel ProductSpecificationModel { get; set; }
    //price
    public ProductReviewOverviewModel ReviewOverviewModel { get; set; }
}