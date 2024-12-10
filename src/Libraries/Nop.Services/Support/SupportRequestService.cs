using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Support;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Services.Support;

public class SupportRequestService : ISupportRequestService
{
    #region Ctor

    public SupportRequestService(
        IRepository<SupportRequest> supportRequestRepository,
        IRepository<SupportMessage> supportMessageRepository,
        ICustomerService customerService,
        ILogger logger)
    {
        _supportRequestRepository = supportRequestRepository;
        _supportMessageRepository = supportMessageRepository;
        _customerService = customerService;
        _logger = logger;
    }

    #endregion

    #region Fields

    protected readonly IRepository<SupportRequest> _supportRequestRepository;
    protected readonly IRepository<SupportMessage> _supportMessageRepository;
    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;

    #endregion

    #region Requests

    /// <summary>
    ///     Asynchronously creates a new support request in the database.
    /// </summary>
    /// <param name="request">
    ///     The support request object to be created. Must not be null and must contain a valid subject and
    ///     customer ID.
    /// </param>
    /// <returns>
    ///     A <see cref="SupportRequestResult" /> object indicating the result of the operation.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input <paramref name="request" /> is null.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the <paramref name="request.Subject" /> is null, empty, or whitespace,
    ///     or if <paramref name="request.CustomerId" /> is set to the default integer value.
    /// </exception>
    public async Task<SupportRequestResult> CreateSupportRequestAsync(SupportRequest request)
    {
        // Error Handling - Input validation
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Subject, nameof(request.Subject));
        if (request.CustomerId == int.MinValue)
            throw new ArgumentException("CustomerId must not be default value.", nameof(request.CustomerId));

        // create new response object
        SupportRequestResult response = new();

