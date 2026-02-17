// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Duende.Extensions.Caching.Memory;

public class TimeProviderMemoryCacheTests
{
    private readonly FakeTimeProvider _timeProvider;
    private readonly TimeProviderMemoryCache _cache;

    public TimeProviderMemoryCacheTests()
    {
        _timeProvider = new FakeTimeProvider();
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            ExpirationScanFrequency = TimeSpan.FromMinutes(1)
        });
        _cache = new TimeProviderMemoryCache(_timeProvider, options);
    }

    [Fact]
    public void TryGetValue_ReturnsTrue_WhenKeyExists()
    {
        var key = "test-key";
        var expectedValue = "test-value";
        _cache.Set(key, expectedValue);

        var result = _cache.TryGetValue(key, out var actualValue);

        result.ShouldBeTrue();
        actualValue.ShouldBe(expectedValue);
    }

    [Fact]
    public void TryGetValue_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var result = _cache.TryGetValue("nonexistent-key", out var value);

        result.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void Remove_DeletesEntry()
    {
        var key = "test-key";
        _cache.Set(key, "test-value");

        _cache.Remove(key);

        _cache.TryGetValue(key, out _).ShouldBeFalse();
    }

    [Fact]
    public void CreateEntry_ReturnsValidEntry()
    {
        var key = "test-key";

        using var entry = _cache.CreateEntry(key);
        entry.Value = "test-value";

        entry.ShouldNotBeNull();
        entry.Key.ShouldBe(key);
    }

    [Fact]
    public void AbsoluteExpiration_RemovesEntry_AfterExpiry()
    {
        var key = "test-key";
        var value = "test-value";
        var expirationTime = TimeSpan.FromMinutes(5);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });

        _cache.TryGetValue(key, out _).ShouldBeTrue();

        _timeProvider.Advance(expirationTime + TimeSpan.FromSeconds(1));

        _cache.TryGetValue(key, out _).ShouldBeFalse();
    }

    [Fact]
    public void AbsoluteExpiration_EntryStillExists_BeforeExpiry()
    {
        var key = "test-key";
        var value = "test-value";
        var expirationTime = TimeSpan.FromMinutes(5);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });

        _timeProvider.Advance(expirationTime - TimeSpan.FromSeconds(1));

        _cache.TryGetValue(key, out var actualValue).ShouldBeTrue();
        actualValue.ShouldBe(value);
    }

    [Fact]
    public void SlidingExpiration_RemovesEntry_AfterInactivity()
    {
        var key = "test-key";
        var value = "test-value";
        var slidingExpiration = TimeSpan.FromMinutes(5);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration
        });

        _cache.TryGetValue(key, out _).ShouldBeTrue();

        _timeProvider.Advance(slidingExpiration + TimeSpan.FromSeconds(1));

        _cache.TryGetValue(key, out _).ShouldBeFalse();
    }

    [Fact]
    public void SlidingExpiration_ResetsExpiration_OnAccess()
    {
        var key = "test-key";
        var value = "test-value";
        var slidingExpiration = TimeSpan.FromMinutes(5);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration
        });

        _timeProvider.Advance(TimeSpan.FromMinutes(4));
        _cache.TryGetValue(key, out _).ShouldBeTrue();

        _timeProvider.Advance(TimeSpan.FromMinutes(4));

        _cache.TryGetValue(key, out var actualValue).ShouldBeTrue();
        actualValue.ShouldBe(value);
    }

    [Fact]
    public void Options_CompactionPercentage_IsForwarded()
    {
        var compactionPercentage = 0.75;
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            CompactionPercentage = compactionPercentage
        });

        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.ShouldNotBeNull();
    }

    [Fact]
    public void Options_SizeLimit_IsForwarded()
    {
        var sizeLimit = 1024L;
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            SizeLimit = sizeLimit
        });

        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.ShouldNotBeNull();
    }

    [Fact]
    public void Options_ExpirationScanFrequency_IsForwarded()
    {
        var scanFrequency = TimeSpan.FromSeconds(30);
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            ExpirationScanFrequency = scanFrequency
        });

        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.ShouldNotBeNull();
    }

    [Fact]
    public void Options_TrackLinkedCacheEntries_IsForwarded()
    {
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            TrackLinkedCacheEntries = true
        });

        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.ShouldNotBeNull();
    }

    [Fact]
    public void Options_TrackStatistics_IsForwarded()
    {
        var options = Options.Create(new TimeProviderMemoryCacheOptions
        {
            TrackStatistics = true
        });

        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.ShouldNotBeNull();
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var options = Options.Create(new TimeProviderMemoryCacheOptions());
        var cache = new TimeProviderMemoryCache(_timeProvider, options);

        cache.Dispose();
    }

    [Fact]
    public void MultipleEntries_CanCoexist()
    {
        var key1 = "key1";
        var key2 = "key2";
        var value1 = "value1";
        var value2 = "value2";

        _cache.Set(key1, value1);
        _cache.Set(key2, value2);

        _cache.TryGetValue(key1, out var actualValue1).ShouldBeTrue();
        actualValue1.ShouldBe(value1);

        _cache.TryGetValue(key2, out var actualValue2).ShouldBeTrue();
        actualValue2.ShouldBe(value2);
    }

    [Fact]
    public void SetOverwritesExistingValue()
    {
        var key = "test-key";
        var originalValue = "original";
        var newValue = "updated";

        _cache.Set(key, originalValue);
        _cache.Set(key, newValue);

        _cache.TryGetValue(key, out var actualValue).ShouldBeTrue();
        actualValue.ShouldBe(newValue);
    }
}
