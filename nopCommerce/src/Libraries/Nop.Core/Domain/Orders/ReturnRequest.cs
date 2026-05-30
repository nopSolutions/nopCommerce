namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents a return request
/// </summary>
public partial class ReturnRequest : BaseEntity
{
    /// <summary>
    /// Custom number of return request
    /// </summary>
    public string CustomNumber { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the order item identifier
    /// </summary>
    public int OrderItemId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the quantity returned to stock
    /// </summary>
    public int ReturnedQuantity { get; set; }

    /// <summary>
    /// Gets or sets the reason to return
    /// </summary>
    public string ReasonForReturn { get; set; }

    /// <summary>
    /// Gets or sets the requested action
    /// </summary>
    public string RequestedAction { get; set; }

    /// <summary>
    /// Gets or sets the customer comments
    /// </summary>
    public string CustomerComments { get; set; }

    /// <summary>
    /// Gets or sets identifier of the file (Download) uploaded by the customer
    /// </summary>
    public int UploadedFileId { get; set; }

    /// <summary>
    /// Gets or sets the staff notes
    /// </summary>
    public string StaffNotes { get; set; }

    /// <summary>
    /// Gets or sets the return status identifier
    /// </summary>
    public int ReturnRequestStatusId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of entity creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of entity update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the return status
    /// </summary>
    public ReturnRequestStatus ReturnRequestStatus
    {
        get => (ReturnRequestStatus)ReturnRequestStatusId;
        set => ReturnRequestStatusId = (int)value;
    }
}