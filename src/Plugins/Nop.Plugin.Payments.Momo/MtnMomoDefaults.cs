public class MtnMomoDefaults
{
    public const string BASE_ADDRESS = "https://sandbox.momodeveloper.mtn.com";
    public const string SUBSCRIPTION_HEADER = "Ocp-Apim-Subscription-Key";
    public const string REFERENCE_ID = "X-Reference-Id";
    public const string TARGET_ENVIRONMENT = "X-Target-Environment";
    
    // API Configuration endpoints
    public const string CREATE_API_USER_ENDPOINT = "/v1_0/apiuser";
    public const string GET_API_USER_ENDPOINT = "/v1_0/apiuser/{0}"; // {0} is replaced with ApiUser
    public const string GENERATE_API_KEY_ENDPOINT = "/v1_0/apiuser/{0}/apikey"; // {0} is replaced with ApiUser
    
    // Payment endpoints
    public const string TOKEN_ENDPOINT = "/collection/token/";
    public const string PAYMENT_ENDPOINT = "/collection/v1_0/requesttopay";
    public const string PAYMENT_STATUS_ENDPOINT = "collection/v2_0/payment";
}