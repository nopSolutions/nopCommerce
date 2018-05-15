using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PollMap : NopEntityTypeConfiguration<Poll>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PollMap()
        {
            this.ToTable("Poll");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired();
            
            this.HasRequired(p => p.Language)
                .WithMany()
                .HasForeignKey(p => p.LanguageId).WillCascadeOnDelete(true);
        }
    }
}