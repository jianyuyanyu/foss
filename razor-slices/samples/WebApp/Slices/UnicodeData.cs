// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace RazorSlices.Samples.WebApp.Slices;

/// <summary>
/// Static Unicode test data for the Unicode slice.
/// Kept in a .cs file to work around a .NET 10 Razor compiler bug where supplementary Unicode
/// characters (e.g., emoji, variation selectors) in .cshtml files cause @functions blocks to be
/// incorrectly placed inside ExecuteAsync instead of as class members.
/// BUG: https://github.com/dotnet/razor/issues/12777
/// </summary>
internal static class UnicodeData
{
    public static readonly string[] Emojis = new[] { "😁", "💩", "🐻", "🐳", "❤️", "🌶️", "😶‍🌫️", "👾", "🫨" };
    public static readonly byte[][] EmojisUtf8 = new[] { "😁"u8.ToArray(), "💩"u8.ToArray(), "🐻"u8.ToArray(), "🐳"u8.ToArray(), "❤️"u8.ToArray(), "🌶️"u8.ToArray(), "😶‍🌫️"u8.ToArray(), "👾"u8.ToArray(), "🫨"u8.ToArray() };
    public static ReadOnlySpan<char> Kanji1 => "西葛西駅";
    public static ReadOnlySpan<char> Kanji2 => "葛\U000E0100城市";
    public static ReadOnlySpan<byte> Kanji1Utf8 => "西葛西駅"u8;
    public static ReadOnlySpan<byte> Kanji2Utf8 => "葛\U000E0100城市"u8;
}
