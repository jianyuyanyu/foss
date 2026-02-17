# Duende.Extensions.Caching.Memory

A `TimeProvider`-based implementation of `IMemoryCache` that enables testable time-dependent caching. This exists because
of [this outstanding issue](https://github.com/dotnet/runtime/issues/114011). This library will be obsolete once that issue is resolved
in some future version of .NET SDK.

## Overview

This library provides a wrapper around `Microsoft.Extensions.Caching.Memory.MemoryCache` that integrates with .NET's `TimeProvider` abstraction. This allows you to:

- Write unit tests that manipulate time using `FakeTimeProvider`
- Verify cache expiration behavior without real delays
- Maintain full compatibility with the standard `IMemoryCache` interface

## Usage

### Direct Instantiation

```csharp
var timeProvider = TimeProvider.System; // or FakeTimeProvider in tests
var options = Options.Create(new TimeProviderMemoryCacheOptions
{
    ExpirationScanFrequency = TimeSpan.FromMinutes(1),
    SizeLimit = 1024
});

var cache = new TimeProviderMemoryCache(timeProvider, options);

// Use like any IMemoryCache
cache.Set("key", "value", TimeSpan.FromMinutes(5));
```

### Dependency Injection

```csharp
// Basic registration (uses TimeProvider.System)
services.AddTimeProviderMemoryCache();

// With configuration
services.AddTimeProviderMemoryCache(options =>
{
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
    options.SizeLimit = 1024;
});
```

## Testing Example

```csharp
var fakeTime = new FakeTimeProvider();
var cache = new TimeProviderMemoryCache(fakeTime, options);

cache.Set("key", "value", TimeSpan.FromMinutes(5));

// Advance time past expiration
fakeTime.Advance(TimeSpan.FromMinutes(6));

// Entry should be expired
cache.TryGetValue("key", out var result).ShouldBeFalse();
```

## License

Apache 2.0 - see [LICENSE](https://github.com/DuendeSoftware/foss/blob/main/LICENSE) for details.
