// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.DPoP;
using Duende.AccessTokenManagement.OpenIdConnect;

namespace Duende.AccessTokenManagement.AccessTokenHandler.Helpers;

public class TestAccessTokens(DPoPProofKey? dPoPJsonWebKey)
{
    public UserToken UserToken =
        new()
        {
            ClientId = ClientId.Parse("clientId"),
            IdentityToken = IdentityToken.Parse("identity_token"),
            AccessToken = AccessToken.Parse("access_token_1"),
            AccessTokenType = AccessTokenType.Parse("Bearer"),
            /*
             * Expiring the token allows us to exercise the Token Endpoint instead of relying on the token retrieved
             * from the authentication service
             */
            Expiration = DateTimeOffset.UtcNow.AddSeconds(-1),
            RefreshToken = RefreshToken.Parse("refresh_token"),
            DPoPJsonWebKey = dPoPJsonWebKey
        };
}
