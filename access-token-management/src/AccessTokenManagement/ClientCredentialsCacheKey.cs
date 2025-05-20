// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

/// <summary>
/// Cache key when caching client credential access tokens. 
/// </summary>
public readonly record struct ClientCredentialsCacheKey : IStronglyTypedValue<ClientCredentialsCacheKey>
{
    public override string ToString() => Value;

    public const int MaxLength = 255;

    private static readonly ValidationRule<string>[] Validators = [
        ValidationRules.MaxLength(MaxLength)
    ];

    public ClientCredentialsCacheKey() => throw new InvalidOperationException("Can't create null value");
    private ClientCredentialsCacheKey(string value) => Value = value;

    private string Value { get; }

    public static ClientCredentialsCacheKey Parse(string value) => StringParsers<ClientCredentialsCacheKey>.Parse(value);

    public static implicit operator ClientCredentialsCacheKey(string value) => Parse(value);
    public static implicit operator string(ClientCredentialsCacheKey key) => key.Value;

    public static bool TryParse(string value, [NotNullWhen(true)] out ClientCredentialsCacheKey? parsed, out string[] errors) =>
        IStronglyTypedValue<ClientCredentialsCacheKey>.TryBuildValidatedObject(value, Validators, out parsed, out errors);

    static ClientCredentialsCacheKey IStronglyTypedValue<ClientCredentialsCacheKey>.Create(string result) => new(result);
}
