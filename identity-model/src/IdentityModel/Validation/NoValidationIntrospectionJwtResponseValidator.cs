// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityModel.Client;

namespace Duende.IdentityModel.Validation;

/// <summary>
/// A no-op implementation of <see cref="ITokenIntrospectionJwtResponseValidator"/>.
/// Does NOT validate the introspection response.
/// </summary>
public class NoValidationIntrospectionJwtResponseValidator : ITokenIntrospectionJwtResponseValidator
{
    public void Validate(TokenIntrospectionResponse response)
    {
        // This validator intentionally does nothing.
    }
}
