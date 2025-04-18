// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using Duende.IdentityModel.Client;

namespace Duende.IdentityModel.OidcClient;

internal class ResponseValidationResult : Result
{
    public ResponseValidationResult()
    {

    }

    public ResponseValidationResult(string error) => Error = error;

    public virtual AuthorizeResponse AuthorizeResponse { get; set; }
    public virtual TokenResponse TokenResponse { get; set; }
    public virtual ClaimsPrincipal User { get; set; }
}
