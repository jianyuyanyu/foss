// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.Internal;

/// <summary>
/// The logic to generate a cache key. Defaults to a key based on <see cref="ClientCredentialsTokenManagementOptions.CacheKeyPrefix"/>, client name, scope, and resource.
/// Customize this if you add additional <see cref="TokenRequestParameters.Parameters"/> to your TokenRequest that
/// impact how this should be cached.
/// </summary>
/// <param name="options"></param>
internal class DefaultClientCredentialsCacheKeyGenerator(
    IOptions<ClientCredentialsTokenManagementOptions> options) : IClientCredentialsCacheKeyGenerator
{
    public ClientCredentialsCacheKey GenerateKey(
        ClientCredentialsClientName clientName,
        TokenRequestParameters? parameters = null)
    {
        var scopePart = "s_" + GetCacheKey(parameters?.Scope ?? string.Empty);
        var resourcePart = "r_" + GetCacheKey(parameters?.Resource ?? string.Empty);

        return ClientCredentialsCacheKey.Parse(options.Value.CacheKeyPrefix + clientName + "::" + scopePart + "::" +
                                               resourcePart);
    }

    private static string GetCacheKey(string value)
    {
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hashBytes);
    }
}
