using System.Collections.Generic;
using System.Linq;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="Dictionary{TKey,TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the key associated with the specified value.
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <param name="dict">The dictionary</param>
    /// <param name="value">The value to search for</param>
    /// <returns>The corresponding key if found; otherwise, default(TKey)</returns>
    public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
    {
        return dict.FirstOrDefault(x => EqualityComparer<TValue>.Default.Equals(x.Value, value)).Key;
    }
}