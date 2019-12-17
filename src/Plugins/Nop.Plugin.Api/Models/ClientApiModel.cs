namespace Nop.Plugin.Api.Models
{
    public class ClientApiModel
    {
        
        public int Id { get; set; }
        public string ClientName { get; set; }

        public string ClientId
        {
            get;
            set;
        }

        public string ClientSecret
        {
            get;
            set;
        }

        public string RedirectUrl { get; set; }

        public int AccessTokenLifetime
        {
            get;set;
        }

        public int RefreshTokenLifetime
        {
            get;set;
        }

        public bool Enabled { get; set; }
    }
}