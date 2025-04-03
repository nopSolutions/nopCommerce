using Nop.Data;
using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the payment token service
/// </summary>
public class PayPalTokenService
{
    #region Fields

    private readonly IRepository<PayPalToken> _tokenRepository;

    #endregion

    #region Ctor

    public PayPalTokenService(IRepository<PayPalToken> tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all payment tokens
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="customerId">Customer identifier; pass 0 to load all tokens</param>
    /// <param name="vaultId">Vault identifier; pass null to load all tokens</param>
    /// <param name="vaultCustomerId">Vault customer identifier; pass null to load all tokens</param>
    /// <param name="type">Token type; pass null to load all tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of payment tokens
    /// </returns>
    public async Task<IList<PayPalToken>> GetAllTokensAsync(string clientId, int customerId = 0,
        string vaultId = null, string vaultCustomerId = null, string type = null)
    {
        return await _tokenRepository.GetAllAsync(query =>
        {
            query = query.Where(token => token.ClientId == clientId);

            if (customerId > 0)
                query = query.Where(token => token.CustomerId == customerId);

            if (!string.IsNullOrEmpty(vaultId))
                query = query.Where(token => token.VaultId == vaultId);

            if (!string.IsNullOrEmpty(vaultCustomerId))
                query = query.Where(token => token.VaultCustomerId == vaultCustomerId);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(token => token.Type == type);

            return query;
        }, null);
    }

    /// <summary>
    /// Get a payment token by identifier
    /// </summary>
    /// <param name="id">Token identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment token
    /// </returns>
    public async Task<PayPalToken> GetByIdAsync(int id)
    {
        return await _tokenRepository.GetByIdAsync(id, null);
    }

    /// <summary>
    /// Insert the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertAsync(PayPalToken token)
    {
        //whether a token with such parameters already exists
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
        var existingToken = tokens
            .FirstOrDefault(existing => existing.VaultId == token.VaultId && existing.VaultCustomerId == token.VaultCustomerId);
        if (existingToken is not null)
        {
            //then just update transaction id
            if (!string.Equals(existingToken.TransactionId, token.TransactionId, StringComparison.InvariantCultureIgnoreCase))
            {
                existingToken.TransactionId = token.TransactionId;
                await UpdateAsync(existingToken);
            }

            return;
        }

        //don't insert tokens with no customer identifier
        if (token.CustomerId == 0)
            return;

        if (!tokens.Any())
            token.IsPrimaryMethod = true;

        //or insert a new one
        await _tokenRepository.InsertAsync(token, false);
    }

    /// <summary>
    /// Update the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateAsync(PayPalToken token)
    {
        await _tokenRepository.UpdateAsync(token, false);
    }

    /// <summary>
    /// Delete the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(PayPalToken token)
    {
        await _tokenRepository.DeleteAsync(token, false);

        //mark one of the remaining tokens as default
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
        if (tokens.Any(existing => existing.IsPrimaryMethod))
            return;

        var existingToken = tokens.FirstOrDefault();
        if (existingToken is null)
            return;

        existingToken.IsPrimaryMethod = true;
        await UpdateAsync(existingToken);
    }

    /// <summary>
    /// Delete payment tokens
    /// </summary>
    /// <param name="token">Payment tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(IList<PayPalToken> tokens)
    {
        await _tokenRepository.DeleteAsync(tokens, false);
    }

    #endregion
}