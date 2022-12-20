namespace Nop.Core.Domain.News
{
    /// <summary>
    /// News comment approved event
    /// </summary>
    public partial class NewsCommentApprovedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="newsComment">News comment</param>
        public NewsCommentApprovedEvent(NewsComment newsComment)
        {
            NewsComment = newsComment;
        }

        /// <summary>
        /// News comment
        /// </summary>
        public NewsComment NewsComment { get; }
    }
}