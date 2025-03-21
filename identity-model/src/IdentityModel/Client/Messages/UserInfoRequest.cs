// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.IdentityModel.Client;

/// <summary>
/// Request for OIDC userinfo
/// </summary>
public class UserInfoRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the token.
    /// </summary>
    /// <value>
    /// The token.
    /// </value>
    public string? Token { get; set; }
}
