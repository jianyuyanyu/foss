// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.RazorSlices;
using Models = RazorSlices.Samples.WebApp.Models;

namespace RazorSlices.Samples.WebApp.Slices;

public abstract class TodoSliceBase : RazorSlice<Models.Todo>;

public abstract class GenericTodosSliceBase<TModel> : RazorSlice<TModel>;

public abstract class DeepGenericTodosSliceBase<TModel> : GenericTodosSliceBase<TModel>;
