namespace Anis.DemoPosClient
{
    public class AuthenticationStateEventArgs
    {
        public static AuthenticationStateEventArgs Authenticated(string accessToken) => new AuthenticationStateEventArgs(true, accessToken);
        public static AuthenticationStateEventArgs NotAuthenticated() => new AuthenticationStateEventArgs(false, null);

        private AuthenticationStateEventArgs(bool isAuthenticated, string? accessToken)
        {
            IsAuthenticated = isAuthenticated;
            AccessToken = accessToken;
        }

        public bool IsAuthenticated { get; }
        public string? AccessToken { get; }
    }
}
