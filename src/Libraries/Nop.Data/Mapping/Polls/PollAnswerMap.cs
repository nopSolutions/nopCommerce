using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PollAnswerMap : NopEntityTypeConfiguration<PollAnswer>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PollAnswerMap()
        {
            this.ToTable("PollAnswer");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Name).IsRequired();

            this.HasRequired(pa => pa.Poll)
                .WithMany(p => p.PollAnswers)
                .HasForeignKey(pa => pa.PollId).WillCascadeOnDelete(true);
        }
    }
}