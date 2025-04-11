using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Services.Messages;

/// <summary>
/// Newsletter subscription type service
/// </summary>
public partial class NewsLetterSubscriptionTypeService : INewsLetterSubscriptionTypeService
{
    #region Fields

    protected readonly IRepository<NewsLetterSubscriptionType> _newsLetterSubscriptionTypeRepository;
    protected readonly IRepository<NewsLetterSubscriptionTypeMapping> _newsLetterSubscriptionTypeMappingRepository;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionTypeService(IRepository<NewsLetterSubscriptionType> newsLetterSubscriptionTypeRepository,
        IRepository<NewsLetterSubscriptionTypeMapping> newsLetterSubscriptionTypeMappingRepository)
    {
        _newsLetterSubscriptionTypeRepository = newsLetterSubscriptionTypeRepository;
        _newsLetterSubscriptionTypeMappingRepository = newsLetterSubscriptionTypeMappingRepository;
    }

    #endregion

    #region Methods

    #region NewsLetter subscription type

    /// <summary>
    /// Inserts a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);

        await _newsLetterSubscriptionTypeRepository.InsertAsync(newsLetterSubscriptionType);
    }

    /// <summary>
    /// Updates a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);

        await _newsLetterSubscriptionTypeRepository.UpdateAsync(newsLetterSubscriptionType);
    }

    /// <summary>
    /// Deletes a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);

        var allTypes = await GetAllNewsLetterSubscriptionTypesAsync();
        if (allTypes.Count == 1)
            throw new Exception("You cannot delete the only configured subscription type");

        await _newsLetterSubscriptionTypeRepository.DeleteAsync(newsLetterSubscriptionType);
    }

    /// <summary>
    /// Gets a newsletter subscription type by newsletter subscription type identifier
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeId">The newsletter subscription type identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription type
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionType> GetNewsLetterSubscriptionTypeByIdAsync(int newsLetterSubscriptionTypeId)
    {
        return await _newsLetterSubscriptionTypeRepository.GetByIdAsync(newsLetterSubscriptionTypeId, cache => default);
    }

    /// <summary>
    /// Gets the newsletter subscription type list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetterSubscriptionType entities
    /// </returns>
    public virtual async Task<IList<NewsLetterSubscriptionType>> GetAllNewsLetterSubscriptionTypesAsync()
    {
        return await _newsLetterSubscriptionTypeRepository.GetAllAsync(
            query => query.OrderBy(subscriptionType => subscriptionType.DisplayOrder).ThenBy(subscriptionType => subscriptionType.Id));
    }

    /// <summary>
    /// Gets list of subscription types by newsletter subscription
    /// </summary>
    /// <param name="newsletter">Newsletter subscription</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of subscription types
    /// </returns>
    public virtual async Task<List<NewsLetterSubscriptionType>> GetSubscriptionTypesByNewsLetterAsync(NewsLetterSubscription newsletter)
    {
        ArgumentNullException.ThrowIfNull(newsletter);

        return await _newsLetterSubscriptionTypeMappingRepository.Table
            .Join(_newsLetterSubscriptionTypeRepository.Table, x => x.NewsLetterSubscriptionTypeId, y => y.Id,
                (x, y) => new { Mapping = x, Type = y })
            .Where(z => z.Mapping.NewsLetterSubscriptionId == newsletter.Id)
            .Select(z => z.Type)
            .ToListAsync();
    }

    #endregion

    #region NewsLetter subscription type mapping

    /// <summary>
    /// Get newsLetter subscription type mappings by subscription type identifier
    /// </summary>
    /// <param name="subscriptionTypeId">The newsLetter subscription type identifier</param>
    /// <param name="subscriptionId">The newsLetter subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription type mappings collection
    /// </returns>
    public virtual async Task<IList<NewsLetterSubscriptionTypeMapping>> GeNewsLetterSubscriptionTypeMappingsAsync(int subscriptionTypeId, int subscriptionId = 0)
    {
        var query =
            from nstm in _newsLetterSubscriptionTypeMappingRepository.Table
            orderby nstm.Id
            where nstm.NewsLetterSubscriptionTypeId == subscriptionTypeId
            select nstm;

        if (subscriptionId > 0)
            query = query.Where(s => s.NewsLetterSubscriptionId == subscriptionId);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Insert a newsLetter subscription type mapping
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeMapping">NewsLetter subscription type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscriptionTypeMapping newsLetterSubscriptionTypeMapping)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionTypeMapping);

        await _newsLetterSubscriptionTypeMappingRepository.InsertAsync(newsLetterSubscriptionTypeMapping);
    }

    /// <summary>
    /// Delete a newsLetter subscription type mapping
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeMapping">NewsLetter subscription type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscriptionTypeMapping newsLetterSubscriptionTypeMapping)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionTypeMapping);

        await _newsLetterSubscriptionTypeMappingRepository.DeleteAsync(newsLetterSubscriptionTypeMapping);
    }

    /// <summary>
    /// Clear all newsLetter subscription type mappings
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscription subscription)
    {
        var query =
            from nstm in _newsLetterSubscriptionTypeMappingRepository.Table
            orderby nstm.Id
            where nstm.NewsLetterSubscriptionId == subscription.Id
            select nstm;

        foreach (var newsLetterSubscriptionTypeMapping in query)
        {
            await _newsLetterSubscriptionTypeMappingRepository.DeleteAsync(newsLetterSubscriptionTypeMapping);
        }
    }

    #endregion

    #endregion
}