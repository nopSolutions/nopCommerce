<<<<<<< HEAD
﻿namespace Nop.Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll answer
    /// </summary>
    public partial class PollAnswer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the poll identifier
        /// </summary>
        public int PollId { get; set; }

        /// <summary>
        /// Gets or sets the poll answer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current number of votes
        /// </summary>
        public int NumberOfVotes { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }        
    }
=======
﻿namespace Nop.Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll answer
    /// </summary>
    public partial class PollAnswer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the poll identifier
        /// </summary>
        public int PollId { get; set; }

        /// <summary>
        /// Gets or sets the poll answer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current number of votes
        /// </summary>
        public int NumberOfVotes { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }        
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}