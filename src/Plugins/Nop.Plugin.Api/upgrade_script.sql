-- Used to migrate the clients tables used prior to 4.0 into the tables used by the Identity Server --

BEGIN TRANSACTION;  

	-- Clients --
	Insert into Clients
	Select 2592000 as AbsoluteRefreshTokenLifetime,
		   3600 as AccessTokenLifetime,
		   0 as AccessTokenType,
		   0 as AllowAccessTokensViaBrowser,
		   1 as AllowOfflineAccess,
		   0 as AllowPlainTextPkce,
		   1 as AllowRememberConsent,
		   0 as AlwaysIncludeUserClaimsInIdToken,
		   0 as AlwaysSendClientClaims,
		   300 as AuthorizationCodeLifetime,
		   [ClientId] as ClientId,
		   [Name] as ClientName,
		   NULL as ClientUri,
		   1 as EnableLocalLogin,
		   [IsActive] as [Enabled],
		   300 as IdentityTokenLifetime,
		   0 as IncludeJwtId,
		   NULL as LogoUri,
		   'oidc' as ProtocolType,
		   1 as RefreshTokenExpiration,
		   1 as RefreshTokenUsage,
		   1 as RequireClientSecret,
		   1 as RequireConsent,
		   0 as RequirePkce,
		   1296000 as SlidingRefreshTokenLifetime, 
		   0 as UpdateAccessTokenClaimsOnRefresh,
		   1 as BackChannelLogoutSessionRequired,
		   NULL as BackChannelLogoutUri,
		   'client_' as ClientClaimsPrefix,
		   NULL as ConsentLifetime,
		   NULL as [Description],
		   1 as FrontChannelLogoutSessionRequired,
		   NULL as FrontChannelLogoutUri,
		   NULL as PairWiseSubjectSalt from API_Clients

	-- ClientClaims --
	Insert into ClientClaims 
	Select Id as ClientId, 'sub' as [Type], ClientId as [Value] from Clients

	Insert into ClientClaims 
	Select Id as ClientId, 'name' as [Type], [ClientName] as [Value] from Clients

	-- ClientGrantTypes --
	Insert into ClientGrantTypes
	Select Id as ClientId, 'authorization_code' as GrantType from Clients

	Insert into ClientGrantTypes
	Select Id as ClientId, 'refresh_token' as GrantType from Clients

	Insert into ClientGrantTypes
	Select Id as ClientId, 'urn:ietf:params:oauth:grant-type:jwt-bearer' as GrantType from Clients

	-- ClientRedirectUris --
	Insert into ClientRedirectUris
	Select Client_id as ClientId, CallbackUrl as RedirectUri from (Select Clients.Id as Client_id, Clients.ClientId, CallbackUrl from Clients
																   inner join API_Clients on Clients.ClientId = API_Clients.ClientId) as JoinedClients
	-- ClientScopes --
	Insert into ClientScopes
	Select Id as ClientId, 'nop_api' as Scope from Clients

	-- ClientSecrets --
	Insert into ClientSecrets
	Select Client_id as ClientId, ClientSecret as [Description], NULL as Expiration, 'SharedSecret' as [Type], ClientSecret as [Value] 
	from (Select Clients.Id as Client_id, Clients.ClientId, ClientSecret from Clients
		  inner join API_Clients on Clients.ClientId = API_Clients.ClientId) as JoinedClients

	drop table API_Clients;
COMMIT;  