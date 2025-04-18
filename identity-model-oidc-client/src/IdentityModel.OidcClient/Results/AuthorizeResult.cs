// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.IdentityModel.OidcClient.Results;

internal class AuthorizeResult : Result
{
    public virtual string Data { get; set; }
    public virtual AuthorizeState State { get; set; }
}
