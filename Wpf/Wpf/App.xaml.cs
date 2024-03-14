using IdentityModel.Client;
using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    internal static string[] Args = null;
    internal static string CustomUriScheme = "oidcclient-wpf-sample";

    private OidcClient _client;
    private AuthorizeState _state;

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (e.Args.Any() && e.Args.Length == 1)
        {
            Args = e.Args;
            await SendCallbackToMainProcess(Args[0]);
            Shutdown();
        }
        else
        {
            // Register custom uri scheme if necessary
            new RegistryConfig(CustomUriScheme).Configure();

            // Display Main Window
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Begin OAuth Flow
            await InitiateChallenge();

            // Wait for the user to login
            var result = await ReceiveCallback();

            // Marshall the update into the UI
            mainWindow.Dispatcher.Invoke(() => mainWindow.UserLoggedIn(result));
        }
        base.OnStartup(e);
    }

    private async Task InitiateChallenge()
    {
        // Configure OidcClient and prepare a login request
        string redirectUri = $"{App.CustomUriScheme}://callback";
        var options = new OidcClientOptions()
        {
            Authority = "https://demo.duendesoftware.com/",
            ClientId = "interactive.public",
            Scope = "openid profile email",
            RedirectUri = redirectUri,
        };
        _client = new OidcClient(options);
        _state = await _client.PrepareLoginAsync();

        // open system browser to start authentication
        Process.Start(new ProcessStartInfo
        {
            FileName = _state.StartUrl,
            UseShellExecute = true
        });
    }

    private async Task<LoginResult> ReceiveCallback()
    {
        var callbackManager = new CallbackManager(_state.State);
        var response = await callbackManager.RunServer();
        return await _client.ProcessResponseAsync(response, _state);
    }


    private async Task SendCallbackToMainProcess(string args)
    {
        var response = new AuthorizeResponse(args);
        if (!String.IsNullOrWhiteSpace(response.State))
        {
            var callbackManager = new CallbackManager(response.State);
            await callbackManager.RunClient(args);
        }
    }
}
