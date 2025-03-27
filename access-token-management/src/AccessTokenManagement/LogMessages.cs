// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography;
using System.Text;
using Duende.IdentityModel;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement;
internal static partial class LogMessages
{
    internal class Parameters
    {
        public const string Scheme = "{scheme}";
        public const string Error = "{error}";
        public const string Url = "{url}";
        public const string ClientName = "{clientname}";
        public const string Expiration = "{expiration}";
        public const string TokenHash = "{tokenhash}";
        public const string Endpoint = "{endpoint}";
        public const string User = "{user}";
        public const string Resource = "{resource}";
        public const string Method = "{method}";
        public const string Address = "{address}";
        public const string CacheKey = "{cachekey}";
        public const string TokenType = "{tokentype}";
        public const string ForceRenewal = "{forcerenewal}";
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot authenticate scheme: {Parameters.Scheme} to acquire user access token.")]
    public static partial void CannotAuthenticateSchemeToAquireUserAccessToken(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Authentication result properties are null for scheme: {Parameters.Scheme} after authentication.")]
    public static partial void AuthenticationResultPropertiesAreNullAfterAuthenticate(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Failed to get a UserToken because no tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.")]
    public static partial void FailedToGetUserTokenDueToMissingTokensInCookie(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = $"Error revoking refresh token. Error = {Parameters.Error}.")]
    public static partial void FailedToRevokeAccessToken(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending DPoP proof token in request to endpoint: {Parameters.Url}")]
    public static partial void SendingDPoPProofToken(this ILogger logger, string? url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Failed to create DPoP proof token for request to endpoint: {Parameters.Url}")]
    public static partial void FailedToCreateDPopProofToken(this ILogger logger, string? url);


    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Access token of type {Parameters.TokenType} to endpoint: {Parameters.Url}.")]
    public static partial void SendAccessTokenToEndpoint(this ILogger logger, string? url, string? tokenType);


    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"DPoP nonce error: '{Parameters.Error}' while invoking endpoint: {Parameters.Url}. Retrying using new nonce")]
    public static partial void RequestFailedWithDPoPErrorWillRetry(this ILogger logger, string? error, string? url);

    /// <summary>
    /// Logs the refreshing of a refresh token. Note, the actual refresh token is not logged, but a hash of the token.
    /// Because hashing can be costly, we're only doing this when the log level is Trace. This is not something the sourcegenerators
    /// can do, so we're wrapping this in a method.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="refreshToken"></param>
    /// <param name="hashAlgorithm"></param>

    public static void RefreshingAccessTokenUsingRefreshToken(this ILogger logger, string refreshToken, Func<string, string> hashAlgorithm)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            RefreshingTokenUsingRefreshTokenImplementation(logger, hashAlgorithm(refreshToken));
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Refreshing access token using refresh token: hash={Parameters.TokenHash}")]
    private static partial void RefreshingTokenUsingRefreshTokenImplementation(this ILogger logger, string tokenHash);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Refresh token request to: {Parameters.Endpoint}")]
    public static partial void SendingRefreshTokenRequest(this ILogger logger, string? endpoint);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "DPoP error during token refresh. Retrying with server nonce")]
    public static partial void DPoPErrorDuringTokenRefreshWillRetryWithServerNonce(this ILogger logger);

