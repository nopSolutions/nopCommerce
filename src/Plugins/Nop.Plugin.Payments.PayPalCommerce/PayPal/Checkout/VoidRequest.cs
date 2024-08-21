namespace PayPalCheckoutSdk.Payments;

/// <summary>
/// Voids, or cancels, an authorized payment, by ID. You cannot void an authorized payment that has been fully captured.
/// </summary>
public class VoidRequest : PayPalHttp.HttpRequest
{
    public VoidRequest(string authorizationId) : base("/v2/payments/authorizations/{authorization_id}/void?", HttpMethod.Post, typeof(Authorization))
    {
        try
        {
            Path = Path.Replace("{authorization_id}", Uri.EscapeDataString(Convert.ToString(authorizationId)));
        }
        catch { }

        ContentType = "application/json";
    }

}