using Nop.Core.Domain.Support;

namespace Nop.Services.Support;

public partial interface ISupportRequestService
{
    // Create
    void CreateSupportRequest(SupportRequest request);
    void CreateSupportMessage(SupportMessage message);
    
    // Read
    SupportRequest GetSupportRequestById(int id);
    IList<SupportRequest> GetSupportRequests();
    IList<SupportMessage> GetSupportRequestMessages(int SupportRequestId);
    
    // Update
    void UpdateSupportRequest(SupportRequest request);
    
    // Delete
    void DeleteSupportRequest(SupportRequest request);
    void DeleteSupportRequestMessages(int SupportRequestId);
}