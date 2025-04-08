// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityModel.Internal;

namespace Duende.IdentityModel.Client;

/// <summary>
/// Represents a discovery endpoint URL parsed into its authority and discovery endpoint components.
/// This is commonly used to resolve URLs for OpenID Connect or OAuth2 discovery documents.
/// </summary>
public class DiscoveryEndpoint
{
    /// <summary>
    /// Parses a given URL into its authority and discovery endpoint components.
    /// </summary>
    /// <param name="input">The full URL of the discovery endpoint to parse.</param>
    /// <param name="path">
    /// An optional custom path to the discovery document. 
    /// Defaults to <c>.well-known/openid-configuration</c> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="DiscoveryEndpoint"/> object containing the parsed authority and discovery endpoint URL.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if the <paramref name="input"/> parameter is <c>null</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the <paramref name="input"/> parameter is a malformed URL or uses an invalid scheme
    /// (neither HTTP nor HTTPS).
    /// </exception>
    public static DiscoveryEndpoint ParseUrl(string input, string? path = null)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        if (string.IsNullOrEmpty(path))
        {
            path = OidcConstants.Discovery.DiscoveryEndpoint;
        }

        var success = Uri.TryCreate(input, UriKind.Absolute, out var uri);
        if (success == false)
        {
            throw new InvalidOperationException("Malformed URL");
        }

        if (!DiscoveryEndpoint.IsValidScheme(uri!))
        {
            throw new InvalidOperationException("Malformed URL");
        }

        var url = input.RemoveTrailingSlash();
        if (path!.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        if (url.EndsWith(path, StringComparison.OrdinalIgnoreCase))
        {
            return new DiscoveryEndpoint(url.Substring(0, url.Length - path.Length - 1), url);
        }
        else
        {
            return new DiscoveryEndpoint(url, url.EnsureTrailingSlash() + path);
        }
    }

    /// <summary>
    /// Determines if the given URI uses a valid scheme for discovery endpoints.
    /// </summary>
    /// <param name="url">The URI to validate.</param>
    /// <returns>
    /// <c>true</c> if the URI scheme is either "http" or "https"; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidScheme(Uri url)
    {
        if (string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the specified URL uses a secure scheme based on the provided discovery policy.
    /// </summary>
    /// <param name="url">The URL to evaluate.</param>
    /// <param name="policy">The discovery policy that defines the security requirements.</param>
    /// <returns>
    /// <c>true</c> if the URL uses a secure scheme according to the policy; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSecureScheme(Uri url, DiscoveryPolicy policy)
    {
        if (policy.RequireHttps == true)
        {
            if (policy.AllowHttpOnLoopback == true)
            {
                var hostName = url.DnsSafeHost;

                foreach (var address in policy.LoopbackAddresses)
                {
                    if (string.Equals(hostName, address, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveryEndpoint"/> class.
    /// </summary>
    /// <param name="authority">The base authority of the discovery endpoint (e.g., https://example.com).</param>
    /// <param name="url">The full URL of the discovery document (e.g.,
    /// https://example.com/.well-known/openid-configuration).</param>
    public DiscoveryEndpoint(string authority, string url)
    {
        Authority = authority;
        Url = url;
    }

    /// <summary>
    /// Gets the base authority of the discovery endpoint.
    /// </summary>
    /// <value>
    /// A string representing the authority portion of the URL (e.g., https://example.com).
    /// </value>
    public string Authority { get; }

    /// <summary>
    /// Gets the full URL to the discovery document.
    /// </summary>
    /// <value>
    /// A string representing the complete discovery endpoint URL, including the path to the discovery document (e.g.,
    /// https://example.com/.well-known/openid-configuration).
    /// </value>
    public string Url { get; }

}
