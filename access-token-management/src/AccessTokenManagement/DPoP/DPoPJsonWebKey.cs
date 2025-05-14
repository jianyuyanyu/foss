// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Duende.AccessTokenManagement.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Duende.AccessTokenManagement.DPoP;

[TypeConverter(typeof(StringValueConverter<DPoPJsonWebKey>))]
[JsonConverter(typeof(StringValueJsonConverter<DPoPJsonWebKey>))]
public readonly record struct DPoPJsonWebKey : IStonglyTypedString<DPoPJsonWebKey>
{
    public bool Equals(DPoPJsonWebKey other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator DPoPJsonWebKey(string value) => Parse(value);

    public override string ToString() => Value;

    private static readonly ValidationRule<string>[] Validators = [
        // Officially, there's no max length for JWTs, but 32k is a good limit
        ValidationRules.MaxLength(32 * 1024),
        IsValidJsonWebKey()
    ];

    private static ValidationRule<string> IsValidJsonWebKey() =>
        (string value, out string message) =>
        {
            message = string.Empty;
            try
            {
                JsonWebKey.Create(value);
                return true;
            }
            catch (Exception e)
            {
                message = "String is not a valid json webkey: " + e.Message;
                return false;
            }
        };

    public DPoPJsonWebKey() => throw new InvalidOperationException("Can't create null value");
    private DPoPJsonWebKey(string value)
    {
        Value = value;
        JsonWebKey = new JsonWebKey(value);
    }

    private string Value { get; }

    public JsonWebKey JsonWebKey { get; }


    public static bool TryParse(string value, [NotNullWhen(true)] out DPoPJsonWebKey? parsed, out string[] errors) =>
        IStonglyTypedString<DPoPJsonWebKey>.TryBuildValidatedObject(value, Validators, out parsed, out errors);


    static DPoPJsonWebKey IStonglyTypedString<DPoPJsonWebKey>.Create(string result) => new(result);

    public static DPoPJsonWebKey Parse(string value) => StringParsers<DPoPJsonWebKey>.Parse(value);
    public static DPoPJsonWebKey? ParseOrDefault(string? value) => StringParsers<DPoPJsonWebKey>.ParseOrDefault(value);

}
