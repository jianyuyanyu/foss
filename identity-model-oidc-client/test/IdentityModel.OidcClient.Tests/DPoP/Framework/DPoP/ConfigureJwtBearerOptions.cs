// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Duende.IdentityModel.OidcClient.DPoP.Framework.DPoP;

public class ConfigureJwtBearerOptions(string configScheme)
    : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string name, JwtBearerOptions options)
    {
        if (configScheme != name)
        {
            return;
        }
        if (options.EventsType == null)
        {
            options.EventsType = typeof(DPoPJwtBearerEvents);
        }
        else if (!typeof(DPoPJwtBearerEvents).IsAssignableFrom(options.EventsType))
        {
            throw new Exception(
                "EventsType on JwtBearerOptions must derive from DPoPJwtBearerEvents to work with the DPoP support.");
        }
    }
}
