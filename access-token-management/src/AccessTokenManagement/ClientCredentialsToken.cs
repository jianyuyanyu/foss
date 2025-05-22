// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.DPoP;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Record that captures information about client credentials tokens. 
/// </summary>
public sealed record ClientCredentialsToken : AccessTokenRequestHandler.IToken
{
    /// <summary>
    /// The value of the access token. 
    /// </summary>
    public required AccessToken AccessToken { get; init; }

    /// <summary>
    /// the type of the access token. Typially Bearer or DPoP.
    /// </summary>
    public required AccessTokenType? AccessTokenType { get; init; }

    /// <summary>
    /// When using dpop, this was the proofkey that was used. 
    /// </summary>
    public required DPoPProofKey? DPoPJsonWebKey { get; init; }

    /// <summary>
    /// When the access token expires. When no expiration is set, DateTimeOffset.MaxValue is used.
    /// </summary>
    public required DateTimeOffset Expiration { get; init; }

    /// <summary>
    /// The scope(s) associated with the access token. 
    /// </summary>
    public required Scope? Scope { get; init; }

    /// <summary>
    /// The OIDC ClientID that was used to retrieve the access token. 
    /// </summary>
    public required ClientId ClientId { get; init; }
}
