using System;
using System.Collections.Generic;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="List{T}"/>.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Removes and returns the last element of a List.
    /// </summary>
    /// <param name="source">A source of values.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <returns>The last element.</returns>
    /// <exception cref="InvalidOperationException">Throws when the source is null or the source is empty.</exception>
    public static TSource PopLast<TSource>(this List<TSource> source)
    {
        if (source == null || source.Count == 0)
            throw new InvalidOperationException("List is empty");

        TSource last = source[source.Count - 1];
        source.RemoveAt(source.Count - 1);
        return last;
    }
}