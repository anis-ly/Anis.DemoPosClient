namespace Anis.DemoPosClient
{
    public class AuthenticationOptions
    {
        public AuthenticationOptions()
        {
            Authority = "";
            ClientId = "";
            RedirectUri = "";
            Scope = "";
            ClientSecret = "";
        }

        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
    }
}
