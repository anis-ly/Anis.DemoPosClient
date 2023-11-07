using IdentityModel.OidcClient;
using System;
using System.Threading.Tasks;

namespace Anis.DemoPosClient
{
    public class AuthenticationService
    {
        private readonly OidcClient _oidcClient;

        public AuthenticationService(AuthenticationOptions authenticationOptions)
        {
            var options = new OidcClientOptions()
            {
                Browser = new EmbeddedBrowser(),
                Authority = authenticationOptions.Authority,
                ClientId = authenticationOptions.ClientId,
                Scope = authenticationOptions.Scope,
                RedirectUri = authenticationOptions.RedirectUri,
                ClientSecret = authenticationOptions.ClientSecret,
            };

            _oidcClient = new OidcClient(options);
        }

        public event EventHandler<AuthenticationStateEventArgs> AuthenticationStateChanged = delegate { };
        public event EventHandler<LoginResult> LoginSucceed = delegate { };
        public event EventHandler<LoginResult> LoginFailed = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> LoginError = delegate { };
        public event EventHandler<LogoutResult> LogoutSucceed = delegate { };

        public string? AccessToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTimeOffset? AccessTokenExpiration { get; set; }

        public async Task LoginAsync()
        {
            try
            {
                var request = new LoginRequest();

                var result = await _oidcClient.LoginAsync(request);

                if (result.IsError)
                {
                    Clear();
                    return;
                }

                RecordResult(result);
                AuthenticationStateChanged(this, AuthenticationStateEventArgs.Authenticated(result.AccessToken));
                LoginSucceed(this, result);
                //ScheduleRefresh(); // Enable to support token refresh
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                LoginError(this, new UnhandledExceptionEventArgs(e, false));
                throw;
            }
        }

        // Enable to support token refresh

        //private void ScheduleRefresh()
        //{
        //    Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            await Task.Delay(TimeSpan.FromSeconds(60));
        //            var remaining = AccessTokenExpiration - DateTimeOffset.Now;

        //            if (remaining?.TotalMinutes < 5)
        //            {
        //                await RefreshAsync();
        //            }
        //        }
        //    });
        //}

        //private async Task RefreshAsync()
        //{
        //    try
        //    {
        //        var result = await _oidcClient.LoginAsync(new LoginRequest()
        //        {
        //            BrowserDisplayMode = DisplayMode.Hidden,
        //        });

        //        if (result.IsError)
        //        {
        //            Clear();
        //            LoginFailed(this, result);
        //            AuthenticationStateChanged(this, AuthenticationStateEventArgs.NotAuthenticated());
        //            return;
        //        }

        //        RecordResult(result);
        //        AuthenticationStateChanged(this, AuthenticationStateEventArgs.Authenticated(result.AccessToken));
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Error.WriteLine(e);
        //    }
        //}

        private void Clear()
        {
            IsAuthenticated = false;
            AccessToken = null;
            AccessTokenExpiration = null;
        }

        private void RecordResult(LoginResult result)
        {
            IsAuthenticated = true;
            AccessToken = result.AccessToken;
            AccessTokenExpiration = result.AccessTokenExpiration;
        }

        public async Task LogoutAsync()
        {
            try
            {
                var result = await _oidcClient.LogoutAsync();

                if (result.IsError)
                    Console.Error.WriteLine("Logout error {Error}, {ErrorDescription}", result.Error, result.ErrorDescription);

                Clear();
                AuthenticationStateChanged(this, AuthenticationStateEventArgs.NotAuthenticated());
                LogoutSucceed(this, result);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}
