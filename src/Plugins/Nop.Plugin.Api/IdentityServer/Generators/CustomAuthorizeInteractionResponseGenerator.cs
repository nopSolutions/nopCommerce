// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Nop.Plugin.Api.IdentityServer.Generators
{
    using System;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.ResponseHandling;
    using IdentityServer4.Services;
    using IdentityServer4.Validation;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using IAuthenticationService = Microsoft.AspNetCore.Authentication.IAuthenticationService;

    public class NopApiAuthorizeInteractionResponseGenerator : IAuthorizeInteractionResponseGenerator
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The consent service.
        /// </summary>
        protected readonly IConsentService Consent;

        /// <summary>
        /// The profile service.
        /// </summary>
        protected readonly IProfileService Profile;

        /// <summary>
        /// The clock
        /// </summary>
        protected readonly ISystemClock Clock;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeInteractionResponseGenerator"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="consent">The consent.</param>
        /// <param name="profile">The profile.</param>
        public NopApiAuthorizeInteractionResponseGenerator(
            ISystemClock clock,
            ILogger<AuthorizeInteractionResponseGenerator> logger,
            IConsentService consent,
            IProfileService profile, IHttpContextAccessor httpContextAccessor, IAuthenticationService authenticationService)
        {
            Clock = clock;
            Logger = logger;
            Consent = consent;
            Profile = profile;
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Processes the interaction logic.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        public virtual async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request, ConsentResponse consent = null)
        {
            Logger.LogTrace("ProcessInteractionAsync");
            
            if (consent != null && consent.Granted == false && request.Subject.IsAuthenticated() == false)
            {
                // special case when anonymous user has issued a deny prior to authenticating
                Logger.LogInformation("Error: User denied consent");
                return new InteractionResponse
                {
                    Error = OidcConstants.AuthorizeErrors.AccessDenied
                };
            }

            var identityServerUser = new IdentityServerUser(request.ClientId)
            {
                DisplayName = request.Client.ClientName,
                AdditionalClaims = request.ClientClaims,
                AuthenticationTime = new DateTime?(DateTime.UtcNow)
            };

            request.Subject = identityServerUser.CreatePrincipal();

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow
            };
            
            await _httpContextAccessor.HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, request.Subject, authenticationProperties);
            
            var result = new InteractionResponse();
            
            return result;
        }
    }
}