// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using Duende.AccessTokenManagement.Framework;

namespace Duende.AccessTokenManagement.TokenRequestCustomizer;

internal static class HttpResponseMessageExtensions
{
    internal static JwtSecurityToken ParseTokenFromResponse(this HttpResponseMessage response)
    {
        var result = response.Content.ReadAsStringAsync().Result;
        var tokenResult = System.Text.Json.JsonSerializer.Deserialize<TokenEchoResponse>(result);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenResult!.token.Replace("Bearer ", string.Empty));
        return token;
    }
}
