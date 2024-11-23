using LinqToDB;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Support;
using Nop.Data;

namespace Nop.Services.Support;

public partial class SupportRequestService : ISupportRequestService
{
    protected readonly IRepository<SupportRequest> _supportRequestRepository;
    protected readonly IRepository<SupportMessage> _supportMessageRepository;

    public SupportRequestService(IRepository<SupportRequest> supportRequestRepository, IRepository<SupportMessage> supportMessageRepository)
    {
        _supportRequestRepository = supportRequestRepository;
        _supportMessageRepository = supportMessageRepository;
    }
    
    #region Requests
    
    public void CreateSupportRequest(SupportRequest request)
    {
        _supportRequestRepository.Insert(request);
    }
    
    public SupportRequest GetSupportRequestById(int id)
    {
        return _supportRequestRepository.Table.FirstOrDefault(x => x.Id == id);
    }

    public IList<SupportRequest> GetSupportRequests()
    {
        return _supportRequestRepository.Table.ToList();
    }

    public void UpdateSupportRequest(SupportRequest request)
    {
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
        _supportMessageRepository.Insert(message);
    }
    
    public IList<SupportMessage> GetSupportRequestMessages(int SupportRequestId)
    {
        return _supportMessageRepository.Table.Where(sm => sm.Id == SupportRequestId).ToList();
    }
    
    public void DeleteSupportRequestMessages(int SupportRequestId)
    {
        _supportMessageRepository.Table.Where(sm => sm.Id == SupportRequestId).Delete();
    }
    
    #endregion
}