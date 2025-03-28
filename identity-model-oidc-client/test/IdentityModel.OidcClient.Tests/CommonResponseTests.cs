// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Duende.IdentityModel.Jwk;

namespace Duende.IdentityModel.OidcClient;

public class CommonResponseTests
{
    readonly OidcClientOptions _options = new OidcClientOptions
    {
        ProviderInformation = new ProviderInformation
        {
            IssuerName = "https://authority",
            AuthorizeEndpoint = "https://authority/authorize",
            TokenEndpoint = "https://authority/token",
            KeySet = new JsonWebKeySet()
        }
    };

    [Fact]
    public async Task Missing_code_should_be_rejected()
    {
        var client = new Duende.IdentityModel.OidcClient.OidcClient(_options);
        var state = await client.PrepareLoginAsync();

        var url = $"?state={state.State}&id_token=foo";
        var result = await client.ProcessResponseAsync(url, state);

        result.IsError.ShouldBeTrue();
        result.Error.ShouldBe("Missing authorization code.");
    }

    [Fact]
    public async Task Missing_state_should_be_rejected()
    {
        var client = new Duende.IdentityModel.OidcClient.OidcClient(_options);
        var state = await client.PrepareLoginAsync();

        var url = $"?code=foo&id_token=foo";
        var result = await client.ProcessResponseAsync(url, state);

        result.IsError.ShouldBeTrue();
        result.Error.ShouldBe("Missing state.");
    }

    [Fact]
    public async Task Invalid_state_should_be_rejected()
    {
        var client = new Duende.IdentityModel.OidcClient.OidcClient(_options);
        var state = await client.PrepareLoginAsync();

        var url = $"?state=invalid&id_token=foo&code=bar";
        var result = await client.ProcessResponseAsync(url, state);

        result.IsError.ShouldBeTrue();
        result.Error.ShouldBe("Invalid state.");
    }
}
