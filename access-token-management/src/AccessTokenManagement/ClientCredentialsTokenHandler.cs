// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Delegating handler that injects a client credentials access token into an outgoing request
/// </summary>
[Obsolete("This type is going to be removed in a future release.")]
public class ClientCredentialsTokenHandler(
    IDPoPProofService dPoPProofService,
    IDPoPNonceStore dPoPNonceStore,
    IClientCredentialsTokenManagementService accessTokenManagementService,
    ILogger<ClientCredentialsTokenHandler> logger,
    string tokenClientName)
    : AccessTokenHandler(dPoPProofService, dPoPNonceStore, logger)
{
    /// <inheritdoc/>
    protected override Task<ClientCredentialsToken> GetAccessTokenAsync(bool forceRenewal, CancellationToken cancellationToken)
    {
        var parameters = new TokenRequestParameters
        {
            ForceRenewal = forceRenewal
        };
        return accessTokenManagementService.GetAccessTokenAsync(tokenClientName, parameters, cancellationToken);
    }
}
