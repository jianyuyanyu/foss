// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Duende.AccessTokenManagement.Internal;

namespace Duende.AccessTokenManagement;

/// <summary>
/// The type of access token. Typically maps to Bearer or DPoP. Can also often
/// be used as the scheme. 
/// </summary>
[JsonConverter(typeof(StringValueJsonConverter<AccessTokenType>))]
public readonly record struct AccessTokenType : IStronglyTypedValue<AccessTokenType>
{
    /// <summary>
    /// The maximum allowed length for the access token type value.
    /// </summary>
    public const int MaxLength = 50;

    /// <summary>
    /// Returns the string representation of the access token type.
    /// </summary>
    public override string ToString() => Value;

    private static readonly ValidationRule<string>[] Validators = [
        ValidationRules.MaxLength(MaxLength),
        ValidationRules.AlphaNumeric()
    ];

    /// <summary>
    /// Initializes a new instance of <see cref="AccessTokenType"/> with the specified value.
    /// </summary>
    /// <param name="value">The access token type value.</param>
    private AccessTokenType(string value) => Value = value;

    /// <summary>
    /// Throws an exception. You can't directly create this type without a value.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public AccessTokenType() => throw new InvalidOperationException("Can't create null value");

    /// <summary>
    /// The string value of the access token type.
    /// </summary>
    private string Value { get; }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AccessTokenType"/>.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    public static implicit operator AccessTokenType(string value) => Parse(value);

    /// <summary>
    /// Implicitly converts an <see cref="AccessTokenType"/> to a string.
    /// </summary>
    /// <param name="value">The <see cref="AccessTokenType"/> to convert.</param>
    public static implicit operator string(AccessTokenType value) => value.ToString();

    /// <summary>
    /// Attempts to parse the specified string into an <see cref="AccessTokenType"/>.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="parsed">The parsed <see cref="AccessTokenType"/>, if successful.</param>
    /// <param name="errors">Any validation errors encountered during parsing.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(string value, [NotNullWhen(true)] out AccessTokenType? parsed, out string[] errors) =>
        IStronglyTypedValue<AccessTokenType>
            .TryBuildValidatedObject(value, Validators, out parsed, out errors);

    /// <summary>
    /// Creates a new <see cref="AccessTokenType"/> from the specified string without validation.
    /// </summary>
    /// <param name="result">The string value.</param>
    static AccessTokenType IStronglyTypedValue<AccessTokenType>.Create(string result) => new(result);

    /// <summary>
    /// Parses the specified string into an <see cref="AccessTokenType"/>.
    /// Throws if the value is invalid.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed <see cref="AccessTokenType"/>.</returns>
    public static AccessTokenType Parse(string value) =>
        StringParsers<AccessTokenType>.Parse(value);

    /// <summary>
    /// Parses the specified string into an <see cref="AccessTokenType"/>, or returns null if the value is invalid.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed <see cref="AccessTokenType"/>, or null if invalid.</returns>
    public static AccessTokenType? ParseOrDefault(string? value) => StringParsers<AccessTokenType>.ParseOrDefault(value);

    /// <summary>
    /// Converts the access token type to a <see cref="Scheme"/>.
    /// </summary>
    /// <returns>The corresponding <see cref="Scheme"/>.</returns>
    public Scheme ToScheme() => Scheme.Parse(Value);

}
