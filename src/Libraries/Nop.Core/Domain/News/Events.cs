namespace Nop.Core.Domain.News
{
    /// <summary>
    /// News comment approved event
    /// </summary>
    public class NewsCommentApprovedEvent
    {
        public NewsCommentApprovedEvent(NewsComment newsComment)
        {
            this.NewsComment = newsComment;
        }

        /// <summary>
        /// News comment
        /// </summary>
        public NewsComment NewsComment { get; private set; }
    }
}