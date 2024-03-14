using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private HttpClient _apiClient;
    private OidcClient _oidcClient;

    public MainWindow()
    {
        InitializeComponent();
        Message.Text = "Logging in...";
    }

    public void DisplayMessage(string message)
    {
        Message.Text = message;
        Show();
    }

    public void Initialize(HttpClient client, OidcClient oidcClient)
    {
        _apiClient = client;
        _oidcClient = oidcClient;
    }

    private async void CallApi_Click(object sender, RoutedEventArgs e)
    {
        var response = await _apiClient.GetAsync("");

        if (response.IsSuccessStatusCode)
        {
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Details.Text = json.RootElement.ToString();
        }
        else
        {
            Details.Text = $"Error: {response.ReasonPhrase}";
        }
    }

    private async void Logout_Click(object sender, RoutedEventArgs e)
    {
        // TODO - persist id token, pass to logout endpoint
        // Add a login button to log back in
        // Consider moving the login flow into the main window to cut down on the back and forth between App and MainWindow (and to allow login button to call the login code easily)
        // Set the post logout redirect uri to also use the custom scheme. Have the app look for the post logout uri as well, and use that to signal that we are "logged out" (don't just directly set the string)
        var session = Session.Get();
        var url = await _oidcClient.PrepareLogoutAsync(new LogoutRequest
        {
            IdTokenHint = session?.IdToken
        });
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        await ReceiveSignoutCallback();


    }

    private async Task ReceiveSignoutCallback()
    {
        var session = Session.Get();
        if (session != null)
        {
            var callbackManager = new CallbackManager(session.Claims.First(c => c.Type == "sid").Value);
            var response = await callbackManager.RunServer();
            if(response != null)
            {
                Activate();
                Session.Delete();
                Message.Text = "Logged out";
            }
            else
            {
                Message.Text = "Logout issue - null response from callback"; // Don't think this is possible
            }
        }
        else 
        {
            Message.Text = "Logout issue - no session";
        }
    }
}
