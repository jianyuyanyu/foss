// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// Delegate to generate a key to store a DPoP nonce in the Cache. 
/// </summary>
/// <param name="context"></param>
/// <returns></returns>
public delegate string DPoPNonceStoreKeyGenerator(DPoPNonceContext context);
