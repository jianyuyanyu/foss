// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Net;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement;
internal static partial class LogMessages
{
    /// <summary>
    /// Log parameters as constants for consistency
    /// Note, these will be inlined by the compiler to be used in the attributes. 
    /// </summary>
    internal class Parameters
    {
        public const string Scheme = "Scheme";
        public const string Error = "Error";
        public const string ErrorDescription = "ErrorDescription";
        public const string Url = "Url";
        public const string ClientId = "ClientId";
        public const string RequestUrl = "RequestUrl";
        public const string ClientName = "ClientName";
        public const string Expiration = "Expiration";
        public const string TokenHash = "TokenHash";
        public const string User = "User";
        public const string Resource = "Resource";
        public const string Method = "Method";
        public const string Address = "Address";
        public const string CacheKey = "CacheKey";
        public const string TokenType = "TokenType";
        public const string ForceRenewal = "ForceRenewal";
        public const string StatusCode = "StatusCode";
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot authenticate scheme: {{{Parameters.Scheme}}} to acquire user access token.")]
    public static partial void CannotAuthenticateSchemeToAcquireUserAccessToken(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Authentication result properties are null for scheme: {{{Parameters.Scheme}}} after authentication.")]
    public static partial void AuthenticationResultPropertiesAreNullAfterAuthenticate(
        this ILogger logger, string scheme);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Failed to get a UserToken because no tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.")]
    public static partial void FailedToGetUserTokenDueToMissingTokensInCookie(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = $"Error revoking refresh token. Error = {{{Parameters.Error}}}")]
    public static partial void FailedToRevokeAccessToken(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending DPoP proof token in request to endpoint: {{{Parameters.Url}}}")]
    public static partial void SendingDPoPProofToken(this ILogger logger, string? url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Failed to create DPoP proof token for request to endpoint: {{{Parameters.Url}}}")]
    public static partial void FailedToCreateDPopProofToken(this ILogger logger, string? url);


    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Access token of type {{{Parameters.TokenType}}} to endpoint: {{{Parameters.Url}}}.")]
    public static partial void SendAccessTokenToEndpoint(this ILogger logger, string? url, string? tokenType);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to obtain an access token while sending the request.")]
    public static partial void FailedToObtainAccessTokenWhileSendingRequest(this ILogger logger);


    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"DPoP nonce error: '{{{Parameters.Error}}}'while invoking endpoint: {{{Parameters.Url}}}. Retrying using new nonce")]
    public static partial void RequestFailedWithDPoPErrorWillRetry(this ILogger logger, string? error, string? url);

