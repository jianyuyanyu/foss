// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.DPoP;

namespace Duende.AccessTokenManagement.Internal;

/// <summary>
/// An <see cref="AccessTokenRequestHandler.ITokenRetriever" /> implementation that retrieves a token using the client credentials flow.
/// </summary>
internal class ClientCredentialsTokenRetriever(
    IClientCredentialsTokenManager clientCredentialsTokenManager,
    ClientCredentialsClientName clientName,
    ITokenRequestCustomizer? customizer = null
) : AccessTokenRequestHandler.ITokenRetriever
{
    /// <inheritdoc />
    public async Task<TokenResult<AccessTokenRequestHandler.IToken>> GetTokenAsync(HttpRequestMessage request, CT ct)
    {
        var baseParameters = new TokenRequestParameters
        {
            ForceTokenRenewal = request.GetForceRenewal()
        };

        var parameters = customizer != null
            ? await customizer.Customize(request.ToHttpRequestContext(), baseParameters, ct)
            : baseParameters;

        var getTokenResult = await clientCredentialsTokenManager.GetAccessTokenAsync(clientName, parameters, ct);

        if (getTokenResult.WasSuccessful(out var token, out var error))
        {
            return token;
        }

        return error;
    }
}
