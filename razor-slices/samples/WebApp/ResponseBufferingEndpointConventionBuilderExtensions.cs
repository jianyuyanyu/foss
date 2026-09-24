// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace RazorSlices.Samples.WebApp;

public static class ResponseBufferingEndpointConventionBuilderExtensions
{
    public static TBuilder DisableResponseBuffering<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.WithMetadata(new DisableResponseBufferingAttribute());
    }
}
