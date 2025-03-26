// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement;
public static partial class LogMessages
{
    internal class Parameters
    {
        public const string Scheme = "{scheme}";
        public const string Error = "{error}";
        public const string Url = "{url}";
        public const string ClientName = "{clientname}";
        public const string Expiration = "{expiration}";
        public const string Token = "{token}";
        public const string Endpoint = "{endpoint}";
        public const string User = "{user}";
        public const string Resource = "{resource}";
        public const string Method = "{method}";
        public const string Address = "{address}";
        public const string CacheKey = "{cachekey}";
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot authenticate scheme: {Parameters.Scheme}")]
    public static partial void CannotAuthenticateScheme(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Authentication result properties are null for scheme: {Parameters.Scheme}")]
    public static partial void InformationAuthenticationResultPropertiesAreNull(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.")]
    public static partial void InformationNoTokensFoundInCookieProperties(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Error revoking refresh token. Error = {Parameters.Error}.")]
    public static partial void InformationFailedToRefreshToken(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending DPoP proof token in request to endpoint: {Parameters.Url}")]
    public static partial void DebugSendingDPoPProofToken(this ILogger logger, string? url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No DPoP proof token in request to endpoint: {Parameters.Url}")]
    public static partial void DebugNoDPoPProofToken(this ILogger logger, string? url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"DPoP nonce error: '{Parameters.Error}' while invoking endpoint: {Parameters.Url}. Retrying using new nonce")]
    public static partial void DebugDPoPNonceErrorRetrying(this ILogger logger, string? error, string? url);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Refreshing refresh token: {Parameters.Token}")]
    public static partial void TraceRefreshingRefreshToken(this ILogger logger, string token);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Refresh token request to: {Parameters.Endpoint}")]
    public static partial void DebugRefreshTokenRequest(this ILogger logger, string? endpoint);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "DPoP error during token refresh. Retrying with server nonce")]
    public static partial void DebugDPoPErrorDuringTokenRefresh(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Revoking refresh token: {Parameters.Token}")]
    public static partial void TraceRevokingRefreshToken(this ILogger logger, string token);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Token revocation request to: {Parameters.Endpoint}")]
    public static partial void DebugTokenRevocationRequest(this ILogger logger, string endpoint);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Starting user token acquisition")]
    public static partial void TraceStartingUserTokenAcquisition(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "No active user. Cannot retrieve token")]
    public static partial void DebugNoActiveUser(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No token data found in user token store for user {Parameters.User}.")]
    public static partial void DebugNoTokenDataFound(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No refresh token found in user token store for user {Parameters.User} / resource {Parameters.Resource}. Returning current access token.")]
    public static partial void DebugNoRefreshTokenFound(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No access token found in user token store for user {Parameters.User} / resource {Parameters.Resource}. Trying to refresh.")]
    public static partial void DebugNoAccessTokenFound(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Token for user {Parameters.User} needs refreshing.")]
    public static partial void DebugTokenNeedsRefreshing(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning refreshed token for user: {Parameters.User}")]
    public static partial void TraceReturningRefreshedToken(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning current token for user: {Parameters.User}")]
    public static partial void TraceReturningCurrentToken(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error refreshing access token. Error = {Parameters.Error}")]
    public static partial void ErrorRefreshingAccessToken(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Token request failed with DPoP nonce error. Retrying with new nonce.")]
    public static partial void DebugTokenRequestFailedWithDPoPNonceError(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "The authorization server has supplied a new nonce on a successful response, which will be stored and used in future requests to the authorization server")]
    public static partial void DebugAuthorizationServerSuppliedNewNonce(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Caching access token for client: {Parameters.ClientName}. Expiration: {Parameters.Expiration}")]
    public static partial void DebugCachingAccessToken(this ILogger logger, string clientName, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error requesting access token for client {Parameters.ClientName}. Error = {Parameters.Error}.")]
    public static partial void ErrorRequestingAccessToken(this ILogger logger, string clientName, string? error);

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
