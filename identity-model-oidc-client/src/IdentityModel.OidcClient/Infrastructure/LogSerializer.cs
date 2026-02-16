// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Duende.IdentityModel.OidcClient.Infrastructure;

/// <summary>
/// Helper to JSON serialize object data for logging.
/// </summary>
public static class LogSerializer
{
    /// <summary>
    /// Allows log serialization to be disabled, for example, for platforms
    /// that don't support serialization of arbitrary objects to JSON.
    /// </summary>
    public static bool Enabled = true;

    static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    [UnconditionalSuppressMessage("AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Code using `JsonOptions` is guarded by `RequiresDynamicCodeAttribute`")]
    static LogSerializer() => JsonOptions.Converters.Add(new JsonStringEnumConverter());

    /// <summary>
    /// Serializes the specified object.
    /// </summary>
    /// <param name="logObject">The object.</param>
    /// <returns></returns>
    [RequiresUnreferencedCode("The log serializer uses reflection in a way that is incompatible with trimming")]
    [RequiresDynamicCode("The log serializer uses reflection in a way that is incompatible with trimming")]
    public static string Serialize(object logObject) => Enabled ? JsonSerializer.Serialize(logObject, JsonOptions) : "Logging has been disabled";

    internal static string Serialize(OidcClientOptions opts) => Serialize<OidcClientOptions>(opts);
    internal static string Serialize(AuthorizeState state) => Serialize<AuthorizeState>(state);

    /// <summary>
    /// Serializes the specified object.
    /// </summary>
    /// <param name="logObject">The object.</param>
    /// <returns></returns>
    private static string Serialize<T>(T logObject) =>
        Enabled ?
            JsonSerializer.Serialize(logObject, (JsonTypeInfo<T>)SourceGenerationContext.Default.GetTypeInfo(typeof(T))) :
            "Logging has been disabled";
}
