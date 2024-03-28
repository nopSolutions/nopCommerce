using Nop.Web.Framework.Models;
namespace Nop.Web.Models.Catalog;

public partial record PopularTopicTagsModel : BaseNopModel
{
    public PopularTopicTagsModel()
    {
        Tags = new List<TopicTagModel>();
    }

#region Utilities

protected virtual int GetFontSize(double weight, double mean, double stdDev)
{
    var factor = (weight - mean);

    if (factor != 0 && stdDev != 0)
        factor /= stdDev;

    return (factor > 2) ? 150 :
        (factor > 1) ? 120 :
        (factor > 0.5) ? 100 :
        (factor > -0.5) ? 90 :
        (factor > -1) ? 85 :
        (factor > -2) ? 80 :
        75;
}

protected virtual double Mean(IEnumerable<double> values)
{
    double sum = 0;
    var count = 0;

    foreach (var d in values)
    {
        sum += d;
        count++;
    }

    if (count == 0)
        return 0;
    return sum / count;
}

protected virtual double StdDev(IEnumerable<double> values, out double mean)
{
    mean = Mean(values);
    double sumOfDiffSquares = 0;
    var count = 0;

    foreach (var d in values)
    {
        var diff = (d - mean);
        sumOfDiffSquares += diff * diff;
        count++;
    }

    if (count == 0)
        return 0;
    return Math.Sqrt(sumOfDiffSquares / count);
}

#endregion

#region Methods

public virtual int GetFontSize(TopicTagModel topicTag)
{
    var itemWeights = new List<double>();
    foreach (var tag in Tags)
        itemWeights.Add(tag.TopicCount);
    var stdDev = StdDev(itemWeights, out double mean);

    return GetFontSize(topicTag.TopicCount, mean, stdDev);
}

#endregion

#region Properties

public int TotalTags { get; set; }

public List<TopicTagModel> Tags { get; set; }

#endregion
}
