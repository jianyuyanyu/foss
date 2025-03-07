// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Client access token cache using IDistributedCache
/// </summary>
public class DistributedClientCredentialsTokenCache(
    [FromKeyedServices(ServiceProviderKeys.DistributedClientCredentialsTokenCache)] IDistributedCache cache,
    ITokenRequestSynchronization synchronization,
    IOptions<ClientCredentialsTokenManagementOptions> options,
    ILogger<DistributedClientCredentialsTokenCache> logger
    )
    : IClientCredentialsTokenCache
{
    private readonly ClientCredentialsTokenManagementOptions _options = options.Value;

    /// <inheritdoc/>
    public async Task SetAsync(
        string clientName,
        ClientCredentialsToken clientCredentialsToken,
        TokenRequestParameters requestParameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clientName);

        var cacheExpiration = clientCredentialsToken.Expiration.AddSeconds(-_options.CacheLifetimeBuffer);
        var data = JsonSerializer.Serialize(clientCredentialsToken);

        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = cacheExpiration
        };

        logger.LogTrace("Caching access token for client: {clientName}. Expiration: {expiration}", clientName, cacheExpiration);

        var cacheKey = GenerateCacheKey(_options, clientName, requestParameters);
        await cache.SetStringAsync(cacheKey, data, entryOptions, token: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ClientCredentialsToken> GetOrCreateAsync(
        string clientName, TokenRequestParameters requestParameters,
        Func<string, TokenRequestParameters, CancellationToken, Task<ClientCredentialsToken>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clientName);

        var cacheKey = GenerateCacheKey(_options, clientName, requestParameters);

        if (!requestParameters.ForceRenewal)
        {
            var token = await GetAsync(clientName, requestParameters, cancellationToken);
            if (token != null)
            {
                return token;
            }
        }

        return await synchronization.SynchronizeAsync(cacheKey, async () =>
        {
            var token = await factory(clientName, requestParameters, cancellationToken).ConfigureAwait(false);
            if (token.IsError)
            {
                logger.LogError(
                    "Error requesting access token for client {clientName}. Error = {error}.",
                    clientName, token.Error);

                return token;
            }

            try
            {
                await SetAsync(clientName, token, requestParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "Error trying to set token in cache for client {clientName}. Error = {error}",
                    clientName, e.Message);
            }

            return token;
        }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ClientCredentialsToken?> GetAsync(
        string clientName,
        TokenRequestParameters requestParameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clientName);

        var cacheKey = GenerateCacheKey(_options, clientName, requestParameters);
        string? entry;

        try
        {
            entry = await cache.GetStringAsync(cacheKey, token: cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to acquire cached item for {clientName} using key: {cacheKey}", clientName, cacheKey);
            return null;
        }

        if (entry != null)
        {
            try
            {
                logger.LogDebug("Cache hit for access token for client: {clientName}", clientName);
                return JsonSerializer.Deserialize<ClientCredentialsToken>(entry);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Error parsing cached access token for client {clientName}", clientName);
                return null;
            }
        }

        logger.LogTrace("Cache miss for access token for client: {clientName}", clientName);
        return null;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(
        string clientName,
        TokenRequestParameters requestParameters,
        CancellationToken cancellationToken = default)
    {
        if (clientName is null) throw new ArgumentNullException(nameof(clientName));

        var cacheKey = GenerateCacheKey(_options, clientName, requestParameters);
        return cache.RemoveAsync(cacheKey, cancellationToken);
    }

    /// <summary>
    /// Generates the cache key based on various inputs
    /// </summary>
    /// <param name="options"></param>
    /// <param name="clientName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected virtual string GenerateCacheKey(
        ClientCredentialsTokenManagementOptions options,
        string clientName,
        TokenRequestParameters? parameters = null)
    {
        var s = "s_" + parameters?.Scope ?? "";
        var r = "r_" + parameters?.Resource ?? "";

        return options.CacheKeyPrefix + clientName + "::" + s + "::" + r;
    }
}
