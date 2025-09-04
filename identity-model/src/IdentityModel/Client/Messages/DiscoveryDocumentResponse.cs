// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json;
using Duende.IdentityModel.Internal;
using Duende.IdentityModel.Jwk;

namespace Duende.IdentityModel.Client;

/// <summary>
/// Represents the response from an OpenID Connect discovery endpoint.
/// </summary>
public class DiscoveryDocumentResponse : ProtocolResponse
{
    /// <summary>
    /// Gets or sets the discovery policy used to configure how the discovery document is processed.
    /// </summary>
    /// <value>
    /// An instance of <see cref="DiscoveryPolicy"/> that represents the processing configuration.
    /// </value>
    public DiscoveryPolicy Policy { get; set; } = default!;

    /// <summary>
    /// Initializes the discovery document response using the provided data.
    /// </summary>
    /// <param name="initializationData">The data used to initialize the response, typically a DiscoveryPolicy object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override Task InitializeAsync(object? initializationData = null)
    {
        if (HttpResponse?.IsSuccessStatusCode != true)
        {
            ErrorMessage = initializationData as string;
            return Task.CompletedTask;
        }

        Policy = initializationData as DiscoveryPolicy ?? new DiscoveryPolicy();

        var validationError = Validate(Policy);

        if (validationError.IsPresent())
        {
            Json = default;

            ErrorType = ResponseErrorType.PolicyViolation;
            ErrorMessage = validationError;
        }

        MtlsEndpointAliases =
            new MtlsEndpointAliases(Json?.TryGetValue(OidcConstants.Discovery.MtlsEndpointAliases));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets or sets the JSON Web Key Set (JWKS) associated with the discovery document.
    /// </summary>
    /// <value>
    /// An instance of <see cref="JsonWebKeySet"/> that contains the public keys used for validating signatures and encryption.
    /// </value>
    public JsonWebKeySet? KeySet { get; set; }

    /// <summary>
    /// Gets the mutual TLS (mTLS) endpoint aliases.
    /// </summary>
    /// <value>
    /// An instance of <see cref="MtlsEndpointAliases"/> that contains the mTLS endpoint aliases.
    /// </value>
    public MtlsEndpointAliases? MtlsEndpointAliases { get; internal set; }

    // strongly typed
    /// <summary>
    /// Gets the issuer identifier for the authorization server.
    /// </summary>
    /// <value>
    /// The issuer URL as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? Issuer => TryGetString(OidcConstants.Discovery.Issuer);

    /// <summary>
    /// Gets the authorization endpoint URL.
    /// </summary>
    /// <value>
    /// The authorization endpoint URL as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? AuthorizeEndpoint => TryGetString(OidcConstants.Discovery.AuthorizationEndpoint);

    /// <summary>
    /// Gets token endpoint URL.
    /// </summary>
    /// <value>
    /// The token endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? TokenEndpoint => TryGetString(OidcConstants.Discovery.TokenEndpoint);

    /// <summary>
    /// Gets user info endpoint URL.
    /// </summary>
    /// <value>
    /// The user info endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? UserInfoEndpoint => TryGetString(OidcConstants.Discovery.UserInfoEndpoint);

    /// <summary>
    /// Gets the introspection endpoint URL.
    /// </summary>
    /// <value>
    /// The introspection endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? IntrospectionEndpoint => TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);

    /// <summary>
    /// Gets the revocation endpoint URL.
    /// </summary>
    /// <value>
    /// The revocation endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? RevocationEndpoint => TryGetString(OidcConstants.Discovery.RevocationEndpoint);

    /// <summary>
    /// Gets the device authorization endpoint URL.
    /// </summary>
    /// <value>
    /// The device authorization endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? DeviceAuthorizationEndpoint => TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);

    /// <summary>
    /// Gets the backchannel authentication endpoint URL.
    /// </summary>
    /// <value>
    /// The backchannel authentication endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? BackchannelAuthenticationEndpoint => TryGetString(OidcConstants.Discovery.BackchannelAuthenticationEndpoint);

    /// <summary>
    /// Gets the URI of the JSON Web Key Set (JWKS).
    /// </summary>
    /// <value>
    /// The JWKS URI as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? JwksUri => TryGetString(OidcConstants.Discovery.JwksUri);

    /// <summary>
    /// Gets the end session endpoint URL.
    /// </summary>
    /// <value>
    /// The end session endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? EndSessionEndpoint => TryGetString(OidcConstants.Discovery.EndSessionEndpoint);

    /// <summary>
    /// Gets the check session iframe URL.
    /// </summary>
    /// <value>
    /// The check session iframe as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? CheckSessionIframe => TryGetString(OidcConstants.Discovery.CheckSessionIframe);

    /// <summary>
    /// Gets the dynamic client registration (DCR) endpoint URL.
    /// </summary>
    /// <value>
    /// The DCR endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? RegistrationEndpoint => TryGetString(OidcConstants.Discovery.RegistrationEndpoint);

    /// <summary>
    /// Gets the pushed authorization request (PAR) endpoint URL.
    /// </summary>
    /// <value>
    /// The PAR endpoint as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? PushedAuthorizationRequestEndpoint => TryGetString(OidcConstants.Discovery.PushedAuthorizationRequestEndpoint);

    /// <summary>
    /// Gets a flag indicating whether front-channel logout is supported.
    /// </summary>
    /// <value>
    /// <c>true</c> if front-channel logout is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? FrontChannelLogoutSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSupported);

    /// <summary>
    /// Gets a flag indicating whether a session ID (sid) parameter is supported at the front-channel logout endpoint.
    /// </summary>
    /// <value>
    /// <c>true</c> if the sid parameter is supported at the front-channel logout endpoint, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? FrontChannelLogoutSessionSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSessionSupported);

    /// <summary>
    /// Gets the supported grant types.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported grant types.
    /// </value>
    public IEnumerable<string> GrantTypesSupported => TryGetStringArray(OidcConstants.Discovery.GrantTypesSupported);

    /// <summary>
    /// Gets the supported code challenge methods.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported code challenge methods, such as "S256".
    /// </value>
    public IEnumerable<string> CodeChallengeMethodsSupported => TryGetStringArray(OidcConstants.Discovery.CodeChallengeMethodsSupported);

    /// <summary>
    /// Gets the supported scopes.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported scopes.
    /// </value>
    public IEnumerable<string> ScopesSupported => TryGetStringArray(OidcConstants.Discovery.ScopesSupported);

    /// <summary>
    /// Gets the supported subject types.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported subject types, e.g., "public" and "pairwise".
    /// </value>
    public IEnumerable<string> SubjectTypesSupported => TryGetStringArray(OidcConstants.Discovery.SubjectTypesSupported);

    /// <summary>
    /// Gets the supported response modes.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported response modes.
    /// </value>
    public IEnumerable<string> ResponseModesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseModesSupported);

