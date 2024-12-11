using Nop.Core.Caching;
using Nop.Core.Domain.Attributes;
using Nop.Data;

namespace Nop.Services.Attributes;

/// <summary>
/// Attribute service
/// </summary>
/// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
/// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
public partial class AttributeService<TAttribute, TAttributeValue> : IAttributeService<TAttribute, TAttributeValue>
    where TAttribute : BaseAttribute
    where TAttributeValue : BaseAttributeValue
{
    #region Fields

    protected readonly IRepository<TAttribute> _attributeRepository;
    protected readonly IRepository<TAttributeValue> _attributeValueRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public AttributeService(IRepository<TAttribute> attributeRepository,
        IRepository<TAttributeValue> attributeValueRepository,
        IStaticCacheManager staticCacheManager)
    {
        _attributeRepository = attributeRepository;
        _attributeValueRepository = attributeValueRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Attributes

    /// <summary>
    /// Gets all attributes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> GetAllAttributesAsync()
    {
        return await _attributeRepository.GetAllAsync(
            query => query.OrderBy(attribute => attribute.DisplayOrder)
                .ThenBy(attribute => attribute.Id),
            _ => default);
    }

    /// <summary>
    /// Gets a attribute 
    /// </summary>
    /// <param name="attributeId"> attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute
    /// </returns>
    public virtual async Task<TAttribute> GetAttributeByIdAsync(int attributeId)
    {
        return await _attributeRepository.GetByIdAsync(attributeId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeAsync(TAttribute attribute)
    {
        await _attributeRepository.InsertAsync(attribute);
    }

    /// <summary>
    /// Updates a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeAsync(TAttribute attribute)
    {
        await _attributeRepository.UpdateAsync(attribute);
    }

    /// <summary>
    /// Deletes a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeAsync(TAttribute attribute)
    {
        await _attributeRepository.DeleteAsync(attribute);
    }

    /// <summary>
    /// Gets attributes 
    /// </summary>
    /// <param name="attributeIds">Attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> GetAttributeByIdsAsync(int[] attributeIds)
    {
        return await _attributeRepository.GetByIdsAsync(attributeIds);
    }

    /// <summary>
    /// Deletes attributes
    /// </summary>
    /// <param name="attributes">Attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributesAsync(IList<TAttribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        await _attributeRepository.DeleteAsync(attributes);
    }

    #endregion

    #region Attribute values

    /// <summary>
    /// Gets attribute values by attribute identifier
    /// </summary>
    /// <param name="attributeId">The attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute values
    /// </returns>
    public virtual async Task<IList<TAttributeValue>> GetAttributeValuesAsync(int attributeId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAttributeDefaults.AttributeValuesByAttributeCacheKey, typeof(TAttribute).Name, attributeId);

        var query = _attributeValueRepository.Table
            .Where(attributeValue => attributeValue.AttributeId == attributeId)
            .OrderBy(attributeValue => attributeValue.DisplayOrder)
            .ThenBy(attributeValue => attributeValue.Id);

        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets a attribute value
    /// </summary>
    /// <param name="attributeValueId"> attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute value
    /// </returns>
    public virtual async Task<TAttributeValue> GetAttributeValueByIdAsync(int attributeValueId)
    {
        return await _attributeValueRepository.GetByIdAsync(attributeValueId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeValueAsync(TAttributeValue attributeValue)
    {
        await _attributeValueRepository.InsertAsync(attributeValue);
    }

    /// <summary>
    /// Updates a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeValueAsync(TAttributeValue attributeValue)
    {
        await _attributeValueRepository.UpdateAsync(attributeValue);
    }

    /// <summary>
    /// Deletes a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeValueAsync(TAttributeValue attributeValue)
    {
        await _attributeValueRepository.DeleteAsync(attributeValue);
    }

    #endregion

    #endregion
}