using System.Net.Http;
using System.Threading.Tasks;

public interface IListrakService
{
    Task<string> GetTokenAsync();
    Task<HttpResponseMessage> SubscribePhoneNumberAsync(string phoneNumber);
}