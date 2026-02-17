// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.Extensions.Caching.Memory;

/// <summary>
/// Specifies options for <see cref="TimeProviderMemoryCache"/>.
/// </summary>
public class TimeProviderMemoryCacheOptions
{
    /// <summary>
    /// Gets or sets the amount the cache is compacted by when the maximum size is exceeded.
    /// </summary>
    public double CompactionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the minimum length of time between successive scans for expired items.
    /// </summary>
    public TimeSpan ExpirationScanFrequency { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the cache.
    /// </summary>
    /// <remarks>
    /// The units are arbitrary. Users specify the size of every entry they add to the cache.
    /// If no size is specified, the entry has no size and the size limit is ignored for that entry.
    /// </remarks>
    public long? SizeLimit { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether linked entries are tracked.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if linked entries are tracked; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool TrackLinkedCacheEntries { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether memory cache statistics are tracked.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if memory cache statistics are tracked; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool TrackStatistics { get; set; }
}
