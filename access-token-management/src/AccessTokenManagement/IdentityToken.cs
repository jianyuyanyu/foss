// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

public readonly record struct IdentityToken : IStronglyTypedValue<IdentityToken>
{
    public const int MaxLength = 32 * 1024;
    public override string ToString() => Value;

    private static readonly ValidationRule<string>[] Validators = [
        // Officially, there's no max length for JWTs, but 32k is a good limit
        ValidationRules.MaxLength(MaxLength)
    ];

    public IdentityToken() => throw new InvalidOperationException("Can't create null value");

    private IdentityToken(string value) => Value = value;

    private string Value { get; }

    public static bool TryParse(string value, [NotNullWhen(true)] out IdentityToken? parsed, out string[] errors) =>
        IStronglyTypedValue<IdentityToken>.TryBuildValidatedObject(value, Validators, out parsed, out errors);

    public static implicit operator IdentityToken(string value) => Parse(value);
    public static implicit operator string(IdentityToken value) => value.ToString();

    static IdentityToken IStronglyTypedValue<IdentityToken>.Create(string result) => new(result);

    public static IdentityToken Parse(string value) => StringParsers<IdentityToken>.Parse(value);

    public static IdentityToken? ParseOrDefault(string? value) => StringParsers<IdentityToken>.ParseOrDefault(value);
}
