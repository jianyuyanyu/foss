// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement;

/// <summary>
/// DPoP nonce store using IDistributedCache
/// </summary>
public class DistributedDPoPNonceStore(
    IDistributedCache cache,
    ILogger<DistributedDPoPNonceStore> logger) : IDPoPNonceStore
{
    const string CacheKeyPrefix = "DistributedDPoPNonceStore";
    const char CacheKeySeparator = ':';

    /// <inheritdoc/>
    public virtual async Task<string?> GetNonceAsync(DPoPNonceContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var cacheKey = GenerateCacheKey(context);
        var entry = await cache.GetStringAsync(cacheKey, token: cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            logger.CacheHitForDPoPNonce(context.Url, context.Method);
            return entry;
        }

        logger.CacheMissForDPoPNonce(context.Url, context.Method);
        return null;
    }

    /// <inheritdoc/>
    public virtual async Task StoreNonceAsync(DPoPNonceContext context, string nonce, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var cacheExpiration = DateTimeOffset.UtcNow.AddHours(1);
        var data = nonce;

        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = cacheExpiration
        };

        logger.WritingNonceToCache(context.Url, context.Method, cacheExpiration);

        var cacheKey = GenerateCacheKey(context);
        await cache.SetStringAsync(cacheKey, data, entryOptions, token: cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// Generates the cache key based on various inputs
    /// </summary>
    protected virtual string GenerateCacheKey(DPoPNonceContext context)
    {
        return $"{CacheKeyPrefix}{CacheKeySeparator}{context.Url}{CacheKeySeparator}{context.Method}";
    }
}
