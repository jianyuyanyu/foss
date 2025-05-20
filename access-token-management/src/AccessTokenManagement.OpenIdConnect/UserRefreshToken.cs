// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Duende.AccessTokenManagement.DPoP;

namespace Duende.AccessTokenManagement.OpenIdConnect;

/// <summary>
/// A record that captures the information to refresh an access token for a user.
///
/// Minimally, you need a refresh token. If you use dpop, you'll also need the dpop proof key
/// </summary>
public sealed record UserRefreshToken(RefreshToken RefreshToken, DPoPProofKey? DPoPProofKey);
