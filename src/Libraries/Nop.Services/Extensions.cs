using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Services;

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
    #region Fields

    private static readonly ConcurrentDictionary<(Type EntityType, string PropertyName), Delegate> _cache = new();

    #endregion

    #region Methods

    /// <summary>
    /// Get a compiled delegate for the given property selector, reusing a previously cached one
    /// when the selector targets the same property on the same entity type
    /// </summary>
    /// <remarks>
    /// Avoids repeated calls to <see cref="LambdaExpression.Compile"/> for the same property selector,
    /// which would otherwise emit a new <c>DynamicMethod</c> on every invocation and create significant
    /// GC and finalizer pressure under load. Cache key is <c>(EntityType, PropertyName)</c>; selectors
    /// that do not target a property fall back to a plain compile.
    /// </remarks>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="selector">Property selector expression</param>
    /// <returns>Compiled delegate</returns>
    public static Func<TEntity, TPropType> GetCompiled<TEntity, TPropType>(this Expression<Func<TEntity, TPropType>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        //fall back to a plain compile for non-property selectors (e.g. method calls, casts)
        if (selector.Body is not MemberExpression { Member: PropertyInfo propInfo })
            return selector.Compile();

        var key = (typeof(TEntity), propInfo.Name);

        var compiled = (Func<TEntity, TPropType>)_cache.GetOrAdd(key, static (_, sel) => sel.Compile(), selector);

        return compiled;
    }

    /// <summary>
    /// Convert to select list
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="enumObj">Enum</param>
    /// <param name="markCurrentAsSelected">Mark current value as selected</param>
    /// <param name="valuesToExclude">Values to exclude</param>
    /// <param name="useLocalization">Localize</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selectList
    /// </returns>
    public static async Task<SelectList> ToSelectListAsync<TEnum>(this TEnum enumObj,
        bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true) where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
            throw new ArgumentException("An Enumeration type is required.", nameof(enumObj));

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var values = await Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
            .SelectAwait(async enumValue => new
            {
                ID = Convert.ToInt32(enumValue),
                Name = useLocalization
                    ? await localizationService.GetLocalizedEnumAsync(enumValue)
                    : CommonHelper.SplitCamelCaseWord(enumValue.ToString())
            }).ToListAsync();

        object selectedValue = null;
        if (markCurrentAsSelected)
            selectedValue = Convert.ToInt32(enumObj);
        return new SelectList(values, "ID", "Name", selectedValue);
    }

    /// <summary>
    /// Convert to select list
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="objList">List of objects</param>
    /// <param name="selector">Selector for name</param>
    /// <returns>SelectList</returns>
    public static SelectList ToSelectList<T>(this T objList, Func<BaseEntity, string> selector) where T : IEnumerable<BaseEntity>
    {
        return new SelectList(objList.Select(p => new { ID = p.Id, Name = selector(p) }), "ID", "Name");
    }

    /// <summary>
    /// Convert constants defined in the passed type to select list 
    /// </summary>
    /// <param name="items">Collection add items</param>
    /// <param name="type">Type to extract constants</param>
    /// <param name="useLocalization">Localize</param>
    /// <param name="sortItems">Sort resulting items (<see cref="SelectListItem"/>)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the select list
    /// </returns>
    public static async Task<List<SelectListItem>> ConstantsToSelectListAsync(this IList<SelectListItem> items,
        Type type, bool useLocalization = true, bool sortItems = false)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(type);

        var result = new List<SelectListItem>();
        var fields = type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly);

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        foreach (var prop in fields)
        {
            var resourceTypeName = Regex.Replace(type.FullName, "\\W+", ".");
            var resourceConstantName = CommonHelper.SnakeCaseToPascalCase(prop.Name);

            var resourceName = $"{NopLocalizationDefaults.LiteralLocaleStringResourcesPrefix}{resourceTypeName}.{resourceConstantName}";
            var resourceValue = useLocalization ? await localizationService.GetResourceAsync(resourceName) : resourceConstantName;

            result.Add(new SelectListItem { Value = (string)prop.GetRawConstantValue(), Text = resourceValue });
        }

        return sortItems ? result.OrderBy(item => item.Text).ToList() : result;
    }

    /// <summary>
    /// Convert to lookup-like dictionary, for JSON serialization
    /// </summary>
    /// <typeparam name="T">Source type</typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <param name="xs">List of objects</param>
    /// <param name="keySelector">A key-selector function</param>
    /// <param name="valueSelector">A value-selector function</param>
    /// <returns>A dictionary with values grouped by key</returns>
    public static IDictionary<TKey, IList<TValue>> ToGroupedDictionary<T, TKey, TValue>(
        this IEnumerable<T> xs,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector)
    {
        var result = new Dictionary<TKey, IList<TValue>>();

        foreach (var x in xs)
        {
            var key = keySelector(x);
            var value = valueSelector(x);

            if (result.TryGetValue(key, out var list))
                list.Add(value);
            else
                result[key] = new List<TValue> { value };
        }

        return result;
    }

    /// <summary>
    /// Convert to lookup-like dictionary, for JSON serialization
    /// </summary>
    /// <typeparam name="T">Source type</typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <param name="xs">List of objects</param>
    /// <param name="keySelector">A key-selector function</param>
    /// <returns>A dictionary with values grouped by key</returns>
    public static IDictionary<TKey, IList<T>> ToGroupedDictionary<T, TKey>(
        this IEnumerable<T> xs,
        Func<T, TKey> keySelector)
    {
        return xs.ToGroupedDictionary(keySelector, x => x);
    }

    #endregion
}