// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using RazorSlices.Samples.WebApp.Slices.Lorem;
using System.Text;

namespace RazorSlices.Samples.WebApp.Services;

public class LoremService
{
    public string Sentences(int length)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(PageContent.Sentence);
        }
        return sb.ToString();
    }
}
