// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement.OpenIdConnect;

/// <summary>
/// Delegating handler that injects the current access token into an outgoing request
/// </summary>
[Obsolete("This type is going to be made internal in future releases.")]
public class OpenIdConnectClientAccessTokenHandler(
    IDPoPProofService dPoPProofService,
    IDPoPNonceStore dPoPNonceStore,
    IHttpContextAccessor httpContextAccessor,
    ILogger<OpenIdConnectClientAccessTokenHandler> logger,
    UserTokenRequestParameters? parameters = null) : AccessTokenHandler(dPoPProofService, dPoPNonceStore, logger)
{
    private readonly UserTokenRequestParameters _parameters = parameters ?? new UserTokenRequestParameters();

    /// <inheritdoc/>
    protected override async Task<ClientCredentialsToken> GetAccessTokenAsync(bool forceRenewal, CancellationToken cancellationToken)
    {
        var userTokenRequestParameters = new UserTokenRequestParameters
        {
            ChallengeScheme = _parameters.ChallengeScheme,
            Scope = _parameters.Scope,
            Resource = _parameters.Resource,
            Parameters = _parameters.Parameters,
            Assertion = _parameters.Assertion,
            Context = _parameters.Context,
            ForceRenewal = forceRenewal,
        };

        if (httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext is null");
        }

        return await httpContextAccessor.HttpContext.GetClientAccessTokenAsync(
                userTokenRequestParameters,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
