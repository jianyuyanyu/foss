// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Duende.TokenManagement.ClientCredentials;

/// <summary>
/// Abstraction for managing client access tokens
/// </summary>
public interface IClientCredentialsTokenManagementService
{
    /// <summary>
    /// Returns either a cached or a new access token for a given client configuration, the default client or a given token request
    /// </summary>
    /// <param name="clientName">Name of the client configuration, or default is omitted.</param>
    /// <param name="request"></param>
    /// <param name="parameters">Optional parameters.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The access token or null if the no token can be requested.</returns>
    Task<ClientCredentialsAccessToken> GetAccessTokenAsync(
        string clientName = TokenManagementDefaults.DefaultTokenClientName, 
        ClientCredentialsTokenRequest? request = null,
        AccessTokenRequestParameters? parameters = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a client access token from the cache
    /// </summary>
    /// <param name="clientName">Name of the client configuration, or default is omitted.</param>
    /// <param name="parameters">Optional parameters.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The access token or null if the no token can be requested.</returns>
    Task DeleteAccessTokenAsync(
        string clientName = TokenManagementDefaults.DefaultTokenClientName, 
        AccessTokenRequestParameters? parameters = null, 
        CancellationToken cancellationToken = default);
}