// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Net.Http.Headers;
using Duende.IdentityModel.Internal;
using static Duende.IdentityModel.JwtClaimTypes;

namespace Duende.IdentityModel.Client;

/// <summary>
/// HttpClient extensions for OAuth token introspection
/// </summary>
public static class HttpClientTokenIntrospectionExtensions
{
    /// <summary>
    /// Sends an OAuth token introspection request.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task<TokenIntrospectionResponse> IntrospectTokenAsync(
        this HttpMessageInvoker client,
        TokenIntrospectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        clone.Method = HttpMethod.Post;
        clone.Parameters.AddRequired(OidcConstants.TokenIntrospectionRequest.Token, request.Token);
        clone.Parameters.AddOptional(OidcConstants.TokenIntrospectionRequest.TokenTypeHint, request.TokenTypeHint);

        if (request.ResponseFormat is ResponseFormat.Jwt)
        {
            clone.Headers.Accept.Clear();
            clone.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JwtTypes.AsMediaType(JwtTypes.IntrospectionJwtResponse)));
        }

        clone.Prepare();

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<TokenIntrospectionResponse>(ex);
        }

        Action<TokenIntrospectionResponse>? onResponseCreated = null;
        var skipJson = false;

        // Note that HttpResponse.Content can be null in .NET framework, even though it cannot be null in modern .NET
        if (response.Content?.Headers?.ContentType is { MediaType: var mediaType } &&
            string.Equals(mediaType, JwtTypes.AsMediaType(JwtTypes.IntrospectionJwtResponse), StringComparison.OrdinalIgnoreCase))
        {
            skipJson = true;
            onResponseCreated = introspectionResponse => introspectionResponse.JwtResponseValidator = request.JwtResponseValidator;
        }

        return await ProtocolResponse
            .FromHttpResponseAsync(
                httpResponse: response,
                skipJsonParsing: skipJson,
                onResponseCreated: onResponseCreated
            )
            .ConfigureAwait(false);
    }
}
