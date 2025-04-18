// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using System.Text.Json;

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
        if (!IsError)
        {
            if (Json == null)
            {
                throw new InvalidOperationException("Json is null"); // TODO better exception
            }
            var issuer = Json?.TryGetString("iss");
            var claims = Json?.ToClaims(issuer, "scope").ToList() ?? new List<Claim>();

            // due to a bug in identityserver - we need to be able to deal with the scope list both in array as well as space-separated list format
            var scope = Json?.TryGetValue("scope");

            // scope element exists
            // if (scope != null)
            // {
            // it's an array
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
            // }

            Claims = claims;
        }

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

}
