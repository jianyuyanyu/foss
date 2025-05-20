// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

public readonly record struct RefreshToken : IStronglyTypedValue<RefreshToken>
{
    public const int MaxLength = 4 * 1024;
    public override string ToString() => Value;

    private static readonly ValidationRule<string>[] Validators = [
        // Officially, there's no max length refresh tokens, but 4k is a good limit
        ValidationRules.MaxLength(MaxLength)
    ];
    public RefreshToken() => throw new InvalidOperationException("Can't create null value");

    private RefreshToken(string value) => Value = value;
    private string Value { get; }

    public static implicit operator RefreshToken(string value) => Parse(value);
    public static implicit operator string(RefreshToken value) => value.ToString();

    public static bool TryParse(string value, [NotNullWhen(true)] out RefreshToken? parsed, out string[] errors) =>
        IStronglyTypedValue<RefreshToken>.TryBuildValidatedObject(value, Validators, out parsed, out errors);

    static RefreshToken IStronglyTypedValue<RefreshToken>.Create(string result) => new(result);
    public static RefreshToken Parse(string value) => StringParsers<RefreshToken>.Parse(value);
}
