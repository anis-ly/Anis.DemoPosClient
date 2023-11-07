using IdentityModel.OidcClient.Browser;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Anis.DemoPosClient
{
    public class EmbeddedBrowser : IBrowser
    {
        private readonly WebView2 _webView;
        private SemaphoreSlim? _semaphoreSlim;
        private BrowserOptions? _options = null;
        private BrowserResult? _browserResult;
        private Window? _signinWindow;

        public EmbeddedBrowser()
        {
            _webView = new WebView2();
            _webView.NavigationStarting += (s, e) =>
            {
                if (IsBrowserNavigatingToRedirectUri(new Uri(e.Uri)))
                {
                    e.Cancel = true;

                    _browserResult = new BrowserResult()
                    {
                        ResultType = BrowserResultType.Success,
                        Response = new Uri(e.Uri).AbsoluteUri
                    };

                    _semaphoreSlim?.Release();
                    _signinWindow?.Close();
                }
            };
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            _options = options;
            _semaphoreSlim = new SemaphoreSlim(0, 1);
            _browserResult = new BrowserResult()
            {
                ResultType = BrowserResultType.UserCancel,
            };

            _signinWindow = options.DisplayMode == DisplayMode.Visible
                ? new Window()
                {
                    Width = 400,
                    Height = 600,
                    Title = "Sign In",
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }
                : new Window()
                {
                    Width = 1,
                    Height = 1,
                    Opacity = 100,
                    WindowState = WindowState.Minimized,
                    Title = "Sign In",
                    WindowStartupLocation = WindowStartupLocation.Manual
                };
            _signinWindow.Closing += (s, e) =>
            {
                _semaphoreSlim.Release();
            };


            _signinWindow.Content = _webView;
            _signinWindow.Show();

            // Initialization
            await _webView.EnsureCoreWebView2Async(null);

            // Delete existing Cookies so previous logins won't remembered

            if (options.DisplayMode == DisplayMode.Visible)
                _webView.CoreWebView2.CookieManager.DeleteAllCookies();

            // Navigate
            _webView.CoreWebView2.Navigate(_options.StartUrl);
            await _semaphoreSlim.WaitAsync(cancellationToken);
            return _browserResult;
        }

        private bool IsBrowserNavigatingToRedirectUri(Uri uri)
        {
            if (_options == null)
                return false;

            return uri.AbsoluteUri.StartsWith(_options.EndUrl);
        }
    }
}
