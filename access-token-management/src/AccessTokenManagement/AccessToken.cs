// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Strongly typed representation of an access token. This can be a Client Credentials or User token.
/// </summary>
[JsonConverter(typeof(StringValueJsonConverter<AccessToken>))]
public readonly record struct AccessToken : IStronglyTypedValue<AccessToken>
{
    public override string ToString() => Value;

    // Officially, there's no max length for JWTs, but 32k is a good limit
    public const int MaxLength = 32 * 1024; // 32k

    private static readonly ValidationRule<string>[] Validators = [
        ValidationRules.MaxLength(MaxLength)
    ];

    public static implicit operator AccessToken(string value) => Parse(value);
    public static implicit operator string(AccessToken value) => value.ToString();

    public AccessToken() => throw new InvalidOperationException("Can't create null value");
    private AccessToken(string value) => Value = value;

    private string Value { get; }

    public static bool TryParse(string value, [NotNullWhen(true)] out AccessToken? parsed, out string[] errors) =>
        IStronglyTypedValue<AccessToken>.TryBuildValidatedObject(value, Validators, out parsed, out errors);


    static AccessToken IStronglyTypedValue<AccessToken>.Create(string result) => new(result);

    public static AccessToken Parse(string value) => StringParsers<AccessToken>.Parse(value);
}
