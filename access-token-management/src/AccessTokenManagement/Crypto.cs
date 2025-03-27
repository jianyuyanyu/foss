// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography;
using System.Text;
using Duende.IdentityModel;

namespace Duende.AccessTokenManagement;

internal class Crypto
{
    public static string HashData(string data)
    {
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));

            var leftPart = new byte[16];
            Array.Copy(hash, leftPart, 16);

            return Base64Url.Encode(leftPart);
        }
    }
}
