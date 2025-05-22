// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WorkerDI;

namespace WorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()

            .ConfigureServices((services) =>
            {
                services.AddDistributedMemoryCache();

                services.AddClientCredentialsTokenManagement();
                services.AddSingleton(new DiscoveryCache("https://demo.duendesoftware.com"));
                services.AddSingleton<IConfigureOptions<ClientCredentialsClient>, ClientCredentialsClientConfigureOptions>();

                // alternative way to add a client
                services.Configure<ClientCredentialsClient>("demo", client =>
                {
                    client.TokenEndpoint = new Uri("https://demo.duendesoftware.com/connect/token");

                    client.ClientId = ClientId.Parse("m2m.short");
                    client.ClientSecret = ClientSecret.Parse("secret");

                    client.Scope = Scope.Parse("api");
                });

                services.AddClientCredentialsHttpClient("client", ClientCredentialsClientName.Parse("demo"), client =>
                {
                    client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
                });

                services.AddHttpClient<TypedClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
                    })
                    .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("demo"));

                services.AddTransient<IClientAssertionService, ClientAssertionService>();

                //services.AddHostedService<WorkerManual>();
                services.AddHostedService<WorkerManualJwt>();
                //services.AddHostedService<WorkerHttpClient>();
                //services.AddHostedService<WorkerTypedHttpClient>();
            });

        return host;
    }

}
