// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement.TokenRequestCustomizer;

internal class TestTokenRequestCustomizer(Func<HttpRequestContext, TokenRequestParameters> customizeParameters)
    : ITokenRequestCustomizer
{
    public Task<TokenRequestParameters> Customize(
        HttpRequestContext httpRequest,
        TokenRequestParameters baseParameters,
        CancellationToken cancellationToken = default)
    {
        var customized = customizeParameters(httpRequest);

        var merged = baseParameters with
        {
            ForceTokenRenewal = customized.ForceTokenRenewal,
            Scope = customized.Scope ?? baseParameters.Scope,
            Resource = customized.Resource ?? baseParameters.Resource,
            Parameters = customized.Parameters.Count > 0 ? customized.Parameters : baseParameters.Parameters,
            Assertion = customized.Assertion ?? baseParameters.Assertion,
            Context = customized.Context.Count > 0 ? customized.Context : baseParameters.Context
        };

        return Task.FromResult(merged);
    }
}
