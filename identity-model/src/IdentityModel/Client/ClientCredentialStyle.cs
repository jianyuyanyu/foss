// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.IdentityModel.Client;

/// <summary>
/// Specifies how the client will transmit client ID and secret
/// </summary>
public enum ClientCredentialStyle
{
    /// <summary>
    /// HTTP basic authentication
    /// </summary>
    AuthorizationHeader,

    /// <summary>
    /// Post values in body
    /// </summary>
    PostBody
};
