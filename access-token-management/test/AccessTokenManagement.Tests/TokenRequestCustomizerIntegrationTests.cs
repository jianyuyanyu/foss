// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using Duende.AccessTokenManagement.Framework;
using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Duende.AccessTokenManagement;

public class TokenRequestCustomizerIntegrationTests(ITestOutputHelper output) : IntegrationTestBase(output,
    configureUserTokenManagementOptions: opt => { opt.UseChallengeSchemeScopedTokens = true; })
{
    [Fact]
    public async Task Customizer_is_called_during_client_credentials_flow()
    {
        var customizer = new TestTokenRequestCustomizer(Scope.Parse("scope2"));
        AppHost.OnConfigureServices += services =>
        {
            services.AddClientCredentialsTokenManagement()
                .AddClient("pure_client_credentials", client =>
                {
                    client.ClientId = ClientId.Parse("client_credentials_client");
                    client.ClientSecret = ClientSecret.Parse("secret");
                    client.TokenEndpoint = new Uri("https://identityserver/connect/token");
                    client.Scope = Scope.Parse("scope1");
                    client.HttpClient = IdentityServerHost.HttpClient;
                });

            services.AddHttpClient("clientCredentialsApi")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(ApiHost.Url()))
                .ConfigurePrimaryHttpMessageHandler(() => ApiHost.HttpMessageHandler)
                .AddClientCredentialsTokenHandler(customizer,
                    ClientCredentialsClientName.Parse("pure_client_credentials"));
        };

        AppHost.OnConfigure += app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/exercise_client_credentials", async (IHttpClientFactory factory, HttpContext _) =>
                {
                    var httpClient = factory.CreateClient("clientCredentialsApi");
                    var response = await httpClient.GetAsync("test");
                    return await response.Content.ReadFromJsonAsync<TokenEchoResponse>();
                });
            });
        };
        await InitializeAsync();

        var response = await AppHost.BrowserClient.GetAsync(AppHost.Url("/exercise_client_credentials"));
        response.EnsureSuccessStatusCode();
        var token = ParseTokenFromResponse(response);
        token.Claims.ShouldContain(c => c.Type == "scope" && c.Value.Contains("scope2"));
    }

    [Fact]
    public async Task Customizer_is_called_during_openid_connect_user_access_token_flow()
    {
        var customizer = new TestTokenRequestCustomizer(Scope.Parse("scope2"));
        AppHost.OnConfigureServices += services =>
        {
            services.AddHttpClient("callApi")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(ApiHost.Url()))
                .ConfigurePrimaryHttpMessageHandler(() => ApiHost.HttpMessageHandler)
                .AddUserAccessTokenHandler(customizer, new UserTokenRequestParameters
                {
                    Scope = Scope.Parse("scope1")
                });
        };

        await InitializeAsync();

        await AppHost.LoginAsync("alice");

        var response = await AppHost.BrowserClient.GetAsync(AppHost.Url("/call_api"));
        response.EnsureSuccessStatusCode();
        var token = ParseTokenFromResponse(response);
        token.Claims.ShouldContain(c => c.Type == "scope" && c.Value.Contains("scope2"));
    }

    [Fact]
    public async Task Customizer_is_called_during_openid_connect_client_access_token_flow()
    {
        var customizer = new TestTokenRequestCustomizer(Scope.Parse("scope2"));
        AppHost.OnConfigureServices += services =>
        {
            services.AddHttpClient("oidcClientApi")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(ApiHost.Url()))
                .ConfigurePrimaryHttpMessageHandler(() => ApiHost.HttpMessageHandler)
                .AddClientAccessTokenHandler(customizer, new UserTokenRequestParameters
                {
                    Scope = Scope.Parse("scope1")
                });
        };

        AppHost.OnConfigure += app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/exercise_oidc_client_token", async (IHttpClientFactory factory, HttpContext _) =>
                {
                    var httpClient = factory.CreateClient("oidcClientApi");
                    var response = await httpClient.GetAsync("test");
                    return await response.Content.ReadFromJsonAsync<TokenEchoResponse>();
                });
            });
        };
        await InitializeAsync();

        var response = await AppHost.BrowserClient.GetAsync(AppHost.Url("/exercise_oidc_client_token"));
        response.EnsureSuccessStatusCode();
        var token = ParseTokenFromResponse(response);
        token.Claims.ShouldContain(c => c.Type == "scope" && c.Value.Contains("scope2"));
    }

    private static JwtSecurityToken ParseTokenFromResponse(HttpResponseMessage response)
    {
        var result = response.Content.ReadAsStringAsync().Result;
        var tokenResult = System.Text.Json.JsonSerializer.Deserialize<TokenEchoResponse>(result);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenResult!.token.Replace("Bearer ", string.Empty));
        return token;
    }
}

public class TestTokenRequestCustomizer(Scope scopeToRequest) : ITokenRequestCustomizer
{
    public Task<TokenRequestParameters> Customize(
        HttpRequestMessage httpRequest,
        TokenRequestParameters baseParameters,
        CancellationToken cancellationToken = default) => Task.FromResult(baseParameters with
        {
            Scope = scopeToRequest
        });
}
