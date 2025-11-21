// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Duende.AccessTokenManagement.OpenIdConnect;

internal static class HttpRequestContextExtensions
{
    public static HttpRequestContext ToHttpRequestContext(this HttpRequest request) =>
        new()
        {
            Method = request.Method,
            RequestUri = new Uri(request.GetEncodedUrl()),
            Headers = request.Headers.Select(h => new KeyValuePair<string, IEnumerable<string>>(h.Key, h.Value))
        };
}
