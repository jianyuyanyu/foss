// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.DPoP;

using Microsoft.AspNetCore.Http;

namespace Duende.AccessTokenManagement.OpenIdConnect.Internal;

internal class OpenIdConnectClientAccessTokenRetriever(
    IHttpContextAccessor httpContextAccessor,
    UserTokenRequestParameters? parameters = null)
    : AccessTokenRequestHandler.ITokenRetriever
{
    private readonly UserTokenRequestParameters _parameters = parameters ?? new UserTokenRequestParameters();

    public async Task<TokenResult<AccessTokenRequestHandler.IToken>> GetToken(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userTokenRequestParameters = new UserTokenRequestParameters
        {
            ChallengeScheme = _parameters.ChallengeScheme,
            Scope = _parameters.Scope,
            Resource = _parameters.Resource,
            Parameters = _parameters.Parameters,
            Assertion = _parameters.Assertion,
            Context = _parameters.Context,
            ForceTokenRenewal = request.GetForceRenewal()
        };

        if (httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext is null");
        }

        var getTokenResult = await httpContextAccessor.HttpContext.GetClientAccessTokenAsync(
                userTokenRequestParameters,
                cancellationToken)
            .ConfigureAwait(false);

        if (getTokenResult.WasSuccessful(out var token, out var error))
        {
            return token;
        }

        return error;
    }
}
