// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebJarJwt;

/// <summary>
/// Signs the authorization request (JAR) by wrapping the OpenID Connect event handlers.
///
/// Access token management registers its own event handlers (e.g. for DPoP and for sending client assertions
/// during code exchange and pushed authorization requests). Those handlers are wrapped here instead of being
/// replaced, which is why this sample does not use <see cref="OpenIdConnectOptions.EventsType"/>: setting it
/// replaces all event handlers configured on <see cref="OpenIdConnectOptions.Events"/> at runtime.
/// </summary>
/// <remarks>
/// The <see cref="IClientAssertionService"/> depends on the OpenID Connect options, so it is resolved from the
/// request services at runtime instead of being injected into this class (which would cause a circular dependency).
/// </remarks>
public class ConfigureJar : IPostConfigureOptions<OpenIdConnectOptions>
{
    public void PostConfigure(string? name, OpenIdConnectOptions options)
    {
        if (name != "oidc")
        {
            return;
        }

        // Without PAR: let the wrapped handlers add their parameters first (e.g. dpop_jkt), then sign all parameters.
        var redirect = options.Events.OnRedirectToIdentityProvider;
        options.Events.OnRedirectToIdentityProvider = async context =>
        {
            await redirect(context);

            // Only sign here when PAR is not actually used. With the default UseIfAvailable behavior, the handler
            // falls back to a regular redirect (and skips OnPushAuthorization) when discovery has no PAR endpoint,
            // so the request must be signed on this path as well.
            if (!await UsesPushedAuthorization(context.Options, context.HttpContext.RequestAborted))
            {
                await SignRequest(context.HttpContext, context.ProtocolMessage, keepRedirectUri: true);
            }
        };

        // With PAR: sign all parameters first, then let access token management add the client assertion.
        var push = options.Events.OnPushAuthorization;
        options.Events.OnPushAuthorization = async context =>
        {
            await SignRequest(context.HttpContext, context.ProtocolMessage, keepRedirectUri: false);

            await push(context);
        };
    }

    // Mirrors the OpenID Connect handler: Disable never uses PAR, Require always does, and UseIfAvailable only
    // uses PAR when the discovery document advertises a pushed authorization request endpoint.
    private static async Task<bool> UsesPushedAuthorization(OpenIdConnectOptions options, CancellationToken cancellationToken)
    {
        switch (options.PushedAuthorizationBehavior)
        {
            case PushedAuthorizationBehavior.Disable:
                return false;
            case PushedAuthorizationBehavior.Require:
                return true;
            default:
                var configuration = options.Configuration;
                if (configuration == null && options.ConfigurationManager != null)
                {
                    configuration = await options.ConfigurationManager.GetConfigurationAsync(cancellationToken);
                }

                return !string.IsNullOrEmpty(configuration?.PushedAuthorizationRequestEndpoint);
        }
    }

    private static async Task SignRequest(HttpContext httpContext, OpenIdConnectMessage message, bool keepRedirectUri)
    {
        // IClientAssertionService is expected to be registered in DI as this sample's ClientAssertionService
        // (see Startup.cs), which also knows how to sign authorize requests.
        var assertionService = (ClientAssertionService)httpContext.RequestServices.GetRequiredService<IClientAssertionService>();
        var request = await assertionService.SignAuthorizeRequest(message, httpContext.RequestAborted);
        var clientId = message.ClientId;
        var redirectUri = message.RedirectUri;

        message.Parameters.Clear();
        message.ClientId = clientId;
        if (keepRedirectUri)
        {
            message.RedirectUri = redirectUri;
        }

        message.SetParameter("request", request);
    }
}
