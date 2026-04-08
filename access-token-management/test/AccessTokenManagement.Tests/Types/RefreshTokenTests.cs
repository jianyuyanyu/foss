// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement.Types;

/// <summary>
/// Both the <see cref="RefreshTokenTests"/> and <see cref="RefreshTokenSetMaxLengthTests"/> classes share a collection to prevent parallel execution,
/// since <see cref="RefreshToken.SetMaxLength"/> mutates static state.
/// </summary>
[Collection(nameof(RefreshTokenTests))]
public class RefreshTokenTests
{
    [Fact]
    public void Parse_ValidValue_ReturnsRefreshToken()
    {
        // Arrange
        var validValue = "valid_refresh_token";

        // Act
        var result = RefreshToken.Parse(validValue);

        // Assert
        result.ToString().ShouldBe(validValue);
    }

    [Fact]
    public void Parse_InvalidValue_ThrowsException()
    {
        // Arrange
        var invalidValue = new string('a', RefreshToken.MaxLength + 1); // Exceeds max length

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => RefreshToken.Parse(invalidValue));
        exception.Message.ShouldContain("exceeds maximum length");
    }

    [Fact]
    public void TryParse_ValidValue_ReturnsTrueAndRefreshToken()
    {
        // Arrange
        var validValue = "valid_refresh_token";

        // Act
        var success = RefreshToken.TryParse(validValue, out var result, out var errors);

        // Assert
        success.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.ToString().ShouldBe(validValue);
        errors.ShouldBeEmpty();
    }

    [Fact]
    public void TryParse_InvalidValue_ReturnsFalseAndErrors()
    {
        // Arrange
        var invalidValue = new string('a', RefreshToken.MaxLength + 1); // Exceeds max length

        // Act
        var success = RefreshToken.TryParse(invalidValue, out var result, out var errors);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBeNull();
        errors.ShouldNotBeEmpty();
        errors[0].ShouldContain("exceeds maximum length");
    }

    [Fact]
    public void Parse_AtExactMaxLength_ReturnsRefreshToken()
    {
        // Arrange
        var value = new string('a', RefreshToken.MaxLength);

        // Act
        var result = RefreshToken.Parse(value);

        // Assert
        result.ToString().ShouldBe(value);
    }

    [Fact]
    public void Parse_NullValue_ThrowsException() =>
        // Act & Assert
        Should.Throw<InvalidOperationException>(() => RefreshToken.Parse(null!));

    [Fact]
    public void Parse_EmptyValue_ThrowsException() =>
        // Act & Assert
        Should.Throw<InvalidOperationException>(() => RefreshToken.Parse(string.Empty));

    [Fact]
    public void Parse_WhitespaceValue_ThrowsException() =>
        // Act & Assert
        Should.Throw<InvalidOperationException>(() => RefreshToken.Parse("   "));

    [Fact]
    public void ParameterlessConstructor_ThrowsException()
    {
        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => new RefreshToken());
        exception.Message.ShouldContain("Can't create null value");
    }

    [Fact]
    public void ImplicitStringConversion_ReturnsValue()
    {
        // Arrange
        var value = "my_refresh_token";
        var token = RefreshToken.Parse(value);

        // Act
        string converted = token;

        // Assert
        converted.ShouldBe(value);
    }
}
