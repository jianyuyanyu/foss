// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Duende.AccessTokenManagement.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Duende.AccessTokenManagement.DPoP;

[TypeConverter(typeof(StringValueConverter<DPoPProofKey>))]
[JsonConverter(typeof(StringValueJsonConverter<DPoPProofKey>))]
public readonly record struct DPoPProofKey : IStronglyTypedValue<DPoPProofKey>
{
    public bool Equals(DPoPProofKey other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator DPoPProofKey(string value) => Parse(value);
    public static implicit operator string(DPoPProofKey value) => value.ToString();

    private readonly JsonWebKey _jsonWebKey;

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
                message = "String is not a valid json web key: " + e.Message;
                return false;
            }
        };

    public DPoPProofKey() => throw new InvalidOperationException("Can't create null value");
    private DPoPProofKey(string value)
    {
        Value = value;
        _jsonWebKey = JsonWebKey.Create(value);
    }

    private string Value { get; }

    public JsonWebKey ToJsonWebKey() => _jsonWebKey;

    public static bool TryParse(string value, [NotNullWhen(true)] out DPoPProofKey? parsed, out string[] errors) =>
        IStronglyTypedValue<DPoPProofKey>.TryBuildValidatedObject(value, Validators, out parsed, out errors);


    static DPoPProofKey IStronglyTypedValue<DPoPProofKey>.Create(string result) => new(result);

    public static DPoPProofKey Parse(string value) => StringParsers<DPoPProofKey>.Parse(value);
    public static DPoPProofKey? ParseOrDefault(string? value) => StringParsers<DPoPProofKey>.ParseOrDefault(value);

}
