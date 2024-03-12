using IdentityModel.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static string[] Args = null;
        internal static string CustomUriScheme = "oidcclient-wpf-sample";

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any() && e.Args.Length == 1)
            {
                Args = e.Args;
                await ProcessCallback(Args[0]);
                Shutdown();
            }
            else
            {
                new RegistryConfig(CustomUriScheme).Configure();
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            base.OnStartup(e);
        }

        private async Task ProcessCallback(string args)
        {
            var response = new AuthorizeResponse(args);
            if (!String.IsNullOrWhiteSpace(response.State))
            {
                var callbackManager = new CallbackManager(response.State);
                await callbackManager.RunClient(args);
            }
        }
    }
}
