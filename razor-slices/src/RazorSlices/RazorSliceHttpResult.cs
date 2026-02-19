// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.RazorSlices;

namespace Microsoft.AspNetCore.Http.HttpResults;

/// <summary>
/// A <see cref="RazorSlice" /> template that is also an <see cref="IResult" /> so it can be directly returned from
/// a route handler delegate. When executed it will render the template to the response.
/// </summary>
[Obsolete("This class is no longer needed. RazorSlice itself implements IResult.")]
public abstract class RazorSliceHttpResult : RazorSlice
{

}
