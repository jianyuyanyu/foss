// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Duende.RazorSlices.Tests;

public class RazorSliceWriteTests
{
    [Fact]
    public async Task WriteReadOnlyMemoryByte_WritesHtmlEncodedUtf8Text()
    {
        var value = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("Héllo <Wörld> 日本語"));
        var slice = new WriteReadOnlyMemoryByteSlice(value);

        var result = await slice.RenderAsync(HtmlEncoder.Create(UnicodeRanges.All));

        Assert.Equal("Héllo &lt;Wörld&gt; 日本語", result);
    }

    private sealed class WriteReadOnlyMemoryByteSlice(ReadOnlyMemory<byte> value) : RazorSlice
    {
        public override Task ExecuteAsync()
        {
            Write(value);
            return Task.CompletedTask;
        }
    }
}
