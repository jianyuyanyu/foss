// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;

namespace Duende.AspNetCore.Authentication.OAuth2Introspection.Infrastructure;

internal class ClaimSerializer : IHybridCacheSerializer<List<Claim>>
{
    public List<Claim> Deserialize(ReadOnlySequence<byte> source)
    {
        var reader = new Utf8JsonReader(source);

        var claimsLight = JsonSerializer.Deserialize(ref reader, DuendeIntrospectionSerializationContext.Default.ListClaimLite)!;

        return claimsLight.Select(claimLite => new Claim(claimLite.Type, claimLite.Value)).ToList();
    }

    public void Serialize(List<Claim> value, IBufferWriter<byte> target)
    {
        using var writer = new Utf8JsonWriter(target);
        var claimsLite = value.Select(claim => new ClaimLite { Type = claim.Type, Value = claim.Value }).ToList();

        JsonSerializer.Serialize(writer, claimsLite, DuendeIntrospectionSerializationContext.Default.ListClaimLite);
    }
}
