// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.IdentityModel.Validation;

public interface ITokenIntrospectionJwtResponseValidator
{
    /// <summary>
    /// Perform additional validation on the introspection response.
    /// </summary>
    /// <param name="rawJwtResponse">The raw token introspection response.</param>
    void Validate(string rawJwtResponse);
}
