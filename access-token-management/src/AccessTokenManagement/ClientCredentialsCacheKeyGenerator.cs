// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// Delegate to generate a cache key for a client credentials token request
/// </summary>
/// <param name="clientName">The name of the client</param>
/// <param name="parameters">The parameters</param>
/// <returns></returns>
public delegate string ClientCredentialsCacheKeyGenerator(
    string clientName,
    TokenRequestParameters? parameters = null);
