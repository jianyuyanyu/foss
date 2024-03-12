using IdentityModel.OidcClient;
using System.Diagnostics;
using System.Windows;

namespace Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        string redirectUri = $"{App.CustomUriScheme}://callback";

        var options = new OidcClientOptions()
        {
            Authority = "https://demo.duendesoftware.com/",
            ClientId = "interactive.public",
            Scope = "openid profile email",
            RedirectUri = redirectUri,
        };

        var client = new OidcClient(options);
        var state = await client.PrepareLoginAsync();

        var callbackManager = new CallbackManager(state.State);

        // open system browser to start authentication
        Process.Start(new ProcessStartInfo
        {
            FileName = state.StartUrl,
            UseShellExecute = true
        });

        var response = await callbackManager.RunServer();

        var result = await client.ProcessResponseAsync(response, state);

        if (result.IsError)
        {
            txbMessage.Text = result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error;
        }
        else
        {
            txbMessage.Text = result.User.Identity.Name;
        }

        // Focus the UI
        Activate();
    }

    
}
