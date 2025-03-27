// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// Client access token options
/// </summary>
public class ClientCredentialsTokenManagementOptions
{
    public ClientCredentialsTokenManagementOptions()
    {
        GenerateCacheKey = ((clientName, parameters) =>
        {
            var s = "s_" + parameters?.Scope ?? "";
            var r = "r_" + parameters?.Resource ?? "";

            return CacheKeyPrefix + clientName + "::" + s + "::" + r;
        });
    }

    /// <summary>
    /// Used to prefix the cache key
    /// </summary>
    public string CacheKeyPrefix { get; set; } = "Duende.AccessTokenManagement.Cache::";

    /// <summary>
    /// Value to subtract from token lifetime for the cache entry lifetime (defaults to 60 seconds)
    /// </summary>
    public int CacheLifetimeBuffer { get; set; } = 60;

    /// <summary>
    /// The logic to generate a cache key. Defaults to a simple key based on client name, scope, and resource.
    /// Customize this if you add additional <see cref="TokenRequestParameters.Parameters"/> to your TokenRequest that
    /// impact how this should be cached. 
    /// </summary>
    public ClientCredentialsCacheKeyGenerator GenerateCacheKey { get; private set; }
    
}

public delegate string ClientCredentialsCacheKeyGenerator(
    string clientName,
    TokenRequestParameters? parameters = null);
