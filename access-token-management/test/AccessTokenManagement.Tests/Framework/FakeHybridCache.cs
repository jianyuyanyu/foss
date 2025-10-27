// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Hybrid;

namespace Duende.AccessTokenManagement.Framework;

public class FakeHybridCache : HybridCache
{
    public int GetOrCreateCount;

    public string? CacheKey;

    public Action OnGetOrCreate = () => { };

    public HybridCacheEntryOptions? LastOptions { get; private set; }

    public override async ValueTask<T> GetOrCreateAsync<TState, T>(string key, TState state,
        Func<TState, CancellationToken, ValueTask<T>> factory, HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = new())
    {
        CacheKey = key;
        LastOptions = options;
        Interlocked.Increment(ref GetOrCreateCount);
        OnGetOrCreate();
        return await factory(state, cancellationToken);
    }

    public override ValueTask SetAsync<T>(string key, T value, HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();
}
