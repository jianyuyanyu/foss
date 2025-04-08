// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json;

namespace Duende.IdentityModel.Client;

/// <summary>
/// Represents aliases for mutual TLS (mTLS) endpoints in an OpenID Connect discovery document.
/// Provides access to specific mTLS-based endpoints such as token, revocation, and device authorization endpoints.
/// </summary>
public class MtlsEndpointAliases
{
    /// <summary>
    /// The raw <see cref="JsonElement"/> that contains the mTLS endpoint aliases.
    /// </summary>
    public JsonElement? Json { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MtlsEndpointAliases"/> class.
    /// </summary>
    /// <param name="json">The raw <see cref="JsonElement"/> that contains the mTLS endpoint aliases.</param>
    public MtlsEndpointAliases(JsonElement? json) => Json = json;

    /// <summary>
    /// Gets the token endpoint address.
    /// </summary>
    public string? TokenEndpoint =>
        Json?.TryGetString(OidcConstants.Discovery.TokenEndpoint);

    /// <summary>
    /// Gets the revocation endpoint address.
    /// </summary>
    public string? RevocationEndpoint =>
        Json?.TryGetString(OidcConstants.Discovery.RevocationEndpoint);

    /// <summary>
    /// Gets the device authorization endpoint address.
    /// </summary>
    public string? DeviceAuthorizationEndpoint =>
        Json?.TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);

    /// <summary>
    /// Gets the introspection endpoint address.
    /// </summary>
    public string? IntrospectionEndpoint =>
        Json?.TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);


    /// <summary>
    /// Gets the pushed authorization endpoint address.
    /// </summary>
    public string? PushedAuthorizationRequestEndpoint =>
        Json?.TryGetString(OidcConstants.Discovery.PushedAuthorizationRequestEndpoint);

}