    /// <summary>
    /// Logs the revocation of a refresh token. Note, the actual refresh token is not logged, but a hash of the token.
    /// Because hashing can be costly, we're only doing this when the log level is Trace. This is not something the sourcegenerators
    /// can do, so we're wrapping this in a method.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="refreshToken"></param>
    /// <param name="hashAlgorithm"></param>
    public static void RevokingRefreshToken(this ILogger logger, string refreshToken, Func<string, string> hashAlgorithm)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            RevokingRefreshTokenImplementation(logger, hashAlgorithm(refreshToken));
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Revoking refresh token: hash={Parameters.TokenHash}")]
    private static partial void RevokingRefreshTokenImplementation(this ILogger logger, string tokenHash);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Token revocation request to: {Parameters.Endpoint}")]
    public static partial void SendingTokenRevocationRequest(this ILogger logger, string endpoint);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Starting user token acquisition")]
    public static partial void StartingUserTokenAcquisition(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Cannot retrieve token: No active user")]
    public static partial void CannotRetrieveAccessTokenDueToNoActiveUser(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot retrieve token: No token data found in user token store for user {Parameters.User}.")]
    public static partial void CannotRetrieveAccessTokenDueToNoTokenDataFound(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot retrieve token: No refresh token found in user token store for user {Parameters.User} / resource {Parameters.Resource}. Returning current access token.")]
    public static partial void CannotRetrieveAccessTokenDueToNoRefreshTokenFound(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No access token found in user token store for user {Parameters.User} / resource {Parameters.Resource}. Trying to refresh.")]
    public static partial void NoAccessTokenFoundWillRefresh(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Token for user {Parameters.User} will be refreshed. Expiration: {Parameters.Expiration}, ForceRenewal:{Parameters.ForceRenewal}")]
    public static partial void DebugTokenNeedsRefreshing(this ILogger logger, string user, DateTimeOffset expiration, bool forceRenewal);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning refreshed token for user: {Parameters.User}")]
    public static partial void ReturningRefreshedToken(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning current token for user: {Parameters.User}")]
    public static partial void ReturningCurrentTokenForUser(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error refreshing access token. Error = {Parameters.Error}")]
    public static partial void FailedToRefreshAccessToken(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "The authorization server has supplied a new nonce on a successful response, which will be stored and used in future requests to the authorization server")]
    public static partial void AuthorizationServerSuppliedNewNonce(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Caching access token for client: {Parameters.ClientName}. Expiration: {Parameters.Expiration}")]
    public static partial void CachingAccessToken(this ILogger logger, string clientName, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error requesting access token for client {Parameters.ClientName}. Error = {Parameters.Error}. This failure will not be cached.")]
    public static partial void FailedToRequestAccessTokenForClient(this ILogger logger, string clientName, string? error);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error trying to set token in cache for client {Parameters.ClientName}")]
    public static partial void ErrorSettingTokenInCache(this ILogger logger, Exception ex, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Cache hit for access token for client: {Parameters.ClientName}")]
    public static partial void DebugCacheHitForAccessToken(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Cache hit for DPoP nonce for URL: {Parameters.Url}, method: {Parameters.Method}")]
    public static partial void DebugCacheHitForDPoPNonce(this ILogger logger, string url, string method);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Caching DPoP nonce for URL: {Parameters.Url}, method: {Parameters.Method}. Expiration: {Parameters.Expiration}")]
    public static partial void DebugCachingDPoPNonce(this ILogger logger, string url, string method, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Cache miss for DPoP nonce for URL: {Parameters.Url}, method: {Parameters.Method}")]
    public static partial void TraceCacheMissForDPoPNonce(this ILogger logger, string url, string method);

    [LoggerMessage(
        Level = LogLevel.Critical,
        Message = $"Error parsing cached access token for client {Parameters.ClientName}")]
    public static partial void CriticalErrorParsingCachedAccessToken(this ILogger logger, Exception ex, string clientName);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Cache miss for access token for client: {Parameters.ClientName}")]
    public static partial void TraceCacheMissForAccessToken(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Creating DPoP proof token for token request.")]
    public static partial void DebugCreatingDPoPProofToken(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Requesting client credentials access token at endpoint: {Parameters.Endpoint}")]
    public static partial void DebugRequestingClientCredentialsAccessToken(this ILogger logger, string endpoint);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Creating DPoP proof token for client {Parameters.ClientName}")]
    public static partial void DebugCreatingDPoPProofTokenForClient(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Requesting client credentials access token from {Parameters.Address}")]
    public static partial void DebugRequestingClientCredentialsAccessTokenFromAddress(this ILogger logger, string address);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Token request failed with DPoP nonce error for client {Parameters.ClientName}")]
    public static partial void DebugTokenRequestFailedWithDPoPNonceErrorForClient(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error trying to obtain token from cache for client {Parameters.ClientName} using cacheKey {Parameters.CacheKey}. Will obtain new token.")]
    public static partial void ErrorTryingToObtainTokenFromCache(this ILogger logger, Exception ex, string clientName, string cacheKey);
}

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
