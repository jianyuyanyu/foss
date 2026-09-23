// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO.Pipelines;
using System.Text;

namespace Duende.RazorSlices.Tests;

public class RazorSliceUtf8LiteralTests
{
    private const string ExpectedMarkup = "<p>hello</p><p>héllo wörld ✓</p>";

    [Fact]
    public async Task RenderAsync_TextWriter_WritesUtf8LiteralsAsText()
    {
        // The Razor compiler in .NET SDK 10.0.400+ emits WriteLiteral(ReadOnlySpan<byte>) with "..."u8 literals,
        // so static markup takes this path regardless of which SDK compiled the template.
        var slice = new Utf8LiteralSlice();
        using var writer = new StringWriter();

        await slice.RenderAsync(writer);

        writer.ToString().ShouldBe(ExpectedMarkup);
    }

    [Fact]
    public async Task RenderAsync_PipeWriter_WritesUtf8LiteralsAsText()
    {
        var slice = new Utf8LiteralSlice();
        using var stream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(stream);

        await slice.RenderAsync(pipeWriter);
        await pipeWriter.FlushAsync();

        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe(ExpectedMarkup);
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
}
