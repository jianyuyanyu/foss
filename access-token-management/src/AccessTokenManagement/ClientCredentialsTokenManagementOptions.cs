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

        GenerateNonceStoreKey = (context => $"{NonceStoreKeyPrefix}:{context.Url}:{context.Method}");
    }

    /// <summary>
    /// Used to prefix the cache key
    /// </summary>
    public string CacheKeyPrefix { get; set; } = "Duende.AccessTokenManagement.Cache::";

    public string NonceStoreKeyPrefix { get; set; } = "Duende.AccessTokenManagement.DPoPNonceStore::";

    /// <summary>
    /// Value to subtract from token lifetime for the cache entry lifetime (defaults to 60 seconds)
    /// </summary>
    public int CacheLifetimeBuffer { get; set; } = 60;

    /// <summary>
    /// The logic to generate a cache key. Defaults to a key based on <see cref="CacheKeyPrefix"/>, client name, scope, and resource.
    /// Customize this if you add additional <see cref="TokenRequestParameters.Parameters"/> to your TokenRequest that
    /// impact how this should be cached. 
    /// </summary>
    public ClientCredentialsCacheKeyGenerator GenerateCacheKey { get; private set; }

    /// <summary>
    /// The logic to generate a key to store a DPoP nonce in the Cache. Defaults to
    /// <see cref="NonceStoreKeyPrefix"/> + URL + Method.
    /// </summary>
    public DPoPNonceStoreKeyGenerator GenerateNonceStoreKey { get; private set; }

}
