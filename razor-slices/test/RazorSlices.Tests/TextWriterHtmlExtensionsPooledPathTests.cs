// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Encodings.Web;

namespace Duende.RazorSlices.Tests;

// The upstream HtmlEncodeAndWriteSpanFormattable tests all use values shorter than
// BufferSizes.SmallFormattableWriteCharSize, so they take the stackalloc fast path and never rent from
// ArrayPool<char>. These tests use values longer than that threshold to exercise the pooled path and its
// final flush, which must write only the encoded chars rather than the whole rented buffer.
public class TextWriterHtmlExtensionsPooledPathTests
{
    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_WritesOnlyTheFormattedChars()
    {
        var value = new FixedStringFormattable(new string('a', BufferSizes.SmallFormattableWriteCharSize + 8));
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(value, HtmlEncoder.Default);

        writer.ToString().ShouldBe(value.Value);
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_DoesNotLeakPreviousRenterContent()
    {
        // Dirty the shared pool with a long value whose tail is distinctive, then write a shorter value that
        // still exceeds the small-path threshold so it rents (and likely receives) the same pooled array.
        var longValue = new FixedStringFormattable(new string('X', 200));
        var shortValue = new FixedStringFormattable(new string('y', BufferSizes.SmallFormattableWriteCharSize + 8));

        using var warmup = new StringWriter();
        warmup.HtmlEncodeAndWriteSpanFormattable(longValue, HtmlEncoder.Default);

        using var writer = new StringWriter();
        writer.HtmlEncodeAndWriteSpanFormattable(shortValue, HtmlEncoder.Default);

        writer.ToString().ShouldBe(shortValue.Value);
        writer.ToString().ShouldNotContain("X");
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_EncodesHtml()
    {
        var value = new FixedStringFormattable("<" + new string('b', BufferSizes.SmallFormattableWriteCharSize + 8) + ">");
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(value, HtmlEncoder.Default);

        writer.ToString().ShouldBe("&lt;" + new string('b', BufferSizes.SmallFormattableWriteCharSize + 8) + "&gt;");
    }

    private readonly struct FixedStringFormattable(string value) : ISpanFormattable
    {
        public string Value { get; } = value;

        public string ToString(string? format, IFormatProvider? formatProvider) => Value;

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            if (destination.Length < Value.Length)
            {
                charsWritten = 0;
                return false;
            }

            Value.AsSpan().CopyTo(destination);
            charsWritten = Value.Length;
            return true;
        }
    }
}