    /// <summary>
    /// Logs the refreshing of a refresh token. Note, the actual refresh token is not logged, but a hash of the token.
    /// Because hashing can be costly, we're only doing this when the log level is Trace. This is not something the source generators
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
        Message = $"Refreshing access token using refresh token: hash={{{Parameters.TokenHash}}}")]
    private static partial void RefreshingTokenUsingRefreshTokenImplementation(this ILogger logger, string tokenHash);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Refresh token request to: {{{Parameters.Url}}}")]
    public static partial void SendingRefreshTokenRequest(this ILogger logger, string? url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"DPoP error '{{{Parameters.Error}}}' during token refresh. Retrying with server nonce")]
    public static partial void DPoPErrorDuringTokenRefreshWillRetryWithServerNonce(this ILogger logger, string? error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Failed to get DPoP Nonce because server didn't respond with ok. StatusCode was: {{{Parameters.StatusCode}}}")]
    public static partial void FailedToGetDPoPNonce(this ILogger logger, HttpStatusCode statusCode);

    /// <summary>
    /// Logs the revocation of a refresh token. Note, the actual refresh token is not logged, but a hash of the token.
    /// Because hashing can be costly, we're only doing this when the log level is Trace. This is not something the source generators
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
        Message = $"Revoking refresh token: hash={{{Parameters.TokenHash}}}")]
    private static partial void RevokingRefreshTokenImplementation(this ILogger logger, string tokenHash);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Sending Token revocation request to: {{{Parameters.Url}}}")]
    public static partial void SendingTokenRevocationRequest(this ILogger logger, string url);

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
        Message = $"Cannot retrieve token: No token data found in user token store for user {{{Parameters.User}}}.")]
    public static partial void CannotRetrieveAccessTokenDueToNoTokenDataFound(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = $"Cannot retrieve token: No refresh token found in user token store for user {{{Parameters.User}}} / resource {{{Parameters.Resource}}}. Returning current access token.")]
    public static partial void CannotRetrieveAccessTokenDueToNoRefreshTokenFound(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"No access token found in user token store for user {{{Parameters.User}}} / resource {{{Parameters.Resource}}}. Trying to refresh.")]
    public static partial void NoAccessTokenFoundWillRefresh(this ILogger logger, string user, string resource);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Token for user {{{Parameters.User}}} will be refreshed. Expiration: {{{Parameters.Expiration}}}, ForceRenewal:{{{Parameters.ForceRenewal}}}")]
    public static partial void DebugTokenNeedsRefreshing(this ILogger logger, string user, DateTimeOffset expiration, bool forceRenewal);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning refreshed token for user: {{{Parameters.User}}}")]
    public static partial void ReturningRefreshedToken(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Returning current token for user: {{{Parameters.User}}}")]
    public static partial void ReturningCurrentTokenForUser(this ILogger logger, string user);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error refreshing access token. Error = {{{Parameters.Error}}}, Description: {{{Parameters.ErrorDescription}}}")]
    public static partial void FailedToRefreshAccessToken(this ILogger logger, string? error, string? errorDescription);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Access Token of type {{{Parameters.TokenType}}} refreshed with expiration: {{{Parameters.Expiration}}}")]
    public static partial void UserAccessTokenRefreshed(this ILogger logger, string? tokenType, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "The authorization server has supplied a new nonce on a successful response, which will be stored and used in future requests to the authorization server")]
    public static partial void AuthorizationServerSuppliedNewNonce(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Caching access token for client: {{{Parameters.ClientName}}}. Expiration: {{{Parameters.Expiration}}}")]
    public static partial void CachingAccessToken(this ILogger logger, string clientName, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Will not cache token result with error for {{{Parameters.ClientName}}}. Error = {{{Parameters.Error}}}")]
    public static partial void WillNotCacheTokenResultWithError(this ILogger logger, string clientName, string? error);


    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error requesting access token for client {{{Parameters.ClientName}}}. Error = {{{Parameters.Error}}}, Description: {{{Parameters.ErrorDescription}}}")]
    public static partial void FailedToRequestAccessTokenForClient(this ILogger logger, string clientName, string? error, string? errorDescription);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error trying to set token in cache for client {{{Parameters.ClientName}}}")]
    public static partial void ErrorSettingTokenInCache(this ILogger logger, Exception ex, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Cache hit for obtaining access token for client: {{{Parameters.ClientName}}}")]
    public static partial void CacheHitForObtainingAccessToken(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Cache hit for DPoP nonce for URL: {{{Parameters.Url}}}, method: {{{Parameters.Method}}}")]
    public static partial void CacheHitForDPoPNonce(this ILogger logger, string url, string method);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Writing DPoP nonce to Cache for URL: {{{Parameters.Url}}}, method: {{{Parameters.Method}}}. Expiration: {{{Parameters.Expiration}}}")]
    public static partial void WritingNonceToCache(this ILogger logger, string url, string method, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Cache miss for DPoP nonce for URL: {{{Parameters.Url}}}, method: {{{Parameters.Method}}}")]
    public static partial void CacheMissForDPoPNonce(this ILogger logger, string url, string method);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Error parsing cached access token for client {{{Parameters.ClientName}}}")]
    public static partial void FailedToCacheAccessToken(this ILogger logger, Exception ex, string clientName);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = $"Cache miss while retrieving access token for client: {{{Parameters.ClientName}}}")]
    public static partial void CacheMissWhileRetrievingAccessToken(this ILogger logger, string clientName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Creating DPoP proof token for token request.")]
    public static partial void CreatingDPoPProofToken(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Requesting client credentials access token at endpoint: {{{Parameters.Url}}}")]
    public static partial void RequestingClientCredentialsAccessToken(this ILogger logger, string url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = $"Client Credentials token of type '{{{Parameters.TokenType}}}' for Client: {{{Parameters.ClientName}}} retrieved with expiration {{{Parameters.Expiration}}} ")]
    public static partial void ClientCredentialsTokenForClientRetrieved(this ILogger logger, string clientName, string? tokenType, DateTimeOffset expiration);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = $"Failed to obtain token from cache for client {{{Parameters.ClientName}}} using cacheKey {{{Parameters.CacheKey}}}. Will obtain new token.")]
    public static partial void FailedToObtainTokenFromCache(this ILogger logger, Exception ex, string clientName, string cacheKey);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to parse JsonWebKey")]
    public static partial void FailedToParseJsonWebKey(this ILogger logger, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create thumbprint from JSON web key.")]
    public static partial void FailedToCreateThumbprintFromJsonWebKey(this ILogger logger, Exception ex);

    public static IDisposable BeginScopeKvp(this ILogger logger, params (string Key, string? Value)[] parameters)
    {
        var logParameters = parameters
            .Where(x => x.Value != null)
            .ToDictionary(x => x.Key, x => (object)(x.Value!));

        return logger.BeginScope(logParameters)
               ?? new EmptyDisposable();
    }
    private struct EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
