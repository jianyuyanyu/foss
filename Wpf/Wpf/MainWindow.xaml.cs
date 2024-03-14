using IdentityModel.OidcClient;
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
        Message.Text = "Logging in...";
    }

    public void UserLoggedIn(LoginResult result)
    {
        Message.Text = $"User logged in: {result.User.Identity.Name}";

        Details.Text =
            $"""
            Access Token: {result.AccessToken}
            
            Id Token: {result.IdentityToken}
            """;
        // Focus the UI
        Activate();
    }
}
