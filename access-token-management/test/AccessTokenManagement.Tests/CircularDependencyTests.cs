// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Tests that verify the DI registration does not create circular dependencies.
/// See https://github.com/DuendeSoftware/foss/pull/347 for context.
/// </summary>
public class CircularDependencyTests
{
    /// <summary>
    /// Reproduces the circular dependency described in PR #347:
    ///
    /// IClientAssertionService (user impl)
    ///   → IOpenIdConnectConfigurationService
    ///     → IOptionsMonitor&lt;OpenIdConnectOptions&gt;
    ///       → IConfigureOptions&lt;OpenIdConnectOptions&gt; (ConfigureOpenIdConnectOptions)
    ///         → IClientAssertionService  ← CYCLE
    ///
    /// The fix in ConfigureOpenIdConnectOptions resolves IClientAssertionService
    /// lazily via IServiceProvider instead of constructor injection, breaking the cycle.
    /// </summary>
    [Fact]
    public void IClientAssertionService_depending_on_IOpenIdConnectConfigurationService_should_not_cause_circular_dependency()
    {
        var services = new ServiceCollection();

        // Register authentication with an OpenIdConnect scheme (minimal setup).
        services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignInScheme = "cookie";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://demo.duendesoftware.com";
                options.ClientId = "test-client";
                options.ClientSecret = "secret";
            });

        // Register ATM's OpenIdConnect services (includes ConfigureOpenIdConnectOptions).
        services.AddOpenIdConnectAccessTokenManagement();

        // Register a custom IClientAssertionService that depends on
        // IOpenIdConnectConfigurationService — the exact pattern from the
        // WebJarJwt sample that triggered the circular dependency before the fix.
        services.AddTransient<IClientAssertionService, ClientAssertionServiceWithOidcDependency>();

        // ValidateOnBuild detects circular dependencies at container build time.
        // Before the fix, this would throw:
        //   "A circular dependency was detected for the service of type
        //    'Duende.AccessTokenManagement.IClientAssertionService'."
        var act = () => services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        act.ShouldNotThrow();
    }

    /// <summary>
    /// A test implementation of IClientAssertionService that depends on
    /// IOpenIdConnectConfigurationService, reproducing the dependency chain
    /// from the WebJarJwt sample that caused the circular dependency.
    /// </summary>
    private sealed class ClientAssertionServiceWithOidcDependency(
        IOpenIdConnectConfigurationService configurationService) : IClientAssertionService
    {
        // Keep a reference to prove DI resolved the dependency successfully.
        private readonly IOpenIdConnectConfigurationService _configurationService = configurationService;

        public Task<ClientAssertion?> GetClientAssertionAsync(
            ClientCredentialsClientName? clientName = null,
            TokenRequestParameters? parameters = null,
            CancellationToken ct = default) =>
            Task.FromResult<ClientAssertion?>(null);
    }
}
