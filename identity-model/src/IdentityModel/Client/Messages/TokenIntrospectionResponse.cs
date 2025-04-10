// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityModel.Validation;
using static Duende.IdentityModel.JwtClaimTypes;

namespace Duende.IdentityModel.Client;

/// <summary>
/// Models an OAuth 2.0 introspection response
/// </summary>
/// <seealso cref="ProtocolResponse" />
public class TokenIntrospectionResponse : ProtocolResponse
{
    /// <summary>
    /// Allows to initialize instance specific data.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    /// <returns></returns>
    protected override Task InitializeAsync(object? initializationData = null)
    {
        if (IsError)
        {
            return Task.CompletedTask;
        }

        // Note that HttpResponse.Content can be null in .NET framework, even though it cannot be null in modern .NET
        if (HttpResponse?.Content?.Headers?.ContentType is { MediaType: var mediaType } &&
            string.Equals(mediaType, JwtTypes.AsMediaType(JwtTypes.IntrospectionJwtResponse), StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(Raw))
        {
            Json = ExtractJsonFromJwt(Raw!);
            JwtResponseValidator?.Validate(Raw!);
        }

        if (Json == null)
        {
            throw new InvalidOperationException("Json is null"); // TODO better exception
        }

        var issuer = Json?.TryGetString("iss");
        var claims = Json?.ToClaims(issuer, "scope").ToList() ?? new List<Claim>();

        // due to a bug in identityserver - we need to be able to deal with the scope list both in array as well as space-separated list format
        var scope = Json?.TryGetValue("scope");

        if (scope?.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in scope?.EnumerateArray() ?? Enumerable.Empty<JsonElement>())
            {
                claims.Add(new Claim("scope", item.ToString(), ClaimValueTypes.String, issuer));
            }
        }
        else
        {
            // it's a string
            var scopeString = scope.ToString() ?? "";

            var scopes = scopeString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var scopeValue in scopes)
            {
                claims.Add(new Claim("scope", scopeValue, ClaimValueTypes.String, issuer));
            }
        }

        Claims = claims;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a value indicating whether the token is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the token is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive => Json?.TryGetBoolean("active") ?? false;

    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <value>
    /// The claims.
    /// </value>
    public IEnumerable<Claim> Claims { get; protected set; } = Enumerable.Empty<Claim>();

    /// <summary>
    /// Gets the custom validator instance for validating a JWT introspection response.
    /// If set, this validator will be invoked to perform any additional or custom validation on the JWT response (for example, verifying its signature, expiration, or other claims).
    /// If left null, no JWT validation is performed, although the claims will still be extracted and the raw JWT string will be accessible.
    /// It is the caller's responsibility to provide an implementation of <see cref="ITokenIntrospectionJwtResponseValidator"/> if JWT validation is desired.
    /// </summary>
    public ITokenIntrospectionJwtResponseValidator? JwtResponseValidator { get; set; }

    /// <summary>
    /// Extracts the JSON from the JWT token.
    /// </summary>
    /// <param name="rawToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static JsonElement ExtractJsonFromJwt(string rawToken)
    {
        // Split the token into parts.
        var parts = rawToken.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("Invalid JWT format");
        }

        // Decode and parse the payload.
        var payload = parts[1];
        var jsonString = Base64Url.Decode(payload);
        using var document = JsonDocument.Parse(jsonString);

        // Look for the "token_introspection" property.
        if (document.RootElement.TryGetProperty("token_introspection", out var introspectionElement))
        {
            return introspectionElement.Clone();
        }
        else
        {
            throw new InvalidOperationException("token_introspection claim not found in JWT payload");
        }
    }
}
