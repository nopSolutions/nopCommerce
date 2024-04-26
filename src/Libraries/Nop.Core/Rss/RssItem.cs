using System.Xml.Linq;

namespace Nop.Core.Rss;

/// <summary>
/// Represents the item of RSS feed
/// </summary>
public partial class RssItem
{
    /// <summary>
    /// Initialize new instance of RSS feed item
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="content">Content</param>
    /// <param name="link">Link</param>
    /// <param name="id">Unique identifier</param>
    /// <param name="pubDate">Last build date</param>
    public RssItem(string title, string content, Uri link, string id, DateTimeOffset pubDate)
    {
        Title = new XElement(NopRssDefaults.Title, title);
        Content = new XElement(NopRssDefaults.Description, content);
        Link = new XElement(NopRssDefaults.Link, link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
    }

    /// <summary>
    /// Initialize new instance of RSS feed item
    /// </summary>
    /// <param name="item">XML view of rss item</param>
    public RssItem(XContainer item)
    {
        var title = item.Element(NopRssDefaults.Title)?.Value ?? string.Empty;
        var content = item.Element(NopRssDefaults.Content)?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(content))
            content = item.Element(NopRssDefaults.Description)?.Value ?? string.Empty;
        var link = new Uri(item.Element(NopRssDefaults.Link)?.Value ?? string.Empty);
        var pubDateValue = item.Element(NopRssDefaults.PubDate)?.Value;
        var pubDate = pubDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(pubDateValue, "r", null);
        var id = item.Element(NopRssDefaults.Guid)?.Value ?? string.Empty;

        Title = new XElement(NopRssDefaults.Title, title);
        Content = new XElement(NopRssDefaults.Description, content);
        Link = new XElement(NopRssDefaults.Link, link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
    }

    #region Methods

    /// <summary>
    /// Get representation item of RSS feed as XElement object
    /// </summary>
    /// <returns></returns>
    public XElement ToXElement()
    {
        var element = new XElement(NopRssDefaults.Item, Id, Link, Title, Content);

        foreach (var elementExtensions in ElementExtensions)
        {
            element.Add(elementExtensions);
        }

        return element;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Title
    /// </summary>
    public XElement Title { get; }

    /// <summary>
    /// Get title text
    /// </summary>
    public string TitleText => Title?.Value ?? string.Empty;

    /// <summary>
    /// Content
    /// </summary>
    public XElement Content { get; }

    /// <summary>
    /// Link
    /// </summary>
    public XElement Link { get; }

    /// <summary>
    /// Get URL
    /// </summary>
    public Uri Url => new(Link.Value);

    /// <summary>
    /// Unique identifier
    /// </summary>
    public XElement Id { get; }

    /// <summary>
    /// Last build date
    /// </summary>
    public XElement PubDate { get; }

    /// <summary>
    /// Publish date
    /// </summary>
    public DateTimeOffset PublishDate => PubDate?.Value == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(PubDate.Value, "r", null);

    /// <summary>
    /// Element extensions
    /// </summary>
    public List<XElement> ElementExtensions { get; } = new List<XElement>();

    #endregion
}