    /// <summary>
    /// Gets the supported response types.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported response types.
    /// </value>
    public IEnumerable<string> ResponseTypesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseTypesSupported);

    /// <summary>
    /// Gets the supported claims.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported claims.
    /// </value>
    public IEnumerable<string> ClaimsSupported => TryGetStringArray(OidcConstants.Discovery.ClaimsSupported);

    /// <summary>
    /// Gets the authentication methods supported by the token endpoint.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported authentication methods.
    /// </value>
    public IEnumerable<string> TokenEndpointAuthenticationMethodsSupported => TryGetStringArray(OidcConstants.Discovery.TokenEndpointAuthenticationMethodsSupported);

    /// <summary>
    /// Gets the signing algorithms supported by the token endpoint for the signature on the JWT used to authenticate
    /// the client at the token endpoint for the "private_key_jwt" and "client_secret_jwt" authentication methods.
    /// </summary>
    public IEnumerable<string> TokenEndpointAuthenticationSigningAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.TokenEndpointAuthSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the supported backchannel token delivery modes.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported backchannel token delivery modes.
    /// </value>
    public IEnumerable<string> BackchannelTokenDeliveryModesSupported => TryGetStringArray(OidcConstants.Discovery.BackchannelTokenDeliveryModesSupported);

    /// <summary>
    /// Gets a flag indicating whether the backchannel user code parameter is supported.
    /// </summary>
    /// <value>
    /// <c>true</c> if the backchannel user code parameter is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? BackchannelUserCodeParameterSupported => TryGetBoolean(OidcConstants.Discovery.BackchannelUserCodeParameterSupported);

    /// <summary>
    /// Gets a flag indicating whether the use of pushed authorization requests (PAR) is required.
    /// </summary>
    /// <value>
    /// <c>true</c> if the PAR is required, <c>false</c> if it is explicitly not required, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? RequirePushedAuthorizationRequests => TryGetBoolean(OidcConstants.Discovery.RequirePushedAuthorizationRequests);

    /// <summary>
    /// Gets the signing algorithms supported for introspection responses.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported signing algorithms for token introspection.
    /// </value>
    public IEnumerable<string> IntrospectionSigningAlgorithmsSupported =>
        TryGetStringArray(OidcConstants.Discovery.IntrospectionSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the encryption "alg" values supported for encrypted JWT introspection responses.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "alg" values.
    /// </value>
    public IEnumerable<string> IntrospectionEncryptionAlgorithmsSupported =>
        TryGetStringArray(OidcConstants.Discovery.IntrospectionEncryptionAlgorithmsSupported);

    /// <summary>
    /// Gets the encryption "enc" values supported for encrypted JWT introspection responses.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "enc" values.
    /// </value>
    public IEnumerable<string> IntrospectionEncryptionEncValuesSupported =>
        TryGetStringArray(OidcConstants.Discovery.IntrospectionEncryptionEncValuesSupported);

    /// <summary>
    /// Gets the service documentation URL.
    /// </summary>
    /// <value>
    /// The service documentation URL as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? ServiceDocumentation => TryGetString(OidcConstants.Discovery.ServiceDocumentation);

    /// <summary>
    /// Gets the languages and scripts supported for the user interface, represented as BCP47 language tags.
    /// </summary>
    /// <value>
    /// A collection of language tag values from BCP47 representing the supported languages and scripts for the user interface.
    /// </value>
    public IEnumerable<string> UILocalesSupported => TryGetStringArray(OidcConstants.Discovery.UILocalesSupported);

    /// <summary>
    /// Gets the URL that the authorization server provides to the person registering the client to read about the authorization server's
    /// requirements on how the client can use the data provided by the authorization server.
    /// </summary>
    /// <value>
    /// The op policy URL as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? OpPolicyUri => TryGetString(OidcConstants.Discovery.OpPolicyUri);

    /// <summary>
    /// Gets the URL that the authorization server provides to the person registering the client to read about the
    /// authorization server's terms of service.
    /// </summary>
    /// <value>
    /// The op terms of service URL as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? OpTosUri => TryGetString(OidcConstants.Discovery.OpTosUri);

    /// <summary>
    /// Gets the authentication methods supported by the revocation endpoint.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported authentication methods.
    /// </value>
    public IEnumerable<string> RevocationEndpointAuthenticationMethodsSupported =>
        TryGetStringArray(OidcConstants.Discovery.RevocationEndpointAuthenticationMethodsSupported);

    /// <summary>
    /// Gets the signing algorithms supported by the revocation endpoint for the signature on the JWT used to authenticate
    /// the client at the token endpoint for the "private_key_jwt" and "client_secret_jwt" authentication methods.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported signing algorithms.
    /// </value>
    public IEnumerable<string> RevocationEndpointAuthenticationSigningAlgorithmsSupported =>
        TryGetStringArray(OidcConstants.Discovery.RevocationEndpointAuthSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the authentication methods supported by the introspection endpoint.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported authentication methods.
    /// </value>
    public IEnumerable<string> IntrospectionEndpointAuthenticationMethodsSupported =>
        TryGetStringArray(OidcConstants.Discovery.IntrospectionEndpointAuthenticationMethodsSupported);

    /// <summary>
    /// Gets the signing algorithms supported by the introspection endpoint for the signature on the JWT used to authenticate
    /// the client at the token endpoint for the "private_key_jwt" and "client_secret_jwt" authentication methods.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported signing algorithms.
    /// </value>
    public IEnumerable<string> IntrospectionEndpointAuthenticationSigningAlgorithmsSupported =>
        TryGetStringArray(OidcConstants.Discovery.IntrospectionEndpointAuthSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the signed JWT containing the metadata about the authorization server as claims.
    /// </summary>
    /// <value>
    /// The signed metadata as a string, or <c>null</c> if it is not found in the discovery document.
    /// </value>
    public string? SignedMetadata => TryGetString(OidcConstants.Discovery.SignedMetadata);

    /// <summary>
    /// Gets a flag indicating whether the authorization server supports TLS client certificate bound access tokens.
    /// </summary>
    /// <value>
    /// <c>true</c> if TLS client certificate bound access tokens are supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? TlsClientCertificateBoundAccessTokens => TryGetBoolean(OidcConstants.Discovery.TlsClientCertificateBoundAccessTokens);

    /// <summary>
    /// Gets the Authentication Context Class Reference (ACR) values supported by the OP.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported ACR values.
    /// </value>
    public IEnumerable<string> AcrValuesSupported => TryGetStringArray(OidcConstants.Discovery.AcrValuesSupported);

    /// <summary>
    /// Gets the JWS "alg" values supported by the OP for the ID token.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported signing algorithms for the ID token.
    /// </value>
    public IEnumerable<string> IdTokenSigningAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.IdTokenSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "alg" values supported by the OP for the ID token.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "alg" values for the ID token.
    /// </value>
    public IEnumerable<string> IdTokenEncryptionAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.IdTokenEncryptionAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "enc" values supported by the OP for the ID token.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "enc" values for the ID token.
    /// </value>
    public IEnumerable<string> IdTokenEncryptionEncValuesSupported => TryGetStringArray(OidcConstants.Discovery.IdTokenEncryptionEncValuesSupported);

    /// <summary>
    /// Gets the JWS "alg" values supported by the UserInfo endpoint.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported signing algorithms for the UserInfo endpoint.
    /// </value>
    public IEnumerable<string> UserInfoSigningAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.UserInfoSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "alg" values supported by the UserInfo endpoint.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "alg" values for the UserInfo endpoint.
    /// </value>
    public IEnumerable<string> UserInfoEncryptionAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.UserInfoEncryptionAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "enc" values supported by the UserInfo endpoint.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "enc" values for the UserInfo endpoint.
    /// </value>
    public IEnumerable<string> UserInfoEncryptionEncValuesSupported => TryGetStringArray(OidcConstants.Discovery.UserInfoEncryptionEncValuesSupported);

    /// <summary>
    /// Gets the JWS "alg" values supported by the OP for request objects.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported signing algorithms for request objects.
    /// </value>
    public IEnumerable<string> RequestObjectSigningAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.RequestObjectSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "alg" values supported by the OP for request objects.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "alg" values for request objects.
    /// </value>
    public IEnumerable<string> RequestObjectEncryptionAlgorithmsSupported => TryGetStringArray(OidcConstants.Discovery.RequestObjectEncryptionAlgorithmsSupported);

    /// <summary>
    /// Gets the JWE "enc" values supported by the OP for request objects.
    /// </summary>
    /// <value>
    /// A collection of algorithm identifier strings representing the supported encryption "enc" values for request objects.
    /// </value>
    public IEnumerable<string> RequestObjectEncryptionEncValuesSupported => TryGetStringArray(OidcConstants.Discovery.RequestObjectEncryptionEncValuesSupported);

    /// <summary>
    /// Gets the display parameter values the OpenID Provider supports.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported display parameter values.
    /// </value>
    public IEnumerable<string> DisplayValuesSupported => TryGetStringArray(OidcConstants.Discovery.DisplayValuesSupported);

    /// <summary>
    /// Gets the claim types supported by the OpenID Provider.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported claim types.
    /// </value>
    public IEnumerable<string> ClaimTypesSupported => TryGetStringArray(OidcConstants.Discovery.ClaimTypesSupported);

    /// <summary>
    /// Get the languages and scripts supported for claims, represented as BCP47 language tags.
    /// </summary>
    /// <value>
    /// A collection of language tag values from BCP47 representing the supported languages and scripts for claims.
    /// </value>
    public IEnumerable<string> ClaimsLocalesSupported => TryGetStringArray(OidcConstants.Discovery.ClaimsLocalesSupported);

    /// <summary>
    /// Gets a flag indicating whether the OP supports the use of the "claims" parameter.
    /// </summary>
    /// <value>
    /// <c>true</c> if the claims parameter is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? ClaimsParameterSupported => TryGetBoolean(OidcConstants.Discovery.ClaimsParameterSupported);

    /// <summary>
    /// Gets a flag indicating whether the OP supports the use of the "request" parameter.
    /// </summary>
    /// <value>
    /// <c>true</c> if the request parameter is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? RequestParameterSupported => TryGetBoolean(OidcConstants.Discovery.RequestParameterSupported);

    /// <summary>
    /// Gets a flag indicating whether the OP supports the use of the "request_uri" parameter.
    /// </summary>
    /// <value>
    /// <c>true</c> if the request_uri parameter is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? RequestUriParameterSupported => TryGetBoolean(OidcConstants.Discovery.RequestUriParameterSupported);

    /// <summary>
    /// Gets a flag indicating whether the OP requires any request_uri values used to be pre-registered.
    /// </summary>
    /// <value>
    /// <c>true</c> if request_uri values must be pre-registered, <c>false</c> if it is explicitly not required, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? RequireRequestUriRegistration => TryGetBoolean(OidcConstants.Discovery.RequireRequestUriRegistration);

    /// <summary>
    /// Gets a flag indicating whether the authorization server requires authorization requests to be protected as a Request
    /// Object provided through either the "request" or "request_uri" parameters.
    /// </summary>
    /// <value>
    /// <c>true</c> if a signed request object is required, <c>false</c> if it is explicitly not required, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? RequireSignedRequestObject => TryGetBoolean(OidcConstants.Discovery.RequireSignedRequestObject);

    /// <summary>
    /// Gets a flag indicating whether the authorization server provides the "iss" parameter in the authorization response.
    /// </summary>
    /// <value>
    /// <c>true</c> if the "iss" parameter is provided in the authorization response, <c>false</c> if it is explicitly not provided, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? AuthorizationResponseIssParameterSupported => TryGetBoolean(OidcConstants.Discovery.AuthorizationResponseIssParameterSupported);

    /// <summary>
    /// Gets a flag indicating if the OP supports back-channel logout.
    /// </summary>
    /// <value>
    /// <c>true</c> if back-channel logout is supported, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? BackChannelLogoutSupported => TryGetBoolean(OidcConstants.Discovery.BackChannelLogoutSupported);

    /// <summary>
    /// Gets a flag indicating if the OP supports passing a "sid" (Session ID) claim in the logout token to identify the RP
    /// session with the OP.
    /// </summary>
    /// <value>
    /// <c>true</c> if the "sid" claim is supported in the logout token, <c>false</c> if it is explicitly not supported, or
    /// <c>null</c> if it is not found in the discovery document.
    /// </value>
    public bool? BackChannelLogoutSessionSupported => TryGetBoolean(OidcConstants.Discovery.BackChannelLogoutSessionSupported);

    /// <summary>
    /// Gets the JWS "alg" values supported for validation of signed CIBA authentication requests.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported signing algorithms.
    /// </value>
    public IEnumerable<string> BackchannelAuthenticationRequestSigningAlgValuesSupported =>
        TryGetStringArray(OidcConstants.Discovery.BackchannelAuthenticationRequestSigningAlgValuesSupported);

    /// <summary>
    /// Gets the authorization details types supported by the authorization server.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported authorization details types.
    /// </value>
    public IEnumerable<string> AuthorizationDetailsTypesSupported =>
        TryGetStringArray(OidcConstants.Discovery.AuthorizationDetailsTypesSupported);

    /// <summary>
    /// Gets the JWS "alg" values supported for DPoP proof JWTs.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported signing algorithms for DPoP.
    /// </value>
    public IEnumerable<string> DPoPSigningAlgorithmsSupported =>
        TryGetStringArray(OidcConstants.Discovery.DPoPSigningAlgorithmsSupported);

    /// <summary>
    /// Gets the prompt values supported by the OP.
    /// </summary>
    /// <value>
    /// A collection of strings representing the supported prompt values.
    /// </value>
    public IEnumerable<string> PromptValuesSupported =>
        TryGetStringArray(OidcConstants.Discovery.PromptValuesSupported);

    /// <summary>
    /// Attempts to retrieve a JSON value for a given property name from the discovery document.
    /// </summary>
    /// <param name="name">The name of the property whose value is to be retrieved.</param>
    /// <returns>A <see cref="JsonElement"/> containing the value if found, and <c>null</c> otherwise.</returns>
    public JsonElement? TryGetValue(string name) => Json?.TryGetValue(name);

    /// <summary>
    /// Attempts to retrieve a string value for a given property name from the discovery document.
    /// </summary>
    /// <param name="name">The name of the property whose value is to be retrieved.</param>
    /// <returns>The string value if found, and <c>null</c> otherwise.</returns>
    public string? TryGetString(string name) => Json?.TryGetString(name);

    /// <summary>
    /// Attempts to retrieve a boolean value for a given property name from the discovery document.
    /// </summary>
    /// <param name="name">The name of the property whose value is to be retrieved.</param>
    /// <returns>The boolean value if found, and <c>null</c> otherwise.</returns>
    public bool? TryGetBoolean(string name) => Json?.TryGetBoolean(name);

    /// <summary>
    /// Attempts to retrieve a string array for a given property name from the discovery document.
    /// </summary>
    /// <param name="name">The name of the property whose value is to be retrieved.</param>
    /// <returns>The collection of strings if found, and an empty collection otherwise.</returns>
    public IEnumerable<string> TryGetStringArray(string name) => Json?.TryGetStringArray(name) ?? [];

    private string Validate(DiscoveryPolicy policy)
    {
        if (policy.ValidateIssuerName)
        {
            var strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;

            var issuerValidationResult = strategy.IsIssuerNameValid(Issuer!, policy.Authority);

            if (!issuerValidationResult.Success)
            {
                return issuerValidationResult.ErrorMessage;
            }
        }

        var error = ValidateEndpoints(Json, policy);
        if (error.IsPresent())
        {
            return error;
        }

        return string.Empty;
    }

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <returns></returns>
    public bool ValidateIssuerName(string issuer, string authority) =>
        DiscoveryPolicy.DefaultAuthorityValidationStrategy.IsIssuerNameValid(issuer, authority).Success;

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <param name="nameComparison">The comparison mechanism that should be used when performing the match.</param>
    /// <returns></returns>
    public bool ValidateIssuerName(string issuer, string authority, StringComparison nameComparison) =>
        new StringComparisonAuthorityValidationStrategy(nameComparison).IsIssuerNameValid(issuer, authority).Success;

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <param name="validationStrategy">The strategy to use.</param>
    /// <returns></returns>
    private bool ValidateIssuerName(string issuer, string authority, IAuthorityValidationStrategy validationStrategy) =>
        validationStrategy.IsIssuerNameValid(issuer, authority).Success;

    /// <summary>
    /// Validates the endoints and jwks_uri according to the security policy.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="policy">The policy.</param>
    /// <returns></returns>
    public string ValidateEndpoints(JsonElement? json, DiscoveryPolicy policy)
    {
        if (json == null)
        {
            throw new ArgumentNullException(nameof(json));
        }

        // allowed hosts
        var allowedHosts = new HashSet<string>(policy.AdditionalEndpointBaseAddresses.Select(e => new Uri(e).Authority))
        {
            new Uri(policy.Authority).Authority
        };

        // allowed authorities (hosts + base address)
        var allowedAuthorities = new HashSet<string>(policy.AdditionalEndpointBaseAddresses)
        {
            policy.Authority
        };

        // Can't actually be null, because we check that and throw at the beginning of the method
        foreach (var element in json?.EnumerateObject()!)
        {
            if (element.Name.EndsWith("endpoint", StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.JwksUri, StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.CheckSessionIframe, StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = element.Value.ToString();

                var isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out var uri);
                if (!isValidUri)
                {
                    return $"Malformed endpoint: {endpoint}";
                }

                if (!DiscoveryEndpoint.IsValidScheme(uri!))
                {
                    return $"Malformed endpoint: {endpoint}";
                }

                if (!DiscoveryEndpoint.IsSecureScheme(uri!, policy))
                {
                    return $"Endpoint does not use HTTPS: {endpoint}";
                }

                if (policy.ValidateEndpoints)
                {
                    // if endpoint is on exclude list, don't validate
                    if (policy.EndpointValidationExcludeList.Contains(element.Name))
                    {
                        continue;
                    }

                    var isAllowed = false;
                    foreach (var host in allowedHosts)
                    {
                        if (string.Equals(host, uri!.Authority))
                        {
                            isAllowed = true;
                        }
                    }

                    if (!isAllowed)
                    {
                        return $"Endpoint is on a different host than authority: {endpoint}";
                    }

                    var strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;
                    var endpointValidationResult = strategy.IsEndpointValid(endpoint, allowedAuthorities);
                    if (!endpointValidationResult.Success)
                    {
                        return endpointValidationResult.ErrorMessage;
                    }
                }
            }
        }

        if (policy.RequireKeySet)
        {
            if (string.IsNullOrWhiteSpace(JwksUri))
            {
                return "Keyset is missing";
            }
        }

        return string.Empty;
    }
}
