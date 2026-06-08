using Nop.Data.Mapping;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Mapping;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class ForumNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new() {
        { typeof(ForumGroup), "Forums_Group" },
        { typeof(Forum), "Forums_Forum" },
        { typeof(ForumPost), "Forums_Post" },
        { typeof(ForumPostVote), "Forums_PostVote" },
        { typeof(ForumSubscription), "Forums_Subscription" },
        { typeof(ForumTopic), "Forums_Topic" }
    };
    public Dictionary<(Type, string), string> ColumnName => [];
}
