// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

internal static class HttpRequestContextExtensions
{
    public static HttpRequestContext ToHttpRequestContext(this HttpRequestMessage request) =>
        new()
        {
            Method = request.Method.Method,
            RequestUri = request.RequestUri,
            Headers = request.Headers
        };
}
