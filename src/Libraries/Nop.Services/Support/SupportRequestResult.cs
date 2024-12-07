#nullable enable
namespace Nop.Services.Support;

public class SupportRequestResult
{
    public List<string> Errors { get; set; } = new();
    public bool Success => !Errors.Any();
    
    public void AddError(string error)
    {
        Errors.Add(error);
    }
    
}

public class SupportRequestResult<T> : SupportRequestResult
{
    public T? Result { get; set; }
}