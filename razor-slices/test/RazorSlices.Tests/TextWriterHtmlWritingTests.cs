// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Encodings.Web;

namespace Duende.RazorSlices.Tests;

public class TextWriterHtmlWritingTests
{
    [Fact]
    public async Task RenderAsync_TextWriter_WritesUtf8LiteralsAsText()
    {
        // The Razor compiler in .NET SDK 10.0.400+ emits WriteLiteral(ReadOnlySpan<byte>) with "..."u8 literals,
        // so static markup takes this path regardless of which SDK compiled the template.
        var slice = new Utf8LiteralSlice();
        using var writer = new StringWriter();

        await slice.RenderAsync(writer);

        writer.ToString().ShouldBe("<p>hello</p><p>héllo wörld ✓</p>");
    }

    [Fact]
    public async Task RenderAsync_PipeWriter_WritesUtf8LiteralsAsText()
    {
        var slice = new Utf8LiteralSlice();
        using var stream = new MemoryStream();
        var pipeWriter = System.IO.Pipelines.PipeWriter.Create(stream);

        await slice.RenderAsync(pipeWriter);
        await pipeWriter.FlushAsync();

        System.Text.Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("<p>hello</p><p>héllo wörld ✓</p>");
    }

    [Fact]
    public void WriteUtf8_DecodesAsUtf8()
    {
        using var writer = new StringWriter();

        writer.WriteUtf8("<p>héllo wörld ✓</p>"u8);

        writer.ToString().ShouldBe("<p>héllo wörld ✓</p>");
    }

    [Fact]
    public void HtmlEncodeAndWriteUtf8_DecodesAsUtf8AndEncodesHtml()
    {
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteUtf8("<p>héllo</p>"u8, HtmlEncoder.Default);

        writer.ToString().ShouldBe("&lt;p&gt;h&#xE9;llo&lt;/p&gt;");
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_WritesOnlyTheFormattedChars()
    {
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(SampleStatus.Open, HtmlEncoder.Default);

        writer.ToString().ShouldBe("Open");
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_DoesNotLeakPreviousRenterContent()
    {
        // Dirty the shared pool with a long value first, then write a short one. Without slicing to the
        // encoded length the short write carries the tail of the long one into the output.
        using var warmup = new StringWriter();
        warmup.HtmlEncodeAndWriteSpanFormattable(1234567890123456789L, HtmlEncoder.Default);

        using var writer = new StringWriter();
        writer.HtmlEncodeAndWriteSpanFormattable(SampleStatus.Posted, HtmlEncoder.Default);

        writer.ToString().ShouldBe("Posted");
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_EncodesHtmlOnSmallPath()
    {
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(new HtmlishFormattable(), HtmlEncoder.Default);

        writer.ToString().ShouldBe("a&lt;b&gt;c");
    }

    private sealed class Utf8LiteralSlice : RazorSlice
    {
        public override Task ExecuteAsync()
        {
            WriteLiteral("<p>hello</p>"u8);
            WriteLiteral("<p>héllo wörld ✓</p>"u8);
            return Task.CompletedTask;
        }
    }

    private enum SampleStatus
    {
        Open = 0,
        Posted = 1
    }

    private readonly struct HtmlishFormattable : ISpanFormattable
    {
        private const string Value = "a<b>c";

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
