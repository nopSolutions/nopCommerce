using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a templates model
/// </summary>
public partial record TemplatesModel : BaseNopModel
{
    #region Ctor

    public TemplatesModel()
    {
        TemplatesCategory = new CategoryTemplateSearchModel();
        TemplatesManufacturer = new ManufacturerTemplateSearchModel();
        TemplatesProduct = new ProductTemplateSearchModel();
        TemplatesTopic = new TopicTemplateSearchModel();

        AddCategoryTemplate = new CategoryTemplateModel();
        AddManufacturerTemplate = new ManufacturerTemplateModel();
        AddProductTemplate = new ProductTemplateModel();
        AddTopicTemplate = new TopicTemplateModel();
    }

    #endregion

    #region Properties

    public CategoryTemplateSearchModel TemplatesCategory { get; set; }

    public ManufacturerTemplateSearchModel TemplatesManufacturer { get; set; }

    public ProductTemplateSearchModel TemplatesProduct { get; set; }

    public TopicTemplateSearchModel TemplatesTopic { get; set; }

    public CategoryTemplateModel AddCategoryTemplate { get; set; }

    public ManufacturerTemplateModel AddManufacturerTemplate { get; set; }

    public ProductTemplateModel AddProductTemplate { get; set; }

    public TopicTemplateModel AddTopicTemplate { get; set; }

    #endregion
}