namespace Nop.Core.Domain.News
{
    /// <summary>
    /// News comment approved event
    /// </summary>
    public class NewsCommentApprovedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="newsComment">News comment</param>
        public NewsCommentApprovedEvent(NewsComment newsComment)
        {
            this.NewsComment = newsComment;
        }

        /// <summary>
        /// News comment
        /// </summary>
        public NewsComment NewsComment { get; }
    }
}