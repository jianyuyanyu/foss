// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Duende.Extensions.Caching.Memory;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTimeProviderMemoryCache_RegistersIMemoryCache()
    {
        var services = new ServiceCollection();

        services.AddTimeProviderMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetService<IMemoryCache>();
        cache.ShouldNotBeNull();
        cache.ShouldBeOfType<TimeProviderMemoryCache>();
    }

    [Fact]
    public void AddTimeProviderMemoryCache_RegistersTimeProvider()
    {
        var services = new ServiceCollection();

        services.AddTimeProviderMemoryCache();
        var provider = services.BuildServiceProvider();

        var timeProvider = provider.GetService<TimeProvider>();
        timeProvider.ShouldNotBeNull();
        timeProvider.ShouldBe(TimeProvider.System);
    }

    [Fact]
    public void AddTimeProviderMemoryCache_WithOptions_ConfiguresOptions()
    {
        var services = new ServiceCollection();
        var expectedScanFrequency = TimeSpan.FromMinutes(5);
        var expectedSizeLimit = 2048L;

        services.AddTimeProviderMemoryCache(options =>
        {
            options.ExpirationScanFrequency = expectedScanFrequency;
            options.SizeLimit = expectedSizeLimit;
        });
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IMemoryCache>();
        cache.ShouldNotBeNull();
        cache.ShouldBeOfType<TimeProviderMemoryCache>();
    }

    [Fact]
    public void AddTimeProviderMemoryCache_DoesNotOverrideExistingTimeProvider()
    {
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();
        services.AddSingleton<TimeProvider>(customTimeProvider);

        services.AddTimeProviderMemoryCache();
        var provider = services.BuildServiceProvider();

        var timeProvider = provider.GetService<TimeProvider>();
        timeProvider.ShouldBe(customTimeProvider);
    }

    [Fact]
    public void AddTimeProviderMemoryCache_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddTimeProviderMemoryCache();

        result.ShouldBe(services);
    }

    [Fact]
    public void AddTimeProviderMemoryCache_WithOptions_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddTimeProviderMemoryCache(options => { });

        result.ShouldBe(services);
    }
}
