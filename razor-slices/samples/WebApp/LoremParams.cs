// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace RazorSlices.Samples.WebApp;
public struct LoremParams
{
    public int ParagraphCount;
    public int ParagraphSentenceCount;

    public LoremParams(int? paragraphCount, int? paragraphSentenceCount)
    {
        ParagraphCount = paragraphCount ?? 3;
        ParagraphSentenceCount = paragraphSentenceCount ?? 5;
    }
}
