// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Duende.RazorSlices.Tests;

public class RazorSliceHtmlAttributesTests
{
    [Theory]
    [InlineData(true, " checked=\"checked\"")]
    [InlineData(false, "")]
    public async Task ObjectTypedBoolean_RendersConditionalAttribute(bool value, string expected)
    {
        var slice = new ObjectAttributeSlice("checked", value);

        Assert.Equal(expected, await slice.RenderAsync());
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", " class=\"\"")]
    [InlineData("active", " class=\"active\"")]
    public async Task ObjectTypedString_RendersConditionalAttribute(string? value, string expected)
    {
        var slice = new ObjectAttributeSlice("class", value);

        Assert.Equal(expected, await slice.RenderAsync());
    }

    private sealed class ObjectAttributeSlice(string name, object? value) : RazorSlice
    {
        public override Task ExecuteAsync()
        {
            BeginWriteAttribute(name, $" {name}=\"", 0, "\"", 0, 1);
            WriteAttributeValue("", 0, value, 0, 0, false);
            EndWriteAttribute();
            return Task.CompletedTask;
        }
    }
}
