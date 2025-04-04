// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityModel.Client;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Defines a client credentials client
/// </summary>
public class ClientCredentialsClient
{
    /// <summary>
    /// The address of the token endpoint
    /// </summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>
    /// The client ID 
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// The static (shared) client secret
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// The client credential transmission style
    /// </summary>
    public ClientCredentialStyle ClientCredentialStyle { get; set; }

    /// <summary>
    /// Gets or sets the basic authentication header style (classic HTTP vs OAuth 2).
    /// </summary>
    /// <value>
    /// The basic authentication header style.
    /// </value>
    public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

    /// <summary>
    /// The scope
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// The resource
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// The HTTP client name to use for the backchannel operations, will fall back to the standard backchannel client if not set
    /// </summary>
    public string? HttpClientName { get; set; }

    /// <summary>
    /// Additional parameters to send with token requests.
    /// </summary>
    public Parameters Parameters { get; set; } = [];

    /// <summary>
    /// The HTTP client instance to use for the back-channel operations, will override the HTTP client name if set
    /// </summary>
    public HttpClient? HttpClient { get; set; }

    /// <summary>
    /// The string representation of the JSON web key to use for DPoP.
    /// </summary>
    public string? DPoPJsonWebKey { get; set; }
}
