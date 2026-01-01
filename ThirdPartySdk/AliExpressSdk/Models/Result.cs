using System.Text.Json;

namespace AliExpressSdk.Models;

public class Result<T>
{
    public bool Ok { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public JsonElement? ErrorResponse { get; set; }
    public string? RequestId { get; set; }
}
