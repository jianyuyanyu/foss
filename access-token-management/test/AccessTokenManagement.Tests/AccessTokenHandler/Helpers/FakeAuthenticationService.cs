// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Duende.AccessTokenManagement.AccessTokenHandler.Helpers;

public class FakeAuthenticationService(
    IStoreTokensInAuthenticationProperties storeTokens,
    TestAccessTokens testAccessTokens)
    : IAuthenticationService
{
    public ClaimsPrincipal Principal = new(new ClaimsIdentity([new Claim(JwtClaimTypes.Subject, "sub")], "test"));

    public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
    {
        var properties = await BuildProperties();
        return AuthenticateResult.Success(new AuthenticationTicket(Principal, properties, "oidc"));
    }

    private async Task<AuthenticationProperties> BuildProperties()
    {
        var properties = new AuthenticationProperties();
        await storeTokens.SetUserTokenAsync(testAccessTokens.UserToken, properties);
        return properties;
    }

    public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;

    public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;

    public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal,
        AuthenticationProperties? properties) => Task.CompletedTask;

    public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;
}
