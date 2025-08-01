// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Duende.AspNetCore.Authentication.OAuth2Introspection.Infrastructure;

internal static class StringExtensions
{
    [DebuggerStepThrough]
    public static string EnsureTrailingSlash(this string input)
    {
        if (!input.EndsWith("/"))
        {
            return input + "/";
        }

        return input;
    }

    [DebuggerStepThrough]
    public static bool IsMissing(this string value) => string.IsNullOrWhiteSpace(value);

    [DebuggerStepThrough]
    public static bool IsPresent(this string value) => !string.IsNullOrWhiteSpace(value);

    internal static string Sha256(this string input)
    {
        if (input.IsMissing())
        {
            return string.Empty;
        }

        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
