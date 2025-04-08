// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.IdentityModel.Validation;

/// <summary>
/// A no-op implementation of <see cref="ITokenIntrospectionJwtResponseValidator"/>.
/// Does NOT validate the introspection response.
/// </summary>
public class NoValidationIntrospectionJwtResponseValidator : ITokenIntrospectionJwtResponseValidator
{
    public void Validate(string rawJwtResponse)
    {
        // This validator intentionally does nothing.
    }
}
