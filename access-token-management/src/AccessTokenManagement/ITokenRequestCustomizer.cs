// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// Service for customizing token request parameters based on HTTP request context
/// </summary>
public interface ITokenRequestCustomizer
{
    /// <summary>
    /// Customize token request parameters based on the incoming HTTP request
    /// </summary>
    /// <param name="httpRequest">The incoming HTTP request that triggered the token request</param>
    /// <param name="baseParameters">The base token request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customized token request parameters</returns>
    Task<TokenRequestParameters> Customize(
        HttpRequestMessage httpRequest,
        TokenRequestParameters baseParameters,
        CancellationToken cancellationToken = default);
}
