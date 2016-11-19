namespace Nop.Web.Factories
{
    public partial interface ICountryModelFactory
    {
        dynamic GetStatesByCountryId(string countryId, bool addSelectStateItem);
    }
}
