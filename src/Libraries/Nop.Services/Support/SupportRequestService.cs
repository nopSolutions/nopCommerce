using LinqToDB;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Support;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Support;

public partial class SupportRequestService : ISupportRequestService
{
    protected readonly IRepository<SupportRequest> _supportRequestRepository;
    protected readonly IRepository<SupportMessage> _supportMessageRepository;
    protected readonly ICustomerService _customerService;

    public SupportRequestService(
        IRepository<SupportRequest> supportRequestRepository,
        IRepository<SupportMessage> supportMessageRepository,
        ICustomerService customerService)
    {
        _supportRequestRepository = supportRequestRepository;
        _supportMessageRepository = supportMessageRepository;
        _customerService = customerService;
    }
    
    #region Requests
    
    public void CreateSupportRequest(SupportRequest request)
    {
        request.CreatedOnUtc = DateTime.UtcNow;
        request.UpdatedOnUtc = DateTime.UtcNow;
        request.Read = false;
        request.Status = StatusEnum.AwaitingSupport;
        
        _supportRequestRepository.Insert(request);
    }
    
    public SupportRequest GetSupportRequestById(int id)
    {
        return _supportRequestRepository.Table.FirstOrDefault(x => x.Id == id);
    }

    public IList<SupportRequest> GetAllSupportRequests(bool sortByCreatedDateDsc = true)
    {
        var query = _supportRequestRepository.Table;
        
        // sort by created date
        query = sortByCreatedDateDsc ? query.OrderByDescending(x => x.CreatedOnUtc) : query.OrderBy(x => x.CreatedOnUtc);
        
        return query.ToList();
    }
    
    public IList<SupportRequest> GetUserSupportRequests(int userId)
    {
        return _supportRequestRepository.Table.Where(sr => sr.CustomerId == userId).ToList();
    }

    public void UpdateSupportRequest(SupportRequest request)
    {
        request.UpdatedOnUtc = DateTime.UtcNow;
        _supportRequestRepository.Update(request);
    }

    public void DeleteSupportRequest(SupportRequest request)
    {
        _supportRequestRepository.Delete(request);
    }
    
    #endregion
    
    #region Messages
    
    public void CreateSupportMessage(SupportMessage message)
    {
        var customer = _customerService.GetCustomerByIdAsync(message.AuthorId).Result;
        var isAdmin = _customerService.IsAdminAsync(customer).Result;
        
        message.CreatedOnUtc = DateTime.UtcNow;
        message.IsAdmin = isAdmin;
        
        _supportMessageRepository.Insert(message);
        
        // update associated support request
        var request = GetSupportRequestById(message.RequestId);
        request.Status = isAdmin ? StatusEnum.AwaitingCustomer : StatusEnum.AwaitingSupport;
        
        UpdateSupportRequest(request);
    }
    
    public IList<SupportMessage> GetSupportRequestMessages(int supportRequestId)
    {
        return _supportMessageRepository.Table.Where(sm => sm.RequestId == supportRequestId).ToList();
    }
    
    public void DeleteSupportRequestMessages(int supportRequestId)
    {
        _supportMessageRepository.Table.Where(sm => sm.RequestId == supportRequestId).Delete();
    }
    
    #endregion
}