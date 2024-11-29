using Nop.Core.Domain.Support;

namespace Nop.Services.Support;

public partial interface ISupportRequestService
{
    // Create
    public void CreateSupportRequest(SupportRequest request);
    public void CreateSupportMessage(SupportMessage message);
    
    // Read
    public SupportRequest GetSupportRequestById(int id);
    public IList<SupportRequest> GetAllSupportRequests();
    public IList<SupportRequest> GetUserSupportRequests(int userId);
    public IList<SupportMessage> GetSupportRequestMessages(int supportRequestId);
    
    // Update
    public void UpdateSupportRequest(SupportRequest request);
    
    // Delete
    public void DeleteSupportRequest(SupportRequest request);
    public void DeleteSupportRequestMessages(int supportRequestId);
}