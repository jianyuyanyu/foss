// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

/// <summary>
/// The name of the token client, that's used to configure the openid connect
/// configuration in the client application. This may be, but doesn't have to be,
/// the same value as the <see cref="ClientId"/> which is used to identify the client in
/// the token endpoint.
/// </summary>
[TypeConverter(typeof(StringValueConverter<TokenClientName>))]
public readonly record struct TokenClientName : IStronglyTypedValue<TokenClientName>
{
    public static implicit operator TokenClientName(string value) => Parse(value);
    public static implicit operator string(TokenClientName value) => value.ToString();

    public override string ToString() => Value;

    private static readonly ValidationRule<string>[] Validators = [
        ValidationRules.MaxLength(1024)
    ];

    public TokenClientName() => throw new InvalidOperationException("Can't create null value");
    private TokenClientName(string value) => Value = value;

    private string Value { get; }

    public static bool TryParse(string value, [NotNullWhen(true)] out TokenClientName? parsed, out string[] errors) =>
        IStronglyTypedValue<TokenClientName>.TryBuildValidatedObject(value, Validators, out parsed, out errors);

    public static TokenClientName Parse(string value) => StringParsers<TokenClientName>.Parse(value);

    static TokenClientName IStronglyTypedValue<TokenClientName>.Create(string result) => new(result);
}
