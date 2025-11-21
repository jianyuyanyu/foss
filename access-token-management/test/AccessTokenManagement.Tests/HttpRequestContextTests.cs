// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace Duende.AccessTokenManagement;

public class HttpRequestContextTests
{
    [Fact]
    public void HttpRequestMessageContext_can_be_mapped_from_HttpRequestMessage()
    {
        var uri = new Uri("https://example.com/api/test?param=value");
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Headers =
            {
                { "X-Test", "value" },
                { "Accept", ["application/json", "text/plain"] }
            }
        };
        var context = request.ToHttpRequestContext();

        context.Method.ShouldBe("POST");
        context.RequestUri.ShouldBe(uri);

        var xTestHeader = context.Headers.FirstOrDefault(h => h.Key == "X-Test");
        xTestHeader.Key.ShouldBe("X-Test");
        xTestHeader.Value.ShouldContain("value");

        var acceptHeader = context.Headers.FirstOrDefault(h => h.Key == "Accept");
        acceptHeader.Key.ShouldBe("Accept");
        acceptHeader.Value.ShouldContain("application/json");
        acceptHeader.Value.ShouldContain("text/plain");
    }

    [Fact]
    public void HttpRequestContext_can_be_mapped_from_HttpRequest()
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Method = "POST",
                Scheme = "https",
                Host = new HostString("example.com"),
                Path = "/api/test",
                QueryString = new QueryString("?param=value"),
                Headers =
                {
                    ["X-Test"] = "value",
                    ["Accept"] = new[] { "application/json", "text/plain" }
                }
            }
        };

        var context = httpContext.Request.ToHttpRequestContext();

        context.Method.ShouldBe("POST");
        context.RequestUri.ShouldBe(new Uri("https://example.com/api/test?param=value"));

        var xTestHeader = context.Headers.FirstOrDefault(h => h.Key == "X-Test");
        xTestHeader.Key.ShouldBe("X-Test");
        xTestHeader.Value.ShouldContain("value");

        var acceptHeader = context.Headers.FirstOrDefault(h => h.Key == "Accept");
        acceptHeader.Key.ShouldBe("Accept");
        acceptHeader.Value.ShouldContain("application/json");
        acceptHeader.Value.ShouldContain("text/plain");
    }
}
