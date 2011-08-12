//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}