// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register TimeProviderMemoryCache.
/// </summary>
public static class TimeProviderMemoryCacheServiceCollectionExtensions
{
    /// <summary>
    /// Adds a TimeProvider-based IMemoryCache implementation to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTimeProviderMemoryCache(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddOptions<TimeProviderMemoryCacheOptions>();
        services.AddSingleton<IMemoryCache, TimeProviderMemoryCache>();
        return services;
    }

    /// <summary>
    /// Adds a TimeProvider-based IMemoryCache implementation to the service collection with custom options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure the cache options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTimeProviderMemoryCache(
        this IServiceCollection services,
        Action<TimeProviderMemoryCacheOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services.AddTimeProviderMemoryCache();
    }
}
