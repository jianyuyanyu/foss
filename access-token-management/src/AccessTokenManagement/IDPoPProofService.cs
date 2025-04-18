// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement;

/// <summary>
/// Service to create DPoP proof tokens
/// </summary>
public interface IDPoPProofService
{
    /// <summary>
    /// Creates DPoP proof token
    /// </summary>
    Task<DPoPProof?> CreateProofTokenAsync(DPoPProofRequest request);

    /// <summary>
    /// Gets the thumbprint from the JSON web key
    /// </summary>
    string? GetProofKeyThumbprint(DPoPProofRequest request);
}

/// <summary>
/// Models the request information to create a DPoP proof token
/// </summary>
public class DPoPProofRequest
{
    /// <summary>
    /// The HTTP URL of the request
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// The HTTP method of the request
    /// </summary>
    public required string Method { get; set; }

    /// <summary>
    /// The string representation of the JSON web key to use for DPoP.
    /// </summary>
    public required string DPoPJsonWebKey { get; set; }

    /// <summary>
    /// The nonce value for the DPoP proof token.
    /// </summary>
    public string? DPoPNonce { get; set; }

    /// <summary>
    /// The access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Additional claims to add to the DPoP proof payload
    /// </summary>
    public IReadOnlyDictionary<string, string>? AdditionalPayloadClaims { get; set; }
}

/// <summary>
/// Models a DPoP proof token
/// </summary>
public class DPoPProof
{
    /// <summary>
    /// The proof token
    /// </summary>
    public string ProofToken { get; set; } = default!;
}
