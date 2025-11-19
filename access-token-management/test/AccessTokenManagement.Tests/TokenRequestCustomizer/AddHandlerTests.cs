// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Duende.AccessTokenManagement.TokenRequestCustomizer;

public class AddHandlerTests
{
    [Fact]
    public void AddClientCredentialsTokenHandler_throws_when_factory_is_null()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("test");

        Func<IServiceProvider, ITokenRequestCustomizer> nullFactory = null!;

        Should.Throw<ArgumentNullException>(() =>
                builder.AddClientCredentialsTokenHandler(
                    nullFactory,
                    ClientCredentialsClientName.Parse("test")))
            .ParamName.ShouldBe("tokenRequestCustomizerFactory");
    }

    [Fact]
    public void AddUserAccessTokenHandler_throws_when_factory_is_null()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("test");

        Func<IServiceProvider, ITokenRequestCustomizer> nullFactory = null!;

        Should.Throw<ArgumentNullException>(() =>
                builder.AddUserAccessTokenHandler(
                    nullFactory))
            .ParamName.ShouldBe("tokenRequestCustomizerFactory");
    }

    [Fact]
    public void AddClientAccessTokenHandler_throws_when_factory_is_null()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("test");

        Func<IServiceProvider, ITokenRequestCustomizer> nullFactory = null!;

        Should.Throw<ArgumentNullException>(() =>
                builder.AddClientAccessTokenHandler(
                    nullFactory))
            .ParamName.ShouldBe("tokenRequestCustomizerFactory");
    }
}
