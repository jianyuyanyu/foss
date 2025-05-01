// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using Duende.AccessTokenManagement.Internal;
using Duende.AccessTokenManagement.OTel;

using Duende.IdentityModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.OpenIdConnect.Internal;

/// <summary>
/// Implements basic token management logic
/// </summary>
internal class UserAccessAccessTokenManagementService(
    AccessTokenManagementMetrics metrics,
    IUserTokenRequestConcurrencyControl sync,
    IUserTokenStore userAccessTokenStore,
    TimeProvider clock,
    IOptions<UserTokenManagementOptions> options,
    IUserTokenEndpointService tokenEndpointService,
    ILogger<UserAccessAccessTokenManagementService> logger) : IUserTokenManagementService
{

    /// <inheritdoc/>
    public async Task<TokenResult<UserToken>> GetAccessTokenAsync(
        ClaimsPrincipal user,
        UserTokenRequestParameters? parameters = null,
        CancellationToken cancellationToken = default)
    {
        logger.StartingUserTokenAcquisition(LogLevel.Debug);

        parameters ??= new UserTokenRequestParameters();

        if (!user.Identity!.IsAuthenticated)
        {
            logger.CannotRetrieveAccessTokenDueToNoActiveUser(LogLevel.Error);
            return TokenResult.Failure("No active user");
        }

        var userName = user.FindFirst(JwtClaimTypes.Name)?.Value ??
                       user.FindFirst(JwtClaimTypes.Subject)?.Value ?? "unknown";
        var getTokenForSpecificParameters = await userAccessTokenStore.GetTokenAsync(user, parameters, cancellationToken).ConfigureAwait(false);

        if (!getTokenForSpecificParameters.WasSuccessful(out var requestedToken, out var failure))
        {
            return failure;
        }

        if (requestedToken.NoRefreshToken)
        {
            logger.NoRefreshTokenAvailableWillNotRefresh(LogLevel.Debug, userName, parameters.Resource ?? "default");
            return requestedToken.TokenForSpecifiedParameters;
        }

        TokenResult<UserToken> result;

        var refreshAfter = clock.GetUtcNow() + options.Value.RefreshBeforeExpiration;

        var shouldRefresh = parameters.ForceTokenRenewal.Value     // We must refresh the token
                            || requestedToken.TokenForSpecifiedParameters == null   // Or there is no token for the current specified set of parameters
                            || requestedToken.TokenForSpecifiedParameters.Expiration < refreshAfter; // Or the existing token is expired

        if (shouldRefresh)
        {
            logger.TokenNeedsRefreshing(LogLevel.Debug, userName, requestedToken.TokenForSpecifiedParameters?.Expiration, parameters.ForceTokenRenewal);

            // Synchronize access to the token request, meaning multiple concurrent requests
            // for the same token will be grouped together. 
            result = await sync.ExecuteWithConcurrencyControl(
                key: requestedToken.RefreshToken,
                tokenRetriever: async () =>
                {
                    var getRefreshedToken = await tokenEndpointService.RefreshAccessTokenAsync(
                                requestedToken.RefreshToken,
                                parameters,
                                cancellationToken).ConfigureAwait(false);

                    if (!getRefreshedToken.WasSuccessful(out var token, out var refreshError))
                    {
                        return refreshError;
                    }

                    await userAccessTokenStore.StoreTokenAsync(user, token, parameters, cancellationToken).ConfigureAwait(false);
                    logger.ReturningRefreshedToken(LogLevel.Trace, userName);

                    return getRefreshedToken;
                },
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // No refresh will be done. 
            if (requestedToken.TokenForSpecifiedParameters == null)
            {
                return TokenResult.Failure("No access token was found nor a refresh token.");
            }

            logger.ReturningCurrentTokenForUser(LogLevel.Trace, userName);
            result = requestedToken.TokenForSpecifiedParameters;
        }

        if (!result.WasSuccessful(out var refreshedToken, out var error))
        {
            return error;
        }

        metrics.AccessTokenUsed(refreshedToken.ClientId, AccessTokenManagementMetrics.TokenRequestType.User);
        return refreshedToken;
    }

    /// <inheritdoc/>
    public async Task RevokeRefreshTokenAsync(
        ClaimsPrincipal user,
        UserTokenRequestParameters? parameters = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new UserTokenRequestParameters();
        var getToken = await userAccessTokenStore.GetTokenAsync(user, parameters, cancellationToken).ConfigureAwait(false);

        if (getToken.WasSuccessful(out var userToken) && userToken.RefreshToken != null)
        {
            await tokenEndpointService.RevokeRefreshTokenAsync(userToken.RefreshToken, parameters, cancellationToken).ConfigureAwait(false);
            await userAccessTokenStore.ClearTokenAsync(user, parameters, cancellationToken).ConfigureAwait(false);
        }
    }
}
