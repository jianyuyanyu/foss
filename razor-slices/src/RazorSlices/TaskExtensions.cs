// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

internal static class TaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandleSynchronousCompletion(this Task task)
    {
        if (task.IsCompletedSuccessfully)
        {
            task.GetAwaiter().GetResult();
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandleSynchronousCompletion(this in ValueTask valueTask)
    {
        if (valueTask.IsCompletedSuccessfully)
        {
            // Signal consumption to the IValueTaskSource
            valueTask.GetAwaiter().GetResult();
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task GetAsTask(this in ValueTask<FlushResult> valueTask)
    {
        // Try to avoid the allocation from AsTask
        if (valueTask.IsCompletedSuccessfully)
        {
            // Signal consumption to the IValueTaskSource
            var _ = valueTask.GetAwaiter().GetResult();
            return Task.CompletedTask;
        }
        else
        {
            return valueTask.AsTask();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask GetAsValueTask(this in ValueTask<FlushResult> valueTask)
    {
        // Try to avoid the allocation from AsTask
        if (valueTask.IsCompletedSuccessfully)
        {
            // Signal consumption to the IValueTaskSource
            var _ = valueTask.GetAwaiter().GetResult();
            return default;
        }
        else
        {
            return new ValueTask(valueTask.AsTask());
        }
    }
}
