namespace Nop.Core.Domain.Books;

/// <summary>
/// Represents a blog post tag
/// </summary>
public partial class Book : BaseEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedOn { get; set; }
}