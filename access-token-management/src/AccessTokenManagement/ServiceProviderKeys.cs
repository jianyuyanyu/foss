// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// The keys used to store services in the DI container
/// </summary>
public static class ServiceProviderKeys
{
    public const string DistributedClientCredentialsTokenCache = "DistributedClientCredentialsTokenCache";
    public const string DistributedDPoPNonceStore = "DistributedDPoPNonceStore";
}