        // Initialise / set service-controlled values
        request.CreatedOnUtc = DateTime.UtcNow;
        request.UpdatedOnUtc = DateTime.UtcNow;
        request.Read = false;
        request.Status = StatusEnum.AwaitingSupport;

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            await _supportRequestRepository.InsertAsync(request);
        }
        catch (Exception exc)
        {
            var msg = $"Error creating support request: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Retrieves a support request by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the support request to retrieve.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult{T}" /> containing the support request if successful.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    public async Task<SupportRequestResult<SupportRequest>> GetSupportRequestByIdAsync(int id)
    {
        // create new response object
        SupportRequestResult<SupportRequest> response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            response.Result = await _supportRequestRepository.GetByIdAsync(id);
        }
        catch (Exception exc)
        {
            var msg = $"Error getting support request: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Retrieves all support requests with pagination and optional filtering.
    /// </summary>
    /// <param name="sortByCreatedDateDsc">Whether to sort by created date descending. Default: true</param>
    /// <param name="filterByStatus">The status to filter by, empty for no filter.</param>
    /// <param name="searchQuery">Search query to match by ID, customer ID, or subject.</param>
    /// <param name="pageIndex">The index of the page to retrieve. Default: 0</param>
    /// <param name="pageSize">The number of results per page. Default: 5</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult{T}" /> containing the support request if successful as a
    ///     paged list of <see cref="SupportRequest" /> objects.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    public async Task<SupportRequestResult<IPagedList<SupportRequest>>> GetAllSupportRequestsAsync(
        bool sortByCreatedDateDsc = true,
        string filterByStatus = "",
        string searchQuery = "",
        int pageIndex = 0,
        int pageSize = 5)
    {
        // Error Handling - Input validation
        if (pageIndex < 0) throw new ArgumentException("pageIndex must not be negative", nameof(pageIndex));
        if (pageSize < 0) throw new ArgumentException("pageSize must not be negative", nameof(pageSize));

        // Create new response object
        SupportRequestResult<IPagedList<SupportRequest>> response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            response.Result = await _supportRequestRepository.GetAllPagedAsync(query =>
            {
                // filter by status
                if (!string.IsNullOrEmpty(filterByStatus))
                {
                    var validStatus = Enum.TryParse(filterByStatus, out StatusEnum status);

                    if (validStatus) query = query.Where(sr => sr.Status == status);
                }

                // filter by search term
                if (!string.IsNullOrEmpty(searchQuery))
                    query = query.Where(sr => sr.Id.ToString().Contains(searchQuery) ||
                                              sr.CustomerId.ToString().Contains(searchQuery) ||
                                              sr.Subject.Contains(searchQuery));

                // sort by created date
                query = sortByCreatedDateDsc
                    ? query.OrderByDescending(sr => sr.CreatedOnUtc)
                    : query.OrderBy(sr => sr.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize);
        }
        catch (Exception exc)
        {
            var msg = $"Error getting support requests: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Retrieves all support requests submitted by a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user whose support requests are to be fetched.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult{T}" /> containing a list of <see cref="SupportRequest" /> objects
    ///     related to the specified user.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    public async Task<SupportRequestResult<IList<SupportRequest>>> GetUserSupportRequestsAsync(int userId)
    {
        // create new response object
        SupportRequestResult<IList<SupportRequest>> response = new();

        // Catch any error that may happen when querying the database
        try
        {
            response.Result = _supportRequestRepository.Table.Where(sr => sr.CustomerId == userId).ToList();
        }
        catch (Exception exc)
        {
            var msg = $"Error getting support messages: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Updates an existing support request in the database asynchronously.
    /// </summary>
    /// <param name="request">
    ///     The <see cref="SupportRequest" /> object to be updated. Must contain valid and required
    ///     properties.
    /// </param>
    /// <returns>
    ///     A <see cref="SupportRequestResult{T}" /> containing the updated <see cref="SupportRequest" /> object
    ///     if the operation is successful.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="request" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if required fields like <paramref name="request.Subject" /> are invalid.</exception>
    public async Task<SupportRequestResult<SupportRequest>> UpdateSupportRequestAsync(SupportRequest request)
    {
        // Error handling - input validation
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Subject, nameof(request.Subject));
        if (request.CustomerId == int.MinValue)
            throw new ArgumentException("CustomerId must not be default value.", nameof(request.CustomerId));

        // Create new response object
        SupportRequestResult<SupportRequest> response = new();

        // Initialise / set service-controlled values
        request.UpdatedOnUtc = DateTime.UtcNow;

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            await _supportRequestRepository.UpdateAsync(request);
            response.Result = request;
        }
        catch (Exception exc)
        {
            var msg = $"Error updating support request: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Deletes a support request from the database asynchronously.
    /// </summary>
    /// <param name="requestId">The <see cref="SupportRequest" /> object to delete. Must not be null.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult" /> indicating whether the deletion was successful or encountered an error.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="requestId" /> is null.</exception>
    public async Task<SupportRequestResult> DeleteSupportRequestByIdAsync(int requestId)
    {
        // Error handling - input validation
        if (requestId == int.MinValue)
            throw new ArgumentException("requestId must not be default value.", nameof(requestId));

        // Create new response object
        SupportRequestResult response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            // Retrieve request
            var request = await GetSupportRequestByIdAsync(requestId);
            
            // Delete request
            await _supportRequestRepository.DeleteAsync(request.Result);
        }
        catch (Exception exc)
        {
            var msg = $"Error deleting support request: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }

    #endregion

    #region Messages

    /// <summary>
    ///     Creates a support message asynchronously in the database.
    /// </summary>
    /// <param name="message">The message to be created. Must not be null.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult" /> indicating the operation's result.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    public async Task<SupportRequestResult> CreateSupportMessageAsync(SupportMessage message)
    {
        // Error handling - input validation
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        if (message.AuthorId == int.MinValue)
            throw new ArgumentException("AuthorId must not be default value.", nameof(message.AuthorId));
        if (message.RequestId == int.MinValue)
            throw new ArgumentException("RequestId must not be default value.", nameof(message.AuthorId));

        // Create new response object
        SupportRequestResult response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            // Determine if author is admin or user
            var customer = await _customerService.GetCustomerByIdAsync(message.AuthorId);
            var isAdmin = await _customerService.IsAdminAsync(customer);

            // Initialise / set service-controlled values
            message.CreatedOnUtc = DateTime.UtcNow;
            message.IsAdmin = isAdmin;

            // Insert message to database
            await _supportMessageRepository.InsertAsync(message);

            // Update associated support request
            var request = await GetSupportRequestByIdAsync(message.RequestId);

            if (request != null)
            {
                request.Result.Status = isAdmin ? StatusEnum.AwaitingCustomer : StatusEnum.AwaitingSupport;
                await UpdateSupportRequestAsync(request.Result);
            }
        }
        catch (Exception exc)
        {
            var msg = $"Error adding support message: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Retrieves support messages related to a support request.
    /// </summary>
    /// <param name="supportRequestId">The support request ID to fetch messages for.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult{T}" /> containing the support request if successful as a
    ///     paged list of <see cref="SupportMessage" /> objects.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    public async Task<SupportRequestResult<IList<SupportMessage>>> GetSupportRequestMessagesAsync(int supportRequestId)
    {
        // Create new response object
        SupportRequestResult<IList<SupportMessage>> response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            response.Result = _supportMessageRepository.Table.Where(sm => sm.RequestId == supportRequestId).ToList();
        }
        catch (Exception exc)
        {
            var msg = $"Error getting support messages: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }


    /// <summary>
    ///     Deletes all messages associated with a specific support request asynchronously.
    /// </summary>
    /// <param name="supportRequestId">The ID of the support request whose messages should be deleted.</param>
    /// <returns>
    ///     A <see cref="SupportRequestResult" /> indicating whether the deletion was successful or encountered an error.
    ///     If the operation fails due to exceptions or validation errors, the errors will be populated in this result.
    /// </returns>
    /// <exception cref="Exception">Thrown if the database operation fails.</exception>
    public async Task<SupportRequestResult> DeleteSupportRequestMessagesAsync(int supportRequestId)
    {
        // Create new response object
        SupportRequestResult response = new();

        // Catch any error that may happen when querying the database and notify user and/or admin
        try
        {
            await _supportMessageRepository.Table.Where(sm => sm.RequestId == supportRequestId).DeleteAsync();
        }
        catch (Exception exc)
        {
            var msg = $"Error deleting support messages: {exc.Message}";
            await _logger.ErrorAsync(msg, exc); // log error for sys admins
            response.AddError(msg); // let user know what happened
        }

        return response;
    }

    #endregion
}