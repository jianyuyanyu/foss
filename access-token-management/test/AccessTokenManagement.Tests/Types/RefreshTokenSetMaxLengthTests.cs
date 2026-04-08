// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.AccessTokenManagement.Types;

/// <summary>
/// Tests for <see cref="RefreshToken.SetMaxLength"/>. These are isolated into a separate
/// non-parallel collection because <see cref="RefreshToken.SetMaxLength"/> mutates static state
/// that would affect other tests running concurrently.
/// </summary>
[Collection(nameof(RefreshTokenTests))]
public class RefreshTokenSetMaxLengthTests : IDisposable
{
    public void Dispose() =>
        // Reset to the default max length after each test so static state doesn't leak.
        RefreshToken.SetMaxLength(RefreshToken.MaxLength);

    [Fact]
    public void SetMaxLength_AllowsLargerTokens()
    {
        // Arrange
        var largeMaxLength = 16 * 1024; // 16 KB, e.g. for ADFS tokens
        var largeValue = new string('a', RefreshToken.MaxLength + 1); // Exceeds default, within new limit
        RefreshToken.SetMaxLength(largeMaxLength);

        // Act
        var result = RefreshToken.Parse(largeValue);

        // Assert
        result.ToString().ShouldBe(largeValue);
    }

    [Fact]
    public void SetMaxLength_StillRejectsTokensExceedingNewLimit()
    {
        // Arrange
        var newMaxLength = 8 * 1024;
        RefreshToken.SetMaxLength(newMaxLength);
        var tooLargeValue = new string('a', newMaxLength + 1);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => RefreshToken.Parse(tooLargeValue));
        exception.Message.ShouldContain("exceeds maximum length");
    }

    [Fact]
    public void SetMaxLength_TryParse_AllowsLargerTokens()
    {
        // Arrange
        var largeMaxLength = 16 * 1024;
        var largeValue = new string('a', RefreshToken.MaxLength + 1);
        RefreshToken.SetMaxLength(largeMaxLength);

        // Act
        var success = RefreshToken.TryParse(largeValue, out var result, out var errors);

        // Assert
        success.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.ToString().ShouldBe(largeValue);
        errors.ShouldBeEmpty();
    }

    [Fact]
    public void SetMaxLength_TryParse_StillRejectsTokensExceedingNewLimit()
    {
        // Arrange
        var newMaxLength = 8 * 1024;
        RefreshToken.SetMaxLength(newMaxLength);
        var tooLargeValue = new string('a', newMaxLength + 1);

        // Act
        var success = RefreshToken.TryParse(tooLargeValue, out var result, out var errors);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBeNull();
        errors.ShouldNotBeEmpty();
        errors[0].ShouldContain("exceeds maximum length");
    }

    [Fact]
    public void SetMaxLength_CanReduceLimit()
    {
        // Arrange
        var smallerMaxLength = 100;
        RefreshToken.SetMaxLength(smallerMaxLength);
        var value = new string('a', smallerMaxLength + 1);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => RefreshToken.Parse(value));
        exception.Message.ShouldContain("exceeds maximum length");
    }

    [Fact]
    public void SetMaxLength_AtExactNewLimit_Succeeds()
    {
        // Arrange
        var newMaxLength = 8 * 1024;
        RefreshToken.SetMaxLength(newMaxLength);
        var value = new string('a', newMaxLength);

        // Act
        var result = RefreshToken.Parse(value);

        // Assert
        result.ToString().ShouldBe(value);
    }

    [Fact]
    public void SetMaxLength_Zero_ThrowsArgumentOutOfRangeException() =>
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => RefreshToken.SetMaxLength(0));

    [Fact]
    public void SetMaxLength_Negative_ThrowsArgumentOutOfRangeException() =>
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => RefreshToken.SetMaxLength(-1));
}
