// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

namespace Duende.Extensions.Caching.Memory;

/// <summary>
/// An <see cref="IMemoryCache"/> implementation that uses a <see cref="TimeProvider"/>
/// as its time source, enabling testable time-dependent caching.
/// </summary>
public class TimeProviderMemoryCache : IMemoryCache
{
    private readonly MemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of <see cref="TimeProviderMemoryCache"/>.
    /// </summary>
    /// <param name="timeProvider">The <see cref="TimeProvider"/> used as the time source for cache expiration.</param>
    /// <param name="optionsAccessor">The optionsAccessor for configuring the cache.</param>
    public TimeProviderMemoryCache(TimeProvider timeProvider, IOptions<TimeProviderMemoryCacheOptions> optionsAccessor)
    {
        var memoryCacheOptions = new MemoryCacheOptions
        {
            Clock = new TimeProviderSystemClock(timeProvider),
            CompactionPercentage = optionsAccessor.Value.CompactionPercentage,
            ExpirationScanFrequency = optionsAccessor.Value.ExpirationScanFrequency,
            SizeLimit = optionsAccessor.Value.SizeLimit,
            TrackLinkedCacheEntries = optionsAccessor.Value.TrackLinkedCacheEntries,
            TrackStatistics = optionsAccessor.Value.TrackStatistics
        };
        _memoryCache = new MemoryCache(Options.Create(memoryCacheOptions));
    }

    /// <inheritdoc />
    public void Dispose()
        => _memoryCache.Dispose();

    /// <inheritdoc />
    public bool TryGetValue(object key, out object? value)
        => _memoryCache.TryGetValue(key, out value);

    /// <inheritdoc />
    public ICacheEntry CreateEntry(object key)
        => _memoryCache.CreateEntry(key);

    /// <inheritdoc />
    public void Remove(object key)
        => _memoryCache.Remove(key);

    private class TimeProviderSystemClock(TimeProvider timeProvider) : ISystemClock
    {
        public DateTimeOffset UtcNow => timeProvider.GetUtcNow();
    }
}
