// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityModel.Validation;

namespace Duende.IdentityModel.Client;

/// <summary>
/// Request for OAuth token introspection
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class TokenIntrospectionRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the token.
    /// </summary>
    /// <value>
    /// The token.
    /// </value>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Gets or sets the token type hint.
    /// </summary>
    /// <value>
    /// The token type hint.
    /// </value>
    public string? TokenTypeHint { get; set; }

    /// <summary>
    /// Sets the desired format for the introspection response.
    /// </summary>
    public IntrospectionResponseFormat ResponseFormat { get; set; } = IntrospectionResponseFormat.Json;

    /// <summary>
    /// Gets or sets the custom validator instance for validating a JWT introspection response.
    /// If set, this validator will be invoked to perform any additional or custom validation on the JWT response (for example, verifying its signature, expiration, or other claims).
    /// If left null, no JWT validation is performed, although the claims will still be extracted and the raw JWT string will be accessible.
    /// It is the caller's responsibility to provide an implementation of <see cref="ITokenIntrospectionJwtResponseValidator"/> if JWT validation is desired.
    /// </summary>
    public ITokenIntrospectionJwtResponseValidator? JwtResponseValidator { get; set; }
}
