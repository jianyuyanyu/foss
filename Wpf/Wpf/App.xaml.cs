using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.DPoP;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    static readonly string Api = "https://demo.duendesoftware.com/api/dpop/test";
    internal static string[] Args = null;
    internal static string CustomUriScheme = "oidcclient-wpf-sample";

    private OidcClient _oidcClient;
    private AuthorizeState _state;
    internal HttpClient _apiClient;

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (e.Args.Any() && e.Args.Length == 1)
        {
            Args = e.Args;
            await SendCallbackToMainProcess(Args[0]);
            //Shutdown();
        }
        else
        {
            // Register custom uri scheme if necessary
            new RegistryConfig(CustomUriScheme).Configure();

            // Display Main Window
            var mainWindow = new MainWindow();
            mainWindow.Show();

            var proofKey = InitializeOidcClient();

            var session = Session.Get();
            if (session?.RefreshToken != null)
            {
                InitializeApiClient(proofKey, session.RefreshToken);

                // Marshall an update into the UI
                mainWindow.Dispatcher.Invoke(() =>
                {
                    var name = session.Claims.Single(c => c.Type == "name").Value;
                    mainWindow.DisplayMessage($"User previously logged in: {name}");
                });
            }
            else
            {
                _state = await _oidcClient.PrepareLoginAsync();

                // open system browser to start authentication
                OpenBrowser(_state.StartUrl);

                // Wait for the user to login
                var result = await ReceiveCallback();

                InitializeApiClient(result);
                Session.Store(result);

                // Marshall the update into the UI
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.DisplayMessage($"User logged in: {result.User.Identity?.Name}");
                });
            }
            mainWindow.Initialize(_apiClient, _oidcClient);
        }
        base.OnStartup(e);
    }

    private void OpenBrowser(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private string InitializeOidcClient()
    {
        // Configure OidcClient and prepare a login request
        string redirectUri = $"{App.CustomUriScheme}://signin";
        var options = new OidcClientOptions()
        {
            Authority = "https://demo.duendesoftware.com/",
            ClientId = "interactive.public",
            Scope = "openid profile email offline_access",
            RedirectUri = redirectUri,
            PostLogoutRedirectUri = $"{App.CustomUriScheme}://signout"
        };

        // Enable DPoP
        var proofKey = GetProofKey();
        options.ConfigureDPoP(proofKey);

        _oidcClient = new OidcClient(options);
        return proofKey;
    }

    private void InitializeApiClient(string proofKey, string refreshToken)
    {
        // Use saved refresh token
        var handler = _oidcClient.CreateDPoPHandler(proofKey, refreshToken);
        _apiClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(Api)
        };
    }

    private void InitializeApiClient(LoginResult result)
    {
        _apiClient = new HttpClient(result.RefreshTokenHandler)
        {
            BaseAddress = new Uri(Api)
        };
    }

    private async Task<LoginResult> ReceiveCallback()
    {
        var callbackManager = new CallbackManager(_state.State);
        var response = await callbackManager.RunServer();
        return await _oidcClient.ProcessResponseAsync(response, _state);
    }

    private async Task SendCallbackToMainProcess(string callbackUrl)
    {
        if(callbackUrl.StartsWith($"{App.CustomUriScheme}://signin"))
        {
            var response = new AuthorizeResponse(callbackUrl);
            if (!String.IsNullOrWhiteSpace(response.State))
            {
                var callbackManager = new CallbackManager(response.State);
                await callbackManager.RunClient(callbackUrl);
            }
        }
        else if(callbackUrl.StartsWith($"{App.CustomUriScheme}://signout"))
        {
            var session = Session.Get();
            if (session != null)
            {
                var sid = session.Claims.First(c => c.Type == "sid").Value;
                var callbackManager = new CallbackManager(sid);
                
                await callbackManager.RunClient(callbackUrl);
            }
        }
    }

    private string GetProofKey()
    {
        if (File.Exists("proofkey"))
        {
            var protectedKey = File.ReadAllText("proofkey");
            return DataProtector.Unprotect(protectedKey);
        }

        var proofKey = JsonWebKeys.CreateRsaJson();
        File.WriteAllText("proofkey", DataProtector.Protect(proofKey));
        return proofKey;
    }
}
