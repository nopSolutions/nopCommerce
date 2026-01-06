using System.Xml.Linq;

namespace Nop.Plugin.Theme.KungFu.Seeding.JkooSword;

public class SiteMapModel
{
    public string Url { get; set;  } = "https://jkoosword.com/";
    public string Priority { get; set;  } = "1.0";
    public string ChangeFrequency { get; set;  } = "weekly";
    
}

public static class SiteMapModelExtensions
{
    public static SiteMapModel[] ToSiteMap(this string siteMap)
    {
        var siteMapModels = new List<SiteMapModel>();
        // Split the sitemap on spaces
        var parts = siteMap.Split(' ');
        var nextPartStartIndex = 0;
        var numberOfProperties = parts.Length / 3;
        for (var i = 0; i < numberOfProperties; i++)
        {
            // skip 0 items take 3
            // skip 3 items take 3
            // skip 6 items take 3 etc
            var property = parts.Skip(nextPartStartIndex).Take(3).ToArray();
            // index 0,1,2
            nextPartStartIndex += 3;
            var model = new SiteMapModel
            {
                Url = property[0],
                Priority = property[1],
                ChangeFrequency = property[2]
            };
            siteMapModels.Add(model);
        }
        
        return siteMapModels.ToArray();
    }
    
    public static SiteMapModel[] ToSiteMap(this XDocument siteMap)
    {
        var siteMapModels = new List<SiteMapModel>();
        var urlElements = siteMap.Descendants().Where(e => e.Name.LocalName == "url");
        foreach (var urlElement in urlElements)
        {
            var loc = urlElement.Elements().FirstOrDefault(e => e.Name.LocalName == "loc")?.Value;
            var priority = urlElement.Elements().FirstOrDefault(e => e.Name.LocalName == "priority")?.Value;
            var changeFreq = urlElement.Elements().FirstOrDefault(e => e.Name.LocalName == "changefreq")?.Value;

            if (loc != null && priority != null && changeFreq != null)
            {
                var model = new SiteMapModel
                {
                    Url = loc,
                    Priority = priority,
                    ChangeFrequency = changeFreq
                };
                siteMapModels.Add(model);
            }
        }

        return siteMapModels.ToArray();
    }
}
