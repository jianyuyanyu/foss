using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
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
        // That will involve expanding the persisted session
        // You might as well make it a proper session object, with claims
        // Add a login button to log back in
        // Consider moving the login flow into the main window to cut down on the back and forth between App and MainWindow (and to allow login button to call the login code easily)
        // Set the post logout redirect uri to also use the custom scheme. Have the app look for the post logout uri as well, and use that to signal that we are "logged out" (don't just directly set the string)
        var url = await _oidcClient.PrepareLogoutAsync(new LogoutRequest());
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        File.Delete("refresh_token");
        Message.Text = "Logged out";
    }
}
