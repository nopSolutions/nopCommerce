namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represent a sample data for installation process
/// </summary>
public partial class SampleData
{
    /// <summary>
    /// Gets or sets the list of customers
    /// </summary>
    public List<SampleCustomer> Customers { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of checkout attributes
    /// </summary>
    public List<SampleCheckoutAttribute> CheckoutAttributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of specification attributes
    /// </summary>
    public SampleSpecificationAttributes SpecificationAttributes { get; set; }

    /// <summary>
    /// Gets or sets the list of product attributes
    /// </summary>
    public List<SampleProductAttribute> ProductAttributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of categories
    /// </summary>
    public List<SampleCategory> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of manufacturers
    /// </summary>
    public List<SampleManufacturer> Manufacturers { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of products
    /// </summary>
    public SampleProducts Products { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of forum groups
    /// </summary>
    public List<SampleForumGroup> ForumGroups { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of discounts
    /// </summary>
    public List<SampleDiscount> Discounts { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of blog posts
    /// </summary>
    public List<SampleBlogPost> BlogPosts { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of news items
    /// </summary>
    public List<SampleNewsItem> NewsItems { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of polls
    /// </summary>
    public List<SamplePoll> Polls { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of warehouses
    /// </summary>
    public List<SampleWarehouse> Warehouses { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of vendors
    /// </summary>
    public List<SampleVendor> Vendors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of affiliates
    /// </summary>
    public List<SampleAffiliate> Affiliates { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of orders
    /// </summary>
    public List<SampleOrder> Orders { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of activity logs
    /// </summary>
    public List<SampleActivityLog> ActivityLogs { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of search terms
    /// </summary>
    public List<SampleSearchTerm> SearchTerms { get; set; } = new();
}
